using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Events.Returns;
using NLog;

namespace L2Dn.GameServer.Model.Events;

/**
 * @author UnAfraid, Mobius
 */
public class EventDispatcher
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EventDispatcher));
	
	protected EventDispatcher()
	{
	}
	
	/**
	 * @param type EventType
	 * @return {@code true} if global containers have a listener of the given type.
	 */
	public bool hasListener(EventType type)
	{
		return Containers.Global().hasListener(type);
	}
	
	/**
	 * @param type EventType
	 * @param container ListenersContainer
	 * @return {@code true} if container has a listener of the given type.
	 */
	public bool hasListener(EventType type, ListenersContainer container)
	{
		return Containers.Global().hasListener(type) || ((container != null) && container.hasListener(type));
	}
	
	/**
	 * @param type EventType
	 * @param containers ListenersContainer...
	 * @return {@code true} if containers have a listener of the given type.
	 */
	public bool hasListener(EventType type, params ListenersContainer[] containers)
	{
		bool hasListeners = Containers.Global().hasListener(type);
		if (!hasListeners)
		{
			foreach (ListenersContainer container in containers)
			{
				if (container.hasListener(type))
				{
					hasListeners = true;
					break;
				}
			}
		}
		return hasListeners;
	}
	
	/**
	 * @param <T>
	 * @param event
	 * @return
	 */
	public T notifyEvent<T>(IBaseEvent @event)
		where T: AbstractEventReturn
	{
		return notifyEvent<T>(@event, null);
	}
	
	/**
	 * @param <T>
	 * @param event
	 * @param container
	 * @param callbackClass
	 * @return
	 */
	public T notifyEvent<T>(IBaseEvent @event, ListenersContainer container)
		where T:AbstractEventReturn
	{
		try
		{
			return notifyEventImpl<T>(@event, container);
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't notify event " + @event.GetType().Name + ": " + e);
		}
		return null;
	}
	
	/**
	 * Executing current listener notification asynchronously
	 * @param event
	 * @param container
	 */
	public void notifyEventAsync(IBaseEvent @event, ListenersContainer container)
	{
		if (@event == null)
		{
			throw new ArgumentNullException(nameof(@event), "Event cannot be null!");
		}
		
		ThreadPool.execute(() => notifyEventToSingleContainer<AbstractEventReturn>(@event, container));
	}
	
	/**
	 * Executing current listener notification asynchronously
	 * @param event
	 * @param containers
	 */
	public void notifyEventAsync(IBaseEvent @event, params ListenersContainer[] containers)
	{
		if (@event == null)
		{
			throw new ArgumentNullException(nameof(@event), "Event cannot be null!");
		}
		
		ThreadPool.execute(() => notifyEventToMultipleContainers<AbstractEventReturn>(@event, containers));
	}
	
	/**
	 * @param <T>
	 * @param event
	 * @param container
	 * @param callbackClass
	 * @return
	 */
	private T notifyEventToSingleContainer<T>(IBaseEvent @event, ListenersContainer container)
		where T: AbstractEventReturn
	{
		if (@event == null)
		{
			throw new ArgumentNullException(nameof(@event), "Event cannot be null!");
		}
		
		try
		{
			// Local listener container.
			T callback = null;
			if (container != null)
			{
				callback = notifyToListeners(container.getListeners(@event.getType()), @event, callback);
			}
			
			// Global listener container.
			if ((callback == null) || !callback.abort())
			{
				callback = notifyToListeners(Containers.Global().getListeners(@event.getType()), @event, callback);
			}
			
			return callback;
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't notify event " + @event.GetType().Name + ": " + e);
		}
		return null;
	}
	
	/**
	 * @param <T>
	 * @param event
	 * @param containers
	 * @param callbackClass
	 * @return
	 */
	private T notifyEventToMultipleContainers<T>(IBaseEvent @event, ListenersContainer[] containers)
		where T: AbstractEventReturn
	{
		if (@event == null)
		{
			throw new ArgumentNullException(nameof(@event), "Event cannot be null!");
		}
		
		try
		{
			T callback = null;
			if (containers != null)
			{
				// Local listener containers.
				foreach (ListenersContainer container in containers)
				{
					if ((callback == null) || !callback.abort())
					{
						callback = notifyToListeners(container.getListeners(@event.getType()), @event, callback);
					}
				}
			}
			
			// Global listener container.
			if ((callback == null) || !callback.abort())
			{
				callback = notifyToListeners(Containers.Global().getListeners(@event.getType()), @event, callback);
			}
			
			return callback;
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't notify event " + @event.GetType().Name + ": " + e);
		}
		return null;
	}
	
	/**
	 * @param <T>
	 * @param event
	 * @param container
	 * @param callbackClass
	 * @return {@link AbstractEventReturn} object that may keep data from the first listener, or last that breaks notification.
	 */
	private T notifyEventImpl<T>(IBaseEvent @event, ListenersContainer container)
		where T: AbstractEventReturn
	{
		if (@event == null)
		{
			throw new ArgumentNullException(nameof(@event), "Event cannot be null!");
		}
		
		// Local listener container.
		T callback = null;
		if (container != null)
		{
			callback = notifyToListeners(container.getListeners(@event.getType()), @event, callback);
		}
		
		// Global listener container.
		if ((callback == null) || !callback.abort())
		{
			callback = notifyToListeners(Containers.Global().getListeners(@event.getType()), @event, callback);
		}
		
		return callback;
	}
	
	/**
	 * @param <T>
	 * @param listeners
	 * @param event
	 * @param returnBackClass
	 * @param callbackValue
	 * @return
	 */
	private T notifyToListeners<T>(ICollection<AbstractEventListener> listeners, IBaseEvent @event, T callbackValue)
		where T: AbstractEventReturn
	{
		T callback = callbackValue;
		foreach (AbstractEventListener listener in listeners)
		{
			try
			{
				T rb = listener.executeEvent<T>(@event);
				if (rb == null)
				{
					continue;
				}
				if ((callback == null) || rb.@override()) // Let's check if this listener wants to override previous return object or we simply don't have one
				{
					callback = rb;
				}
				else if (rb.abort()) // This listener wants to abort the notification to others.
				{
					break;
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Exception during notification of event: " + @event.GetType().Name + " listener: " + listener.GetType().Name + ": " + e);
			}
		}
		return callback;
	}
	
	public static EventDispatcher getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly EventDispatcher INSTANCE = new EventDispatcher();
	}
}