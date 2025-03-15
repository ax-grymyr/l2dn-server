using System.Runtime.CompilerServices;
using NLog;

namespace L2Dn.Events;

internal abstract class SubscriberList
{
    protected static readonly object DummyOwner = new();

    public abstract void UnsubscribeAll(object owner);
}

internal sealed class SubscriberList<TArg>: SubscriberList
    where TArg: EventBase
{
    // TODO: think about weak events
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SubscriberList<EventBase>));
    private const int _arraySize = 8;
    private SubscriberArray _array;
    private Node? _node;
    private int _count;

    public void Subscribe(object owner, Action<TArg> action)
    {
        Interlocked.Increment(ref _count);
        if (AddSubscriberToArray(ref _array, owner, action))
            return;

        Node node = GetOrCreateNode(ref _node);
        while (!AddSubscriberToArray(ref node.Array, owner, action))
            node = GetOrCreateNode(ref node.Next);
    }

    public override void UnsubscribeAll(object owner)
    {
        int removedCount = RemoveByOwnerFromArray(ref _array, owner);
        for (Node? node = _node; node != null; node = node.Next)
            removedCount += RemoveByOwnerFromArray(ref node.Array, owner);

        if (removedCount != 0)
            Interlocked.Add(ref _count, -removedCount);
    }

    public void Unsubscribe(Action<TArg> action)
    {
        int removedCount = RemoveByDelegateFromArray(ref _array, action);
        for (Node? node = _node; node != null; node = node.Next)
            removedCount += RemoveByDelegateFromArray(ref node.Array, action);

        if (removedCount != 0)
            Interlocked.Add(ref _count, -removedCount);
    }

    public bool Notify(TArg arg)
    {
        if (arg.Abort)
            return false;

        bool result = NotifySubscribersInArray(ref _array, arg);
        for (Node? node = _node; node != null && !arg.Abort; node = node.Next)
            result |= NotifySubscribersInArray(ref node.Array, arg);

        return result;
    }

    public bool HasListeners => _count != 0;

    private static bool NotifySubscribersInArray(ref SubscriberArray array, TArg arg)
    {
        bool result = false;
        for (int i = 0; i < _arraySize; i++)
        {
            Action<TArg>? action = array[i].Action;
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

    private static int RemoveByDelegateFromArray(ref SubscriberArray array, Action<TArg> action)
    {
        int count = 0;
        for (int i = 0; i < _arraySize; i++)
        {
            ref SubscriberArrayItem itemRef = ref array[i];
            Action<TArg>? itemAction = itemRef.Action;
            object? owner = itemRef.Owner;

            if (owner == null || itemAction == null)
                continue;

            if (!ReferenceEquals(itemAction.Target, action.Target) ||
                !ReferenceEquals(itemAction.Method, action.Method))
            {
                continue;
            }

            if (Interlocked.CompareExchange(ref itemRef.Action, null, itemAction) != itemAction)
                continue;

            itemRef.Action = null;
            itemRef.Owner = null;
            count++;
        }

        return count;
    }

    private static int RemoveByOwnerFromArray(ref SubscriberArray array, object owner)
    {
        int count = 0;
        for (int i = 0; i < _arraySize; i++)
        {
            ref SubscriberArrayItem itemRef = ref array[i];
            if (!ReferenceEquals(itemRef.Owner, owner))
                continue;

            // Owner is assigned to DummyOwner to make sure another thread will not set the item with
            // a new owner and action before action is set to null.
            if (Interlocked.CompareExchange(ref itemRef.Owner, DummyOwner, owner) != owner)
                continue;

            itemRef.Action = null;
            itemRef.Owner = null;
            count++;
        }

        return count;
    }

    private static bool AddSubscriberToArray(ref SubscriberArray array, object owner, Action<TArg> action)
    {
        for (int i = 0; i < _arraySize; i++)
        {
            ref SubscriberArrayItem itemRef = ref array[i];
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
        if (node != null)
            return node;

        Node newNode = new Node();
        return Interlocked.CompareExchange(ref node, newNode, null) ?? newNode;
    }

    private struct SubscriberArrayItem
    {
        // If Owner is null, the cell is free and can be filled by owner/action pair.
        // Therefore, when adding the subscriber, owner is assigned first using Interlocked.CompareExchange,
        // and only then action is assigned.

        // Removal occurs in the opposite order: action is assigned to null first.

        public object? Owner;
        public Action<TArg>? Action;
    }

    [InlineArray(_arraySize)]
    private struct SubscriberArray
    {
        public SubscriberArrayItem Items;
    }

    private sealed class Node
    {
        public SubscriberArray Array;
        public Node? Next;
    }
}