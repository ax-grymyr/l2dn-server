namespace L2Dn.GameServer.Model.Events.Impl;

public class OnServerStart: IBaseEvent
{
    public OnServerStart()
    {
    }

    public EventType getType()
    {
        return EventType.ON_SERVER_START;
    }
}