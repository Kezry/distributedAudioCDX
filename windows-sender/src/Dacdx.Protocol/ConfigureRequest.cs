using System.Text.Json.Serialization;

namespace Dacdx.Protocol;

public sealed record ConfigureRequest(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("request_id")] string RequestId,
    [property: JsonPropertyName("alias")] string Alias,
    [property: JsonPropertyName("active_mode")] string ActiveMode,
    [property: JsonPropertyName("assigned_channels")] string[] AssignedChannels,
    [property: JsonPropertyName("latency_profile")] string LatencyProfile,
    [property: JsonPropertyName("manual_delay_ms")] int ManualDelayMs)
{
    public const string MessageType = "dacdx.configure";
}

public sealed record ConfigureResult(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("request_id")] string RequestId,
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("error")] string? Error)
{
    public const string MessageType = "dacdx.configure.result";
}

