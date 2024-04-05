using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ReturnOfTheQueenAnt2, Chronicles.MasterClass - 1)]
public sealed class LCoinShopProductV4
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public LCoinShopProductRecord[] Records { get; set; } = Array.Empty<LCoinShopProductRecord>();

    public LCoinShopProductHead[] Heads { get; set; } = Array.Empty<LCoinShopProductHead>();

    public sealed class LCoinShopProductRecord
    {
        public ushort ProductId { get; set; }
        public byte Category { get; set; }
        public byte MarkType { get; set; } // enum lcoinshopproduct_mark_type
        public byte MaxBuyCount { get; set; }
        public uint ProductItem { get; set; }
        public uint ProductCount { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public short LimitMin { get; set; }
        public short LimitMax { get; set; }

        [ArrayLengthType(ArrayLengthType.Fixed, 12)]
        public byte[] Unknown1 { get; set; } = Array.Empty<byte>();

        public byte[] Unknown2 { get; set; } = Array.Empty<byte>();
    }
    
    public sealed class LCoinShopProductHead
    {
        public short HeadId { get; set; }
        public string HeadLine { get; set; } = string.Empty;
    }
}