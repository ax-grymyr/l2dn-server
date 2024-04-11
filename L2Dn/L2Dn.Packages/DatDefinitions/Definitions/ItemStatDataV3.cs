using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.MasterClass2 - 1)]
public sealed class ItemStatDataV3
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemStatDataRecord[] Records { get; set; } = [];

    public sealed class ItemStatDataRecord
    {
        public uint ItemId { get; set; }
        public ushort PDefense { get; set; }
        public ushort MDefense { get; set; }
        public ushort PAttack { get; set; }
        public ushort MAttack { get; set; }
        public ushort PAttackSpeed { get; set; }
        public float PHit { get; set; }
        public float MHit { get; set; }
        public float PCritical { get; set; }
        public float MCritical { get; set; }
        public ushort Speed { get; set; }
        public ushort ShieldDefense { get; set; }
        public byte ShieldDefenseRate { get; set; }
        public float PAvoid { get; set; }
        public float MAvoid { get; set; }
        public ushort Params { get; set; }
    }
}