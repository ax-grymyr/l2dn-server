using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using L2Dn.Conversion;

namespace L2Dn.Packets;

public readonly ref struct PacketBitWriter
{
    private readonly byte[] _buffer;
    private readonly ref int _offset;

    public PacketBitWriter(byte[] buffer, ref int offset)
    {
        _buffer = buffer;
        _offset = ref offset;
    }
    
    public void Skip(int count)
    {
        if (count < 0 || count + _offset > _buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(count));
        
        _offset += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte value)
    {
        _buffer[_offset] = value;
        _offset++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBoolean(bool value) => WriteByte(value ? (byte)1 : (byte)0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValue<T>(T value)
        where T: unmanaged
    {
        LittleEndianBitConverter.WriteValue(_buffer.AsSpan(_offset), value);
        _offset += Unsafe.SizeOf<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16(short value) => WriteValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16(ushort value) => WriteValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32(int value) => WriteValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32(uint value) => WriteValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64(long value) => WriteValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt64(ulong value) => WriteValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDouble(double value) => WriteValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEnum<TEnum>(TEnum value)
        where TEnum: unmanaged, Enum => WriteValue(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        value.CopyTo(_buffer.AsSpan(_offset));
        _offset += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteZeros(int count)
    {
        _buffer.AsSpan(_offset, count).Clear();
        _offset += count;
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        // TODO: big endian architectures
        WriteBytes(MemoryMarshal.Cast<char, byte>(value));
        WriteInt16(0);
    }

    public void WriteSizedString(ReadOnlySpan<char> value)
    {
        WriteInt16((short)value.Length);

        // TODO: big endian architectures
        WriteBytes(MemoryMarshal.Cast<char, byte>(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePacketCode(byte code)
    {
        WriteByte(code);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePacketCode(byte code, ushort exCode)
    {
        WriteByte(code);
        WriteUInt16(exCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePacketCode(int code)
    {
        if (code is < 0 or > 0xFFFFFF)
            throw new ArgumentOutOfRangeException(nameof(code));
        
        if (code <= 0xFF)
            WriteByte((byte)code);
        else
        {
            WriteByte((byte)(code >> 16));
            WriteUInt16((ushort)code);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WritePacketCode<TPacketCode, TExPacketCode>(TPacketCode code, TExPacketCode exCode)
        where TPacketCode: unmanaged, Enum
        where TExPacketCode: unmanaged, Enum
    {
        WriteValue(code);
        WriteValue(exCode);
    }
}