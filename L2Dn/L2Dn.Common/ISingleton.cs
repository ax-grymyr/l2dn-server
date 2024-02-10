namespace L2Dn;

public interface ISingleton<out T>
    where T: class, ISingleton<T>
{
    static abstract T Instance { get; }
}