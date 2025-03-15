using L2Dn.IO;
using L2Dn.Packages.Internal;

namespace L2Dn.Packages.Unreal;

public sealed class UPackage: ISerializableObject
{
    private const int _packageSignature = unchecked((int)0x9E2A83C1);
    private const int _lineage2Signature = 0x0069004C;
    private readonly UPackageManager _packageManager;
    private PackageHeader _header;
    private readonly List<UName> _names = new();
    private readonly List<Guid> _heritages = new();
    private readonly List<UImport> _imports = new();
    private readonly List<UExport> _exports = new();

    public int Version => _header.Version;
    public List<UName> Names => _names;
    public List<UImport> Imports => _imports;
    public List<UExport> Exports => _exports;

    public UPackage(UPackageManager packageManager)
    {
        _packageManager = packageManager;
    }

    public string GetName(int index) => _names[index].Name;

    public UObjectRef? GetObjectRef(int index) =>
        index == 0 ? null : index < 0 ? _imports[-index - 1] : _exports[index];

    public void Read(UBinaryReader reader)
    {
        int signature = reader.ReadInt32();
        if (signature != _packageSignature)
            throw new InvalidOperationException("Not an unreal package.");

        _header.Read(reader);

        List<OffsetActionPair> tables = new();
        if (_header.NameCount > 0)
        {
            tables.Add(new OffsetActionPair(_header.NameOffset,
                static (p, reader) =>
                {
                    p._names.AddRange(reader.ReadObjects<UName>(p._header.NameCount));
                    reader.Names = p._names;
                }));
        }

        if (_header.HeritageCount > 0)
        {
            tables.Add(new OffsetActionPair(_header.HeritageOffset,
                static (p, reader) => p._heritages.AddRange(reader.ReadValues<Guid>(p._header.HeritageCount))));
        }

        if (_header.ExportCount > 0)
        {
            tables.Add(new OffsetActionPair(_header.ExportOffset,
                static (p, reader) =>
                    p._exports.AddRange(reader.ReadObjects<UExport>(p._header.ExportCount, () => new UExport(p)))));
        }

        if (_header.ImportCount > 0)
        {
            tables.Add(new OffsetActionPair(_header.ImportOffset,
                static (p, reader) => p._imports.AddRange(reader.ReadObjects<UImport>(p._header.ImportCount))));
        }

        tables.Sort(static (x, y) => x.Offset.CompareTo(y.Offset));
        foreach (OffsetActionPair table in tables)
        {
            reader.SeekToPosition(table.Offset);
            table.Action(this, reader);
        }

        foreach (UImport import in _imports)
            import.ResolveDependencies(this);

        foreach (UExport export in _exports)
            export.ResolveDependencies(this);

        foreach (UExport export in _exports.OrderBy(x =>x.SerialOffset))
            export.ReadData(reader);
    }

    public static UPackage LoadFrom(UPackageManager packageManager, Stream stream)
    {
        UBinaryReader reader = new(stream);
        UPackage package = new(packageManager);
        package.Read(reader);
        return package;
    }

    public static UPackage LoadFrom(UPackageManager packageManager, string filePath)
    {
        using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        UBinaryReader reader = new(stream);
        int signature = reader.ReadInt32();
        stream.Seek(0, SeekOrigin.Begin);

        if (signature == _lineage2Signature)
        {
            // Lineage 2 encrypted file
            using EncryptedStream encryptedStream = EncryptedStream.OpenRead(stream, Path.GetFileName(filePath));
            return LoadFrom(packageManager, encryptedStream);
        }

        return LoadFrom(packageManager, stream);
    }

    internal UObject LoadObject(UExport export)
    {
        string className = export.Class?.Name ??
                           throw new InvalidOperationException($"Object '{export.Name}' is a class");

        MemoryStream stream = new(export.RawData, false);
        UBinaryReader reader = new(stream)
        {
            PackageVersion = _header.Version,
            LicenseeVersion = _header.LicenseeVersion,
            Names = _names,
            Package = this
        };

        UObject obj = _packageManager.CreateObject(export, className);
        obj.Read(reader);
        return obj;
    }
}

file record struct OffsetActionPair(uint Offset, Action<UPackage, UBinaryReader> Action);