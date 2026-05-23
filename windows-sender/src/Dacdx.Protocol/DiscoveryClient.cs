using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Dacdx.Protocol;

public sealed class DiscoveryClient
{
    public const int DiscoveryPort = 39002;
    public const string ProbeMessage = "dacdx.probe";

    public async Task<IReadOnlyList<DiscoveredDevice>> ScanAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var udp = new UdpClient();
        udp.EnableBroadcast = true;
        var probe = Encoding.UTF8.GetBytes(ProbeMessage);
        await udp.SendAsync(probe, probe.Length, new IPEndPoint(IPAddress.Broadcast, DiscoveryPort));

        var devices = new Dictionary<string, DiscoveredDevice>();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        while (!cts.IsCancellationRequested)
        {
            try
            {
                var result = await udp.ReceiveAsync(cts.Token);
                var json = Encoding.UTF8.GetString(result.Buffer);
                var announcement = DacdxJson.Deserialize<DeviceAnnouncement>(json);
                if (announcement.Type != DeviceAnnouncement.MessageType)
                {
                    continue;
                }

                devices[announcement.DeviceId] = new DiscoveredDevice
                {
                    DeviceId = announcement.DeviceId,
                    Alias = announcement.Alias,
                    Ip = announcement.Ip,
                    ControlPort = announcement.ControlPort,
                    AudioPort = announcement.AudioPort,
                    ActiveMode = announcement.ActiveMode,
                    Signal = "online",
                    LatencyMs = 0
                };
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                // Ignore malformed packets during discovery.
            }
        }

        return devices.Values.OrderBy(d => d.Alias).ToArray();
    }
}
