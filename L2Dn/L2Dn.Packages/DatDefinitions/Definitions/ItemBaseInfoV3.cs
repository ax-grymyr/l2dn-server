using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.PreludeOfWar3, Chronicles.MasterClass2 - 1)]
public sealed class ItemBaseInfoV3
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemBaseInfoRecord[] Records { get; set; } = [];

    public sealed class ItemBaseInfoRecord
    {
        public uint ItemId { get; set; }
        public long DefaultPrice { get; set; }
        public uint GrindPoint { get; set; }
        public uint GrindCommission { get; set; }
        public byte IsLocked { get; set; }
    }
}