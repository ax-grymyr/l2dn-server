using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace L2Dn.Events;

public sealed class EventContainer(string containerName, EventContainer? parent = null)
{
    private readonly ConcurrentDictionary<Type, object> _subscribers = new();

    public string Name => containerName;
    public EventContainer? Parent => parent;

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
        ArgumentNullException.ThrowIfNull(callback.Target, nameof(callback.Target));
        object owner = callback.Target;
        SubscriberList<TEvent> list = GetOrCreateSubscriberList<TEvent>();
        list.Subscribe(owner, callback);
    }

    public void UnsubscribeAll<TEvent>(object owner)
        where TEvent: EventBase
    {
        if (TryGetList<TEvent>(out SubscriberList<TEvent>? list))
            list.UnsubscribeAll(owner);
    }

    public void UnsubscribeAllTypes(object owner)
    {
        foreach (SubscriberList list in _subscribers.Values)
            list.UnsubscribeAll(owner);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> callback)
        where TEvent: EventBase
    {
        if (TryGetList<TEvent>(out SubscriberList<TEvent>? list))
            list.Unsubscribe(callback);
    }

    public bool HasSubscribers<TEvent>()
        where TEvent: EventBase
    {
        if (TryGetList<TEvent>(out SubscriberList<TEvent>? list) && list.HasListeners)
            return true;

        return parent is not null && parent.HasSubscribers<TEvent>();
    }

    public bool Notify<TEvent>(TEvent ev)
        where TEvent: EventBase
    {
        bool result = false;
        if (TryGetList<TEvent>(out SubscriberList<TEvent>? list))
            result = list.Notify(ev);

        if (parent is not null)
            result |= parent.Notify(ev);

        return result;
    }

    public void NotifyAsync<TEvent>(TEvent ev)
        where TEvent: EventBase
    {
        ThreadPool.QueueUserWorkItem(state => Notify((TEvent)state!), ev);
    }

    public override string ToString() => $"Event Container: {containerName}";

    private bool TryGetList<TEvent>([NotNullWhen(true)] out SubscriberList<TEvent>? list)
        where TEvent: EventBase
    {
        bool result = _subscribers.TryGetValue(typeof(TEvent), out object? obj);
        list = (SubscriberList<TEvent>?)obj;
        return result;
    }

    private SubscriberList<TEvent> GetOrCreateSubscriberList<TEvent>()
        where TEvent: EventBase
    {
        return (SubscriberList<TEvent>)_subscribers.GetOrAdd(typeof(TEvent), _ => new SubscriberList<TEvent>());
    }
}