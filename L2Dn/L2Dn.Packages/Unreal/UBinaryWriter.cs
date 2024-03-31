using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using L2Dn.Conversion;
using L2Dn.IO;

namespace L2Dn.Packages.Unreal;

public class UBinaryWriter(Stream stream): 
    BinaryWriter<LittleEndianBitConverter>(stream)
{
    public void WriteIndex(int value)
    {
        bool negative = value < 0;
        uint val = (uint)(negative ? -value : value);
        uint v = val & 0x3F;
        WriteByte((byte)((negative ? 0x80u : 0u) | (v != val ? 0x40u : 0u) | v));
        val >>= 6;
        while (val != 0)
        {
            v = val & 0x7F;
            WriteByte((byte)((v != val ? 0x80u : 0u) | v));
            val >>= 7;
        }
    }

    public void WriteUString(string value, bool utf16Le = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            WriteIndex(0);
            return;
        }
        
        int length = value.Length;
        bool addNullChar = value[^1] != '\0';
        if (addNullChar)
            length++;

        if (utf16Le)
        {
            WriteIndex(-length);

            // TODO: big endian architectures
            ReadOnlySpan<byte> bytes = MemoryMarshal.Cast<char, byte>(value);
            WriteBytes(bytes);
            
            if (addNullChar)
                WriteInt16(0);
            
            return;
        }

        WriteIndex(length);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(value.Length * 2);
        try
        {
            int len = Encoding.UTF8.GetBytes(value, buffer);
            WriteBytes(buffer.AsSpan(0, len));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
        
        if (addNullChar)
            WriteByte(0);
    }
}