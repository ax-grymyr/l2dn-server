using System.Collections.Immutable;

namespace L2Dn.Extensions;

public static class ImmutableArrayExtensions
{
    public static void ForEach<T>(this ImmutableArray<T> collection, Action<T> action)
    {
        foreach (T item in collection)
            action(item);
    }

    public static int GetSequenceHashCode<T>(this ImmutableArray<T> collection)
    {
        const int seedValue = 0x2D2816FE;
        return collection.IsDefaultOrEmpty ? seedValue : collection.Aggregate(seedValue, HashCode.Combine);
    }

    public static T GetRandomElement<T>(this ImmutableArray<T> collection)
        => collection[Random.Shared.Next(collection.Length)];

    public static T? GetRandomElementOrDefault<T>(this ImmutableArray<T> collection)
        => collection.IsDefaultOrEmpty ? default : collection[Random.Shared.Next(collection.Length)];
}