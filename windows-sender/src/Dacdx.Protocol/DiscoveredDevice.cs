namespace Dacdx.Protocol;

public sealed class DiscoveredDevice
{
    public string DeviceId { get; set; } = "";
    public string Alias { get; set; } = "";
    public string Ip { get; set; } = "";
    public int ControlPort { get; set; }
    public int AudioPort { get; set; }
    public string ActiveMode { get; set; } = "sound_card";
    public string Signal { get; set; } = "unknown";
    public bool Enabled { get; set; }
    public ChannelRole AssignedChannel { get; set; } = ChannelRole.FrontLeft;
    public int LatencyMs { get; set; }
    public int JitterMs { get; set; }
    public double PacketLoss { get; set; }
}

