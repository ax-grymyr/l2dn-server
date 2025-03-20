using System.Collections.Concurrent;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Skills;

/**
 * An Enum to hold some important references to commonly used skills.
 * Values correspond to client IDs.
 * @author DrHouse
 */
public enum CommonSkill
{
	RAID_CURSE = 4215,
	RAID_CURSE2 = 4515,
	SEAL_OF_RULER = 246,
	BUILD_HEADQUARTERS = 247,
	WYVERN_BREATH = 4289,
	STRIDER_SIEGE_ASSAULT = 325,
	FIREWORK = 5965,
	LARGE_FIREWORK = 2025,
	BLESSING_OF_PROTECTION = 5182,
	VOID_BURST = 3630,
	VOID_FLOW = 3631,
	THE_VICTOR_OF_WAR = 5074,
	THE_VANQUISHED_OF_WAR = 5075,
	SPECIAL_TREE_RECOVERY_BONUS = 2139, // TODO: not present in classic 3.0 datapack
	WEAPON_GRADE_PENALTY = 6209,
	ARMOR_GRADE_PENALTY = 6213,
	CREATE_DWARVEN = 172,
	LUCKY = 194,
	EXPERTISE = 239,
	CRYSTALLIZE = 248,
	ONYX_BEAST_TRANSFORMATION = 617, // TODO: not present in classic 3.0 datapack 
	CREATE_COMMON = 1320,
	DIVINE_INSPIRATION = 1405,
	CARAVANS_SECRET_MEDICINE = 2341, // TODO: not present in classic 3.0 datapack
	MY_TELEPORT = 2588,
	FEATHER_OF_BLESSING = 7008,
	IMPRIT_OF_LIGHT = 19034,
	IMPRIT_OF_DARKNESS = 19035,
	ABILITY_OF_LIGHT = 19032,
	ABILITY_OF_DARKNESS = 19033,
	CLAN_ADVENT = 19009,
	HAIR_ACCESSORY_SET = 17192,
	ALCHEMY_CUBE = 17943, // TODO: not present in classic 3.0 datapack
	ALCHEMY_CUBE_RANDOM_SUCCESS = 17966, // TODO: not present in classic 3.0 datapack
	PET_SWITCH_STANCE = 6054,
	WEIGHT_PENALTY = 4270,
	POTION_MASTERY = 45184,
	STR_INCREASE_BONUS = 45191, // TODO: these 6 skills has levels 1, 2, 3  // TODO: not present in classic 3.0 datapack
	INT_INCREASE_BONUS = 45192, // TODO: not present in classic 3.0 datapack
	DEX_INCREASE_BONUS = 45193, // TODO: not present in classic 3.0 datapack
	WIT_INCREASE_BONUS = 45194, // TODO: not present in classic 3.0 datapack
	CON_INCREASE_BONUS = 45195, // TODO: not present in classic 3.0 datapack
	MEN_INCREASE_BONUS = 45196, // TODO: not present in classic 3.0 datapack
	FORTUNE_SEAKER_MARK = 47202, // TODO: not present in classic 3.0 datapack
	FLAG_DISPLAY = 52001, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_WARRIOR = 52002, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_KNIGHT = 52003, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_ROGUE = 52004, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_ARCHER = 52005, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_MAGE = 52006, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_SUMMONER = 52007, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_HEALER = 52008, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_ENCHANTER = 52009, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_BARD = 52010, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_SHAMAN = 52011, // TODO: not present in classic 3.0 datapack
	FLAG_POWER_FAST_RUN = 52012, // TODO: not present in classic 3.0 datapack
	FLAG_EQUIP = 52013, // TODO: not present in classic 3.0 datapack
	REMOTE_FLAG_DISPLAY = 52017, // TODO: not present in classic 3.0 datapack
	RED_LAMP = 52038, // TODO: not present in classic 3.0 datapack
	PURPLE_LAMP = 52039, // TODO: not present in classic 3.0 datapack
	BLUE_LAMP = 52040, // TODO: not present in classic 3.0 datapack
	GREEN_LAMP = 52041, // TODO: not present in classic 3.0 datapack
	EINHASAD_OVERSEEING = 60002,
	TELEPORT = 60018,
	BRUTALITY = 87366, // TODO: not present in classic 3.0 datapack
}

public static class CommonSkillUtil
{
	private static readonly ConcurrentDictionary<(CommonSkill, int), SkillHolder> _skills = new();

	public static Skill getSkill(this CommonSkill commonSkill, int level = 1)
	{
		return _skills.GetOrAdd((commonSkill, level), cs => new((int)cs.Item1, cs.Item2)).getSkill();
	}
}