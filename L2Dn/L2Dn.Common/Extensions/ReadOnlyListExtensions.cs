namespace L2Dn.Extensions;

public static class ReadOnlyListExtensions
{
    public static void ForEach<T>(this IReadOnlyList<T> collection, Action<T> action)
    {
        for (int index = 0; index < collection.Count; index++)
            action(collection[index]);
    }

    public static T GetRandomElement<T>(this IReadOnlyList<T> collection)
    {
        int index = Random.Shared.Next(collection.Count);
        return collection[index];
    }
}