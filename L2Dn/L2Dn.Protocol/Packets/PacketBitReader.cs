using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using L2Dn.Conversion;

namespace L2Dn.Packets;

public struct PacketBitReader
{
    private readonly byte[] _buffer;
    private int _offset;
    private int _length;

    public PacketBitReader(byte[] buffer, int offset, int length)
    {
        _buffer = buffer;
        _offset = offset;
        _length = length;
    }

    public int Length => _length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        ValidateLength(1);
        byte result = _buffer[_offset];
        Advance(1);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    {
        ValidateLength(1);
        bool result = _buffer[_offset] != 0;
        Advance(1);
        return result;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadValue<T>()
        where T: unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        ValidateLength(size);
        
        T result = LittleEndianBitConverter.ToValue<T>(_buffer.AsSpan(_offset, size));
        Advance(size);
        return result;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TEnum ReadEnum<TEnum>()
        where TEnum: unmanaged, Enum => ReadValue<TEnum>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16() => ReadValue<ushort>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16() => ReadValue<short>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32() => ReadValue<int>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32() => ReadValue<uint>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBytes(scoped Span<byte> value)
    {
        ValidateLength(value.Length);
        _buffer.AsSpan(_offset, value.Length).CopyTo(value);
        Advance(value.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        ValidateLength(length);
        ReadOnlySpan<byte> result = _buffer.AsSpan(_offset, length);
        Advance(length);
        return result;
    }

    public string ReadString()
    {
        // TODO: big endian architectures
        ReadOnlySpan<char> chars = MemoryMarshal.Cast<byte, char>(_buffer.AsSpan(_offset));
        int index = chars.IndexOf('\0');
        if (index >= 0)
        {
            chars = chars.Slice(0, index);
            Advance(2 + index * 2);
        }
        else
            Advance(_length);

        return new string(chars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Skip(int length)
    {
        ValidateLength(length);
        Advance(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateLength(int requiredLength)
    {
        if (_length < requiredLength)
            throw new InvalidOperationException("Reading beyond the end of the packet");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Advance(int size)
    {
        _offset += size;
        _length -= size;
    }
}
