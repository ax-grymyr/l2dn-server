using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Homunculus2, Chronicles.MasterClass2 - 1)]
public sealed class ItemNameV15
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemNameRecord[] Records { get; set; } = Array.Empty<ItemNameRecord>();

    public ItemMacro[] Macros { get; set; } = Array.Empty<ItemMacro>();

    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemEx[] ItemExs { get; set; } = Array.Empty<ItemEx>();

    public sealed class ItemNameRecord
    {
        public uint Id { get; set; }
        public IndexedString Name { get; set; }
        public string AdditionalName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public short Popup { get; set; }
        public string DefaultAction { get; set; } = string.Empty;
        public uint UseOrder { get; set; }
        public short NameClass { get; set; }
        public byte Color { get; set; }
        public IndexedString TooltipTexture { get; set; }
        public IndexedString TooltipBgTexture { get; set; }
        public IndexedString TooltipBgTextureCompare { get; set; }
        public IndexedString TooltipBgDecoTexture { get; set; }
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
    }

    public sealed class CreateItem
    {
	    public uint ItemClassId { get; set; }
	    public uint Count { get; set; }
	    public uint EnchantValue { get; set; }
	    public byte MarkType{ get; set; } // enum create_item_list_mark_type
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