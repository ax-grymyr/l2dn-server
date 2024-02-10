namespace L2Dn.GameServer.Utilities;

public class AtomicBoolean
{
    private int _value;

    public void compareAndSet(bool expect, bool val)
    {
        Interlocked.CompareExchange(ref _value, val ? 1 : 0, expect ? 1 : 0);
    }

    public void set(bool b)
    {
        _value = b ? 1 : 0;
    }

    public bool get()
    {
        return _value != 0;
    }
}