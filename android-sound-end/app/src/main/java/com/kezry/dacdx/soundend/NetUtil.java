package com.kezry.dacdx.soundend;

import android.content.Context;
import android.net.wifi.WifiManager;
import android.text.format.Formatter;

public final class NetUtil {
    private NetUtil() {
    }

    public static String localIp(Context context) {
        WifiManager wifi = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        if (wifi == null || wifi.getConnectionInfo() == null) {
            return "0.0.0.0";
        }
        return Formatter.formatIpAddress(wifi.getConnectionInfo().getIpAddress());
    }
}

