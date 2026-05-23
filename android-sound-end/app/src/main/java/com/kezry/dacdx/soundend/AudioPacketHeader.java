package com.kezry.dacdx.soundend;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public final class AudioPacketHeader {
    public static final int SIZE = 44;
    public static final int MAGIC = 0xCDAC;
    public int streamId;
    public long sequence;
    public int sampleRate;
    public int channelLayout;
    public int frameCount;
    public long captureTimestampUs;
    public long playAtTimestampUs;

    public static AudioPacketHeader parse(byte[] data, int length) {
        if (length < SIZE) {
            throw new IllegalArgumentException("Audio packet too small");
        }
        ByteBuffer buffer = ByteBuffer.wrap(data, 0, SIZE).order(ByteOrder.LITTLE_ENDIAN);
        int magic = buffer.getShort() & 0xffff;
        int version = buffer.getShort() & 0xffff;
        if (magic != MAGIC || version != 1) {
            throw new IllegalArgumentException("Unsupported DACDX audio packet");
        }
        AudioPacketHeader header = new AudioPacketHeader();
        header.streamId = buffer.getInt();
        header.sequence = buffer.getInt() & 0xffffffffL;
        header.sampleRate = buffer.getInt();
        header.channelLayout = buffer.getInt();
        header.frameCount = buffer.getInt();
        header.captureTimestampUs = buffer.getLong();
        header.playAtTimestampUs = buffer.getLong();
        return header;
    }
}

