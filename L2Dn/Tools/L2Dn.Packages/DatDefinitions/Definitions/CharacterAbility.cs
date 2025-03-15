using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ReturnOfTheQueenAnt, Chronicles.MasterClass - 1)]
public sealed class CharacterAbility
{
    public CharacterAbilityRecord[] Records { get; set; } = Array.Empty<CharacterAbilityRecord>();

    public sealed class CharacterAbilityRecord
    {
        public IndexedString Category { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte Unknown { get; set; }
    }
}