using L2Dn.GameServer.Model.Events.Impl;

namespace L2Dn.GameServer.Model.Events.Listeners;

/**
 * Runnable event listener provides callback operation without any parameters and return object.
 * @author UnAfraid
 */
public class DummyEventListener: AbstractEventListener
{
    public DummyEventListener(ListenersContainer container, EventType type, Object owner): base(container, type, owner)
    {
    }

    public override R executeEvent<R>(IBaseEvent @event)
    {
        return null;
    }
}