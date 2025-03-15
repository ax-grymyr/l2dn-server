using System.Collections.Immutable;

namespace L2Dn.Packages.DatDefinitions;

public record DatFileDefinition(string FileNamePattern, Type DataType)
{
    public static ImmutableDictionary<Chronicles, ImmutableList<DatFileDefinition>> Definitions { get; } =
        CreateDefinitions();

    private static ImmutableDictionary<Chronicles, ImmutableList<DatFileDefinition>> CreateDefinitions()
    {
        Dictionary<Chronicles, ImmutableList<DatFileDefinition>> dictionary = new();

        return dictionary.ToImmutableDictionary();
    }
}