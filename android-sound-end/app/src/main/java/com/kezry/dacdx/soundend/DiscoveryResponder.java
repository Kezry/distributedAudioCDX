package com.kezry.dacdx.soundend;

import android.content.Context;

import org.json.JSONArray;
import org.json.JSONObject;

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;

public final class DiscoveryResponder implements Runnable {
    private final Context context;
    private final DeviceConfig config;
    private volatile boolean running;
    private Thread thread;
    private DatagramSocket socket;

    public DiscoveryResponder(Context context, DeviceConfig config) {
        this.context = context.getApplicationContext();
        this.config = config;
    }

    public void start() {
        running = true;
        thread = new Thread(this, "dacdx-discovery");
        thread.start();
    }

    public void stop() {
        running = false;
        if (socket != null) socket.close();
    }

    @Override
    public void run() {
        try {
            socket = new DatagramSocket(DeviceConfig.DISCOVERY_PORT);
            socket.setBroadcast(true);
            byte[] buffer = new byte[2048];
            while (running) {
                DatagramPacket packet = new DatagramPacket(buffer, buffer.length);
                socket.receive(packet);
                String probe = new String(packet.getData(), packet.getOffset(), packet.getLength(), "UTF-8");
                if ("dacdx.probe".equals(probe)) {
                    byte[] response = announcement().getBytes("UTF-8");
                    DatagramPacket out = new DatagramPacket(response, response.length, packet.getAddress(), packet.getPort());
                    socket.send(out);
                }
            }
        } catch (Exception ignored) {
        }
    }

    private String announcement() throws Exception {
        JSONObject json = new JSONObject();
        json.put("type", "dacdx.announce");
        json.put("protocol_version", 1);
        json.put("device_id", config.deviceId);
        json.put("alias", config.alias);
        json.put("ip", NetUtil.localIp(context));
        json.put("control_port", DeviceConfig.CONTROL_PORT);
        json.put("audio_port", DeviceConfig.AUDIO_PORT);
        json.put("modes", new JSONArray().put("sound_card").put("dlna"));
        json.put("active_mode", config.activeMode);
        json.put("sample_rates", new JSONArray().put(44100).put(48000));
        json.put("formats", new JSONArray().put("pcm_s16le"));
        json.put("channels", new JSONArray().put("front_left").put("front_right").put("mono"));
        json.put("version", "0.1.0");
        return json.toString();
    }
}

