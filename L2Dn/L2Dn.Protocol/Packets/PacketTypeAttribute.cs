namespace L2Dn.Packets;

public sealed class PacketTypeAttribute: Attribute
{
    public PacketTypeAttribute(Type packetType)
    {
        PacketType = packetType;
    }

    public PacketTypeAttribute(Type packetType, object allowedState)
    {
        PacketType = packetType;

        if (allowedState is long value)
            AllowedState = value;
        else if (allowedState.GetType().IsEnum)
            AllowedState = Convert.ToInt64(allowedState);
        else
            throw new ArgumentException("State must be of long or enum type");
    }

    public Type PacketType { get; }

    public long? AllowedState { get; }
}