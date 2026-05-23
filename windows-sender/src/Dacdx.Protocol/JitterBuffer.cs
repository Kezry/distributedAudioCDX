namespace Dacdx.Protocol;

public sealed class JitterBuffer<T>
{
    private readonly SortedDictionary<uint, T> _packets = new();
    private uint? _nextSequence;

    public int Count => _packets.Count;

    public void Push(uint sequence, T packet)
    {
        _packets[sequence] = packet;
        if (_nextSequence is null || sequence < _nextSequence.Value)
        {
            _nextSequence = sequence;
        }
    }

    public bool TryPop(out T? packet)
    {
        packet = default;
        if (_nextSequence is null)
        {
            return false;
        }

        if (_packets.Remove(_nextSequence.Value, out var exact))
        {
            packet = exact;
            _nextSequence++;
            return true;
        }

        if (_packets.Count > 8)
        {
            var first = _packets.First();
            _packets.Remove(first.Key);
            _nextSequence = first.Key + 1;
            packet = first.Value;
            return true;
        }

        return false;
    }
}
