using System.Collections.Immutable;

namespace L2Dn.Packages.DatDefinitions;

public readonly struct IndexedString
{
    private readonly ImmutableArray<string> _strings;
    private readonly int _index;

    public IndexedString(ImmutableArray<string> strings, int index)
    {
        if (strings.IsDefaultOrEmpty)
            throw new ArgumentException(nameof(strings));
        
        if (index < 0 || index >= strings.Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        _strings = strings;
        _index = index + 1;
    }

    public int Index => _index - 1;
    public ImmutableArray<string> Strings => _strings;

    public static implicit operator string(IndexedString s) => s.ToString();
    public override string ToString() => _index == 0 ? string.Empty : _strings[_index - 1];
}