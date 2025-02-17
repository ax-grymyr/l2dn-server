using L2Dn.Utilities;

namespace L2Dn.Collections;

public struct EnumSet<TEnum, TStorage>
    where TEnum: unmanaged, Enum
    where TStorage: unmanaged, IFixedBitSet
{
    private TStorage _storage;

    public EnumSet()
    {
        if (_storage.Capacity < EnumUtil.GetMaxValue<TEnum>().ToInt64())
            throw new InvalidOperationException("Storage capacity is too small");
    }

    public int Count => _storage.Count;
    public void Clear() => _storage.Clear();
    public bool Contains(TEnum value) => _storage[value.ToInt32()];
    public void Add(TEnum value) => _storage.SetBit(value.ToInt32());
    public void Remove(TEnum value) => _storage.ClearBit(value.ToInt32());
}