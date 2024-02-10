using L2Dn.GameServer.Model.Events.Impl;
using NLog;

namespace L2Dn.GameServer.Model.Events.Listeners;

/// <summary>
/// Annotation event listener provides dynamically attached callback
/// to any method operation with or without any return object.
/// </summary>
public class AnnotationEventListener: AbstractEventListener
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AnnotationEventListener));
    private readonly Func<object, IBaseEvent, object> _callback;

    public AnnotationEventListener(ListenersContainer container, EventType type,
        Func<object, IBaseEvent, object> callback, object owner, int priority): base(container, type, owner)
    {
        _callback = callback;
        setPriority(priority);
    }

    public override R executeEvent<R>(IBaseEvent @event)
    {
        try
        {
            object result = _callback(getOwner(), @event);
            if (result is R @return)
            {
                return @return;
            }
        }
        catch (Exception e)
        {
            LOGGER.Warn(
                nameof(AnnotationEventListener) + ": Error while invoking " + _callback.Method.Name + " on " +
                getOwner(),
                e);
        }

        return null;
    }
}
