namespace L2Dn.GameServer.Model.Events.Annotations;

public interface Id
{
    int[] value();
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class IdAttribute: Attribute
{
    private readonly int[] _value;

    public IdAttribute(params int[] value)
    {
        _value = value;
    }

    public int[] Value => _value;
}