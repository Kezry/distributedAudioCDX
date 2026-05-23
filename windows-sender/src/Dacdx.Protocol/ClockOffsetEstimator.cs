namespace Dacdx.Protocol;

public readonly record struct ClockSample(long ClientSendUs, long ServerReceiveUs, long ServerSendUs, long ClientReceiveUs);

public static class ClockOffsetEstimator
{
    public static long EstimateOffsetUs(ClockSample sample)
    {
        var clientMidpoint = (sample.ClientSendUs + sample.ClientReceiveUs) / 2;
        var serverMidpoint = (sample.ServerReceiveUs + sample.ServerSendUs) / 2;
        return serverMidpoint - clientMidpoint;
    }

    public static long EstimateRoundTripUs(ClockSample sample)
    {
        return Math.Max(0, (sample.ClientReceiveUs - sample.ClientSendUs) - (sample.ServerSendUs - sample.ServerReceiveUs));
    }
}

