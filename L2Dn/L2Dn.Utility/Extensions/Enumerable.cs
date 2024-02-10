namespace L2Dn.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T value in collection)
            action(value);
    }
}