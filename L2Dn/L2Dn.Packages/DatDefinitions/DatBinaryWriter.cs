using System.Runtime.InteropServices;
using L2Dn.Packages.Unreal;

namespace L2Dn.Packages.DatDefinitions;

public class DatBinaryWriter(Stream stream): UBinaryWriter(stream)
{
    public void WriteString(string value)
    {
        bool utf = value.Any(c => c >= 128);
        WriteUString(value, utf);
    }
    
    public void WriteUtfString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            WriteInt32(0);
            return;
        }

        if (value.Length > 1000000)
            throw new Exception("To much data.");

        WriteInt32(value.Length * 2);
        
        // TODO: big endian architectures
        ReadOnlySpan<byte> bytes = MemoryMarshal.Cast<char, byte>(value);
        WriteBytes(bytes);
    }
}