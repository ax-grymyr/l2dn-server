namespace L2Dn.GameServer.Model.Events.Annotations;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class LevelRangeAttribute(int fromLevel, int toLevel): Attribute
{
    public int FromLevel => fromLevel;
    public int ToLevel => toLevel;
}