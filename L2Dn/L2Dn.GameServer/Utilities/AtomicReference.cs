namespace L2Dn.GameServer.Utilities;

public class AtomicReference<T>
    where T: class
{
    private T? _value;
    
    public AtomicReference(T? value = null)
    {
        _value = value;
    }

    public T? Value => _value;

    public void set(T? value)
    {
        Interlocked.Exchange(ref _value, value);
    }

    public T? get()
    {
        return _value;
    }

    public T? getAndSet(T? value)
    {
        return Interlocked.Exchange(ref _value, value);
    }
}