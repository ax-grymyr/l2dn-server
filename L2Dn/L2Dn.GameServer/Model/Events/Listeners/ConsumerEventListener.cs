using L2Dn.GameServer.Model.Events.Impl;

namespace L2Dn.GameServer.Model.Events.Listeners;

/**
 * Consumer event listener provides callback operation without any return object.
 * @author UnAfraid
 */
public class ConsumerEventListener: AbstractEventListener
{
    private readonly Action<IBaseEvent> _callback;

    public ConsumerEventListener(ListenersContainer container, EventType type, Action<IBaseEvent> callback,
        Object owner): base(container, type, owner)
    {
        _callback = (Action<IBaseEvent>)callback;
    }

    public override R executeEvent<R>(IBaseEvent @event)
    {
        _callback(@event);
        return null;
    }
}