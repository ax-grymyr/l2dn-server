using System.Buffers;
using System.Runtime.CompilerServices;
using L2Dn.Conversion;

namespace L2Dn.IO;

public class BinaryReader<TBitConverter>(Stream stream, long position = 0)
    where TBitConverter: struct, IBitConverter
{
    public long Position => position;

    public byte ReadByte()
    {
        int b = stream.ReadByte();
        if (b == -1)
            throw new EndOfStreamException();

        position++;
        return (byte)b;
    }

    public sbyte ReadSByte() => (sbyte)ReadByte();

    public T ReadValue<T>()
        where T: unmanaged
    {
        Span<byte> span = stackalloc byte[Unsafe.SizeOf<T>()];
        stream.ReadExactly(span);
        position += Unsafe.SizeOf<T>();
        return TBitConverter.ToValue<T>(span);
    }

    public short ReadInt16() => ReadValue<short>();
    public ushort ReadUInt16() => ReadValue<ushort>();

    public int ReadInt32() => ReadValue<int>();
    public uint ReadUInt32() => ReadValue<uint>();

    public long ReadInt64() => ReadValue<long>();
    public ulong ReadUInt64() => ReadValue<ulong>();

    public float ReadFloat() => ReadValue<float>();
    public double ReadDouble() => ReadValue<double>();

    public Guid ReadGuid() => ReadValue<Guid>();

    public void ReadBytes(Span<byte> destination)
    {
        stream.ReadExactly(destination);
        position += destination.Length;
    }

    public int Read7BitEncodedInt()
    {
        uint result = 0;
        byte byteReadJustNow;
        const int maxBytesWithoutOverflow = 4;
        for (int shift = 0; shift < maxBytesWithoutOverflow * 7; shift += 7)
        {
            byteReadJustNow = ReadByte();
            result |= (byteReadJustNow & 0x7Fu) << shift;
            if (byteReadJustNow <= 0x7Fu)
                return (int)result;
        }

        byteReadJustNow = ReadByte();
        if (byteReadJustNow > 0b_1111u)
            throw new FormatException("Invalid 7-bit encoded integer");

        result |= (uint)byteReadJustNow << (maxBytesWithoutOverflow * 7);
        return (int)result;
    }

    public long Read7BitEncodedInt64()
    {
        ulong result = 0;
        byte byteReadJustNow;
        const int maxBytesWithoutOverflow = 9;
        for (int shift = 0; shift < maxBytesWithoutOverflow * 7; shift += 7)
        {
            byteReadJustNow = ReadByte();
            result |= (byteReadJustNow & 0x7Ful) << shift;
            if (byteReadJustNow <= 0x7Fu)
                return (long)result;
        }

        byteReadJustNow = ReadByte();
        if (byteReadJustNow > 0b_1u)
            throw new FormatException("Invalid 7-bit encoded integer");

        result |= (ulong)byteReadJustNow << (maxBytesWithoutOverflow * 7);
        return (long)result;
    }

    public void Skip(long count)
    {
        if (count == 0)
            return;

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (stream.CanSeek)
        {
            stream.Seek(count, SeekOrigin.Current);
            position += count;
            return;
        }

        byte[] buf = ArrayPool<byte>.Shared.Rent(4096);
        try
        {
            long size = count;
            while (size >= buf.Length)
            {
                stream.ReadExactly(buf);
                size -= buf.Length;
            }

            stream.ReadExactly(buf.AsSpan(0, (int)size));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buf);
        }

        position += count;
    }

    public void SeekToPosition(long newPosition)
    {
        if (newPosition >= position)
        {
            Skip(newPosition - position);
            return;
        }

        if (!stream.CanSeek)
            throw new NotSupportedException("Stream is not seekable");

        stream.Seek(newPosition - position, SeekOrigin.Current);
        position = newPosition;
    }
}
