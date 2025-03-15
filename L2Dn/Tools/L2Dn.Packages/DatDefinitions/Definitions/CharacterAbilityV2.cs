using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass, Chronicles.Latest)]
public sealed class CharacterAbilityV2
{
    public CharacterAbilityRecord[] Records { get; set; } = Array.Empty<CharacterAbilityRecord>();

    public sealed class CharacterAbilityRecord
    {
        public IndexedString Category { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte Unknown { get; set; }
        public string Detail { get; set; } = string.Empty;
    }
}