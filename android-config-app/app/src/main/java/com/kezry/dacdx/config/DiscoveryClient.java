package com.kezry.dacdx.config;

import org.json.JSONObject;

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketTimeoutException;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public final class DiscoveryClient {
    public List<DeviceInfo> scan(int timeoutMs) throws Exception {
        DatagramSocket socket = new DatagramSocket();
        socket.setBroadcast(true);
        socket.setSoTimeout(timeoutMs);
        byte[] probe = "dacdx.probe".getBytes("UTF-8");
        socket.send(new DatagramPacket(probe, probe.length, InetAddress.getByName("255.255.255.255"), 39002));

        LinkedHashMap<String, DeviceInfo> devices = new LinkedHashMap<String, DeviceInfo>();
        byte[] buffer = new byte[4096];
        long deadline = System.currentTimeMillis() + timeoutMs;
        while (System.currentTimeMillis() < deadline) {
            try {
                DatagramPacket packet = new DatagramPacket(buffer, buffer.length);
                socket.receive(packet);
                String json = new String(packet.getData(), packet.getOffset(), packet.getLength(), "UTF-8");
                JSONObject object = new JSONObject(json);
                if (!"dacdx.announce".equals(object.optString("type"))) continue;
                DeviceInfo info = new DeviceInfo();
                info.deviceId = object.optString("device_id");
                info.alias = object.optString("alias");
                info.ip = object.optString("ip");
                info.controlPort = object.optInt("control_port", 39000);
                info.audioPort = object.optInt("audio_port", 39001);
                info.activeMode = object.optString("active_mode");
                devices.put(info.deviceId, info);
            } catch (SocketTimeoutException ignored) {
                break;
            } catch (Exception ignored) {
            }
        }
        socket.close();
        return new ArrayList<DeviceInfo>(devices.values());
    }
}
