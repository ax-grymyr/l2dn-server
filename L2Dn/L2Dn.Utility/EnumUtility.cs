using System.Runtime.CompilerServices;

namespace L2Dn;

public static class EnumUtility
{
    public static TEnum GetMaxValue<TEnum>()
        where TEnum: struct, Enum
    {
        TEnum[] values = Enum.GetValues<TEnum>();
        return values.Length == 0 ? default : values.Max();
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
            return BitUtility.Compare(Unsafe.As<TEnum, byte>(ref left), Unsafe.As<TEnum, byte>(ref right));

        if (Unsafe.SizeOf<TEnum>() == 2)
            return BitUtility.Compare(Unsafe.As<TEnum, ushort>(ref left), Unsafe.As<TEnum, ushort>(ref right));

        if (Unsafe.SizeOf<TEnum>() == 4)
            return BitUtility.Compare(Unsafe.As<TEnum, uint>(ref left), Unsafe.As<TEnum, uint>(ref right));

        if (Unsafe.SizeOf<TEnum>() == 8)
            return BitUtility.Compare(Unsafe.As<TEnum, ulong>(ref left), Unsafe.As<TEnum, ulong>(ref right));

        throw new InvalidOperationException("Unsupported enum size");
    }
}
