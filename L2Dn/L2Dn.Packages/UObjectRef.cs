namespace L2Dn.Packages;

public abstract class UObjectRef
{
    /// <summary>
    /// The object containing this imported object.
    /// Resources may be contained in groups
    /// (which are subpackages), functions may
    /// be contained in states or classes, and so on.
    /// </summary>
    public UObjectRef? Outer { get; set; }

    /// <summary>
    /// The name of the object.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
