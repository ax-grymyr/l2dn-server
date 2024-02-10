namespace L2Dn.GameServer.Model.Events.Annotations;

public interface RegisterType
{
    ListenerRegisterType value();
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class RegisterTypeAttribute: Attribute
{
    private readonly ListenerRegisterType _listenerRegisterType;

    public RegisterTypeAttribute(ListenerRegisterType listenerRegisterType)
    {
        _listenerRegisterType = listenerRegisterType;
    }
    
    public ListenerRegisterType RegisterType => _listenerRegisterType;
}