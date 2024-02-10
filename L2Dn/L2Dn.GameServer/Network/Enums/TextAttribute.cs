namespace L2Dn.GameServer.Network.Enums;

public sealed class TextAttribute(string text): Attribute
{
    public string Text { get; } = text;
}