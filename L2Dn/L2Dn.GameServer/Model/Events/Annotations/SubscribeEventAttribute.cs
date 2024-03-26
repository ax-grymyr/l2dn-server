namespace L2Dn.GameServer.Model.Events.Annotations;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SubscribeEventAttribute(SubscriptionType type, params int[] ids): IdListAttribute(ids)
{
    public SubscribeEventAttribute(SubscriptionType type)
        : this(type, Array.Empty<int>())
    {
    }

    public SubscriptionType Type { get; } = type;
}