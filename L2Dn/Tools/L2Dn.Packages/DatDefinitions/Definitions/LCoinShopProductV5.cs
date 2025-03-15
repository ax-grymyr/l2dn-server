using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass, Chronicles.Shinemaker - 1)]
public sealed class LCoinShopProductV5
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public LCoinShopProductRecord[] Records { get; set; } = Array.Empty<LCoinShopProductRecord>();

    public LCoinShopProductHead[] Heads { get; set; } = Array.Empty<LCoinShopProductHead>();

    public sealed class LCoinShopProductRecord
    {
        public ushort ProductId { get; set; }
        public byte Category { get; set; }
        public byte MarkType { get; set; } // enum lcoinshopproduct_mark_type

        [ArrayLengthType(ArrayLengthType.Byte)]
        public LCoinShopProductBuyItem[] BuyItems { get; set; } = Array.Empty<LCoinShopProductBuyItem>();

        public byte ProductType { get; set; }
        public byte LimitType { get; set; }
        public byte ResetType { get; set; } // enum lcoin_reset_type
        public uint LimitCountMax { get; set; }
        public uint ServerCountMax { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductHtml { get; set; } = string.Empty;
    }

    public sealed class LCoinShopProductBuyItem
    {
        public uint ProductItemId { get; set; }
        public uint ProductCount { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public short LimitMin { get; set; }
        public short LimitMax { get; set; }
    }    
    
    public sealed class LCoinShopProductHead
    {
        public short HeadProductId { get; set; }
        public string HeadName { get; set; } = string.Empty;
    }
}