using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.EtinasFate, Chronicles.Latest)]
public sealed class CardCollectData
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CardCollectDataRecord[] Records { get; set; } = Array.Empty<CardCollectDataRecord>();

    public sealed class CardCollectDataRecord
    {
        public uint EventId { get; set; }
        public uint CollectGroupId { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] CardIds { get; set; } = Array.Empty<uint>();
        
        [ArrayLengthType(ArrayLengthType.Byte)]
        [StringType(StringType.NameDataIndex)]
        public string[] CardRewardTextures { get; set; } = Array.Empty<string>();
        
        [StringType(StringType.NameDataIndex)]
        public string SubTitle { get; set; } = string.Empty;
    }
}