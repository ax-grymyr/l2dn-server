using System.Runtime.CompilerServices;

namespace L2Dn.Collections;

public struct FixedBitSet128: IFixedBitSet
{
    private const int Size = 2;

    [InlineArray(Size)]
    private struct Bits
    {
        public ulong Items;
    }

    private Bits _bits;

    public int Capacity => sizeof(ulong) * Size * 8;

    public int Count => (int)(ulong.PopCount(_bits[0]) + ulong.PopCount(_bits[1]));

    public bool this[int index]
    {
        get => (_bits[index / (sizeof(ulong) * 8)] & (1UL << index)) != 0;
        set
        {
            if (value)
                SetBit(index);
            else
                ClearBit(index);
        }
    }

    public void SetBit(int index)
    {
        ref ulong bits = ref _bits[index / (sizeof(ulong) * 8)];
        Interlocked.Or(ref bits, 1UL << index);
    }

    public void ClearBit(int index)
    {
        ref ulong bits = ref _bits[index / (sizeof(ulong) * 8)];
        Interlocked.And(ref bits, ~(1UL << index));
    }

    public void Clear()
    {
        _bits[0] = 0;
        _bits[1] = 0;
    }
}