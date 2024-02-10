namespace L2Dn.GameServer.Model.Events.Annotations;

public interface Priority
{
    int value();
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class PriorityAttribute: Attribute
{
    private readonly int _value;
    
    public PriorityAttribute(int value)
    {
        _value = value;
    }

    public int Value => _value;
}