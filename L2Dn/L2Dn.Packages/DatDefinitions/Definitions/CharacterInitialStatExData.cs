using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Salvation, Chronicles.Latest)]
public sealed class CharacterInitialStatExData
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CharacterInitialStatExDataRecord[] Records { get; set; } = Array.Empty<CharacterInitialStatExDataRecord>();

    public sealed class CharacterInitialStatExDataRecord
    {
        public CharacterClass Class { get; set; }
        public Race Race { get; set; }
        public Sex Sex { get; set; }
        public ushort Str { get; set; }
        public ushort Dex { get; set; }
        public ushort Con { get; set; }
        public ushort Int { get; set; }
        public ushort Wit { get; set; }
        public ushort Men { get; set; }
        public ushort Luc { get; set; }
        public ushort Cha { get; set; }
    }
}