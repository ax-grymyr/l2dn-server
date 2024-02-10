using System.Runtime.CompilerServices;

namespace L2Dn.Conversion;

public readonly struct BigEndianBitConverter: IBitConverter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ToValue<T>(ReadOnlySpan<byte> value) where T: unmanaged =>
        BitConverterHelper.ToValueBigEndian<T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ToInt16(ReadOnlySpan<byte> value) => ToValue<short>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(ReadOnlySpan<byte> value) => ToValue<int>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToInt64(ReadOnlySpan<byte> value) => ToValue<long>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int128 ToInt128(ReadOnlySpan<byte> value) => ToValue<Int128>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ToUInt16(ReadOnlySpan<byte> value) => ToValue<ushort>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUInt32(ReadOnlySpan<byte> value) => ToValue<uint>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64(ReadOnlySpan<byte> value) => ToValue<ulong>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 ToUInt128(ReadOnlySpan<byte> value) => ToValue<UInt128>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteValue<T>(Span<byte> destination, T value)
        where T: unmanaged =>
        BitConverterHelper.WriteValueBigEndian(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt16(Span<byte> destination, short value) => WriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt32(Span<byte> destination, int value) => WriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt64(Span<byte> destination, long value) => WriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt128(Span<byte> destination, Int128 value) => WriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt16(Span<byte> destination, ushort value) => WriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt32(Span<byte> destination, uint value) => WriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt64(Span<byte> destination, ulong value) => WriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt128(Span<byte> destination, UInt128 value) => WriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteValue<T>(Span<byte> destination, T value)
        where T: unmanaged =>
        BitConverterHelper.TryWriteValueBigEndian(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteInt16(Span<byte> destination, short value) => TryWriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteInt32(Span<byte> destination, int value) => TryWriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteInt64(Span<byte> destination, long value) => TryWriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteInt128(Span<byte> destination, Int128 value) => TryWriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteUInt16(Span<byte> destination, ushort value) => TryWriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteUInt32(Span<byte> destination, uint value) => TryWriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteUInt64(Span<byte> destination, ulong value) => TryWriteValue(destination, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteUInt128(Span<byte> destination, UInt128 value) => TryWriteValue(destination, value);
}
