using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.TheSourceOfFlame, Chronicles.Latest)]
public sealed class AdditionalNpcGrpPartsV4
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalNpcGrpPartsRecord[] Records { get; set; } = Array.Empty<AdditionalNpcGrpPartsRecord>();

    public sealed class AdditionalNpcGrpPartsRecord
    {
        public uint NpcId { get; set; }
        public uint Class { get; set; }
        public uint Chest { get; set; }
        public uint Legs { get; set; }
        public uint Gloves { get; set; }
        public uint Feet { get; set; }
        public uint Back { get; set; }
        public uint HairAcc { get; set; }
        public uint HairStyle { get; set; }
        public uint RHand { get; set; }
        public uint LHand { get; set; }
        public uint Beast { get; set; }
        public uint WeaponEnchant { get; set; }
        public uint ArmorEnchant { get; set; }
    }
}