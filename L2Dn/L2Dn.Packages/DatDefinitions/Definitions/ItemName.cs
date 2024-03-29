using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Interlude - 1)]
public sealed class ItemName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemNameRecord[] Records { get; set; } = Array.Empty<ItemNameRecord>();
    
    public sealed class ItemNameRecord
    {
        public uint Id { get; set; }
        
        [StringType(StringType.Utf16)]
        public string Name { get; set; } = string.Empty;

        [StringType(StringType.Utf16)]
        public string AdditionalName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int Popup { get; set; }
    } 
}