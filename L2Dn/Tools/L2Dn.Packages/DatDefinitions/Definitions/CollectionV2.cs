using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass2, Chronicles.Latest)]
public sealed class CollectionV2
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
        public byte IsEvent { get; set; }
        public byte Unknown { get; set; }

        [Condition(nameof(IsEvent), 1)] 
        public EventCheckData EventCheck { get; set; } = new EventCheckData();
        
        public ushort[] CompleteItemType { get; set; } = Array.Empty<ushort>();
        public ushort[] CompleteSkillType { get; set; } = Array.Empty<ushort>();

        [Condition(nameof(IsEvent), 1)] 
        public EventPeriodData EventPeriod { get; set; } = new EventPeriodData();
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

    public sealed class EventCheckData
    {
        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public byte Unknown4 { get; set; }
        public byte Unknown5 { get; set; }
        public byte Unknown6 { get; set; }
    }

    public sealed class EventPeriodData
    {
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }
}