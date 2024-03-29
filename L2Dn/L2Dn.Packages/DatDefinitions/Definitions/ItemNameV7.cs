using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Ertheia, Chronicles.Underground - 1)]
public sealed class ItemNameV7
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemNameRecord[] Records { get; set; } = Array.Empty<ItemNameRecord>();

    public sealed class ItemNameRecord
    {
        public uint Id { get; set; }

        [StringType(StringType.Utf16)] 
        public string Name { get; set; } = string.Empty;

        public string AdditionalName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Popup { get; set; }
        public uint NameClass { get; set; }
        public uint Color { get; set; }

        [StringType(StringType.Utf16)] 
        public string TooltipTexture { get; set; } = string.Empty;

        public uint IsTrade { get; set; }
        public uint IsDrop { get; set; }
        public uint IsDestruct { get; set; }
        public uint IsPrivateStore { get; set; }
        public uint KeepType { get; set; }
        public uint IsNpcTrade { get; set; }
        public uint IsCommissionStore { get; set; }
    }
}