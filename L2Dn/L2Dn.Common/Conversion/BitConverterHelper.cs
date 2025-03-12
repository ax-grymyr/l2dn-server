using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace L2Dn.Conversion;

internal static class BitConverterHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T ToValueNativeEndian<T>(ReadOnlySpan<byte> value)
        where T: unmanaged
    {
        if (value.Length < Unsafe.SizeOf<T>())
            throw new ArgumentException("Span is too short", nameof(value));

        return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteValueNativeEndian<T>(Span<byte> destination, T value)
        where T: unmanaged
    {
        if (destination.Length < Unsafe.SizeOf<T>())
            throw new ArgumentException("Span is too short", nameof(destination));

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryWriteValueNativeEndian<T>(Span<byte> destination, T value)
        where T: unmanaged
    {
        if (destination.Length < Unsafe.SizeOf<T>())
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T ToValueLittleEndian<T>(ReadOnlySpan<byte> value)
        where T: unmanaged
    {
        if (value.Length < Unsafe.SizeOf<T>())
            throw new ArgumentException("Span is too short", nameof(value));

        return BitConverter.IsLittleEndian
            ? Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(value))
            : ToValueReverseByteOrder<T>(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T ToValueBigEndian<T>(ReadOnlySpan<byte> value)
        where T: unmanaged
    {
        if (value.Length < Unsafe.SizeOf<T>())
            throw new ArgumentException("Span is too short", nameof(value));

        return !BitConverter.IsLittleEndian
            ? Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(value))
            : ToValueReverseByteOrder<T>(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteValueLittleEndian<T>(Span<byte> destination, T value)
        where T: unmanaged
    {
        if (destination.Length < Unsafe.SizeOf<T>())
            throw new ArgumentException("Span is too short", nameof(destination));

        if (BitConverter.IsLittleEndian)
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        else
            WriteValueReverseByteOrder(destination, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteValueBigEndian<T>(Span<byte> destination, T value)
        where T: unmanaged
    {
        if (destination.Length < Unsafe.SizeOf<T>())
            throw new ArgumentException("Span is too short", nameof(destination));

        if (!BitConverter.IsLittleEndian)
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        else
            WriteValueReverseByteOrder(destination, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryWriteValueLittleEndian<T>(Span<byte> destination, T value)
        where T: unmanaged
    {
        if (destination.Length < Unsafe.SizeOf<T>())
            return false;

        if (BitConverter.IsLittleEndian)
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        else
            WriteValueReverseByteOrder(destination, value);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryWriteValueBigEndian<T>(Span<byte> destination, T value)
        where T: unmanaged
    {
        if (destination.Length < Unsafe.SizeOf<T>())
            return false;

        if (!BitConverter.IsLittleEndian)
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        else
            WriteValueReverseByteOrder(destination, value);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T ToValueReverseByteOrder<T>(ReadOnlySpan<byte> value)
        where T: unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        T result = default;
        Span<byte> destination = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref result, 1));
        ref byte srcIt = ref MemoryMarshal.GetReference(value);
        ref byte endIt = ref Unsafe.Add(ref srcIt, size);
        ref byte dstIt = ref destination[size - 1];
        while (!Unsafe.AreSame(ref srcIt, ref endIt))
        {
            dstIt = srcIt;
            srcIt = ref Unsafe.Add(ref srcIt, 1);
            dstIt = ref Unsafe.Subtract(ref dstIt, 1);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteValueReverseByteOrder<T>(Span<byte> destination, T value)
        where T: unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        Span<byte> source = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));
        ref byte srcIt = ref source[0];
        ref byte endIt = ref Unsafe.Add(ref srcIt, size);
        ref byte dstIt = ref destination[size - 1];
        while (!Unsafe.AreSame(ref srcIt, ref endIt))
        {
            dstIt = srcIt;
            srcIt = ref Unsafe.Add(ref srcIt, 1);
            dstIt = ref Unsafe.Subtract(ref dstIt, 1);
        }
    }
}