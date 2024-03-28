namespace L2Dn.Packages.DatDefinitions.Fafurion;

public class Quest
{
	public int Tag { get; set; }
	public int Id { get; set; }
	public int Level { get; set; }
	public string Title { get; set; } = string.Empty;
	public string SubName { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int[] GoalIds { get; set; } = Array.Empty<int>();
	public int[] GoalTypes { get; set; } = Array.Empty<int>();
	public int[] GoalNumbers { get; set; } = Array.Empty<int>();
	public Location TargetLocation { get; set; } = Location.Zero;
	public Location[] AddTargetLocations { get; set; } = Array.Empty<Location>();
	public int[] Levels { get; set; } = Array.Empty<int>();
	public int MinLevel { get; set; }
	public int MaxLevel { get; set; }
	public int JournalDisplay { get; set; }
	public string EntityName { get; set; } = string.Empty;
	public int GetItemInQuest { get; set; }
	public int Unknown1 { get; set; }
	public int Unknown2 { get; set; }
	public int[] StartNpcIds { get; set; } = Array.Empty<int>();
	public Location StartNpcLocation { get; set; } = Location.Zero;
	public string Requirement { get; set; } = string.Empty;
	public string Intro { get; set; } = string.Empty;
	public int[] ClassLimits { get; set; } = Array.Empty<int>();
	public int[] HaveItems { get; set; } = Array.Empty<int>();
	public int ClanPetQuest { get; set; }
	public int ClearedQuest { get; set; }
	public int MarkType { get; set; }
	public int CategoryId { get; set; }
	public int PriorityLevel { get; set; }
	public int SearchZoneId { get; set; }
	public int IsCategory { get; set; }
	public int[] RewardIds { get; set; } = Array.Empty<int>();
	public long[] RewardNumbers { get; set; } = Array.Empty<long>();
	public int[] PreLevels { get; set; } = Array.Empty<int>();
	public int FactionId { get; set; }	
	public int FactionMinLevel { get; set; }	
	public int FactionMaxLevel { get; set; }	
}