using System.Collections;
using System.Collections.Concurrent;
using L2Dn.Extensions;

namespace L2Dn.GameServer.Utilities;

public sealed class Set<T>: ICollection<T>, IReadOnlyCollection<T>
    where T: notnull
{
    private readonly ConcurrentDictionary<T, bool> _dictionary;

    public Set()
    {
        _dictionary = new ConcurrentDictionary<T, bool>();
    }

    public Set(IEqualityComparer<T>? comparer)
    {
        _dictionary = new ConcurrentDictionary<T, bool>(comparer);
    }

    public bool isEmpty()
    {
        return _dictionary.IsEmpty;
    }

    public bool add(T value)
    {
        return _dictionary.TryAdd(value, false);
    }

    public void addAll(IEnumerable<T> items)
    {
        items.ForEach(x => add(x));
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _dictionary.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        _dictionary.TryAdd(item, false);
    }

    public void Clear()
    {
        _dictionary.Clear();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _dictionary.Keys.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _dictionary.TryRemove(item, out _);
    }

    bool ICollection<T>.IsReadOnly => false;

    public int Count => _dictionary.Count;

    public bool Contains(T item)
    {
        return _dictionary.ContainsKey(item);
    }

    public bool remove(T item)
    {
        return _dictionary.TryRemove(item, out _);
    }

    public void clear()
    {
        _dictionary.Clear();
    }

    public bool removeAll(IEnumerable<T> values)
    {
        bool any = false;
        foreach (T value in values)
            any |= remove(value);

        return any;
    }

    public bool removeIf(Predicate<T> predicate)
    {
        bool result = false;
        List<T> keys = _dictionary.Keys.Where(x => predicate(x)).ToList(); // TODO: optimize
        foreach (T key in keys)
            result |= remove(key);

        return result;
    }

    public bool containsAll(IEnumerable<T> values)
    {
        return values.All(x => _dictionary.ContainsKey(x));
    }

    public int size()
    {
        return _dictionary.Count;
    }
}