using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass2, Chronicles.MasterClass3 - 1)]
public sealed class ItemNameV16
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemNameRecord[] Records { get; set; } = Array.Empty<ItemNameRecord>();

    public ItemMacro[] Macros { get; set; } = Array.Empty<ItemMacro>();

    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemEx[] ItemExs { get; set; } = Array.Empty<ItemEx>();

    public sealed class ItemNameRecord
    {
        public uint Id { get; set; }

        [StringType(StringType.NameDataIndex)] 
        public string Name { get; set; } = string.Empty;

        public string AdditionalName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public short Popup { get; set; }
        public string DefaultAction { get; set; } = string.Empty;
        public uint UseOrder { get; set; }
        public short NameClass { get; set; }
        public byte Color { get; set; }

        [StringType(StringType.NameDataIndex)] 
        public string TooltipTexture { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)] 
        public string TooltipBgTexture { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)] 
        public string TooltipBgTextureCompare { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)] 
        public string TooltipBgDecoTexture { get; set; } = string.Empty;
        
        public byte IsTrade { get; set; }
        public byte IsDrop { get; set; }
        public byte IsDestruct { get; set; }
        public byte IsPrivateStore { get; set; }
        public byte KeepType { get; set; }
        public byte IsNpcTrade { get; set; }
        public byte IsCommissionStore { get; set; }
        public uint EnchantBless { get; set; }
        public long DbDelete { get; set; }
        public CreateItem[] CreateItemList { get; set; } = Array.Empty<CreateItem>();
        public byte SortOrder { get; set; }
        public uint AuctionCategory { get; set; }
    }

    public sealed class CreateItem
    {
        public byte Unknown { get; set; }
        public RandomCreateItem[] RandomCreateItemList { get; set; } = Array.Empty<RandomCreateItem>();
    }

    public sealed class RandomCreateItem
    {
        public uint ItemClassId { get; set; }
        public uint Count { get; set; }
        public uint EnchantValue { get; set; }
        public uint Unknown { get; set; }
    }
    
    public sealed class ItemEx
    {
        public uint ItemExId { get; set; }
        public byte KeepTypeSelection { get; set; }
        public uint KeepEnchantCondition { get; set; }
        public byte KeepOption1 { get; set; }
        public byte KeepOption2 { get; set; }
    }
}