namespace L2Dn.GameServer.Handlers;

[AttributeUsage(AttributeTargets.Class)]
public sealed class HandlerStringKeyAttribute(string key): HandlerKeyAttribute<string>(key);