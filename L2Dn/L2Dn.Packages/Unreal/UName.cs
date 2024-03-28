namespace L2Dn.Packages.Unreal;

/// <summary>
/// Generally the name table does not contain any duplicate entries,
/// including no different capitalization. The only exception
/// may be the name 'None', which may be occur several times if
/// the package was conformed to a previous version.
/// Names in the name table are grouped by the generation they were added in.
/// Every generation's name group starts with a new 'None' name.
/// The indices of additional 'None' names correspond to
/// the name counts of previous generations in the package header's generation info list.
/// </summary>
public sealed class UName: ISerializableObject
{
    public const string NoneString = "None";

    /// <summary>
    /// The string this name represents.
    /// </summary>
    public string Name { get; set; } = NoneString;

    /// <summary>
    /// Flags associated with this name.
    /// </summary>
    public UObjectFlags Flags { get; set; }

    public void Read(UBinaryReader reader)
    {
        Name = reader.ReadUString();
        if (reader.PackageVersion >= 141)
            Flags = (UObjectFlags)reader.ReadUInt64();
        else
            Flags = (UObjectFlags)reader.ReadUInt32();
    }
}
