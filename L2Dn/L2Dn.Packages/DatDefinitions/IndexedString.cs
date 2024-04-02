namespace L2Dn.Packages.DatDefinitions;

public readonly struct IndexedString(string text, int index)
{
    private readonly string? _text = text;
    private readonly int _index = index + 1;

    public int Index => _index - 1;
    public string Text => _text ?? string.Empty;

    public override string ToString() => _text ?? string.Empty;
}