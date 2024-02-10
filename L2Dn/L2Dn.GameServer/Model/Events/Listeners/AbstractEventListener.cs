using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Model.Events.Returns;

namespace L2Dn.GameServer.Model.Events.Listeners;

/**
 * @author UnAfraid
 */
public abstract class AbstractEventListener: IComparable<AbstractEventListener>
{
    private int _priority = 0;
    private ListenersContainer _container;
    private EventType _type;
    private object _owner;

    public AbstractEventListener(ListenersContainer container, EventType type, Object owner)
    {
        _container = container;
        _type = type;
        _owner = owner;
    }

    /**
     * @return the container on which this listener is being registered (Used to unregister when unloading scripts)
     */
    public ListenersContainer getContainer()
    {
        return _container;
    }

    /**
     * @return the type of event which listener is listening for.
     */
    public EventType getType()
    {
        return _type;
    }

    /**
     * @return the owner of the listener, the object that registered this listener.
     */
    public Object getOwner()
    {
        return _owner;
    }

    /**
     * @return priority of execution (Higher the sooner)
     */
    public int getPriority()
    {
        return _priority;
    }

    /**
     * Sets priority of execution.
     * @param priority
     */
    public void setPriority(int priority)
    {
        _priority = priority;
    }

    /**
     * Method invoked by EventDispatcher that will use the callback.
     * @param <R>
     * @param event
     * @param returnBackClass
     * @return
     */
    public abstract R executeEvent<R>(IBaseEvent @event)
        where R: AbstractEventReturn;

    /**
     * Unregisters detaches and unregisters current listener.
     */
    public void unregisterMe()
    {
        _container.removeListener(this);
    }

    public int CompareTo(AbstractEventListener o)
    {
        return o.getPriority().CompareTo(getPriority());
    }
}