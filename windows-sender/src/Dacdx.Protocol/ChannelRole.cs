namespace Dacdx.Protocol;

public enum ChannelRole
{
    FrontLeft = 0,
    FrontRight = 1,
    FrontCenter = 2,
    LowFrequency = 3,
    RearLeft = 4,
    RearRight = 5,
    SideLeft = 6,
    SideRight = 7
}

public static class ChannelRoleNames
{
    public static string ToWireName(ChannelRole role) => role switch
    {
        ChannelRole.FrontLeft => "front_left",
        ChannelRole.FrontRight => "front_right",
        ChannelRole.FrontCenter => "front_center",
        ChannelRole.LowFrequency => "low_frequency",
        ChannelRole.RearLeft => "rear_left",
        ChannelRole.RearRight => "rear_right",
        ChannelRole.SideLeft => "side_left",
        ChannelRole.SideRight => "side_right",
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };

    public static ChannelRole Parse(string value) => value switch
    {
        "front_left" => ChannelRole.FrontLeft,
        "front_right" => ChannelRole.FrontRight,
        "front_center" => ChannelRole.FrontCenter,
        "low_frequency" => ChannelRole.LowFrequency,
        "rear_left" => ChannelRole.RearLeft,
        "rear_right" => ChannelRole.RearRight,
        "side_left" => ChannelRole.SideLeft,
        "side_right" => ChannelRole.SideRight,
        _ => throw new ArgumentException($"Unknown channel role '{value}'.", nameof(value))
    };
}

