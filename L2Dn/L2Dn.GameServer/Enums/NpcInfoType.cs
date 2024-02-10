namespace L2Dn.GameServer.Enums;

public enum NpcInfoType
{
	// 0
	ID = 0x00,
	ATTACKABLE = 0x01,
	RELATIONS = 0x02,
	NAME = 0x03,
	POSITION = 0x04,
	HEADING = 0x05,
	VEHICLE_ID = 0x06,
	ATK_CAST_SPEED = 0x07,

	// 1
	SPEED_MULTIPLIER = 0x08,
	EQUIPPED = 0x09,
	STOP_MODE = 0x0A,
	MOVE_MODE = 0x0B,
	SWIM_OR_FLY = 0x0E,
	TEAM = 0x0F,

	// 2
	ENCHANT = 0x10,
	FLYING = 0x11,
	CLONE = 0x12,
	PET_EVOLUTION_ID = 0x13,
	DISPLAY_EFFECT = 0x16,
	TRANSFORMATION = 0x17,

	// 3
	CURRENT_HP = 0x18,
	CURRENT_MP = 0x19,
	MAX_HP = 0x1A,
	MAX_MP = 0x1B,
	SUMMONED = 0x1C,
	FOLLOW_INFO = 0x1D,
	TITLE = 0x1E,
	NAME_NPCSTRINGID = 0x1F,

	// 4
	TITLE_NPCSTRINGID = 0x20,
	PVP_FLAG = 0x21,
	REPUTATION = 0x22,
	CLAN = 0x23,
	ABNORMALS = 0x24,
	VISUAL_STATE = 0x25
}

// public enum NpcInfoType
// {
// // 0
// ID(0x00, 4),
// ATTACKABLE(0x01, 1),
// RELATIONS(0x02, 8),
// NAME(0x03, 2),
// POSITION(0x04, (3 * 4)),
// HEADING(0x05, 4),
// VEHICLE_ID(0x06, 4),
// ATK_CAST_SPEED(0x07, (2 * 4)),
// 	
// // 1
// SPEED_MULTIPLIER(0x08, (2 * 4)),
// EQUIPPED(0x09, (3 * 4)),
// STOP_MODE(0x0A, 1),
// MOVE_MODE(0x0B, 1),
// SWIM_OR_FLY(0x0E, 1),
// TEAM(0x0F, 1),
// 	
// // 2
// ENCHANT(0x10, 4),
// FLYING(0x11, 4),
// CLONE(0x12, 4),
// PET_EVOLUTION_ID(0x13, 4),
// DISPLAY_EFFECT(0x16, 4),
// TRANSFORMATION(0x17, 4),
// 	
// // 3
// CURRENT_HP(0x18, 4),
// CURRENT_MP(0x19, 4),
// MAX_HP(0x1A, 4),
// MAX_MP(0x1B, 4),
// SUMMONED(0x1C, 1),
// FOLLOW_INFO(0x1D, (2 * 4)),
// TITLE(0x1E, 2),
// NAME_NPCSTRINGID(0x1F, 4),
// 	
// // 4
// TITLE_NPCSTRINGID(0x20, 4),
// PVP_FLAG(0x21, 1),
// REPUTATION(0x22, 4),
// CLAN(0x23, (5 * 4)),
// ABNORMALS(0x24, 0),
// VISUAL_STATE(0x25, 4);
// 	
// private final int _mask;
// private final int _blockLength;
// 	
// private NpcInfoType(int mask, int blockLength)
// {
//     _mask = mask;
//     _blockLength = blockLength;
// }
// 	
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