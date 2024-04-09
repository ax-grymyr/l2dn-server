using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using L2Dn.Conversion;
using L2Dn.IO;

namespace L2Dn.Packages.Unreal;

public class UBinaryReader(Stream stream, long position = 0):
    BinaryReader<LittleEndianBitConverter>(stream, position)
{
    private int _packageVersion;
    private bool _newVersionIndex;

    public int PackageVersion
    {
        get => _packageVersion;
        set => (_packageVersion, _newVersionIndex) = (value, value >= 178);
    }

    public int LicenseeVersion { get; set; }

    public IReadOnlyList<UName> Names { get; set; } = Array.Empty<UName>();

    public UPackage? Package { get; set; } // TODO: refactor

    public int ReadIndex()
    {
        if (_newVersionIndex)
            return ReadInt32();

        byte b = ReadByte();
        bool neg = (b & (1 << 7)) != 0;
        int index = b & 0x3f;
        if ((b & (1 << 6)) != 0)
        {
            int shift = 6;
            do
            {
                b = ReadByte();
                index |= (int)((b & 0x7fu) << shift);
                shift += 7;
            } while ((b & (1 << 7)) != 0 && shift < 32);
        }

        return neg ? -index : index;
    }

    public string ReadUString()
    {
        int length = ReadIndex();
        if (length == 0)
            return string.Empty;

        int size = length;
        bool unicode = length < 0;
        if (unicode)
        {
            length = -length;
            size = length * 2;
        }

        byte[] buf = ArrayPool<byte>.Shared.Rent(size);
        Span<byte> span = buf.AsSpan(0, size);
        try
        {
            ReadBytes(span);

            if (unicode)
            {
                // TODO: big endian architectures
                ReadOnlySpan<char> chars = MemoryMarshal.Cast<byte, char>(span);
                if (chars[^1] == '\0')
                    chars = chars[..^1];

                return new string(chars);
            }

            if (span[^1] == 0)
                span = span[..^1];

            return Encoding.UTF8.GetString(span);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buf);
        }
    }

    public IEnumerable<T> ReadValues<T>(int count)
        where T: unmanaged
    {
        for (int i = 0; i < count; i++)
            yield return ReadValue<T>();
    }

    public T ReadObject<T>()
        where T: ISerializableObject, new()
    {
        T value = new();
        value.Read(this);
        return value;
    }

    public IEnumerable<T> ReadObjects<T>(int count)
        where T: ISerializableObject, new()
    {
        for (int i = 0; i < count; i++)
        {
            T value = new();
            value.Read(this);
            yield return value;
        }
    }
}
