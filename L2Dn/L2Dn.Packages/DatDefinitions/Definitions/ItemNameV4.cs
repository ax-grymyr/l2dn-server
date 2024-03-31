using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.HighFive, Chronicles.Awakening - 1)]
public sealed class ItemNameV4
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemNameRecord[] Records { get; set; } = Array.Empty<ItemNameRecord>();
    
    public sealed class ItemNameRecord
    {
        public uint Id { get; set; }
        
        [StringType(StringType.Utf16Le)]
        public string Name { get; set; } = string.Empty;

        [StringType(StringType.Utf16Le)]
        public string AdditionalName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int Popup { get; set; }
        public uint SuperCnt0 { get; set; }

        [ArrayLengthType(ArrayLengthType.Int32)]
        [StringType(StringType.Utf16Le)]
        public string[] SetIds { get; set; } = Array.Empty<string>();
        
        public string SetBonusDesc { get; set; } = string.Empty;
        
        public uint SuperCnt1 { get; set; }

        [ArrayLengthType(ArrayLengthType.Int32)]
        [StringType(StringType.Utf16Le)]
        public string[] SetExtraIds { get; set; } = Array.Empty<string>();
        
        public string SetExtraDesc { get; set; } = string.Empty;

        [ArrayLengthType(ArrayLengthType.Fixed, 9)]
        public byte[] Unknown { get; set; } = Array.Empty<byte>();

        public uint SpecialEnchantAmount { get; set; }
        public string SpecialEnchantDesc { get; set; } = string.Empty;
        public uint Color { get; set; }
    } 
}