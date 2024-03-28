using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Shinemaker;

public class Items
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemName[] Names { get; set; } = Array.Empty<ItemName>();
    
    public ItemMacro[] Macros { get; set; } = Array.Empty<ItemMacro>();

    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemExData[] ExData { get; set; } = Array.Empty<ItemExData>();
}

public class ItemExData
{
	public int ItemExId { get; set; }
	public byte KeepTypeSelection { get; set; } // enum keep_type_selection
	public int KeepEnchantCondition { get; set; }
	public byte KeepOption1 { get; set; }
	public byte KeepOption2 { get; set; }
}

public class ItemMacro
{
	public int MacroId { get; set; }
	public byte AutomaticUse { get; set; }
}

public class ItemName
{
	public int Id { get; set; }

	[StringType(StringType.NameDataIndex)]
	public string Name { get; set; } = string.Empty;

	public string AdditionalName { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public short Popup { get; set; }
	public int DefaultAction { get; set; } // enum item_default_action	 
	public int UseOrder { get; set; }
	public short NameClass { get; set; }
	public byte Color { get; set; }

	[StringType(StringType.NameDataIndex)]
	public string TooltipTexture { get; set; } = string.Empty;

	[StringType(StringType.NameDataIndex)]
	public string TooltipBgTexture { get; set; } = string.Empty;

	[StringType(StringType.NameDataIndex)]
	public string TooltipBgTextureCompare { get; set; } = string.Empty;

	[StringType(StringType.NameDataIndex)]
	public string TooltipBgDecoCompare { get; set; } = string.Empty;

	public byte IsTrade { get; set; }
	public byte IsDrop { get; set; }
	public byte IsDestruct { get; set; }
	public byte IsPrivateStore { get; set; }
	public byte KeepType { get; set; }
	public byte IsNpcTrade { get; set; }
	public byte IsCommissionStore { get; set; }
	public int EnchantBless { get; set; }
	public ItemCreate[] CreateItemList { get; set; } = Array.Empty<ItemCreate>();
	public byte SortOrder { get; set; }
	public int AuctionCategory { get; set; }
}

public class ItemCreate
{
	public byte DropType { get; set; }
	public ItemRandomCreate[] RandomCreateList { get; set; } = Array.Empty<ItemRandomCreate>(); 
}

public class ItemRandomCreate
{
	public int ItemClassId { get; set; }
	public int Count { get; set; }
	public int EnchantLevel { get; set; }
	public int Unknown { get; set; }
}