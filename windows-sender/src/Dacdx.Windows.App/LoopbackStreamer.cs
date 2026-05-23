using System.Net;
using System.Net.Sockets;
using Dacdx.Protocol;
using NAudio.Wave;

namespace Dacdx.Windows.App;

public sealed class LoopbackStreamer : IDisposable
{
    private const int PacketDurationMs = 10;
    private readonly DiscoveredDevice[] _devices;
    private readonly UdpClient _udp = new();
    private WasapiLoopbackCapture? _capture;
    private uint _streamId;
    private uint _sequence;

    public LoopbackStreamer(IReadOnlyList<DiscoveredDevice> devices)
    {
        _devices = devices.ToArray();
    }

    public void Start()
    {
        _streamId = (uint)Environment.TickCount;
        _capture = new WasapiLoopbackCapture();
        _capture.DataAvailable += OnDataAvailable;
        _capture.StartRecording();
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (_capture is null || e.BytesRecorded <= 0)
        {
            return;
        }

        var sourceFormat = _capture.WaveFormat;
        byte[] pcm16Stereo = ConvertToPcm16Stereo(e.Buffer, e.BytesRecorded, sourceFormat);
        if (pcm16Stereo.Length == 0)
        {
            return;
        }

        var totalFrames = pcm16Stereo.Length / 4;
        var framesPerPacket = Math.Max(1, sourceFormat.SampleRate * PacketDurationMs / 1000);
        for (var frameOffset = 0; frameOffset < totalFrames; frameOffset += framesPerPacket)
        {
            var framesInPacket = Math.Min(framesPerPacket, totalFrames - frameOffset);
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000;
            foreach (var device in _devices)
            {
                var mono = ExtractMono(pcm16Stereo, frameOffset, framesInPacket, device.AssignedChannel);
                var packet = new byte[AudioPacketHeader.Size + mono.Length];
                var header = AudioPacketHeader.Create(
                    _streamId,
                    _sequence,
                    (uint)sourceFormat.SampleRate,
                    2,
                    (uint)framesInPacket,
                    now,
                    now + 80_000);
                header.WriteTo(packet);
                Buffer.BlockCopy(mono, 0, packet, AudioPacketHeader.Size, mono.Length);
                _udp.Send(packet, packet.Length, new IPEndPoint(IPAddress.Parse(device.Ip), device.AudioPort));
            }

            _sequence++;
        }
    }

    private static byte[] ConvertToPcm16Stereo(byte[] input, int count, WaveFormat sourceFormat)
    {
        if (sourceFormat.Encoding == WaveFormatEncoding.IeeeFloat && sourceFormat.BitsPerSample == 32)
        {
            var sourceChannels = sourceFormat.Channels;
            var sourceFrames = count / (sizeof(float) * sourceChannels);
            var output = new byte[sourceFrames * 4];
            for (var frame = 0; frame < sourceFrames; frame++)
            {
                var left = ReadFloat(input, ((frame * sourceChannels) + 0) * sizeof(float));
                var right = sourceChannels > 1 ? ReadFloat(input, ((frame * sourceChannels) + 1) * sizeof(float)) : left;
                WritePcm16(output, (frame * 4) + 0, left);
                WritePcm16(output, (frame * 4) + 2, right);
            }

            return output;
        }

        if (sourceFormat.Encoding == WaveFormatEncoding.Pcm && sourceFormat.BitsPerSample == 16 && sourceFormat.Channels >= 2)
        {
            var sourceFrames = count / (sizeof(short) * sourceFormat.Channels);
            var output = new byte[sourceFrames * 4];
            for (var frame = 0; frame < sourceFrames; frame++)
            {
                var sourceOffset = frame * sourceFormat.Channels * sizeof(short);
                output[(frame * 4) + 0] = input[sourceOffset + 0];
                output[(frame * 4) + 1] = input[sourceOffset + 1];
                output[(frame * 4) + 2] = input[sourceOffset + 2];
                output[(frame * 4) + 3] = input[sourceOffset + 3];
            }

            return output;
        }

        return Array.Empty<byte>();
    }

    private static byte[] ExtractMono(byte[] stereoPcm16, int frameOffset, int frameCount, ChannelRole role)
    {
        var right = role is ChannelRole.FrontRight or ChannelRole.RearRight or ChannelRole.SideRight;
        var output = new byte[frameCount * 2];
        var sourceOffset = right ? 2 : 0;
        for (var frame = 0; frame < frameCount; frame++)
        {
            var sourceFrame = frameOffset + frame;
            output[(frame * 2) + 0] = stereoPcm16[(sourceFrame * 4) + sourceOffset + 0];
            output[(frame * 2) + 1] = stereoPcm16[(sourceFrame * 4) + sourceOffset + 1];
        }

        return output;
    }

    private static float ReadFloat(byte[] buffer, int offset) => BitConverter.ToSingle(buffer, offset);

    private static void WritePcm16(byte[] buffer, int offset, float sample)
    {
        sample = Math.Clamp(sample, -1.0f, 1.0f);
        var value = (short)(sample * short.MaxValue);
        buffer[offset] = (byte)(value & 0xff);
        buffer[offset + 1] = (byte)((value >> 8) & 0xff);
    }

    public void Dispose()
    {
        if (_capture is not null)
        {
            _capture.DataAvailable -= OnDataAvailable;
            _capture.StopRecording();
            _capture.Dispose();
            _capture = null;
        }

        _udp.Dispose();
    }
}
