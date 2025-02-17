namespace L2Dn.Collections;

public struct FixedBitSet64: IFixedBitSet
{
    private ulong _bits;

    public int Capacity => sizeof(ulong) * 8;
    public int Count => (int)ulong.PopCount(_bits);

    public bool this[int index]
    {
        get => (_bits & (1UL << index)) != 0;
        set
        {
            if (value)
                SetBit(index);
            else
                ClearBit(index);
        }
    }

    public void SetBit(int index) => Interlocked.Or(ref _bits, 1UL << index);
    public void ClearBit(int index) => Interlocked.And(ref _bits, ~(1UL << index));
    public void Clear() => _bits = 0;
}