namespace L2Dn.Collections;

public struct FixedBitSet64: IFixedBitSet
{
    private const int _capacity = sizeof(ulong) * 8;
    private ulong _bits;

    public static int Capacity => _capacity;
    public int Count => (int)ulong.PopCount(_bits);

    public bool this[int index]
    {
        get => (_bits & (1UL << index)) != 0;
        set
        {
            ulong mask = 1UL << index;
            if (value)
                Interlocked.Or(ref _bits, mask);
            else
                Interlocked.And(ref _bits, ~mask);
        }
    }

    public void SetBit(int index) => Interlocked.Or(ref _bits, 1UL << index);
    public void ClearBit(int index) => Interlocked.And(ref _bits, ~(1UL << index));
    public void Clear() => _bits = 0;
}