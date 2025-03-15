using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Epilogue, Chronicles.Awakening - 1)]
public sealed class CharCreateGrp
{
    [ArrayLengthType(ArrayLengthType.Fixed, 20)]
    public CharCreateGrpRecord[] Records { get; set; } = Array.Empty<CharCreateGrpRecord>();

    public sealed class CharCreateGrpRecord
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Unknown { get; set; }
        public uint Chest { get; set; }
        public uint Legs { get; set; }
        public uint Gloves { get; set; }
        public uint Feet { get; set; }
        public uint RHand { get; set; }
        public uint LHand { get; set; }
    }
}