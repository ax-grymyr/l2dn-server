using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Interlude, Chronicles.Epilogue - 1)]
public sealed class QuestNameV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public QuestNameRecord[] Records { get; set; } = Array.Empty<QuestNameRecord>();
    
    public sealed class QuestNameRecord
    {
        public uint Tag { get; set; }
        public uint Id { get; set; }
        public uint Level { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SubName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public uint[] GoalIds { get; set; } = Array.Empty<uint>();
        public uint[] GoalNums { get; set; } = Array.Empty<uint>();
        public Location TargetLoc { get; set; } = new Location();
        public uint LvlMin { get; set; }
        public uint LvlMax { get; set; }
        public uint QuestType { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public uint GetItemInQuest { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint StartNpcId { get; set; }
        public Location StartNpcLoc { get; set; } = new Location();
        public string Requirements { get; set; } = string.Empty;
        public string Intro { get; set; } = string.Empty;
        public uint[] ClassLimits { get; set; } = Array.Empty<uint>();
        public uint[] HaveItems { get; set; } = Array.Empty<uint>();
        public uint ClanPetQuest { get; set; }
        public uint ReqQuestComplete { get; set; }
        public uint Unknown4 { get; set; }
        public uint AreaId { get; set; }
    }
}