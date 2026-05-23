package com.kezry.dacdx.soundend;

import java.util.TreeMap;

public final class JitterBuffer {
    private final TreeMap<Long, byte[]> packets = new TreeMap<Long, byte[]>();
    private Long nextSequence;

    public synchronized void push(long sequence, byte[] payload) {
        packets.put(sequence, payload);
        if (nextSequence == null) {
            nextSequence = sequence;
        }
        notifyAll();
    }

    public synchronized byte[] pop(long timeoutMs) throws InterruptedException {
        long deadline = System.currentTimeMillis() + timeoutMs;
        while (packets.isEmpty()) {
            long waitMs = deadline - System.currentTimeMillis();
            if (waitMs <= 0) return null;
            wait(waitMs);
        }
        if (nextSequence != null && packets.containsKey(nextSequence)) {
            byte[] exact = packets.remove(nextSequence);
            nextSequence++;
            return exact;
        }
        if (packets.size() > 8) {
            Long first = packets.firstKey();
            byte[] payload = packets.remove(first);
            nextSequence = first + 1;
            return payload;
        }
        return null;
    }
}

