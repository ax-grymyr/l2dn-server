using System.Numerics;
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
    public static TEnum BitwiseComplement<TEnum>(this TEnum value)
        where TEnum: unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return BitwiseComplementPrivate<byte, TEnum>(value);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return BitwiseComplementPrivate<ushort, TEnum>(value);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return BitwiseComplementPrivate<uint, TEnum>(value);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return BitwiseComplementPrivate<ulong, TEnum>(value);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum BitwiseOr<TEnum>(TEnum left, TEnum right)
        where TEnum: unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return BitwiseOrPrivate<byte, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return BitwiseOrPrivate<ushort, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return BitwiseOrPrivate<uint, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return BitwiseOrPrivate<ulong, TEnum>(left, right);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum BitwiseAnd<TEnum>(this TEnum left, TEnum right)
        where TEnum: unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return BitwiseAndPrivate<byte, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return BitwiseAndPrivate<ushort, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return BitwiseAndPrivate<uint, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return BitwiseAndPrivate<ulong, TEnum>(left, right);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum BitwiseXor<TEnum>(TEnum left, TEnum right)
        where TEnum: unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return BitwiseXorPrivate<byte, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return BitwiseXorPrivate<ushort, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return BitwiseXorPrivate<uint, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return BitwiseXorPrivate<ulong, TEnum>(left, right);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool Equal<TEnum>(this TEnum left, TEnum right)
        where TEnum: unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return EqualPrivate<byte, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return EqualPrivate<ushort, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return EqualPrivate<uint, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return EqualPrivate<ulong, TEnum>(left, right);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int Compare<TEnum>(this TEnum left, TEnum right)
        where TEnum: unmanaged, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return ComparePrivate<byte, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return ComparePrivate<ushort, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return ComparePrivate<uint, TEnum>(left, right);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return ComparePrivate<ulong, TEnum>(left, right);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool EqualPrivate<TInteger, TEnum>(TEnum value1, TEnum value2)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged, IComparisonOperators<TInteger, TInteger, bool> =>
        ToIntPrivate<TInteger, TEnum>(value1) == ToIntPrivate<TInteger, TEnum>(value2);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int ComparePrivate<TInteger, TEnum>(TEnum value1, TEnum value2)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged, IComparisonOperators<TInteger, TInteger, bool> =>
        ToIntPrivate<TInteger, TEnum>(value1).CompareFast(ToIntPrivate<TInteger, TEnum>(value2));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static TEnum BitwiseAndPrivate<TInteger, TEnum>(TEnum value1, TEnum value2)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger> =>
        ToEnumPrivate<TInteger, TEnum>(ToIntPrivate<TInteger, TEnum>(value1) & ToIntPrivate<TInteger, TEnum>(value2));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static TEnum BitwiseOrPrivate<TInteger, TEnum>(TEnum value1, TEnum value2)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger> =>
        ToEnumPrivate<TInteger, TEnum>(ToIntPrivate<TInteger, TEnum>(value1) | ToIntPrivate<TInteger, TEnum>(value2));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static TEnum BitwiseXorPrivate<TInteger, TEnum>(TEnum value1, TEnum value2)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger> =>
        ToEnumPrivate<TInteger, TEnum>(ToIntPrivate<TInteger, TEnum>(value1) ^ ToIntPrivate<TInteger, TEnum>(value2));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static TEnum BitwiseComplementPrivate<TInteger, TEnum>(TEnum value)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged, IBitwiseOperators<TInteger, TInteger, TInteger> =>
        ToEnumPrivate<TInteger, TEnum>(~ToIntPrivate<TInteger, TEnum>(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static unsafe TInteger ToIntPrivate<TInteger, TEnum>(TEnum value)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged => *(TInteger*)&value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static unsafe TEnum ToEnumPrivate<TInteger, TEnum>(TInteger value)
        where TEnum: unmanaged, Enum
        where TInteger: unmanaged => *(TEnum*)&value;
}