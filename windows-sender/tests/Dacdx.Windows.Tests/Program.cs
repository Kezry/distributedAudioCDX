using Dacdx.Protocol;

Run("JSON announcement round trip", JsonAnnouncementRoundTrip);
Run("Audio packet header round trip", AudioHeaderRoundTrip);
Run("Channel matrix extracts stereo roles", ChannelMatrixExtractsStereoRoles);
Run("Clock offset estimate", ClockOffsetEstimate);
Run("Jitter buffer reorders packets", JitterBufferReordersPackets);
Console.WriteLine("All protocol tests passed.");

static void Run(string name, Action test)
{
    try
    {
        test();
        Console.WriteLine("[PASS] " + name);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("[FAIL] " + name + ": " + ex.Message);
        Environment.ExitCode = 1;
        throw;
    }
}

static void JsonAnnouncementRoundTrip()
{
    var input = new DeviceAnnouncement(
        DeviceAnnouncement.MessageType,
        1,
        "A4F9-21C8-77B2",
        "Desk Left",
        "192.168.1.42",
        39000,
        39001,
        ["sound_card", "dlna"],
        "sound_card",
        [44100, 48000],
        ["pcm_s16le"],
        ["front_left", "front_right"],
        "0.1.0");

    var json = DacdxJson.Serialize(input);
    var output = DacdxJson.Deserialize<DeviceAnnouncement>(json);
    Assert(output.DeviceId == input.DeviceId, "Device id mismatch.");
    Assert(output.SampleRates.Length == 2, "Sample rates missing.");
}

static void AudioHeaderRoundTrip()
{
    var header = AudioPacketHeader.Create(7, 42, 48000, 2, 480, 1000, 61000);
    Span<byte> bytes = stackalloc byte[AudioPacketHeader.Size];
    header.WriteTo(bytes);
    var parsed = AudioPacketHeader.ReadFrom(bytes);
    Assert(parsed.Sequence == 42, "Sequence mismatch.");
    Assert(parsed.FrameCount == 480, "Frame count mismatch.");
    Assert(parsed.PlayAtTimestampUs == 61000, "Play timestamp mismatch.");
}

static void ChannelMatrixExtractsStereoRoles()
{
    short[] interleaved = [1, 2, 3, 4, 5, 6];
    var left = ChannelMatrix.Stereo.ExtractPcm16Interleaved(interleaved, 2, ChannelRole.FrontLeft);
    var right = ChannelMatrix.Stereo.ExtractPcm16Interleaved(interleaved, 2, ChannelRole.FrontRight);
    Assert(left.Length == 3 && left[0] == 1 && left[1] == 3 && left[2] == 5, "Left channel extraction failed.");
    Assert(right.Length == 3 && right[0] == 2 && right[1] == 4 && right[2] == 6, "Right channel extraction failed.");
}

static void ClockOffsetEstimate()
{
    var sample = new ClockSample(100, 140, 160, 210);
    Assert(ClockOffsetEstimator.EstimateRoundTripUs(sample) == 90, "RTT mismatch.");
    Assert(ClockOffsetEstimator.EstimateOffsetUs(sample) == -5, "Offset mismatch.");
}

static void JitterBufferReordersPackets()
{
    var buffer = new JitterBuffer<string>();
    buffer.Push(2, "two");
    buffer.Push(1, "one");
    Assert(buffer.TryPop(out var first) && first == "one", "Buffer did not pop the lowest available sequence.");
    Assert(buffer.TryPop(out var second) && second == "two", "Buffer did not pop the next sequence.");
    buffer.Push(3, "three");
    Assert(buffer.TryPop(out var third) && third == "three", "Buffer did not continue next sequence.");
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
