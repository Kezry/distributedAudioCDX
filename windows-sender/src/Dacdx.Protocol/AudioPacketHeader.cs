using System.Buffers.Binary;

namespace Dacdx.Protocol;

public readonly record struct AudioPacketHeader(
    ushort Magic,
    ushort Version,
    uint StreamId,
    uint Sequence,
    uint SampleRate,
    uint ChannelLayout,
    uint FrameCount,
    long CaptureTimestampUs,
    long PlayAtTimestampUs)
{
    public const ushort CurrentMagic = 0xCDAC;
    public const ushort CurrentVersion = 1;
    public const int Size = 44;

    public static AudioPacketHeader Create(
        uint streamId,
        uint sequence,
        uint sampleRate,
        uint channelLayout,
        uint frameCount,
        long captureTimestampUs,
        long playAtTimestampUs)
    {
        return new AudioPacketHeader(
            CurrentMagic,
            CurrentVersion,
            streamId,
            sequence,
            sampleRate,
            channelLayout,
            frameCount,
            captureTimestampUs,
            playAtTimestampUs);
    }

    public void WriteTo(Span<byte> destination)
    {
        if (destination.Length < Size)
        {
            throw new ArgumentException("Destination buffer is too small.", nameof(destination));
        }

        BinaryPrimitives.WriteUInt16LittleEndian(destination[0..2], Magic);
        BinaryPrimitives.WriteUInt16LittleEndian(destination[2..4], Version);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[4..8], StreamId);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[8..12], Sequence);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[12..16], SampleRate);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[16..20], ChannelLayout);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[20..24], FrameCount);
        BinaryPrimitives.WriteInt64LittleEndian(destination[24..32], CaptureTimestampUs);
        BinaryPrimitives.WriteInt64LittleEndian(destination[32..40], PlayAtTimestampUs);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[40..44], 0);
    }

    public static AudioPacketHeader ReadFrom(ReadOnlySpan<byte> source)
    {
        if (source.Length < Size)
        {
            throw new ArgumentException("Source buffer is too small.", nameof(source));
        }

        var header = new AudioPacketHeader(
            BinaryPrimitives.ReadUInt16LittleEndian(source[0..2]),
            BinaryPrimitives.ReadUInt16LittleEndian(source[2..4]),
            BinaryPrimitives.ReadUInt32LittleEndian(source[4..8]),
            BinaryPrimitives.ReadUInt32LittleEndian(source[8..12]),
            BinaryPrimitives.ReadUInt32LittleEndian(source[12..16]),
            BinaryPrimitives.ReadUInt32LittleEndian(source[16..20]),
            BinaryPrimitives.ReadUInt32LittleEndian(source[20..24]),
            BinaryPrimitives.ReadInt64LittleEndian(source[24..32]),
            BinaryPrimitives.ReadInt64LittleEndian(source[32..40]));

        if (header.Magic != CurrentMagic)
        {
            throw new InvalidDataException($"Invalid audio packet magic 0x{header.Magic:X4}.");
        }

        if (header.Version != CurrentVersion)
        {
            throw new InvalidDataException($"Unsupported audio packet version {header.Version}.");
        }

        return header;
    }
}

