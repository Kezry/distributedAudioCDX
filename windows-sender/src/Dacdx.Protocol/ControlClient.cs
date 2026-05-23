using System.Net.Sockets;
using System.Text;

namespace Dacdx.Protocol;

public sealed class ControlClient
{
    public async Task<ConfigureResult> ConfigureAsync(
        DiscoveredDevice device,
        ConfigureRequest request,
        CancellationToken cancellationToken)
    {
        using var tcp = new TcpClient();
        await tcp.ConnectAsync(device.Ip, device.ControlPort, cancellationToken);
        await using var stream = tcp.GetStream();
        var payload = Encoding.UTF8.GetBytes(DacdxJson.Serialize(request) + "\n");
        await stream.WriteAsync(payload, cancellationToken);

        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        var line = await reader.ReadLineAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(line))
        {
            return new ConfigureResult(ConfigureResult.MessageType, request.RequestId, false, "Empty response.");
        }

        return DacdxJson.Deserialize<ConfigureResult>(line);
    }
}

