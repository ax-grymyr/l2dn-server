using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.GrandCrusade - 1)]
public sealed class ItemNameV9
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemNameRecord[] Records { get; set; } = Array.Empty<ItemNameRecord>();

    public sealed class ItemNameRecord
    {
        public uint Id { get; set; }

        [StringType(StringType.NameDataIndex)] 
        public string Name { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)] 
        public string AdditionalName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public short Popup { get; set; }
        public string DefaultAction { get; set; } = string.Empty;
        public uint UseOrder { get; set; }
        public short NameClass { get; set; }
        public byte Color { get; set; }

        [StringType(StringType.NameDataIndex)] 
        public string TooltipTexture { get; set; } = string.Empty;

        public byte IsTrade { get; set; }
        public byte IsDrop { get; set; }
        public byte IsDestruct { get; set; }
        public byte IsPrivateStore { get; set; }
        public byte KeepType { get; set; }
        public byte IsNpcTrade { get; set; }
        public byte IsCommissionStore { get; set; }
    }
}