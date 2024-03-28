namespace L2Dn.GameServer.Utilities;

public sealed class AtomicInteger
{
    private int _value;

    public AtomicInteger(int value = 0)
    {
        _value = value;
    }

    public int get()
    {
        return _value;
    }

    public int incrementAndGet()
    {
        return Interlocked.Increment(ref _value);
    }

    public int decrementAndGet()
    {
        return Interlocked.Decrement(ref _value);
    }

    public void set(int value)
    {
        _value = value;
    }

    public int getAndIncrement()
    {
        return Interlocked.Increment(ref _value) - 1;
    }

    public int addAndGet(int val)
    {
        return Interlocked.Add(ref _value, val);
    }
}