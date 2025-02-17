using System.Runtime.CompilerServices;

namespace L2Dn;

public static class EnumExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int ToInt32<TEnum>(this TEnum value)
        where TEnum: unmanaged, Enum
    {
        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(sbyte))
            return ToIntPrivate<sbyte, TEnum>(value);

        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(byte))
            return ToIntPrivate<byte, TEnum>(value);

        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(short))
            return ToIntPrivate<short, TEnum>(value);

        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(ushort))
            return ToIntPrivate<ushort, TEnum>(value);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return ToIntPrivate<int, TEnum>(value);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return (int)ToIntPrivate<long, TEnum>(value);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static long ToInt64<TEnum>(this TEnum value)
        where TEnum: unmanaged, Enum
    {
        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(sbyte))
            return ToIntPrivate<sbyte, TEnum>(value);

        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(byte))
            return ToIntPrivate<byte, TEnum>(value);

        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(short))
            return ToIntPrivate<short, TEnum>(value);

        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(ushort))
            return ToIntPrivate<ushort, TEnum>(value);

        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(int))
            return ToIntPrivate<int, TEnum>(value);

        if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(uint))
            return ToIntPrivate<uint, TEnum>(value);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return ToIntPrivate<long, TEnum>(value);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum ToEnum<TEnum>(this int value)
        where TEnum: unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return ToEnumPrivate<byte, TEnum>((byte)value);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return ToEnumPrivate<short, TEnum>((short)value);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return ToEnumPrivate<int, TEnum>(value);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return ToEnumPrivate<long, TEnum>(value);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum ToEnum<TEnum>(this long value)
        where TEnum: unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return ToEnumPrivate<byte, TEnum>((byte)value);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return ToEnumPrivate<short, TEnum>((short)value);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return ToEnumPrivate<int, TEnum>((int)value);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return ToEnumPrivate<long, TEnum>(value);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static unsafe TInteger ToIntPrivate<TInteger, TEnum>(TEnum value)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged => *(TInteger*)&value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static unsafe TEnum ToEnumPrivate<TInteger, TEnum>(TInteger value)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged => *(TEnum*)&value;
}