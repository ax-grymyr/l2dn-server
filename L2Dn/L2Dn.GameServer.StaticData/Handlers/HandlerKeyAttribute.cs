namespace L2Dn.GameServer.Handlers;

[AttributeUsage(AttributeTargets.Class)]
public class HandlerKeyAttribute<TKey>(TKey key): Attribute
    where TKey: notnull
{
    public TKey Key => key;
}