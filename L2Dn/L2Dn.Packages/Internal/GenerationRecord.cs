namespace L2Dn.Packages.Internal;

internal struct GenerationRecord: ISerializableObject
{
    /// <summary>
    /// The number of export table entries in that generation of the package.
    /// </summary>
    public int ExportCount;

    /// <summary>
    /// The number of name table entries in that generation of the package.
    /// </summary>
    public int NameCount;

    public int NetObjectCount;

    public void Read(UBinaryReader reader)
    {
        ExportCount = reader.ReadInt32();
        NameCount = reader.ReadInt32();

        if (reader.PackageVersion >= 322)
        {
            NetObjectCount = reader.ReadInt32();
        }
    }
}
