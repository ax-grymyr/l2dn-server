namespace L2Dn.GameServer.Model.Events.Annotations;

public interface RegisterEvent
{
    //EventType value();
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class RegisterEventAttribute: Attribute
{
    // private readonly EventType _eventType;
    //
    // public RegisterEventAttribute(EventType eventType)
    // {
    //     _eventType = eventType;
    // }
    //
    // public EventType EventType => _eventType;
}