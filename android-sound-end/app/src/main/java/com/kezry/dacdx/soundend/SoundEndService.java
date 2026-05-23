package com.kezry.dacdx.soundend;

import android.app.Service;
import android.content.Intent;
import android.net.wifi.WifiManager;
import android.os.IBinder;

public class SoundEndService extends Service {
    private volatile boolean running;
    private DiscoveryResponder discoveryResponder;
    private ControlServer controlServer;
    private AudioReceiver audioReceiver;
    private DlnaRenderer dlnaRenderer;
    private WifiManager.MulticastLock multicastLock;

    @Override
    public void onCreate() {
        super.onCreate();
        running = true;
        WifiManager wifi = (WifiManager) getApplicationContext().getSystemService(WIFI_SERVICE);
        if (wifi != null) {
            multicastLock = wifi.createMulticastLock("dacdx");
            multicastLock.setReferenceCounted(false);
            multicastLock.acquire();
        }
        DeviceConfig config = DeviceConfig.load(this);
        AudioPlayer player = new AudioPlayer();
        discoveryResponder = new DiscoveryResponder(this, config);
        controlServer = new ControlServer(this, config);
        audioReceiver = new AudioReceiver(config, player);
        dlnaRenderer = new DlnaRenderer(this, config);
        discoveryResponder.start();
        controlServer.start();
        audioReceiver.start();
        dlnaRenderer.start();
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        return START_STICKY;
    }

    @Override
    public void onDestroy() {
        running = false;
        if (discoveryResponder != null) discoveryResponder.stop();
        if (controlServer != null) controlServer.stop();
        if (audioReceiver != null) audioReceiver.stop();
        if (dlnaRenderer != null) dlnaRenderer.stop();
        if (multicastLock != null && multicastLock.isHeld()) multicastLock.release();
        super.onDestroy();
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }
}

