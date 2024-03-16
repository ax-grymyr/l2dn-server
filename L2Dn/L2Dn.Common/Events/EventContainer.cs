using System.Collections.Concurrent;

namespace L2Dn.Events;

public abstract class EventContainer(EventContainer? parent = null)
{
    private readonly ConcurrentDictionary<Type, object> _events = new();
    
    public void Subscribe<TEvent>(object owner, Action<TEvent> callback)
        where TEvent: EventBase
    {
        ArgumentNullException.ThrowIfNull(owner, nameof(owner));        
        SubscriberList<TEvent> list = GetOrCreateSubscriberList<TEvent>();
        list.Subscribe(owner, callback);
    }

    public void Subscribe<TEvent>(Action<TEvent> callback)
        where TEvent: EventBase
    {
        ArgumentNullException.ThrowIfNull(owner, nameof(owner));        
        SubscriberList<TEvent> list = GetOrCreateSubscriberList<TEvent>();
        list.Subscribe(callback);
    }

    void Unsubscribe<TEvent>(object owner)
        where TEvent: EventBase;

    void Unsubscribe<TEvent>(Action<TEvent> callback)
        where TEvent: EventBase;

    bool HasSubscribers<TEvent>()
        where TEvent: EventBase;

    private SubscriberList<TEvent> GetOrCreateSubscriberList<TEvent>()
        where TEvent: EventBase =>
        (SubscriberList<TEvent>)_events.GetOrAdd(typeof(TEvent), t => new SubscriberList<TEvent>());
}