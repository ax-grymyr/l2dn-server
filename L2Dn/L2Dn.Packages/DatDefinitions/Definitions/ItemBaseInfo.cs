using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Ertheia, Chronicles.Salvation - 1)]
public sealed class ItemBaseInfo
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemBaseInfoRecord[] Records { get; set; } = [];

    public sealed class ItemBaseInfoRecord
    {
        public uint ItemId { get; set; }
        public long DefaultPrice { get; set; }
        public uint IsPremium { get; set; }
    }
}