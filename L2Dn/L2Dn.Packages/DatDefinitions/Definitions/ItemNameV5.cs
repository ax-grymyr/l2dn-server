using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Awakening, Chronicles.Valiance - 1)]
public sealed class ItemNameV5
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

        [ArrayLengthType(ArrayLengthType.Int32)]
        public ItemNameClass[] NameClasses { get; set; } = Array.Empty<ItemNameClass>();
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public string[] SetIds { get; set; } = Array.Empty<string>();

        [ArrayLengthType(ArrayLengthType.Int32)]
        public ItemNameClass[] NameClasses2 { get; set; } = Array.Empty<ItemNameClass>();

        [ArrayLengthType(ArrayLengthType.Int32)]
        [StringType(StringType.Utf16)]
        public string[] SetExtraIds { get; set; } = Array.Empty<string>();

        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        public uint SpecialEnchantAmount { get; set; }
        public string SpecialEnchantDesc { get; set; } = string.Empty;
        public uint Color { get; set; }
    } 

    public sealed class ItemNameClass
    {
        [ArrayLengthType(ArrayLengthType.Int32)]
        public uint[] NameClassSubs { get; set; } = Array.Empty<uint>();
    }
}