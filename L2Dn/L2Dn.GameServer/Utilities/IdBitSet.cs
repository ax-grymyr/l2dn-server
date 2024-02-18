using System.Numerics;

namespace L2Dn.GameServer.Utilities;

/// <summary>
/// BitSet of 8 * 1024 * 1024 bits size; allocates 1MB of memory. 
/// </summary>
public struct IdBitSet
{
    public const int BitCount = 8 * 1024 * 1024;
    private const int BitsInULong = 64;
    private const int LongWordCount = BitCount / BitsInULong;

    private ulong[]? _data;
    private int _setBitCount;

    public int SetBitCount => _setBitCount;

    public int? ReserveId()
    {
        // First, increment set bit count
        int setBitCount = Interlocked.Increment(ref _setBitCount); 
        if (setBitCount > BitCount)
        {
            // all bits set
            Interlocked.Decrement(ref _setBitCount);
            return null;
        }
            
        ulong[] bits = GetOrAllocateArray(ref _data);

        // Guess the position of free bits
        int index = _setBitCount >> 6;
        while (index < LongWordCount)
        {
            index += bits.AsSpan(index).IndexOfAnyExcept(ulong.MaxValue);
            int? bitIndex = ReserveAnyBit(ref bits[index]);
            if (bitIndex != null)
                return (index << 6) + bitIndex.Value;
        }

        // If not found, search from the start of array
        index = 0;
        while (index < LongWordCount)
        {
            index += bits.AsSpan(index).IndexOfAnyExcept(ulong.MaxValue);
            int? bitIndex = ReserveAnyBit(ref bits[index]);
            if (bitIndex != null)
                return (index << 6) + bitIndex.Value;
        }

        throw new InvalidOperationException("Invalid bitset state");
    }

    public void ReserveId(int id)
    {
        // First, increment set bit count
        int setBitCount = Interlocked.Increment(ref _setBitCount);
        if (setBitCount > BitCount)
        {
            // all bits set
            Interlocked.Decrement(ref _setBitCount);
            throw new InvalidOperationException($"Bit {id} was set");
        }
         
        ulong[] bits = GetOrAllocateArray(ref _data);
        ulong bitMask = 1uL << (id & 0x3F);
        ref ulong itemRef = ref bits[id >> 6];

        while (true) // returns or throws
        {
            ulong value = Interlocked.Read(ref itemRef);
            ulong newValue = value | bitMask;
            if (value == newValue)
            {
                Interlocked.Decrement(ref _setBitCount);
                throw new InvalidOperationException($"Bit {id} was set");
            }

            bool success = Interlocked.CompareExchange(ref itemRef, newValue, value) == value;
            if (success)
                return;

            // another thread changed the value
        }
    }

    public void ReleaseId(int id)
    {
        ulong[]? bits = _data;
        if (bits is not null)
        {
            ulong bitMask = 1uL << (id & 0x3F);
            ref ulong itemRef = ref bits[id >> 6];

            while (true)
            {
                ulong value = Interlocked.Read(ref itemRef);
                if ((value & bitMask) == 0)
                    break; // error, bit not set

                ulong newValue = value & ~bitMask;
                bool success = Interlocked.CompareExchange(ref itemRef, newValue, value) == value;
                if (success)
                {
                    // decrement set bit count
                    Interlocked.Decrement(ref _setBitCount);
                    return;
                }
                
                // another thread changed the value
            }
        }

        throw new InvalidOperationException($"Bit {id} is not set");
    }

    private static ulong[] GetOrAllocateArray(ref ulong[]? data)
    {
        ulong[]? bits = data;
        if (bits is null)
        {
            ulong[] newArray = new ulong[LongWordCount];

            // If result of CompareExchange is null, this thread won the race.
            bits = Interlocked.CompareExchange(ref data, newArray, null) ?? newArray;
        }

        return bits;
    }
    
    private static int? ReserveAnyBit(ref ulong itemRef)
    {
        ulong value = Interlocked.Read(ref itemRef);
        while (value != ulong.MaxValue)
        {
            int zeroBitIndex = 0x3F - BitOperations.LeadingZeroCount(~value);

            // set bit
            ulong newValue = value | (1uL << zeroBitIndex);

            // put to array
            bool success = Interlocked.CompareExchange(ref itemRef, newValue, value) == value;
            if (success)
                return zeroBitIndex;
                
            // another thread changed the value
            value = Interlocked.Read(ref itemRef);
        }

        return null;
    }
}