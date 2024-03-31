using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.GrandCrusade, Chronicles.Homunculus2 - 1)]
public sealed class CombinationItemData
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CombinationItemDataRecord[] Records { get; set; } = Array.Empty<CombinationItemDataRecord>();

    public sealed class CombinationItemDataRecord
    {
        public uint Slot1 { get; set; }
        public uint Slot2 { get; set; }
        public ResultItem[] ResultItems { get; set; } = Array.Empty<ResultItem>();
        public byte ResultEffectType { get; set; }
        public uint[] ApplyCountry { get; set; } = Array.Empty<uint>();
    }

    public sealed class ResultItem
    {
	    public uint ItemId { get; set; }
	    public uint Count { get; set; }
    }
}