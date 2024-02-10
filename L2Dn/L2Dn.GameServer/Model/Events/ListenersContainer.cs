using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Events;

/**
 * @author UnAfraid
 */
public class ListenersContainer
{
	private Map<EventType, Set<AbstractEventListener>>? _listeners;
	
	/**
	 * Registers listener for a callback when specified event is executed.
	 * @param listener
	 * @return
	 */
	public AbstractEventListener addListener(AbstractEventListener listener)
	{
		if (listener is null)
			throw new ArgumentNullException(nameof(listener), "Listener cannot be null!");
		
		getListeners().computeIfAbsent(listener.getType(), k => new()).add(listener);
		return listener;
	}
	
	/**
	 * Unregisters listener for a callback when specified event is executed.
	 * @param listener
	 * @return
	 */
	public AbstractEventListener removeListener(AbstractEventListener listener)
	{
		if (listener is null)
			throw new ArgumentNullException(nameof(listener), "Listener cannot be null!");

		if (_listeners is null)
			throw new InvalidOperationException("Listeners container is not initialized!");

		if (!_listeners.ContainsKey(listener.getType()))
		{
			throw new InvalidOperationException("Listeners container doesn't had " + listener.getType() + " event type added!");
		}

		_listeners[listener.getType()].remove(listener);
		return listener;
	}
	
	public void removeListenerIf(EventType type, Predicate<AbstractEventListener> filter)
	{
		if (_listeners == null)
		{
			return;
		}
		
		foreach (AbstractEventListener listener in getListeners(type))
		{
			if (filter(listener))
			{
				listener.unregisterMe();
			}
		}
	}
	
	public void removeListenerIf(Predicate<AbstractEventListener> filter)
	{
		if (_listeners is null)
		{
			return;
		}
		
		foreach (Set<AbstractEventListener> queue in getListeners().Values)
		{
			foreach (AbstractEventListener listener in queue)
			{
				if (filter(listener))
				{
					listener.unregisterMe();
				}
			}
		}
	}

	public bool hasListener(EventType type)
	{
		var listeners = _listeners;
		return listeners is not null && listeners.TryGetValue(type, out var queue) && queue.Count > 0;
	}

	/**
	 * @param type
	 * @return {@code List} of {@link AbstractEventListener} by the specified type
	 */
	public Set<AbstractEventListener> getListeners(EventType type)
	{
		return _listeners != null && _listeners.TryGetValue(type, out var listeners)
			? listeners
			: new();
	}

	/**
	 * Creates the listeners container map if doesn't exists.
	 * @return the listeners container map.
	 */
	private Map<EventType, Set<AbstractEventListener>> getListeners()
	{
		if (_listeners is null)
		{
			lock (this)
			{
				if (_listeners == null)
				{
					_listeners = new();
				}
			}
		}
		
		return _listeners;
	}
}