using L2Dn.Packages.Internal;

namespace L2Dn.Packages.Unreal;

public sealed class UPackageManager
{
    private readonly Dictionary<string, ObjectFactory> _factories = new(StringComparer.Ordinal);

    public UPackageManager()
    {
        RegisterClass<UTexture>("Texture", export => new UTexture(export));
        RegisterClass<UPalette>("Palette", export => new UPalette(export));
    }

    public void RegisterClass<TObject>(string className, Func<UExport, TObject> factory)
        where TObject: UObject =>
        _factories.Add(className, new ObjectFactory<TObject>(factory));

    internal UObject CreateObject(UExport export, string className)
    {
        if (_factories.TryGetValue(className, out ObjectFactory? factory))
            return factory.Create(export);

        return new UObject(export);
    }
}
