using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Shinemaker, Chronicles.Latest)]
public sealed class LCoinShopProductV6
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public LCoinShopProductRecord[] Records { get; set; } = Array.Empty<LCoinShopProductRecord>();

    public LCoinShopProductHead[] Heads { get; set; } = Array.Empty<LCoinShopProductHead>();

    public sealed class LCoinShopProductRecord
    {
        public ushort ProductId { get; set; }
        public byte Category { get; set; }
        public LCoinShopProductMarkType MarkType { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public LCoinShopProductBuyItem[] BuyItems { get; set; } = Array.Empty<LCoinShopProductBuyItem>();

        public byte ProductType { get; set; }
        public byte LimitType { get; set; }
        public LCoinResetType ResetType { get; set; }
        public uint LimitCountMax { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductHtml { get; set; } = string.Empty;
    }

    public sealed class LCoinShopProductBuyItem
    {
        public uint ProductItemId { get; set; }
        public uint ProductCount { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public ushort LimitMin { get; set; }
        public ushort LimitMax { get; set; }
    }    
    
    public sealed class LCoinShopProductHead
    {
        public short HeadProductId { get; set; }
        public string HeadName { get; set; } = string.Empty;
    }
}