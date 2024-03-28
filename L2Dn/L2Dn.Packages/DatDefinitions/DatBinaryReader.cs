using System.Buffers;
using System.Runtime.InteropServices;
using L2Dn.Packages.Unreal;

namespace L2Dn.Packages.DatDefinitions;

public class DatBinaryReader(Stream stream, long position = 0): UBinaryReader(stream, position)
{
    public string ReadDatString()
    {
        return ReadUString();
    }

    public string ReadUnicodeString()
    {
        int size = ReadInt32();
        if (size <= 0)
            return string.Empty;
		
        if (size > 1000000)
            throw new Exception("To much data.");

        byte[] buf = ArrayPool<byte>.Shared.Rent(size);
        Span<byte> span = buf.AsSpan(0, size);
        try
        {
            ReadBytes(span);

            // TODO: big endian architectures
            ReadOnlySpan<char> chars = MemoryMarshal.Cast<byte, char>(span);
            if (chars[^1] == '\0')
                chars = chars[..^1];

            return new string(chars);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buf);
        }
    }
}