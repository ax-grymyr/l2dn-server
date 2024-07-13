namespace L2Dn.Extensions;

public static class ReadOnlyListExtensions
{
    public static bool ContainsNull<T>(this IReadOnlyList<T?> collection)
        where T: class
    {
        for (int index = 0; index < collection.Count; index++)
        {
            if (collection[index] is null)
                return true;
        }

        return false;
    }

    public static void ForEach<T>(this IReadOnlyList<T> collection, Action<T> action)
    {
        for (int index = 0; index < collection.Count; index++)
            action(collection[index]);
    }

    public static T GetRandomElement<T>(this IReadOnlyList<T> collection)
        => collection[Random.Shared.Next(collection.Count)];

    public static T? GetRandomElementOrDefault<T>(this IReadOnlyList<T> collection)
        => collection.Count != 0 ? collection[Random.Shared.Next(collection.Count)] : default;
}