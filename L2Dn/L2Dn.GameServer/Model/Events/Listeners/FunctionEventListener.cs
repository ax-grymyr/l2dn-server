using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Model.Events.Returns;
using NLog;

namespace L2Dn.GameServer.Model.Events.Listeners;

/**
 * Function event listener provides callback operation with return object possibility.
 * @author UnAfraid
 */
public class FunctionEventListener: AbstractEventListener
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FunctionEventListener));
    private readonly Func<IBaseEvent, AbstractEventReturn> _callback;

    public FunctionEventListener(ListenersContainer container, EventType type,
        Func<IBaseEvent, AbstractEventReturn> callback, object owner): base(container, type, owner)
    {

        _callback = callback;
    }

    public override R executeEvent<R>(IBaseEvent ev)
    {
        try
        {
            AbstractEventReturn res = _callback(ev);
            if (res is R result)
                return result;
        }
        catch (Exception e)
        {
            LOGGER.Warn(GetType().Name + ": Error while invoking " + ev + " on " + getOwner() + ": " + e);
        }

        return null;
    }
}