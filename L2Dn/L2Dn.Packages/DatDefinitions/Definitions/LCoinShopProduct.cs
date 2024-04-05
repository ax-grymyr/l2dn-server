using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.PreludeOfWar3, Chronicles.Homunculus - 1)]
public sealed class LCoinShopProduct
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public LCoinShopProductRecord[] Records { get; set; } = Array.Empty<LCoinShopProductRecord>();

    public LCoinShopProductHead[] Heads { get; set; } = Array.Empty<LCoinShopProductHead>();

    public sealed class LCoinShopProductRecord
    {
        public ushort ProductId { get; set; }
        public byte Category { get; set; }
        public byte MarkType { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public uint ProductItem { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public LCoinShopProductBuyItem[] BuyItems { get; set; } = Array.Empty<LCoinShopProductBuyItem>();

        public short LimitMin { get; set; }
        public short LimitMax { get; set; }
        public byte ProductType { get; set; }
        public byte PopularPriority { get; set; }
        public byte BuyType1 { get; set; }
        public byte BuyType2 { get; set; }
        public uint BuyType3 { get; set; }
        public uint BuyType4 { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductHtml { get; set; } = string.Empty;
    }

    public sealed class LCoinShopProductBuyItem
    {
        public uint ItemId { get; set; }
        public uint Count { get; set; }
        public float Chance { get; set; }
    }    
    
    public sealed class LCoinShopProductHead
    {
        public short HeadId { get; set; }
        public string HeadLine { get; set; } = string.Empty;
    }
}