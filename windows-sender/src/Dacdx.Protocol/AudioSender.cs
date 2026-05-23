using System.Net;
using System.Net.Sockets;

namespace Dacdx.Protocol;

public sealed class AudioSender
{
    private const int SampleRate = 48000;
    private const int PacketDurationMs = 10;
    private const int FramesPerPacket = SampleRate * PacketDurationMs / 1000;

    public async Task SendTestToneAsync(
        IReadOnlyList<DiscoveredDevice> devices,
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        var enabled = devices.Where(d => d.Enabled).ToArray();
        if (enabled.Length == 0)
        {
            return;
        }

        using var udp = new UdpClient();
        var streamId = (uint)Random.Shared.Next();
        var sequence = 0u;
        var packetCount = (int)(duration.TotalMilliseconds / PacketDurationMs);
        var stereoTone = TestToneGenerator.GenerateStereoSine(SampleRate, 880, PacketDurationMs);
        var matrix = ChannelMatrix.Stereo;

        for (var packetIndex = 0; packetIndex < packetCount && !cancellationToken.IsCancellationRequested; packetIndex++)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000;
            foreach (var device in enabled)
            {
                var channel = matrix.ExtractPcm16Interleaved(stereoTone, 2, device.AssignedChannel);
                var payloadBytes = new byte[channel.Length * sizeof(short)];
                Buffer.BlockCopy(channel, 0, payloadBytes, 0, payloadBytes.Length);
                var packet = new byte[AudioPacketHeader.Size + payloadBytes.Length];
                var header = AudioPacketHeader.Create(
                    streamId,
                    sequence,
                    SampleRate,
                    2,
                    (uint)channel.Length,
                    now,
                    now + 60_000);
                header.WriteTo(packet);
                Buffer.BlockCopy(payloadBytes, 0, packet, AudioPacketHeader.Size, payloadBytes.Length);
                await udp.SendAsync(packet, packet.Length, new IPEndPoint(IPAddress.Parse(device.Ip), device.AudioPort));
            }

            sequence++;
            await Task.Delay(PacketDurationMs, cancellationToken);
        }
    }
}
