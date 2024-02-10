using System.Text;

namespace L2Dn.Packages;

public class UProperty: ISerializableObject
{
    public string Name { get; set; } = UName.NoneString;
    public int ArrayIndex { get; set; } = -1;
    public object Value { get; set; } = string.Empty;

    public void Read(UBinaryReader reader)
    {
        int nameIndex = reader.ReadIndex();
        Name = reader.Names[nameIndex].Name;
        if (Name == UName.NoneString)
            return;

        PropertyType propertyType = (PropertyType)reader.ReadByte();

        PropertyType type = propertyType & PropertyType.TypeMask;
        bool isArray = (propertyType & PropertyType.ArrayMask) != 0;

        string structType = type == PropertyType.Struct ? reader.Names[reader.ReadIndex()].Name : string.Empty;

        int size = (int)(propertyType & PropertyType.SizeMask) switch
        {
            0x00 => 1,
            0x10 => 2,
            0x20 => 4,
            0x30 => 12,
            0x40 => 16,
            0x50 => reader.ReadByte(),
            0x60 => reader.ReadUInt16(),
            0x70 => reader.ReadInt32(),
            _ => 0
        };

        ArrayIndex = isArray && type != PropertyType.Bool ? ReadArrayIndex(reader) : -1;

        Value = type switch
        {
            PropertyType.Byte => reader.ReadByte(),
            PropertyType.Int => reader.ReadInt32(),
            PropertyType.Bool => isArray,
            PropertyType.Float => reader.ReadFloat(),
            PropertyType.Name => reader.Names[reader.ReadIndex()],
            PropertyType.String => ReadString(reader, size),
            PropertyType.Str => reader.ReadUString(),
            PropertyType.Struct => ReadStruct(reader, structType),
            PropertyType.Object => ReadObject(reader),

            _ => throw new NotSupportedException()
        };
    }

    private static object ReadObject(UBinaryReader reader)
    {
        int index = reader.ReadIndex();
        UPackage package = reader.Package ?? throw new InvalidOperationException("Package is not set");
        return package.GetObjectRef(index) ?? throw new InvalidOperationException("Invalid object property");
    }

    private static object ReadStruct(UBinaryReader reader, string structType) =>
        structType switch
        {
            "Color" => reader.ReadObject<UColor>(),
            "Vector" => reader.ReadObject<UVector>(),
            "Rotator" => reader.ReadObject<URotator>(),
            "Scale" => reader.ReadObject<UScale>(),
            "PointRegion" => reader.ReadObject<UScale>(),
            _ => throw new NotSupportedException($"Unknown structure type: '{structType}'")
        };

    private static int ReadArrayIndex(UBinaryReader reader)
    {
        byte b = reader.ReadByte();
        if ((b & 0x80) == 0)
            return b;

        if ((b & 0xC0) == 0x80)
            return ((b & 0x7F) << 8) | reader.ReadByte();

        return ((b & 0x3F) << 24)
               | (reader.ReadByte() << 16)
               | (reader.ReadByte() << 8)
               | reader.ReadByte();
    }

    private static string ReadString(UBinaryReader reader, int size)
    {
        Span<byte> bytes = new byte[size];
        reader.ReadBytes(bytes);
        return Encoding.ASCII.GetString(bytes);
    }
}

[Flags]
file enum PropertyType
{
    Byte = 1,
    Int = 2,
    Bool = 3,
    Float = 4,
    Object = 5,
    Name = 6,
    String = 7,
    Class = 8,
    Array = 9,
    Struct = 10,
    Vector = 11,
    Rotator = 12,
    Str = 13,
    Map = 14,
    FixedArray = 15,

    TypeMask = 0x0f,
    SizeMask = 0x70,
    ArrayMask = 0x80,
}
