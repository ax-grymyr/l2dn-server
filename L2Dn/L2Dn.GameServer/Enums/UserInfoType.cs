namespace L2Dn.GameServer.Enums;

public enum UserInfoType
{
	RELATION = 0x00,
	BASIC_INFO = 0x01,
	BASE_STATS = 0x02,
	MAX_HPCPMP = 0x03,
	CURRENT_HPMPCP_EXP_SP = 0x04,
	ENCHANTLEVEL = 0x05,
	APPAREANCE = 0x06,
	STATUS = 0x07,

	STATS = 0x08,
	ELEMENTALS = 0x09,
	POSITION = 0x0A,
	SPEED = 0x0B,
	MULTIPLIER = 0x0C,
	COL_RADIUS_HEIGHT = 0x0D,
	ATK_ELEMENTAL = 0x0E,
	CLAN = 0x0F,

	SOCIAL = 0x10,
	VITA_FAME = 0x11,
	SLOTS = 0x12,
	MOVEMENTS = 0x13,
	COLOR = 0x14,
	INVENTORY_LIMIT = 0x15,
	TRUE_HERO = 0x16,

	ATT_SPIRITS = 0x17,

	RANKING = 0x18,

	STAT_POINTS = 0x19,
	STAT_ABILITIES = 0x1A,

	ELIXIR_USED = 0x1B,

	VANGUARD_MOUNT = 0x1C,
	UNK_414 = 0x1D,
}

public static class UserInfoTypeUtil
{
	public static int GetBlockLength(this UserInfoType type) => type switch
	{
		UserInfoType.RELATION => 4,
		UserInfoType.BASIC_INFO => 23,
		UserInfoType.BASE_STATS => 18,
		UserInfoType.MAX_HPCPMP => 14,
		UserInfoType.CURRENT_HPMPCP_EXP_SP => 39,
		UserInfoType.ENCHANTLEVEL => 5,
		UserInfoType.APPAREANCE => 19,
		UserInfoType.STATUS => 6,

		UserInfoType.STATS => 64,
		UserInfoType.ELEMENTALS => 14,
		UserInfoType.POSITION => 18,
		UserInfoType.SPEED => 18,
		UserInfoType.MULTIPLIER => 18,
		UserInfoType.COL_RADIUS_HEIGHT => 18,
		UserInfoType.ATK_ELEMENTAL => 5,
		UserInfoType.CLAN => 32,

		UserInfoType.SOCIAL => 34,
		UserInfoType.VITA_FAME => 19,
		UserInfoType.SLOTS => 12,
		UserInfoType.MOVEMENTS => 4,
		UserInfoType.COLOR => 10,
		UserInfoType.INVENTORY_LIMIT => 13,
		UserInfoType.TRUE_HERO => 9,

		UserInfoType.ATT_SPIRITS => 34,

		UserInfoType.RANKING => 6,

		UserInfoType.STAT_POINTS => 16,
		UserInfoType.STAT_ABILITIES => 18,

		UserInfoType.ELIXIR_USED => 1,

		UserInfoType.VANGUARD_MOUNT => 1,
		UserInfoType.UNK_414 => 1,

		_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
	};
}