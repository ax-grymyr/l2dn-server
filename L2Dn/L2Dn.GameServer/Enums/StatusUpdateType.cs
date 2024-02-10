using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Enums;

public enum StatusUpdateType
{
	LEVEL = 0x01,
	EXP = 0x02,
	STR = 0x03,
	DEX = 0x04,
	CON = 0x05,
	INT = 0x06,
	WIT = 0x07,
	MEN = 0x08,

	CUR_HP = 0x09,
	MAX_HP = 0x0A,
	CUR_MP = 0x0B,
	MAX_MP = 0x0C,
	CUR_LOAD = 0x0E,

	P_ATK = 0x11,
	ATK_SPD = 0x12,
	P_DEF = 0x13,
	EVASION = 0x14,
	ACCURACY = 0x15,
	CRITICAL = 0x16,
	M_ATK = 0x17,
	CAST_SPD = 0x18,
	M_DEF = 0x19,
	PVP_FLAG = 0x1A,
	REPUTATION = 0x1B,

	CUR_CP = 0x21,
	MAX_CP = 0x22,

	CUR_DP = 0x28,
	MAX_DP = 0x29,

	CUR_BP = 0x2B,
	MAX_BP = 0x2C,

	CUR_AP = 0x2D,
	MAX_AP = 0x2E,
}

public static class StatusUpdateTypeUtil
{
	public static int getValue(this StatusUpdateType value, Creature creature)
	{
		return value switch
		{
			StatusUpdateType.LEVEL => creature.getLevel(),
			StatusUpdateType.EXP => (int)creature.getStat().getExp(),
			StatusUpdateType.STR => creature.getSTR(),
			StatusUpdateType.DEX => creature.getDEX(),
			StatusUpdateType.CON => creature.getCON(),
			StatusUpdateType.INT => creature.getINT(),
			StatusUpdateType.WIT => creature.getWIT(),
			StatusUpdateType.MEN => creature.getMEN(),

			StatusUpdateType.CUR_HP => (int)creature.getCurrentHp(),
			StatusUpdateType.MAX_HP => creature.getMaxHp(),
			StatusUpdateType.CUR_MP => (int)creature.getCurrentMp(),
			StatusUpdateType.MAX_MP => creature.getMaxMp(),
			StatusUpdateType.CUR_LOAD => creature.getCurrentLoad(),

			StatusUpdateType.P_ATK => creature.getPAtk(),
			StatusUpdateType.ATK_SPD => creature.getPAtkSpd(),
			StatusUpdateType.P_DEF => creature.getPDef(),
			StatusUpdateType.EVASION => creature.getEvasionRate(),
			StatusUpdateType.ACCURACY => creature.getAccuracy(),
			StatusUpdateType.CRITICAL => (int)creature.getCriticalDmg(1),
			StatusUpdateType.M_ATK => creature.getMAtk(),
			StatusUpdateType.CAST_SPD => creature.getMAtkSpd(),
			StatusUpdateType.M_DEF => creature.getMDef(),
			StatusUpdateType.PVP_FLAG => creature.getPvpFlag(),
			StatusUpdateType.REPUTATION => creature.isPlayer() ? creature.getActingPlayer().getReputation() : 0,

			StatusUpdateType.CUR_CP => (int)creature.getCurrentCp(),
			StatusUpdateType.MAX_CP => creature.getMaxCp(),

			StatusUpdateType.CUR_DP => creature.isPlayer() ? creature.getActingPlayer().getDeathPoints() : 0,
			StatusUpdateType.MAX_DP => creature.isPlayer() ? creature.getActingPlayer().getMaxDeathPoints() : 0,

			StatusUpdateType.CUR_BP => creature.isPlayer() ? creature.getActingPlayer().getBeastPoints() : 0,
			StatusUpdateType.MAX_BP => creature.isPlayer() ? creature.getActingPlayer().getMaxBeastPoints() : 0,

			StatusUpdateType.CUR_AP => creature.isPlayer() ? creature.getActingPlayer().getAssassinationPoints() : 0,
			StatusUpdateType.MAX_AP => creature.isPlayer() ? creature.getActingPlayer().getMaxAssassinationPoints() : 0,
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}

// public enum StatusUpdateType
// {
// 	LEVEL(0x01, Creature::getLevel),
// 	EXP(0x02, creature -> (int) creature.getStat().getExp()),
// 	STR(0x03, Creature::getSTR),
// 	DEX(0x04, Creature::getDEX),
// 	CON(0x05, Creature::getCON),
// 	INT(0x06, Creature::getINT),
// 	WIT(0x07, Creature::getWIT),
// 	MEN(0x08, Creature::getMEN),
// 	
// 	CUR_HP(0x09, creature -> (int) creature.getCurrentHp()),
// 	MAX_HP(0x0A, Creature::getMaxHp),
// 	CUR_MP(0x0B, creature -> (int) creature.getCurrentMp()),
// 	MAX_MP(0x0C, Creature::getMaxMp),
// 	CUR_LOAD(0x0E, Creature::getCurrentLoad),
// 	
// 	P_ATK(0x11, Creature::getPAtk),
// 	ATK_SPD(0x12, Creature::getPAtkSpd),
// 	P_DEF(0x13, Creature::getPDef),
// 	EVASION(0x14, Creature::getEvasionRate),
// 	ACCURACY(0x15, Creature::getAccuracy),
// 	CRITICAL(0x16, creature -> (int) creature.getCriticalDmg(1)),
// 	M_ATK(0x17, Creature::getMAtk),
// 	CAST_SPD(0x18, Creature::getMAtkSpd),
// 	M_DEF(0x19, Creature::getMDef),
// 	PVP_FLAG(0x1A, creature -> (int) creature.getPvpFlag()),
// 	REPUTATION(0x1B, creature -> creature.isPlayer() ? creature.getActingPlayer().getReputation() : 0),
// 	
// 	CUR_CP(0x21, creature -> (int) creature.getCurrentCp()),
// 	MAX_CP(0x22, Creature::getMaxCp),
// 	
// 	CUR_DP(0x28, creature -> creature.isPlayer() ? creature.getActingPlayer().getDeathPoints() : 0),
// 	MAX_DP(0x29, creature -> creature.isPlayer() ? creature.getActingPlayer().getMaxDeathPoints() : 0),
// 	
// 	CUR_BP(0x2B, creature -> creature.isPlayer() ? creature.getActingPlayer().getBeastPoints() : 0),
// 	MAX_BP(0x2C, creature -> creature.isPlayer() ? creature.getActingPlayer().getMaxBeastPoints() : 0),
// 	
// 	CUR_AP(0x2D, creature -> creature.isPlayer() ? creature.getActingPlayer().getAssassinationPoints() : 0),
// 	MAX_AP(0x2E, creature -> creature.isPlayer() ? creature.getActingPlayer().getMaxAssassinationPoints() : 0);
// 	
// 	private int _clientId;
// 	private Function<Creature, Integer> _valueSupplier;
// 	
// 	StatusUpdateType(int clientId, Function<Creature, Integer> valueSupplier)
// 	{
// 		_clientId = clientId;
// 		_valueSupplier = valueSupplier;
// 	}
// 	
// 	public int getClientId()
// 	{
// 		return _clientId;
// 	}
// 	
// 	public int getValue(Creature creature)
// 	{
// 		return _valueSupplier.apply(creature).intValue();
// 	}
// }
