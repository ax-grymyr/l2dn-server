using L2Dn.Packages.Unreal;

namespace L2Dn.Packages.Internal;

internal abstract class ObjectFactory
{
    public abstract UObject Create(UExport export);
}

internal sealed class ObjectFactory<T>(Func<UExport, T> factory): ObjectFactory
    where T: UObject
{
    public override UObject Create(UExport export) => factory(export);
}