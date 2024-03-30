using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.PreludeOfWar3, Chronicles.ReturnOfTheQueenAnt - 1)]
public sealed class CharCreateGrpV6
{
    [ArrayLengthType(ArrayLengthType.Fixed, 85)]
    public CharCreateGrpRecord[] Records { get; set; } = Array.Empty<CharCreateGrpRecord>();

    public sealed class CharCreateGrpRecord
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public uint Chest { get; set; }
        public uint Legs { get; set; }
        public uint Gloves { get; set; }
        public uint Feet { get; set; }
        public uint RHand { get; set; }
        public uint LHand { get; set; }
        
        [StringType(StringType.NameDataIndex)]
        public string IntroAnimName { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)]
        public string IntroWaitAnimName { get; set; } = string.Empty;
    }
}