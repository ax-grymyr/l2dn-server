namespace L2Dn.Packages;

/// <summary>
/// The import table specifies objects from other packages
/// that are required by objects in this package in some way.
///
/// Imported objects form trees with their package as root,
/// but the import table entries are not strictly stored in
/// any tree traversal order. Packages may import a package
/// with the same name to reference native-only classes that
/// don't have any UnrealScript parts. For example the Core
/// package imports another package called Core containing
/// classes such as Class or ClassProperty.
/// </summary>
public sealed class UImport: UObjectRef, ISerializableObject
{
    /// <summary>
    /// The package to import this object from.
    /// Zero if the imported object is a package itself.
    /// </summary>
    private int _package;

    /// <summary>
    /// The class of the imported object.
    /// </summary>
    private int _class;

    /// <summary>
    /// The object containing this imported object.
    /// Resources may be contained in groups
    /// (which are subpackages), functions may
    /// be contained in states or classes, and so on.
    /// </summary>
    private int _outer;

    /// <summary>
    /// The index of the imported object's name in this package's name table.
    /// </summary>
    private int _name;

    /// <summary>
    /// The package of the object.
    /// Null if the imported object is a package itself.
    /// </summary>
    public string? Package { get; set; }

    /// <summary>
    /// The class of the object. Null means this is a class itself.
    /// </summary>
    public string? Class { get; set; }

    public void Read(UBinaryReader reader)
    {
        _package = reader.ReadIndex();
        _class = reader.ReadIndex();
        _outer = reader.ReadInt32();
        _name = reader.ReadIndex();
    }

    internal void ResolveDependencies(UPackage package)
    {
        Package = _package != 0 ? package.GetName(_package) : null;
        string className = package.GetName(_class);
        Class = className == UName.NoneString ? null : className;
        Outer = package.GetObjectRef(_outer);
        Name = package.GetName(_name);
    }
}
