namespace L2Dn.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T item in collection)
            action(item);
    }

    public static void ForEach<T>(this IReadOnlyList<T> collection, Action<T> action)
    {
        for (var index = 0; index < collection.Count; index++)
            action(collection[index]);
    }
}