using System.Collections.Immutable;

namespace L2Dn.Extensions;

public static class ImmutableArrayExtensions
{
    public static void ForEach<T>(this ImmutableArray<T> collection, Action<T> action)
    {
        foreach (T item in collection)
            action(item);
    }
}