package com.kezry.dacdx.soundend;

import android.media.AudioFormat;
import android.media.AudioManager;
import android.media.AudioTrack;

public final class AudioPlayer {
    private AudioTrack track;
    private int sampleRate = 48000;

    public synchronized void ensureStarted(int requestedSampleRate) {
        if (track != null && sampleRate == requestedSampleRate) {
            return;
        }
        stop();
        sampleRate = requestedSampleRate;
        int minBuffer = AudioTrack.getMinBufferSize(
                sampleRate,
                AudioFormat.CHANNEL_OUT_MONO,
                AudioFormat.ENCODING_PCM_16BIT);
        int buffer = Math.max(minBuffer, sampleRate / 5);
        track = new AudioTrack(
                AudioManager.STREAM_MUSIC,
                sampleRate,
                AudioFormat.CHANNEL_OUT_MONO,
                AudioFormat.ENCODING_PCM_16BIT,
                buffer,
                AudioTrack.MODE_STREAM);
        track.play();
    }

    public synchronized void write(byte[] pcm) {
        if (track != null && pcm != null && pcm.length > 0) {
            track.write(pcm, 0, pcm.length);
        }
    }

    public synchronized void stop() {
        if (track != null) {
            try {
                track.pause();
                track.flush();
                track.release();
            } catch (Exception ignored) {
            }
            track = null;
        }
    }
}

