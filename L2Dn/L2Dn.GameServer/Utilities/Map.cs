using System.Collections.Concurrent;

namespace L2Dn.GameServer.Utilities;

public class Map<TKey, TValue>: ConcurrentDictionary<TKey, TValue> 
    where TKey: notnull
{
    public Map()
    {
    }

    public Map(IEqualityComparer<TKey> comparer): base(comparer)
    {
    }

    public bool contains(TKey key)
    {
        return ContainsKey(key);
    }
    
    public ICollection<TValue> values()
    {
        return Values;
    }
    
    public TValue? get(TKey key)
    {
        TryGetValue(key, out var value);
        return value;
    }

    public bool isEmpty()
    {
        return Count == 0;
    }

    public TValue? remove(TKey key)
    {
        TryRemove(key, out TValue? value);
        return value;
    }

    public TValue put(TKey key, TValue value)
    {
        TryGetValue(key, out var oldValue);
        this[key] = value;
        return oldValue;
    }

    public void putAll(Map<TKey, TValue> set)
    {
        foreach (KeyValuePair<TKey,TValue> pair in set)
        {
            this[pair.Key] = pair.Value;
        }
    }

    public bool containsKey(TKey key)
    {
        return ContainsKey(key);
    }

    public void clear()
    {
        Clear();
    }

    public int size()
    {
        return Count;
    }

    public bool containsValue(TValue value)
    {
        return Values.Contains(value);
    }

    public string toString()
    {
        // TODO:
        return "";
    }

    public TValue computeIfAbsent(TKey key, Func<TKey, TValue> factory)
    {
        return GetOrAdd(key, factory);
    }

    public TValue getOrDefault(TKey key, TValue defaultValue)
    {
        if (TryGetValue(key, out var value))
            return value;
        return defaultValue;
    }

    public bool putIfAbsent(TKey key, TValue value)
    {
        return TryAdd(key, value);
    }

    public TResult computeIfPresent<TResult>(TKey key, Func<TKey, TValue, TResult> func)
    {
        if (TryGetValue(key, out var value))
            return func(key, value);

        return default;
    }

    public void compute(TKey key, Func<TKey, TValue, TValue> func)
    {
        if (TryGetValue(key, out TValue? value))
            this[key] = func(key, value);
    }

    public TValue merge(TKey key, TValue newValue, Func<TValue, TValue, TValue> func)
    {
        return AddOrUpdate(key, newValue, (_, value) => func(newValue, value));
    }
}