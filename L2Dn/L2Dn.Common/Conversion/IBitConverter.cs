namespace L2Dn.Conversion;

public interface IBitConverter
{
    static abstract T ToValue<T>(ReadOnlySpan<byte> value) 
        where T: unmanaged;

    static abstract short ToInt16(ReadOnlySpan<byte> value);
    static abstract int ToInt32(ReadOnlySpan<byte> value);
    static abstract long ToInt64(ReadOnlySpan<byte> value);
    static abstract Int128 ToInt128(ReadOnlySpan<byte> value);
    static abstract ushort ToUInt16(ReadOnlySpan<byte> value);
    static abstract uint ToUInt32(ReadOnlySpan<byte> value);
    static abstract ulong ToUInt64(ReadOnlySpan<byte> value);
    static abstract UInt128 ToUInt128(ReadOnlySpan<byte> value);

    static abstract void WriteValue<T>(Span<byte> destination, T value) 
        where T: unmanaged;

    static abstract void WriteInt16(Span<byte> destination, short value);
    static abstract void WriteInt32(Span<byte> destination, int value);
    static abstract void WriteInt64(Span<byte> destination, long value);
    static abstract void WriteInt128(Span<byte> destination, Int128 value);
    static abstract void WriteUInt16(Span<byte> destination, ushort value);
    static abstract void WriteUInt32(Span<byte> destination, uint value);
    static abstract void WriteUInt64(Span<byte> destination, ulong value);
    static abstract void WriteUInt128(Span<byte> destination, UInt128 value);

    static abstract bool TryWriteValue<T>(Span<byte> destination, T value) 
        where T: unmanaged;
    
    static abstract bool TryWriteInt16(Span<byte> destination, short value);
    static abstract bool TryWriteInt32(Span<byte> destination, int value);
    static abstract bool TryWriteInt64(Span<byte> destination, long value);
    static abstract bool TryWriteInt128(Span<byte> destination, Int128 value);
    static abstract bool TryWriteUInt16(Span<byte> destination, ushort value);
    static abstract bool TryWriteUInt32(Span<byte> destination, uint value);
    static abstract bool TryWriteUInt64(Span<byte> destination, ulong value);
    static abstract bool TryWriteUInt128(Span<byte> destination, UInt128 value);
}
