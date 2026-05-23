using System.Text.Json.Serialization;

namespace Dacdx.Protocol;

public sealed record DeviceStatus(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("device_id")] string DeviceId,
    [property: JsonPropertyName("alias")] string Alias,
    [property: JsonPropertyName("active_mode")] string ActiveMode,
    [property: JsonPropertyName("enabled")] bool Enabled,
    [property: JsonPropertyName("assigned_channels")] string[] AssignedChannels,
    [property: JsonPropertyName("latency_ms")] int LatencyMs,
    [property: JsonPropertyName("jitter_ms")] int JitterMs,
    [property: JsonPropertyName("packet_loss")] double PacketLoss,
    [property: JsonPropertyName("buffer_ms")] int BufferMs,
    [property: JsonPropertyName("clock_offset_us")] long ClockOffsetUs)
{
    public const string MessageType = "dacdx.status";
}

