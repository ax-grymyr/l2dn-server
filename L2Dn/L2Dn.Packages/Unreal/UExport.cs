namespace L2Dn.Packages.Unreal;

/// <summary>
/// Exported objects implicitly form a tree with the package as implicit root
/// (it is not included in the export table), but the export table entries
/// are not strictly stored in any tree traversal order.
/// Like the name table, the export table may contain null objects if
/// the package was conformed to an older version.
/// Their indices correspond to the export counts of previous package generations.
/// </summary>
public sealed class UExport: UObjectRef, ISerializableObject
{
    /// <summary>
    /// The class of this object. None means this is a class itself.
    /// </summary>
    private int _class;

    /// <summary>
    /// The object this object inherits from. Only used for struct, states, classes and functions.
    /// </summary>
    private int _super;

    /// <summary>
    /// The object containing this object. Resources may be contained in groups
    /// (which are subpackages), functions may be contained in states or classes, and so on.
    /// </summary>
    private int _outer;

    /// <summary>
    /// The index of this object's name in the name table.
    /// </summary>
    private int _name;

    /// <summary>
    /// The object's archetype reference.
    /// Version >= 220
    /// </summary>
    private int _archetype;

    /// <summary>
    /// Object data.
    /// </summary>
    private byte[] _data = Array.Empty<byte>();

    /// <summary>
    /// The class of the object. Null means this is a class itself.
    /// </summary>
    public UObjectRef? Class { get; set; }

    /// <summary>
    /// The object this object inherits from. Only used for struct, states, classes and functions.
    /// </summary>
    public UObjectRef? Super { get; set; }

    /// <summary>
    /// The object's archetype reference.
    /// Version >= 220
    /// </summary>
    public UObjectRef? Archetype { get; set; }

    /// <summary>
    /// This object's flags.
    /// </summary>
    public UObjectFlags ObjectFlags { get; set; }

    /// <summary>
    /// The size of this object's data in the package file.
    /// Some objects do not include any data.
    /// </summary>
    public int SerialSize { get; set; }

    /// <summary>
    /// The position of this object's data in the package file.
    /// This field is only present if the serial size is non-zero.
    /// </summary>
    public int SerialOffset { get; set; }

    public byte[] RawData => _data;

    public void Read(UBinaryReader reader)
    {
        _class = reader.ReadIndex();
        _super = reader.ReadIndex();
        _outer = reader.ReadInt32();
        _name = reader.ReadIndex();

        if (reader.PackageVersion >= 220)
        {
            _archetype = reader.ReadInt32();
        }

        if (reader.PackageVersion >= 195)
        {
            ObjectFlags = (UObjectFlags)reader.ReadUInt64();
        }
        else
        {
            ObjectFlags = (UObjectFlags)reader.ReadUInt32();
        }

        SerialSize = reader.ReadIndex();
        SerialOffset = SerialSize > 0 ? reader.ReadIndex() : 0;

        if (reader.PackageVersion is >= 220 and < 543)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
        }

        if (reader.PackageVersion >= 247)
        {
            reader.ReadUInt32(); // ExportFlags
        }

        if (reader.PackageVersion >= 322)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                reader.ReadUInt32();
            }

            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
        }

        if (reader.PackageVersion >= 487)
        {
            reader.ReadInt32();
        }
    }

    internal void ResolveDependencies(UPackage package)
    {
        Name = package.GetName(_name);
        Class = package.GetObjectRef(_class);
        Super = package.GetObjectRef(_super);
        Outer = package.GetObjectRef(_outer);
        Archetype = package.GetObjectRef(_archetype);
    }

    internal void ReadData(UBinaryReader reader)
    {
        if (SerialOffset <= 0 || SerialSize <= 0)
            return;

        reader.SeekToPosition(SerialOffset);
        _data = new byte[SerialSize];
        reader.ReadBytes(_data);
    }
}