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

    /// <summary>
    /// Returns a new IEnumerable containing only the duplicated elements from the source sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">An IEnumerable to filter for duplicated elements.</param>
    /// <param name="comparer">An IEqualityComparer to compare elements.</param>
    /// <returns>An IEnumerable that contains the duplicated elements from the input sequence.</returns>
    public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(comparer);
        HashSet<T> seenItems = new HashSet<T>(comparer);
        HashSet<T> duplicatedItems = new HashSet<T>(comparer);
        foreach (T item in source)
        {
            if (!seenItems.Add(item))
                duplicatedItems.Add(item);
        }

        return duplicatedItems;
    }
}