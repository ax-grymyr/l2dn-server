using System.Collections;
using L2Dn.Utilities;

namespace L2Dn.Collections;

public struct EnumSet64<TEnum>: IEnumerable<TEnum>, IEquatable<EnumSet64<TEnum>>
    where TEnum: unmanaged, Enum
{
    private const int _capacity = sizeof(ulong) * 8;
    private ulong _storage;

    public static EnumSet64<TEnum> Create()
    {
        if (_capacity < EnumUtil.GetMaxValue<TEnum>().ToInt64())
        {
            throw new InvalidOperationException(
                $"EnumSet64 capacity {_capacity} is too small for {typeof(TEnum).Name}");
        }

        return default;
    }

    public readonly int Count => (int)ulong.PopCount(_storage);
    public void Clear() => _storage = 0;
    public readonly bool Contains(TEnum value) => (_storage & (1uL << value.ToInt32())) != 0;
    public void Add(TEnum value) => Interlocked.Or(ref _storage, 1uL << value.ToInt32());

    public void AddRange(IEnumerable<TEnum> values)
    {
        foreach (TEnum value in values)
            Add(value);
    }

    public void AddRange(EnumSet64<TEnum> other) => _storage |= other._storage;

    public void Remove(TEnum value) => Interlocked.And(ref _storage, ~(1uL << value.ToInt32()));

    public bool this[TEnum item]
    {
        get => (_storage & (1uL << item.ToInt32())) != 0;
        set
        {
            ulong mask = 1uL << item.ToInt32();
            if (value)
                Interlocked.Or(ref _storage, mask);
            else
                Interlocked.And(ref _storage, ~mask);
        }
    }

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<TEnum> IEnumerable<TEnum>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(EnumSet64<TEnum> other) => _storage == other._storage;
    public override int GetHashCode() => _storage.GetHashCode();
    public override bool Equals(object? obj) => obj is EnumSet64<TEnum> other && Equals(other);

    public struct Enumerator(EnumSet64<TEnum> enumSet): IEnumerator<TEnum>
    {
        private readonly EnumSet64<TEnum> _enumSet = enumSet;
        private int _current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            while (_current < EnumUtil.GetMaxValue<TEnum>().ToInt32())
            {
                _current++;
                if (_enumSet.Contains(_current.ToEnum<TEnum>()))
                    return true;
            }

            return false;
        }

        public void Reset() => _current = -1;
        public TEnum Current => _current.ToEnum<TEnum>();
        object IEnumerator.Current => Current;
    }
}