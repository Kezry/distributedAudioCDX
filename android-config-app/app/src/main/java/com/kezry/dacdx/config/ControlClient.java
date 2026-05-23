package com.kezry.dacdx.config;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.UUID;

public final class ControlClient {
    public boolean configure(DeviceInfo device, String alias, String mode, String channel, String latency, int manualDelayMs) throws Exception {
        Socket socket = new Socket(device.ip, device.controlPort);
        PrintWriter writer = new PrintWriter(socket.getOutputStream(), true);
        BufferedReader reader = new BufferedReader(new InputStreamReader(socket.getInputStream(), "UTF-8"));
        JSONObject request = new JSONObject()
                .put("type", "dacdx.configure")
                .put("request_id", UUID.randomUUID().toString())
                .put("alias", alias)
                .put("active_mode", mode)
                .put("assigned_channels", new JSONArray().put(channel))
                .put("latency_profile", latency)
                .put("manual_delay_ms", manualDelayMs);
        writer.println(request.toString());
        JSONObject result = new JSONObject(reader.readLine());
        socket.close();
        return result.optBoolean("ok");
    }
}

