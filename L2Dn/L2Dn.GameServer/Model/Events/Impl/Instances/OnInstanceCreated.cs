using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;

namespace L2Dn.GameServer.Model.Events.Impl.Instances;

public class OnInstanceCreated: IBaseEvent
{
    private readonly Instance _instance;
    private readonly Player _creator;

    public OnInstanceCreated(Instance instance, Player creator)
    {
        _instance = instance;
        _creator = creator;
    }

    public Instance getInstanceWorld()
    {
        return _instance;
    }

    public Player getCreator()
    {
        return _creator;
    }

    public EventType getType()
    {
        return EventType.ON_INSTANCE_CREATED;
    }
}