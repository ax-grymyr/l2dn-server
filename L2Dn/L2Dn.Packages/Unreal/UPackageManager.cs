using L2Dn.Packages.Internal;

namespace L2Dn.Packages.Unreal;

public sealed class UPackageManager
{
    private readonly Dictionary<string, ObjectFactory> _factories = new(StringComparer.Ordinal);

    public UPackageManager()
    {
        RegisterClass<UTexture>("Texture");
        RegisterClass<UPalette>("Palette");
    }

    public void RegisterClass<TObject>(string className, Func<TObject> factory)
        where TObject: UObject =>
        _factories.Add(className, new ObjectFactory<TObject>(factory));

    public void RegisterClass<TObject>(string className)
        where TObject: UObject, new() =>
        RegisterClass(className, () => new TObject());

    internal UObject CreateObject(string className)
    {
        if (_factories.TryGetValue(className, out ObjectFactory? factory))
            return factory.Create();

        return new UObject();
    }
}
