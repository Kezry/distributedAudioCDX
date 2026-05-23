package com.kezry.dacdx.soundend;

import android.content.Context;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.net.ServerSocket;
import java.net.Socket;

public final class DlnaRenderer {
    private static final int HTTP_PORT = 39080;
    private final Context context;
    private final DeviceConfig config;
    private volatile boolean running;
    private MulticastSocket ssdpSocket;
    private ServerSocket httpSocket;

    public DlnaRenderer(Context context, DeviceConfig config) {
        this.context = context.getApplicationContext();
        this.config = config;
    }

    public void start() {
        running = true;
        new Thread(new Runnable() {
            @Override
            public void run() {
                ssdpLoop();
            }
        }, "dacdx-dlna-ssdp").start();
        new Thread(new Runnable() {
            @Override
            public void run() {
                httpLoop();
            }
        }, "dacdx-dlna-http").start();
    }

    public void stop() {
        running = false;
        if (ssdpSocket != null) ssdpSocket.close();
        try {
            if (httpSocket != null) httpSocket.close();
        } catch (Exception ignored) {
        }
    }

    private void ssdpLoop() {
        try {
            ssdpSocket = new MulticastSocket(1900);
            ssdpSocket.joinGroup(InetAddress.getByName("239.255.255.250"));
            byte[] buffer = new byte[2048];
            while (running) {
                DatagramPacket packet = new DatagramPacket(buffer, buffer.length);
                ssdpSocket.receive(packet);
                String text = new String(packet.getData(), packet.getOffset(), packet.getLength(), "UTF-8").toUpperCase();
                if (text.contains("M-SEARCH") && (text.contains("SSDP:ALL") || text.contains("MEDIARenderer".toUpperCase()))) {
                    byte[] response = ssdpResponse().getBytes("UTF-8");
                    ssdpSocket.send(new DatagramPacket(response, response.length, packet.getAddress(), packet.getPort()));
                }
            }
        } catch (Exception ignored) {
        }
    }

    private String ssdpResponse() {
        String ip = NetUtil.localIp(context);
        return "HTTP/1.1 200 OK\r\n"
                + "CACHE-CONTROL: max-age=1800\r\n"
                + "EXT:\r\n"
                + "LOCATION: http://" + ip + ":" + HTTP_PORT + "/device.xml\r\n"
                + "SERVER: Android/4.4 UPnP/1.0 distributedAudioCDX/0.1\r\n"
                + "ST: urn:schemas-upnp-org:device:MediaRenderer:1\r\n"
                + "USN: uuid:" + config.deviceId + "::urn:schemas-upnp-org:device:MediaRenderer:1\r\n\r\n";
    }

    private void httpLoop() {
        try {
            httpSocket = new ServerSocket(HTTP_PORT);
            while (running) {
                handle(httpSocket.accept());
            }
        } catch (Exception ignored) {
        }
    }

    private void handle(Socket socket) {
        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(socket.getInputStream(), "UTF-8"));
            String request = reader.readLine();
            PrintWriter writer = new PrintWriter(socket.getOutputStream(), true);
            String body = request != null && request.contains("/device.xml") ? deviceXml() : soapOk();
            writer.print("HTTP/1.1 200 OK\r\n");
            writer.print("Content-Type: text/xml; charset=\"utf-8\"\r\n");
            writer.print("Content-Length: " + body.getBytes("UTF-8").length + "\r\n\r\n");
            writer.print(body);
            writer.flush();
            socket.close();
        } catch (Exception ignored) {
        }
    }

    private String deviceXml() {
        return "<?xml version=\"1.0\"?>"
                + "<root xmlns=\"urn:schemas-upnp-org:device-1-0\">"
                + "<specVersion><major>1</major><minor>0</minor></specVersion>"
                + "<device>"
                + "<deviceType>urn:schemas-upnp-org:device:MediaRenderer:1</deviceType>"
                + "<friendlyName>" + config.alias + "</friendlyName>"
                + "<manufacturer>Kezry</manufacturer>"
                + "<modelName>distributedAudioCDX Sound End</modelName>"
                + "<UDN>uuid:" + config.deviceId + "</UDN>"
                + "<serviceList>"
                + "<service><serviceType>urn:schemas-upnp-org:service:AVTransport:1</serviceType><serviceId>urn:upnp-org:serviceId:AVTransport</serviceId><controlURL>/control</controlURL><eventSubURL>/event</eventSubURL><SCPDURL>/avtransport.xml</SCPDURL></service>"
                + "<service><serviceType>urn:schemas-upnp-org:service:RenderingControl:1</serviceType><serviceId>urn:upnp-org:serviceId:RenderingControl</serviceId><controlURL>/control</controlURL><eventSubURL>/event</eventSubURL><SCPDURL>/rendering.xml</SCPDURL></service>"
                + "</serviceList></device></root>";
    }

    private String soapOk() {
        return "<?xml version=\"1.0\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                + "<s:Body><u:OK xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"/></s:Body></s:Envelope>";
    }
}
