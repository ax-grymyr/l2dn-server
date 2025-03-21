using System.Collections.Immutable;
using System.Numerics;

namespace L2Dn.Collections;

public sealed class SmallDictionary<TKey, TValue>
    where TKey: IComparisonOperators<TKey, TKey, bool>
{
    private ImmutableArray<KeyValuePair<TKey, TValue>> _array = ImmutableArray<KeyValuePair<TKey, TValue>>.Empty;

    public TValue this[TKey key]
    {
        get
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            // TODO: binary search?
            foreach (KeyValuePair<TKey, TValue> pair in _array)
            {
                if (pair.Key == key)
                    return pair.Value;
            }

            return default!;
        }
        set
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            ImmutableArray<KeyValuePair<TKey, TValue>> array = _array;
            ImmutableArray<KeyValuePair<TKey, TValue>> newArray = UpdateArray(array, KeyValuePair.Create(key, value));
            while (ImmutableInterlocked.InterlockedCompareExchange(ref _array, newArray, array) != array)
            {
                array = _array;
                newArray = UpdateArray(array, new KeyValuePair<TKey, TValue>(key, value));
            }
        }
    }

    private static ImmutableArray<KeyValuePair<TKey, TValue>> UpdateArray(
        ImmutableArray<KeyValuePair<TKey, TValue>> array, KeyValuePair<TKey, TValue> pair)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Key == pair.Key)
                return array.SetItem(i, pair);

            if (array[i].Key > pair.Key)
                return array.Insert(i, pair);
        }

        return array.Add(pair);
    }
}