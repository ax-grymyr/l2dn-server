namespace L2Dn.CustomAttributes;

public sealed class TextAttribute(string text): Attribute
{
    public string Text { get; } = text;
}