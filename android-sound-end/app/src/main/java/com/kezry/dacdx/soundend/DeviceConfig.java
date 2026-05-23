package com.kezry.dacdx.soundend;

import android.content.Context;
import android.content.SharedPreferences;

import java.util.UUID;

public final class DeviceConfig {
    public static final int CONTROL_PORT = 39000;
    public static final int AUDIO_PORT = 39001;
    public static final int DISCOVERY_PORT = 39002;
    public String deviceId;
    public String alias;
    public String activeMode;
    public String assignedChannels;
    public String latencyProfile;
    public int manualDelayMs;

    private DeviceConfig() {
    }

    public static DeviceConfig load(Context context) {
        SharedPreferences prefs = context.getSharedPreferences("dacdx", Context.MODE_PRIVATE);
        String id = prefs.getString("device_id", null);
        if (id == null) {
            id = UUID.randomUUID().toString();
            prefs.edit().putString("device_id", id).apply();
        }
        DeviceConfig config = new DeviceConfig();
        config.deviceId = id;
        config.alias = prefs.getString("alias", "Android Sound " + id.substring(0, 8));
        config.activeMode = prefs.getString("active_mode", "sound_card");
        config.assignedChannels = prefs.getString("assigned_channels", "front_left");
        config.latencyProfile = prefs.getString("latency_profile", "balanced");
        config.manualDelayMs = prefs.getInt("manual_delay_ms", 0);
        return config;
    }

    public void save(Context context) {
        context.getSharedPreferences("dacdx", Context.MODE_PRIVATE).edit()
                .putString("alias", alias)
                .putString("active_mode", activeMode)
                .putString("assigned_channels", assignedChannels)
                .putString("latency_profile", latencyProfile)
                .putInt("manual_delay_ms", manualDelayMs)
                .apply();
    }
}

