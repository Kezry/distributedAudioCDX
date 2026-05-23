namespace Dacdx.Protocol;

public enum LatencyProfile
{
    Fast,
    Balanced,
    Stable
}

public static class LatencyProfiles
{
    public static int TargetBufferMs(LatencyProfile profile) => profile switch
    {
        LatencyProfile.Fast => 30,
        LatencyProfile.Balanced => 60,
        LatencyProfile.Stable => 140,
        _ => 60
    };

    public static string ToWireName(LatencyProfile profile) => profile switch
    {
        LatencyProfile.Fast => "fast",
        LatencyProfile.Balanced => "balanced",
        LatencyProfile.Stable => "stable",
        _ => "balanced"
    };

    public static LatencyProfile Parse(string value) => value switch
    {
        "fast" => LatencyProfile.Fast,
        "balanced" => LatencyProfile.Balanced,
        "stable" => LatencyProfile.Stable,
        _ => LatencyProfile.Balanced
    };
}

