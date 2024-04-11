using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Awakening, Chronicles.Underground - 1)]
public sealed class ItemStatData
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemStatDataRecord[] Records { get; set; } = [];

    public sealed class ItemStatDataRecord
    {
        public uint ItemId { get; set; }
        public float PDefense { get; set; }
        public float MDefense { get; set; }
        public float PAttack { get; set; }
        public float MAttack { get; set; }
        public float PAttackSpeed { get; set; }
        public float MAttackSpeed { get; set; }
        public float PHit { get; set; }
        public float MHit { get; set; }
        public float PCritical { get; set; }
        public float MCritical { get; set; }
        public float Speed { get; set; }
        public float ShieldDefense { get; set; }
        public float ShieldDefenseRate { get; set; }
        public float PAvoid { get; set; }
        public float MAvoid { get; set; }
        public float Params { get; set; }
    }
}