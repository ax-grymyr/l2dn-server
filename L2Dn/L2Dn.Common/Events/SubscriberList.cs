using System.Runtime.CompilerServices;
using NLog;

namespace L2Dn.Events;

internal abstract class SubscriberList
{
    public abstract void UnsubscribeAll(object owner);
}

internal sealed class SubscriberList<TArg>: SubscriberList
    where TArg: EventBase
{
    // TODO: think about weak events
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SubscriberList<EventBase>));
    private const int ArraySize = 8;
    private SubscriberArray _array = default;
    private Node? _node;
    private int _subscriberCount;

    public int SubscriberCount => _subscriberCount;

    public void Subscribe(object owner, Action<TArg> action)
    {
        Interlocked.Increment(ref _subscriberCount);
        if (AddSubscriberToArray(_array, owner, action))
            return;

        Node node = GetOrCreateNode(ref _node);
        while (!AddSubscriberToArray(node.Array, owner, action))
            node = GetOrCreateNode(ref node.Next);
    }

    public override void UnsubscribeAll(object owner)
    {
        int removedCount = RemoveByOwnerFromArray(_array, owner);
        for (Node? node = _node; node != null; node = node.Next)
            removedCount += RemoveByOwnerFromArray(node.Array, owner);

        if (removedCount != 0)
            Interlocked.Add(ref _subscriberCount, -removedCount);
    }

    public void Unsubscribe(Action<TArg> action)
    {
        int removedCount = RemoveByDelegateFromArray(_array, action);
        for (Node? node = _node; node != null; node = node.Next)
            removedCount += RemoveByDelegateFromArray(node.Array, action);

        if (removedCount != 0)
            Interlocked.Add(ref _subscriberCount, -removedCount);
    }

    public bool Notify(TArg arg)
    {
        if (arg.Abort)
            return false;

        bool result = NotifySubscribersInArray(_array, arg);
        for (Node? node = _node; node != null && !arg.Abort; node = node.Next)
            result |= NotifySubscribersInArray(node.Array, arg);

        return result;
    }

    public bool HasListeners => _subscriberCount > 0;

    private static bool NotifySubscribersInArray(Span<SubscriberArrayItem> span, TArg arg)
    {
        bool result = false;
        for (int i = 0; i < span.Length; i++)
        {
            Action<TArg>? action = span[i].Action;
            if (action is not null)
            {
                result = true;
                try
                {
                    action(arg);
                }
                catch (Exception exception)
                {
                    _logger.Error(exception, "Exception in event listener");
                }

                if (arg.Abort)
                    break;
            }
        }

        return result;
    }

    private static int RemoveByDelegateFromArray(Span<SubscriberArrayItem> span, Action<TArg> action)
    {
        int count = 0;
        for (int i = 0; i < ArraySize; i++)
        {
            ref SubscriberArrayItem itemRef = ref span[i];
            Action<TArg>? itemAction = itemRef.Action;
            object? owner = itemRef.Owner;
            if (owner != null && itemAction != null && ReferenceEquals(itemAction.Target, action.Target) &&
                ReferenceEquals(itemAction.Method, action.Method) &&
                Interlocked.CompareExchange(ref itemRef.Owner, null, owner) == owner)
            {
                itemRef.Action = null;
                count++;
            }
        }

        return count;
    }

    private static int RemoveByOwnerFromArray(Span<SubscriberArrayItem> span, object owner)
    {
        int count = 0;
        for (int i = 0; i < ArraySize; i++)
        {
            ref SubscriberArrayItem itemRef = ref span[i];
            if (ReferenceEquals(owner, itemRef.Owner) &&
                Interlocked.CompareExchange(ref itemRef.Owner, null, owner) == owner)
            {
                itemRef.Action = null;
                count++;
            }
        }

        return count;
    }

    private static bool AddSubscriberToArray(Span<SubscriberArrayItem> span, object owner, Action<TArg> action)
    {
        for (int i = 0; i < span.Length; i++)
        {
            ref SubscriberArrayItem itemRef = ref span[i];
            if (itemRef.Owner is null &&
                Interlocked.CompareExchange(ref itemRef.Owner, owner, null) == null)
            {
                itemRef.Action = action;
                return true;
            }
        }

        return false;
    }

    private static Node GetOrCreateNode(ref Node? node)
    {
        Node? newNode = node;
        if (newNode == null)
        {
            newNode = new Node();
            newNode = Interlocked.CompareExchange(ref node, newNode, null) ?? newNode;
        }

        return newNode;
    }

    private struct SubscriberArrayItem
    {
        public object? Owner;
        public Action<TArg>? Action;
    }

    [InlineArray(ArraySize)]
    private struct SubscriberArray
    {
        public SubscriberArrayItem Items;
    }

    private sealed class Node
    {
        public SubscriberArray Array = default;
        public Node? Next;
    }
}