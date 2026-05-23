namespace Dacdx.Protocol;

public sealed class ChannelMatrix
{
    private readonly Dictionary<ChannelRole, int> _roleToIndex;

    public ChannelMatrix(IReadOnlyDictionary<ChannelRole, int> roleToIndex)
    {
        _roleToIndex = new Dictionary<ChannelRole, int>(roleToIndex);
    }

    public static ChannelMatrix Stereo { get; } = new(new Dictionary<ChannelRole, int>
    {
        [ChannelRole.FrontLeft] = 0,
        [ChannelRole.FrontRight] = 1
    });

    public static ChannelMatrix Surround51 { get; } = new(new Dictionary<ChannelRole, int>
    {
        [ChannelRole.FrontLeft] = 0,
        [ChannelRole.FrontRight] = 1,
        [ChannelRole.FrontCenter] = 2,
        [ChannelRole.LowFrequency] = 3,
        [ChannelRole.RearLeft] = 4,
        [ChannelRole.RearRight] = 5
    });

    public static ChannelMatrix Surround71 { get; } = new(new Dictionary<ChannelRole, int>
    {
        [ChannelRole.FrontLeft] = 0,
        [ChannelRole.FrontRight] = 1,
        [ChannelRole.FrontCenter] = 2,
        [ChannelRole.LowFrequency] = 3,
        [ChannelRole.RearLeft] = 4,
        [ChannelRole.RearRight] = 5,
        [ChannelRole.SideLeft] = 6,
        [ChannelRole.SideRight] = 7
    });

    public short[] ExtractPcm16Interleaved(ReadOnlySpan<short> interleaved, int inputChannels, ChannelRole role)
    {
        if (!_roleToIndex.TryGetValue(role, out var index))
        {
            return Array.Empty<short>();
        }

        if (inputChannels <= index)
        {
            throw new ArgumentException("Input channel count does not contain requested role.", nameof(inputChannels));
        }

        var frames = interleaved.Length / inputChannels;
        var output = new short[frames];
        for (var frame = 0; frame < frames; frame++)
        {
            output[frame] = interleaved[(frame * inputChannels) + index];
        }

        return output;
    }
}

