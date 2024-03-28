namespace L2Dn.GameServer.Network.Enums;

public readonly struct SystemMessageParam(SystemMessageParamType type, object value)
{
    public SystemMessageParamType Type { get; } = type;
    public object Value { get; } = value; // TODO: remove boxing
}