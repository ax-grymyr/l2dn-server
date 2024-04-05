using System.Runtime.CompilerServices;
using L2Dn.Conversion;

namespace L2Dn.IO;

public class BinaryWriter<TBitConverter>(Stream stream)
    where TBitConverter: struct, IBitConverter
{
    public void WriteByte(byte value) => stream.WriteByte(value);
    public void WriteSByte(sbyte value) => stream.WriteByte((byte)value);

    public void WriteValue<T>(T value)
        where T: unmanaged
    {
        Span<byte> span = stackalloc byte[Unsafe.SizeOf<T>()];
        TBitConverter.WriteValue(span, value);
        stream.Write(span);
    }

    public void WriteInt16(short value) => WriteValue(value);
    public void WriteUInt16(ushort value) => WriteValue(value);

    public void WriteInt32(int value) => WriteValue(value);
    public void WriteUInt32(uint value) => WriteValue(value);

    public void WriteInt64(long value) => WriteValue(value);
    public void WriteUInt64(ulong value) => WriteValue(value);

    public void WriteFloat(float value) => WriteValue(value);
    public void WriteDouble(double value) => WriteValue(value);

    public void WriteGuid(Guid value) => WriteValue(value);

    public void WriteBytes(ReadOnlySpan<byte> bytes) => stream.Write(bytes);
}