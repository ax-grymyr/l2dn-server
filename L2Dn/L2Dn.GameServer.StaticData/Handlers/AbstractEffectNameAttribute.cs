namespace L2Dn.GameServer.Handlers;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AbstractEffectNameAttribute(string name): Attribute
{
    public string Name => name;
}