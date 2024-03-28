namespace L2Dn.GameServer.Network.Enums;

public sealed class SystemMessageInfo(SystemMessageId messageId, string text, int paramCount)
{
	public SystemMessageId MessageId { get; } = messageId;
	public string Text { get; } = text;
	public int ParamCount { get; } = paramCount;
}