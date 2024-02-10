using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Events.Listeners;

/**
 * Runnable event listener provides callback operation without any parameters and return object.
 * @author UnAfraid
 */
public class RunnableEventListener: AbstractEventListener
{
    private readonly Runnable _callback;

    public RunnableEventListener(ListenersContainer container, EventType type, Runnable callback, Object owner): base(
        container, type, owner)
    {
        _callback = callback;
    }

    public override R executeEvent<R>(IBaseEvent ev)
    {
        _callback.run();
        return null;
    }
}