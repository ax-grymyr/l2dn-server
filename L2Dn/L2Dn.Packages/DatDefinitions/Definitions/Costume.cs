using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Fafurion, Chronicles.Latest)]
public sealed class Costume
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CostumeRecord[] Records { get; set; } = Array.Empty<CostumeRecord>();

    public sealed class CostumeRecord
    {
        public uint Id { get; set; }
        public uint SkillId { get; set; }
        public uint SkillLevel { get; set; }
        public uint Preview { get; set; }
        public ExtractFee[] ExtractFees { get; set; } = Array.Empty<ExtractFee>();
        public uint NeedItemId { get; set; }
        public uint NeedItemCount { get; set; }
        public uint EvolutionNeedCostume { get; set; }
        public uint OriginType { get; set; }
        public uint Grade { get; set; }
        public uint[] ApplyCountry { get; set; } = Array.Empty<uint>();
    }

    public sealed class ExtractFee
    {
	    public int ItemId { get; set; }
	    public int ItemCount { get; set; }
    }
}