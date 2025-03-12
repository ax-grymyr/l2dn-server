namespace L2Dn.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T item in collection)
            action(item);
    }

    public static int GetSequenceHashCode<T>(this IEnumerable<T>? collection)
    {
        const int seedValue = 0x2D2816FE;
        return collection?.Aggregate(seedValue, HashCode.Combine) ?? seedValue;
    }

    public static int GetSetHashCode<T>(this IEnumerable<T>? collection)
    {
        const int seedValue = 0x2D2816FE;
        return collection?.Aggregate(seedValue, (current, item) => current ^ HashCode.Combine(item)) ?? seedValue;
    }

    public static int GetDictionaryHashCode<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>>? collection)
    {
        const int seedValue = 0x2D2816FE;
        return collection?.Aggregate(seedValue, (current, item) => current ^ HashCode.Combine(item.Key, item.Value)) ??
            seedValue;
    }
}