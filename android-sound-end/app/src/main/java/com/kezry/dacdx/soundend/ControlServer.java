package com.kezry.dacdx.soundend;

import android.content.Context;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

public final class ControlServer implements Runnable {
    private final Context context;
    private final DeviceConfig config;
    private volatile boolean running;
    private ServerSocket serverSocket;

    public ControlServer(Context context, DeviceConfig config) {
        this.context = context.getApplicationContext();
        this.config = config;
    }

    public void start() {
        running = true;
        new Thread(this, "dacdx-control").start();
    }

    public void stop() {
        running = false;
        try {
            if (serverSocket != null) serverSocket.close();
        } catch (Exception ignored) {
        }
    }

    @Override
    public void run() {
        try {
            serverSocket = new ServerSocket(DeviceConfig.CONTROL_PORT);
            while (running) {
                Socket socket = serverSocket.accept();
                handle(socket);
            }
        } catch (Exception ignored) {
        }
    }

    private void handle(Socket socket) {
        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(socket.getInputStream(), "UTF-8"));
            PrintWriter writer = new PrintWriter(socket.getOutputStream(), true);
            String line = reader.readLine();
            JSONObject request = new JSONObject(line);
            String requestId = request.optString("request_id", "");
            if ("dacdx.configure".equals(request.optString("type"))) {
                config.alias = request.optString("alias", config.alias);
                config.activeMode = request.optString("active_mode", config.activeMode);
                JSONArray channels = request.optJSONArray("assigned_channels");
                if (channels != null && channels.length() > 0) {
                    config.assignedChannels = channels.getString(0);
                }
                config.latencyProfile = request.optString("latency_profile", config.latencyProfile);
                config.manualDelayMs = request.optInt("manual_delay_ms", config.manualDelayMs);
                config.save(context);
                writer.println(new JSONObject()
                        .put("type", "dacdx.configure.result")
                        .put("request_id", requestId)
                        .put("ok", true)
                        .toString());
            } else if ("dacdx.status".equals(request.optString("type"))) {
                writer.println(status().toString());
            } else {
                writer.println(new JSONObject()
                        .put("type", "dacdx.configure.result")
                        .put("request_id", requestId)
                        .put("ok", false)
                        .put("error", "Unknown request")
                        .toString());
            }
            socket.close();
        } catch (Exception ignored) {
        }
    }

    private JSONObject status() throws Exception {
        return new JSONObject()
                .put("type", "dacdx.status")
                .put("device_id", config.deviceId)
                .put("alias", config.alias)
                .put("active_mode", config.activeMode)
                .put("enabled", true)
                .put("assigned_channels", new JSONArray().put(config.assignedChannels))
                .put("latency_ms", 60)
                .put("jitter_ms", 0)
                .put("packet_loss", 0)
                .put("buffer_ms", 60)
                .put("clock_offset_us", 0);
    }
}

