using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ReturnOfTheQueenAnt, Chronicles.MasterClass2 - 1)]
public sealed class Collection
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CollectionRecord[] Records { get; set; } = Array.Empty<CollectionRecord>();

    public sealed class CollectionRecord
    {
        public uint CollectionId { get; set; }
        public string CollectionName { get; set; } = string.Empty;
        public uint MainCategory { get; set; }
        public uint Period { get; set; }
        public uint OptionId { get; set; }
        public CollectionDescription[] Descriptions { get; set; } = Array.Empty<CollectionDescription>();
        public CollectionSlot[] Slots { get; set; } = Array.Empty<CollectionSlot>();
        public ushort Unknown { get; set; }
        public ushort[] CompleteItemType { get; set; } = Array.Empty<ushort>();
        public ushort[] CompleteSkillType { get; set; } = Array.Empty<ushort>();
    }

    public sealed class CollectionDescription
    {
	    public string Description { get; set; } = string.Empty;
	    public uint Unknown { get; set; }
	    public float Param { get; set; }
    }

    public sealed class CollectionSlot
    {
	    public uint ItemId { get; set; }
	    public uint ItemId2 { get; set; }
	    public uint ItemCount { get; set; }
	    public ushort Enchant { get; set; }
	    public byte Slot { get; set; }
	    public byte Bless { get; set; }
	    public uint BlessCondition { get; set; }
    }
}