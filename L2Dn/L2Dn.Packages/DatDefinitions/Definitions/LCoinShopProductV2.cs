using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Homunculus, Chronicles.ReturnOfTheQueenAnt - 1)]
public sealed class LCoinShopProductV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public LCoinShopProductRecord[] Records { get; set; } = Array.Empty<LCoinShopProductRecord>();

    public LCoinShopProductHead[] Heads { get; set; } = Array.Empty<LCoinShopProductHead>();

    public sealed class LCoinShopProductRecord
    {
        public byte ShopIndex { get; set; }
        public ushort ProductId { get; set; }
        public byte Filter { get; set; } // enum lcoinshopproduct_filter
        public byte Category { get; set; }
        public byte MarkType { get; set; } // enum lcoinshopproduct_mark_type
        public byte MaxBuyCount { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public uint ProductItem { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public LCoinShopProductBuyItem[] BuyItems { get; set; } = Array.Empty<LCoinShopProductBuyItem>();

        public short LimitMin { get; set; }
        public short LimitMax { get; set; }
        public byte ProductType { get; set; }
        public byte PopularPriority { get; set; }
        public byte LimitType { get; set; } // enum lcoinshopproduct_buy1_type
        public byte ResetType { get; set; } // enum lcoinshopproduct_buy2_type
        public uint LimitCountMax { get; set; }
        public uint ServerCountMax { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductHtml { get; set; } = string.Empty;
    }

    public sealed class LCoinShopProductBuyItem
    {
        public uint ItemClassId { get; set; }
        public uint Count { get; set; }
        public float Chance { get; set; }
        public byte ProductRank { get; set; }
    }    
    
    public sealed class LCoinShopProductHead
    {
        public short HeadId { get; set; }
        public string HeadLine { get; set; } = string.Empty;
    }
}