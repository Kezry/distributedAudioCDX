namespace Dacdx.Protocol;

public static class TestToneGenerator
{
    public static short[] GenerateStereoSine(int sampleRate, double frequencyHz, int durationMs, double amplitude = 0.25)
    {
        var frames = sampleRate * durationMs / 1000;
        var pcm = new short[frames * 2];
        for (var frame = 0; frame < frames; frame++)
        {
            var sample = (short)(Math.Sin(2 * Math.PI * frequencyHz * frame / sampleRate) * short.MaxValue * amplitude);
            pcm[frame * 2] = sample;
            pcm[(frame * 2) + 1] = sample;
        }

        return pcm;
    }
}

