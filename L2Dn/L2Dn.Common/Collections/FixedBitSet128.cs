using System.Runtime.CompilerServices;

namespace L2Dn.Collections;

public struct FixedBitSet128: IFixedBitSet
{
    private const int _size = 2;
    private const int _capacity = sizeof(ulong) * 8 * _size;

    [InlineArray(_size)]
    private struct Bits
    {
        public ulong Items;
    }

    private Bits _bits;

    public static int Capacity => _capacity;

    public int Count => (int)(ulong.PopCount(_bits[0]) + ulong.PopCount(_bits[1]));

    public bool this[int index]
    {
        get => (_bits[index / (sizeof(ulong) * 8)] & (1UL << index)) != 0;
        set
        {
            ref ulong bits = ref _bits[index / (sizeof(ulong) * 8)];
            ulong mask = 1UL << index;
            if (value)
                Interlocked.Or(ref bits, mask);
            else
                Interlocked.And(ref bits, ~mask);
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
        _bits = default;
    }
}