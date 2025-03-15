using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Interlude - 1)]
public sealed class QuestName
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
	    public uint UnknownNpc1 { get; set; }
	    public uint UnknownNpc2 { get; set; }
	    public uint UnknownNpc3 { get; set; }
	    public string EntityName { get; set; } = string.Empty;
	    public uint Unknown1 { get; set; }
	    public uint Unknown2 { get; set; }
	    public uint Unknown3 { get; set; }
	    public uint Unknown4 { get; set; }
	    public Location StartNpcLoc { get; set; } = new Location();
	    public string ClassLimit { get; set; } = string.Empty;
	    public string Intro { get; set; } = string.Empty;
    }
}