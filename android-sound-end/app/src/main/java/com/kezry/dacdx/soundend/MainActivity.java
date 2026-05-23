package com.kezry.dacdx.soundend;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.widget.TextView;

public class MainActivity extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        DeviceConfig config = DeviceConfig.load(this);
        TextView view = new TextView(this);
        view.setPadding(32, 32, 32, 32);
        view.setText("distributedAudioCDX Sound End\n\n"
                + "Device ID: " + config.deviceId + "\n"
                + "Alias: " + config.alias + "\n"
                + "Mode: " + config.activeMode + "\n"
                + "Channels: " + config.assignedChannels + "\n\n"
                + "Discovery: UDP 39002\nControl: TCP 39000\nAudio: UDP 39001\nDLNA: SSDP 1900");
        setContentView(view);
        startService(new Intent(this, SoundEndService.class));
    }
}

