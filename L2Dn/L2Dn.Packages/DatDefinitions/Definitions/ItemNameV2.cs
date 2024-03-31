using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Interlude, Chronicles.Epilogue - 1)]
public sealed class ItemNameV2
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
        public string SetIds { get; set; } = string.Empty;
        public string SetBonusDesc { get; set; } = string.Empty;
        public string SetExtraId { get; set; } = string.Empty;
        public string SetExtraDesc { get; set; } = string.Empty;
        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }
        public uint SetEnchantCount { get; set; }
        public string SetEnchantEffect { get; set; } = string.Empty;
    } 
}