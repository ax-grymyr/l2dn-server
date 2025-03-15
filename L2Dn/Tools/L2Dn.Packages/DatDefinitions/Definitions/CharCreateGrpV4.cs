using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Salvation, Chronicles.PreludeOfWar2 - 1)]
public sealed class CharCreateGrpV4
{
    [ArrayLengthType(ArrayLengthType.Fixed, 82)]
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
    }
}