namespace L2Dn.GameServer.Handlers;

[AttributeUsage(AttributeTargets.Class)]
public sealed class HandlerNameAttribute(string name): Attribute
{
    public string Name => name;
}