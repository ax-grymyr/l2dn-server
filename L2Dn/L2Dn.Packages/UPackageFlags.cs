namespace L2Dn.Packages;

[Flags]
public enum UPackageFlags: uint
{
    None = 0,

    /// <summary>
    /// Allow downloading package.
    /// </summary>
    AllowDownload = 0x0001,

    /// <summary>
    /// Purely optional for clients.
    /// </summary>
    ClientOptional = 0x0002,

    /// <summary>
    /// Only needed on the server side.
    /// </summary>
    ServerSideOnly = 0x0004,

    /// <summary>
    /// Loaded from linker with broken import links.
    /// </summary>
    BrokenLinks = 0x0008,

    /// <summary>
    /// Not trusted.
    /// </summary>
    Unsecure = 0x0010,

    /// <summary>
    /// Client needs to download this package.
    /// </summary>
    Need = 0x8000,
}
