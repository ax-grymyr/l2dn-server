using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.GrandCrusade - 1)]
public sealed class CastleNameV3
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CastleNameRecord[] Records { get; set; } = Array.Empty<CastleNameRecord>();

    public sealed class CastleNameRecord
    {
        public uint Number { get; set; }
        public uint Tag { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        [StringType(StringType.NameDataIndex)]
        public string Mark { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)]
        public string MarkGray { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)]
        public string FlagIcon { get; set; } = string.Empty;

        public string MerchantName { get; set; } = string.Empty;
    }
}