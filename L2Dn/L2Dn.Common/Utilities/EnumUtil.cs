using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace L2Dn.Utilities;

public static class EnumUtil
{
    public static ImmutableArray<TEnum> GetValues<TEnum>()
        where TEnum: struct, Enum => EnumInfo<TEnum>.Values;

    public static TEnum GetMaxValue<TEnum>()
        where TEnum: struct, Enum => EnumInfo<TEnum>.MaxValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int ToInt32<TEnum>(this TEnum value)
        where TEnum: struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return Unsafe.As<TEnum, byte>(ref value); 

        if (Unsafe.SizeOf<TEnum>() == 2)
            return Unsafe.As<TEnum, short>(ref value); 

        if (Unsafe.SizeOf<TEnum>() == 4)
            return Unsafe.As<TEnum, int>(ref value); 

        if (Unsafe.SizeOf<TEnum>() == 8)
            return (int)Unsafe.As<TEnum, long>(ref value); 

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static long ToInt64<TEnum>(this TEnum value)
        where TEnum: struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return Unsafe.As<TEnum, byte>(ref value); 

        if (Unsafe.SizeOf<TEnum>() == 2)
            return Unsafe.As<TEnum, short>(ref value); 

        if (Unsafe.SizeOf<TEnum>() == 4)
            return Unsafe.As<TEnum, int>(ref value); 

        if (Unsafe.SizeOf<TEnum>() == 8)
            return Unsafe.As<TEnum, long>(ref value); 

        throw new InvalidOperationException("Unsupported enum size");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum BitwiseComplement<TEnum>(TEnum value)
        where TEnum: struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
        {
            byte v = (byte)~Unsafe.As<TEnum, byte>(ref value);
            return Unsafe.As<byte, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 2)
        {
            ushort v = (ushort)~Unsafe.As<TEnum, ushort>(ref value);
            return Unsafe.As<ushort, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 4)
        {
            uint v = ~Unsafe.As<TEnum, uint>(ref value);
            return Unsafe.As<uint, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 8)
        {
            ulong v = ~Unsafe.As<TEnum, ulong>(ref value);
            return Unsafe.As<ulong, TEnum>(ref v);
        }

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum BitwiseOr<TEnum>(TEnum left, TEnum right)
        where TEnum: struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
        {
            byte v = (byte)(Unsafe.As<TEnum, byte>(ref left) | Unsafe.As<TEnum, byte>(ref right));
            return Unsafe.As<byte, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 2)
        {
            ushort v = (ushort)(Unsafe.As<TEnum, ushort>(ref left) | Unsafe.As<TEnum, ushort>(ref right));
            return Unsafe.As<ushort, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 4)
        {
            uint v = Unsafe.As<TEnum, uint>(ref left) | Unsafe.As<TEnum, uint>(ref right);
            return Unsafe.As<uint, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 8)
        {
            ulong v = Unsafe.As<TEnum, ulong>(ref left) | Unsafe.As<TEnum, ulong>(ref right);
            return Unsafe.As<ulong, TEnum>(ref v);
        }

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum BitwiseAnd<TEnum>(TEnum left, TEnum right)
        where TEnum: struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
        {
            byte v = (byte)(Unsafe.As<TEnum, byte>(ref left) & Unsafe.As<TEnum, byte>(ref right));
            return Unsafe.As<byte, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 2)
        {
            ushort v = (ushort)(Unsafe.As<TEnum, ushort>(ref left) & Unsafe.As<TEnum, ushort>(ref right));
            return Unsafe.As<ushort, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 4)
        {
            uint v = Unsafe.As<TEnum, uint>(ref left) & Unsafe.As<TEnum, uint>(ref right);
            return Unsafe.As<uint, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 8)
        {
            ulong v = Unsafe.As<TEnum, ulong>(ref left) & Unsafe.As<TEnum, ulong>(ref right);
            return Unsafe.As<ulong, TEnum>(ref v);
        }

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static TEnum BitwiseXor<TEnum>(TEnum left, TEnum right)
        where TEnum: struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
        {
            byte v = (byte)(Unsafe.As<TEnum, byte>(ref left) ^ Unsafe.As<TEnum, byte>(ref right));
            return Unsafe.As<byte, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 2)
        {
            ushort v = (ushort)(Unsafe.As<TEnum, ushort>(ref left) ^ Unsafe.As<TEnum, ushort>(ref right));
            return Unsafe.As<ushort, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 4)
        {
            uint v = Unsafe.As<TEnum, uint>(ref left) ^ Unsafe.As<TEnum, uint>(ref right);
            return Unsafe.As<uint, TEnum>(ref v);
        }

        if (Unsafe.SizeOf<TEnum>() == 8)
        {
            ulong v = Unsafe.As<TEnum, ulong>(ref left) ^ Unsafe.As<TEnum, ulong>(ref right);
            return Unsafe.As<ulong, TEnum>(ref v);
        }

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool Equal<TEnum>(TEnum left, TEnum right)
        where TEnum: struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return Unsafe.As<TEnum, byte>(ref left) == Unsafe.As<TEnum, byte>(ref right);

        if (Unsafe.SizeOf<TEnum>() == 2)
            return Unsafe.As<TEnum, ushort>(ref left) == Unsafe.As<TEnum, ushort>(ref right);

        if (Unsafe.SizeOf<TEnum>() == 4)
            return Unsafe.As<TEnum, uint>(ref left) == Unsafe.As<TEnum, uint>(ref right);

        if (Unsafe.SizeOf<TEnum>() == 8)
            return Unsafe.As<TEnum, ulong>(ref left) == Unsafe.As<TEnum, ulong>(ref right);

        throw new InvalidOperationException("Unsupported enum size");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int Compare<TEnum>(TEnum left, TEnum right)
        where TEnum: struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return BitUtil.Compare(Unsafe.As<TEnum, byte>(ref left), Unsafe.As<TEnum, byte>(ref right));

        if (Unsafe.SizeOf<TEnum>() == 2)
            return BitUtil.Compare(Unsafe.As<TEnum, ushort>(ref left), Unsafe.As<TEnum, ushort>(ref right));

        if (Unsafe.SizeOf<TEnum>() == 4)
            return BitUtil.Compare(Unsafe.As<TEnum, uint>(ref left), Unsafe.As<TEnum, uint>(ref right));

        if (Unsafe.SizeOf<TEnum>() == 8)
            return BitUtil.Compare(Unsafe.As<TEnum, ulong>(ref left), Unsafe.As<TEnum, ulong>(ref right));

        throw new InvalidOperationException("Unsupported enum size");
    }

    private static class EnumInfo<TEnum>
        where TEnum: struct, Enum
    {
        public static ImmutableArray<TEnum> Values { get; }
        public static TEnum MaxValue { get; }

        static EnumInfo()
        {
            Values = Enum.GetValues<TEnum>().ToImmutableArray();
            MaxValue = Values.Length > 0 ? Values.Max() : default;
        }
    }
}