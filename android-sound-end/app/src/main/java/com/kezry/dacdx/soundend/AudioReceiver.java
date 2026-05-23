package com.kezry.dacdx.soundend;

import java.net.DatagramPacket;
import java.net.DatagramSocket;

public final class AudioReceiver {
    private final DeviceConfig config;
    private final AudioPlayer player;
    private final JitterBuffer jitterBuffer = new JitterBuffer();
    private volatile boolean running;
    private DatagramSocket socket;

    public AudioReceiver(DeviceConfig config, AudioPlayer player) {
        this.config = config;
        this.player = player;
    }

    public void start() {
        running = true;
        new Thread(new Runnable() {
            @Override
            public void run() {
                receiveLoop();
            }
        }, "dacdx-audio-rx").start();
        new Thread(new Runnable() {
            @Override
            public void run() {
                playbackLoop();
            }
        }, "dacdx-audio-playback").start();
    }

    public void stop() {
        running = false;
        if (socket != null) socket.close();
        player.stop();
    }

    private void receiveLoop() {
        try {
            socket = new DatagramSocket(DeviceConfig.AUDIO_PORT);
            byte[] buffer = new byte[8192];
            while (running) {
                DatagramPacket packet = new DatagramPacket(buffer, buffer.length);
                socket.receive(packet);
                AudioPacketHeader header = AudioPacketHeader.parse(packet.getData(), packet.getLength());
                player.ensureStarted(header.sampleRate);
                int payloadLength = packet.getLength() - AudioPacketHeader.SIZE;
                if (payloadLength > 0) {
                    byte[] payload = new byte[payloadLength];
                    System.arraycopy(packet.getData(), AudioPacketHeader.SIZE, payload, 0, payloadLength);
                    jitterBuffer.push(header.sequence, payload);
                }
            }
        } catch (Exception ignored) {
        }
    }

    private void playbackLoop() {
        while (running) {
            try {
                byte[] pcm = jitterBuffer.pop(targetBufferMs());
                if (pcm != null) {
                    player.write(pcm);
                }
            } catch (Exception ignored) {
            }
        }
    }

    private int targetBufferMs() {
        if ("fast".equals(config.latencyProfile)) return 30;
        if ("stable".equals(config.latencyProfile)) return 140;
        return 60;
    }
}

