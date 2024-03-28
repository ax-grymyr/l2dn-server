namespace L2Dn.GameServer.Model.Events.Annotations;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class IdRangeAttribute(int fromId, int toId): Attribute
{
    public int FromId => fromId;
    public int ToId => toId;
}