using System.Collections.Immutable;

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

public static class NpcInfoTypeUtil
{
	private static readonly ImmutableSortedDictionary<NpcInfoType, int> _blockLengths = new[]
	{
		// 0
		(NpcInfoType.ID, 4),
		(NpcInfoType.ATTACKABLE, 1),
		(NpcInfoType.RELATIONS, 8),
		(NpcInfoType.NAME, 2),
		(NpcInfoType.POSITION, (3 * 4)),
		(NpcInfoType.HEADING, 4),
		(NpcInfoType.VEHICLE_ID, 4),
		(NpcInfoType.ATK_CAST_SPEED, (2 * 4)),

		// 1
		(NpcInfoType.SPEED_MULTIPLIER, (2 * 4)),
		(NpcInfoType.EQUIPPED, (3 * 4)),
		(NpcInfoType.STOP_MODE, 1),
		(NpcInfoType.MOVE_MODE, 1),
		(NpcInfoType.SWIM_OR_FLY, 1),
		(NpcInfoType.TEAM, 1),

		// 2
		(NpcInfoType.ENCHANT, 4),
		(NpcInfoType.FLYING, 4),
		(NpcInfoType.CLONE, 4),
		(NpcInfoType.PET_EVOLUTION_ID, 4),
		(NpcInfoType.DISPLAY_EFFECT, 4),
		(NpcInfoType.TRANSFORMATION, 4),

		// 3
		(NpcInfoType.CURRENT_HP, 4),
		(NpcInfoType.CURRENT_MP, 4),
		(NpcInfoType.MAX_HP, 4),
		(NpcInfoType.MAX_MP, 4),
		(NpcInfoType.SUMMONED, 1),
		(NpcInfoType.FOLLOW_INFO, (2 * 4)),
		(NpcInfoType.TITLE, 2),
		(NpcInfoType.NAME_NPCSTRINGID, 4),

		// 4
		(NpcInfoType.TITLE_NPCSTRINGID, 4),
		(NpcInfoType.PVP_FLAG, 1),
		(NpcInfoType.REPUTATION, 4),
		(NpcInfoType.CLAN, (5 * 4)),
		(NpcInfoType.ABNORMALS, 0),
		(NpcInfoType.VISUAL_STATE, 4),
	}.ToImmutableSortedDictionary(t => t.Item1, t => t.Item2);

	public static int GetBlockLength(this NpcInfoType type)
	{
		return _blockLengths[type];
	}
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