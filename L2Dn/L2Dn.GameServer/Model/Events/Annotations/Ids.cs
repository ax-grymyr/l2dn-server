namespace L2Dn.GameServer.Model.Events.Annotations;

public interface Ids
{
    Id[] value();
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class IdsAttribute: Attribute
{
    private readonly int[] _value;

    public IdsAttribute(params int[] value)
    {
        _value = value;
    }

    public int[] Value => _value;
}