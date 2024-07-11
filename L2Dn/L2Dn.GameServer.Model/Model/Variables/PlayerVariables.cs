using L2Dn.GameServer.Db;

namespace L2Dn.GameServer.Model.Variables;

public class PlayerVariables: AbstractVariables<CharacterVariable>
{
	// Public variable names.
	public const string INSTANCE_ORIGIN = "INSTANCE_ORIGIN";
	public const string INSTANCE_RESTORE = "INSTANCE_RESTORE";
	public const string RESTORE_LOCATION = "RESTORE_LOCATION";
	public const string HAIR_ACCESSORY_VARIABLE_NAME = "HAIR_ACCESSORY_ENABLED";
	public const string WORLD_CHAT_VARIABLE_NAME = "WORLD_CHAT_USED";
	public const string VITALITY_ITEMS_USED_VARIABLE_NAME = "VITALITY_ITEMS_USED";
	public const string UI_KEY_MAPPING = "UI_KEY_MAPPING";
	public const string CLIENT_SETTINGS = "CLIENT_SETTINGS";
	public const string ATTENDANCE_DATE = "ATTENDANCE_DATE";
	public const string ATTENDANCE_INDEX = "ATTENDANCE_INDEX";
	public const string ABILITY_POINTS_MAIN_CLASS = "ABILITY_POINTS";
	public const string ABILITY_POINTS_DUAL_CLASS = "ABILITY_POINTS_DUAL_CLASS";
	public const string ABILITY_POINTS_USED_MAIN_CLASS = "ABILITY_POINTS_USED";
	public const string ABILITY_POINTS_USED_DUAL_CLASS = "ABILITY_POINTS_DUAL_CLASS_USED";
	public const string REVELATION_SKILL_1_MAIN_CLASS = "RevelationSkill1";
	public const string REVELATION_SKILL_2_MAIN_CLASS = "RevelationSkill2";
	public const string REVELATION_SKILL_1_DUAL_CLASS = "DualclassRevelationSkill1";
	public const string REVELATION_SKILL_2_DUAL_CLASS = "DualclassRevelationSkill2";
	public const string LAST_PLEDGE_REPUTATION_LEVEL = "LAST_PLEDGE_REPUTATION_LEVEL";
	public const string FORTUNE_TELLING_VARIABLE = "FortuneTelling";
	public const string FORTUNE_TELLING_BLACK_CAT_VARIABLE = "FortuneTellingBlackCat";
	public const string DELUSION_RETURN = "DELUSION_RETURN";
	public const string AUTO_USE_SETTINGS = "AUTO_USE_SETTINGS";
	public const string AUTO_USE_SHORTCUTS = "AUTO_USE_SHORTCUTS";
	public const string LAST_HUNTING_ZONE_ID = "LAST_HUNTING_ZONE_ID";
	public const string HUNTING_ZONE_ENTRY = "HUNTING_ZONE_ENTRY_";
	public const string HUNTING_ZONE_TIME = "HUNTING_ZONE_TIME_";
	public const string HUNTING_ZONE_REMAIN_REFILL = "HUNTING_ZONE_REMAIN_REFILL_";
	public const string SAYHA_GRACE_SUPPORT_ENDTIME = "SAYHA_GRACE_SUPPORT_ENDTIME";
	public const string LIMITED_SAYHA_GRACE_ENDTIME = "LIMITED_SAYHA_GRACE_ENDTIME";
	public const string MAGIC_LAMP_EXP = "MAGIC_LAMP_EXP";
	public const string DEATH_POINT_COUNT = "DEATH_POINT_COUNT";
	public const string BEAST_POINT_COUNT = "BEAST_POINT_COUNT";
	public const string ASSASSINATION_POINT_COUNT = "ASSASSINATION_POINT_COUNT";
	public const string FAVORITE_TELEPORTS = "FAVORITE_TELEPORTS";
	public const string ELIXIRS_AVAILABLE = "ELIXIRS_AVAILABLE";
	public const string STAT_POINTS = "STAT_POINTS";
	public const string STAT_STR = "STAT_STR";
	public const string STAT_DEX = "STAT_DEX";
	public const string STAT_CON = "STAT_CON";
	public const string STAT_INT = "STAT_INT";
	public const string STAT_WIT = "STAT_WIT";
	public const string STAT_MEN = "STAT_MEN";
	public const string RESURRECT_BY_PAYMENT_COUNT = "RESURRECT_BY_PAYMENT_COUNT";
	public const string PURGE_LAST_CATEGORY = "PURGE_LAST_CATEGORY";
	public const string CLAN_JOIN_TIME = "CLAN_JOIN_TIME";
	public const string CLAN_DONATION_POINTS = "CLAN_DONATION_POINTS";
	public const string HENNA1_DURATION = "HENNA1_DURATION";
	public const string HENNA2_DURATION = "HENNA2_DURATION";
	public const string HENNA3_DURATION = "HENNA3_DURATION";
	public const string HENNA4_DURATION = "HENNA4_DURATION";
	public const string DYE_POTENTIAL_DAILY_STEP = "DYE_POTENTIAL_DAILY_STEP";
	public const string DYE_POTENTIAL_DAILY_COUNT = "DYE_POTENTIAL_DAILY_COUNT";
	public const string DYE_POTENTIAL_DAILY_COUNT_ENCHANT_RESET = "DYE_POTENTIAL_DAILY_COUNT_ENCHANT_RESET";
	public const string MISSION_LEVEL_PROGRESS = "MISSION_LEVEL_PROGRESS_";
	public const string BALOK_AVAILABLE_REWARD = "BALOK_AVAILABLE_REWARD";
	public const string DUAL_INVENTORY_SLOT = "DUAL_INVENTORY_SLOT";
	public const string DUAL_INVENTORY_SET_A = "DUAL_INVENTORY_SET_A";
	public const string DUAL_INVENTORY_SET_B = "DUAL_INVENTORY_SET_B";
	public const string DAILY_EXTRACT_ITEM = "DAILY_EXTRACT_ITEM";
	public const string SKILL_ENCHANT_STAR = "SKILL_ENCHANT_STAR_";
	public const string SKILL_TRY_ENCHANT = "SKILL_TRY_ENCHANT_";

	private readonly int _objectId;

	public PlayerVariables(int objectId)
	{
		_objectId = objectId;
		restoreMe();
	}

	protected override IQueryable<CharacterVariable> GetQuery(GameServerDbContext ctx)
	{
		return ctx.CharacterVariables.Where(r => r.CharacterId == _objectId);
	}

	protected override CharacterVariable CreateVar()
	{
		return new CharacterVariable()
		{
			CharacterId = _objectId
		};
	}
}