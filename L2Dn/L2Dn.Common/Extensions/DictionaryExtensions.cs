namespace L2Dn.Extensions;

public static class DictionaryExtensions
{
    public static T[] ToValueArray<T>(this IDictionary<int, T> dictionary)
    {
        if (dictionary.Count == 0)
            return [];

        int length = dictionary.Keys.Max() + 1;
        T[] array = new T[length];
        foreach (KeyValuePair<int, T> pair in dictionary)
            array[pair.Key] = pair.Value;

        return array;
    }

    public static bool IsEmpty<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary)
        => dictionary.Count == 0;
}