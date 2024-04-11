using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Salvation, Chronicles.PreludeOfWar3 - 1)]
public sealed class ItemBaseInfoV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemBaseInfoRecord[] Records { get; set; } = [];

    public sealed class ItemBaseInfoRecord
    {
        public uint ItemId { get; set; }
        public long DefaultPrice { get; set; }
        public byte IsLocked { get; set; }
    }
}