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

// public enum UserInfoType implements IUpdateTypeComponent
// {
// RELATION(0x00, 4),
// BASIC_INFO(0x01, 23),
// BASE_STATS(0x02, 18),
// MAX_HPCPMP(0x03, 14),
// CURRENT_HPMPCP_EXP_SP(0x04, 39),
// ENCHANTLEVEL(0x05, 5),
// APPAREANCE(0x06, 19),
// STATUS(0x07, 6),
// 	
// STATS(0x08, 64),
// ELEMENTALS(0x09, 14),
// POSITION(0x0A, 18),
// SPEED(0x0B, 18),
// MULTIPLIER(0x0C, 18),
// COL_RADIUS_HEIGHT(0x0D, 18),
// ATK_ELEMENTAL(0x0E, 5),
// CLAN(0x0F, 32),
// 	
// SOCIAL(0x10, 34),
// VITA_FAME(0x11, 19),
// SLOTS(0x12, 12),
// MOVEMENTS(0x13, 4),
// COLOR(0x14, 10),
// INVENTORY_LIMIT(0x15, 13),
// TRUE_HERO(0x16, 9),
// 	
// ATT_SPIRITS(0x17, 34),
// 	
// RANKING(0x18, 6),
// 	
// STAT_POINTS(0x19, 16),
// STAT_ABILITIES(0x1A, 18),
// 	
// ELIXIR_USED(0x1B, 1),
// 	
// VANGUARD_MOUNT(0x1C, 1),
// UNK_414(0x1D, 1);
// 	
// /** Int mask. */
// private final int _mask;
// private final int _blockLength;
// 	
// private UserInfoType(int mask, int blockLength)
// {
//     _mask = mask;
//     _blockLength = blockLength;
// }
// 	
// /**
//  * Gets the int mask.
//  * @return the int mask
//  */
// @Override
// public int getMask()
// {
//     return _mask;
// }
// 	
// public int getBlockLength()
// {
//     return _blockLength;
// }
// }