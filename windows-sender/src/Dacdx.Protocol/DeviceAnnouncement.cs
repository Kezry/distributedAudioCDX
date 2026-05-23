using System.Text.Json.Serialization;

namespace Dacdx.Protocol;

public sealed record DeviceAnnouncement(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("protocol_version")] int ProtocolVersion,
    [property: JsonPropertyName("device_id")] string DeviceId,
    [property: JsonPropertyName("alias")] string Alias,
    [property: JsonPropertyName("ip")] string Ip,
    [property: JsonPropertyName("control_port")] int ControlPort,
    [property: JsonPropertyName("audio_port")] int AudioPort,
    [property: JsonPropertyName("modes")] string[] Modes,
    [property: JsonPropertyName("active_mode")] string ActiveMode,
    [property: JsonPropertyName("sample_rates")] int[] SampleRates,
    [property: JsonPropertyName("formats")] string[] Formats,
    [property: JsonPropertyName("channels")] string[] Channels,
    [property: JsonPropertyName("version")] string Version)
{
    public const string MessageType = "dacdx.announce";
}

