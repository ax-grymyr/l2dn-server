using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;

namespace L2Dn.Utilities;

public static class EqualityUtil
{
    public static bool EqualsTo<T>(this T self, object? obj)
        where T: notnull
    {
        if (typeof(T).IsValueType)
            return obj is T;

        if (ReferenceEquals(self, obj))
            return true;

        return !ReferenceEquals(obj, null) && self.GetType() == obj.GetType();
    }

    public static bool EqualsTo<T, TValues>(this T self, object? obj, Func<T, TValues> values)
        where T: notnull
    {
        if (!typeof(T).IsValueType)
        {
            if (ReferenceEquals(self, obj))
                return true;

            if (ReferenceEquals(obj, null) || self.GetType() != obj.GetType())
                return false;
        }

        if (obj is not T other)
            return false;

        return EqualityComparer<TValues>.Default.Equals(values(self), values(other));
    }

    public static int GetSingletonHashCode<T>(this T self)
        where T: notnull
    {
        Type type = self.GetType();
        return (type.FullName ?? type.Name).GetHashCode();
    }

    public static SequentialComparable<T> GetSequentialComparable<T>(this IEnumerable<T>? collection) => new(collection);
    public static ImmutableArrayComparable<T> GetSequentialComparable<T>(this ImmutableArray<T> collection) => new(collection);
    public static FrozenSetComparable<T> GetSetComparable<T>(this FrozenSet<T> set) => new(set);

    public static DictionaryComparable<TKey, TValue> GetDictionaryComparable<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> set)
        where TKey: notnull =>
        new(set);

    public readonly struct SequentialComparable<T>(IEnumerable<T>? collection): IEquatable<SequentialComparable<T>>
    {
        private readonly IEnumerable<T>? _collection = collection;

        public static bool operator ==(SequentialComparable<T> left, SequentialComparable<T> right) =>
            left.Equals(right);

        public static bool operator !=(SequentialComparable<T> left, SequentialComparable<T> right) => !(left == right);

        public bool Equals(SequentialComparable<T> other)
        {
            if (_collection is null)
                return other._collection is null || !other._collection.Any();

            if (other._collection is null)
                return !_collection.Any();

            return _collection.SequenceEqual(other._collection);
        }

        public override bool Equals(object? obj) => obj is SequentialComparable<T> other && Equals(other);
        public override int GetHashCode() => _collection.GetSequenceHashCode();
    }

    public readonly struct ImmutableArrayComparable<T>(ImmutableArray<T> collection): IEquatable<ImmutableArrayComparable<T>>
    {
        private readonly ImmutableArray<T> _collection = collection;

        public static bool operator ==(ImmutableArrayComparable<T> left, ImmutableArrayComparable<T> right) =>
            left.Equals(right);

        public static bool operator !=(ImmutableArrayComparable<T> left, ImmutableArrayComparable<T> right) => !(left == right);

        public bool Equals(ImmutableArrayComparable<T> other)
        {
            if (_collection.IsDefaultOrEmpty)
                return other._collection.IsDefaultOrEmpty;

            if (other._collection.IsDefaultOrEmpty)
                return false;

            return _collection.SequenceEqual(other._collection);
        }

        public override bool Equals(object? obj) => obj is ImmutableArrayComparable<T> other && Equals(other);
        public override int GetHashCode() => _collection.GetSequenceHashCode();
    }

    public readonly struct FrozenSetComparable<T>(FrozenSet<T> set): IEquatable<FrozenSetComparable<T>>
    {
        private readonly FrozenSet<T>? _set = set;

        public static bool operator ==(FrozenSetComparable<T> left, FrozenSetComparable<T> right) => left.Equals(right);
        public static bool operator !=(FrozenSetComparable<T> left, FrozenSetComparable<T> right) => !(left == right);

        public bool Equals(FrozenSetComparable<T> other)
        {
            if (_set is null)
                return other._set is null || other._set.Count == 0;

            if (other._set is null)
                return _set.Count == 0;

            return _set.SetEquals(other._set);
        }

        public override bool Equals(object? obj) => obj is FrozenSetComparable<T> other && Equals(other);
        public override int GetHashCode() => _set.GetSetHashCode();
    }

    public readonly struct DictionaryComparable<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary)
        : IEquatable<DictionaryComparable<TKey, TValue>>
        where TKey: notnull
    {
        private readonly IReadOnlyDictionary<TKey, TValue>? _dictionary = dictionary;

        public static bool operator ==(DictionaryComparable<TKey, TValue> left,
            DictionaryComparable<TKey, TValue> right) =>
            left.Equals(right);

        public static bool operator !=(DictionaryComparable<TKey, TValue> left,
            DictionaryComparable<TKey, TValue> right) =>
            !(left == right);

        public bool Equals(DictionaryComparable<TKey, TValue> other)
        {
            if (_dictionary is null)
                return other._dictionary is null || other._dictionary.Count == 0;

            if (other._dictionary is null)
                return _dictionary.Count == 0;

            return _dictionary.DictionaryEqual(other._dictionary);
        }

        public override bool Equals(object? obj) => obj is DictionaryComparable<TKey, TValue> other && Equals(other);
        public override int GetHashCode() => _dictionary.GetDictionaryHashCode();
    }
}