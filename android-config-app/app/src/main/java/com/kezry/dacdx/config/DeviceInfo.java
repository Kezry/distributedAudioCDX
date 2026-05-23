package com.kezry.dacdx.config;

public final class DeviceInfo {
    public String deviceId;
    public String alias;
    public String ip;
    public int controlPort;
    public int audioPort;
    public String activeMode;

    @Override
    public String toString() {
        return alias + "\n" + deviceId + "  " + ip + "  " + activeMode;
    }
}

