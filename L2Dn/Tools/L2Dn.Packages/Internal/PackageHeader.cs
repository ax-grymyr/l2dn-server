using L2Dn.Packages.Unreal;

namespace L2Dn.Packages.Internal;

internal struct PackageHeader: ISerializableObject
{
    /// <summary>
    /// The package file format version.
    ///
    /// <list type="bullet">
    /// <item>UT v346 writes version 69</item>
    /// <item>UT2004 v3369 writes version 128</item>
    /// <item>UT3 v2.1 writes version 512</item>
    /// <item>UE4 writes version -1 to -3, and is then followed by another two DWORDs</item>
    /// </list>
    /// </summary>
    public short Version;

    /// <summary>
    /// UE3Version (writes 868)
    /// </summary>
    public int Ue3Version;

    /// <summary>
    /// UE4Version (writes ~100 - ~300)
    /// </summary>
    public int Ue4Version;

    /// <summary>
    /// Licensees may modify the package format. Their changes may render
    /// packages incompatible to standard packages, so they have this field
    /// as a separate way to handle their own change history.
    /// <list type="bullet">
    /// <item>UT writes version 0</item>
    /// <item>UT2003 writes version 28</item>
    /// <item>UT2004 writes version 29</item>
    /// <item>UT3 writes version 0</item>
    /// </list>
    /// </summary>
    public short LicenseeVersion;

    /// <summary>
    /// Version >= 249
    /// </summary>
    public uint HeaderSize;

    /// <summary>
    /// Version >= 269
    /// </summary>
    public string FolderName;

    public UPackageFlags Flags;

    /// <summary>
    /// The number of entries in the package's name table.
    /// </summary>
    public int NameCount;

    /// <summary>
    /// The byte position of the name table in the package file.
    /// </summary>
    public uint NameOffset;

    /// <summary>
    /// The number of entries in the package's export table.
    /// </summary>
    public int ExportCount;

    /// <summary>
    /// The byte position of the export table in the package file.
    /// </summary>
    public uint ExportOffset;

    /// <summary>
    /// The number of entries in the package's import table.
    /// </summary>
    public int ImportCount;

    /// <summary>
    /// The byte position of the import table in the package file.
    /// </summary>
    public uint ImportOffset;

    /// <summary>
    /// The number of entries in the package's heritage table.
    /// Version &lt; 68
    /// </summary>
    public int HeritageCount;

    /// <summary>
    /// The byte position of the heritage table in the package file.
    /// Version &lt; 68
    /// </summary>
    public uint HeritageOffset;

    /// <summary>
    /// The byte position of the dependencies table in the package file.
    /// The number of entries is equal to Export count. ???
    /// Version >= 415
    /// </summary>
    public uint DependsOffset;

    /// <summary>
    /// Version >= 623
    /// </summary>
    public uint ImportExportGuidsOffset;

    /// <summary>
    /// Version >= 623
    /// </summary>
    public uint ImportGuidsCount;

    /// <summary>
    /// Version >= 623
    /// </summary>
    public uint ExportGuidsCount;

    /// <summary>
    /// Version >= 584
    /// </summary>
    public uint ThumbnailTableOffset;

    /// <summary>
    /// The package's GUID that is used e.g. in the package map.
    /// Before version 68 the last GUID from the heritage table defines the package's GUID.
    /// Version >= 68
    /// </summary>
    public Guid PackageGuid;

    /// <summary>
    /// List of generation infos.
    /// Version >= 68
    /// </summary>
    public List<GenerationRecord>? Generations;

    /// <summary>
    /// Optional: Engine licensee version.
    /// Version >= 245
    /// </summary>
    public int EngineVersion;

    /// <summary>
    /// Optional: Cooker licensee version.
    /// Version >= 277
    /// </summary>
    public int CookerVersion;

    /// <summary>
    /// Version >= 334
    /// </summary>
    public uint CompressionFlag;

    /// <summary>
    /// List of compressed chunks.
    /// Version >= 334
    /// </summary>
    public List<CompressedChunkRecord>? CompressedChunks;

    // TODO: structures related to cooking and textures in newer versions.

    public void Read(UBinaryReader reader)
    {
        Version = reader.ReadInt16();
        reader.PackageVersion = Version;

        if (Version < 0)
        {
            Ue3Version = reader.ReadInt32();
            Ue4Version = reader.ReadInt32();
        }

        LicenseeVersion = reader.ReadInt16();
        reader.LicenseeVersion = LicenseeVersion;
        
        if (Version >= 249)
        {
            HeaderSize = reader.ReadUInt32();

            if (Version >= 269)
                FolderName = reader.ReadUString();
        }

        Flags = reader.ReadValue<UPackageFlags>();
        NameCount = reader.ReadInt32();
        NameOffset = reader.ReadUInt32();
        ExportCount = reader.ReadInt32();
        ExportOffset = reader.ReadUInt32();
        ImportCount = reader.ReadInt32();
        ImportOffset = reader.ReadUInt32();

        if (Version < 68)
        {
            HeritageCount = reader.ReadInt32();
            HeritageOffset = reader.ReadUInt32();
            return;
        }

        if (Version >= 415)
        {
            DependsOffset = reader.ReadUInt32();
        }

        if (Version >= 623)
        {
            ImportExportGuidsOffset = reader.ReadUInt32();
            ImportGuidsCount = reader.ReadUInt32();
            ExportGuidsCount = reader.ReadUInt32();
        }

        if (Version >= 584)
        {
            ThumbnailTableOffset = reader.ReadUInt32();
        }

        PackageGuid = reader.ReadGuid();
        int generationCount = reader.ReadInt32();
        Generations = reader.ReadObjects<GenerationRecord>(generationCount).ToList();

        if (Version >= 245)
        {
            EngineVersion = reader.ReadInt32();
            if (Version >= 277)
            {
                CookerVersion = reader.ReadInt32();
                if (Version >= 334)
                {
                    CompressionFlag = reader.ReadUInt32();
                    int chunkCount = reader.ReadInt32();
                    CompressedChunks = reader.ReadObjects<CompressedChunkRecord>(chunkCount).ToList();
                }
            }
        }
    }
}
