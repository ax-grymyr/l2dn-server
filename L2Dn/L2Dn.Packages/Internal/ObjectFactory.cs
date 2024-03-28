using L2Dn.Packages.Unreal;

namespace L2Dn.Packages.Internal;

internal abstract class ObjectFactory
{
    public abstract UObject Create();
}

internal sealed class ObjectFactory<T>(Func<T> factory): ObjectFactory
    where T: UObject
{
    public override UObject Create() => factory();
}
