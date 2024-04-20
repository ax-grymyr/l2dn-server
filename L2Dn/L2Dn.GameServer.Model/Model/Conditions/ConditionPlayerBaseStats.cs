using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerBaseStats.
 * @author mkizub
 */
public class ConditionPlayerBaseStats: Condition
{
	private readonly BaseStat _stat;
	private readonly int _value;
	
	/**
	 * Instantiates a new condition player base stats.
	 * @param creature the player
	 * @param stat the stat
	 * @param value the value
	 */
	public ConditionPlayerBaseStats(Creature creature, BaseStat stat, int value)
	{
		_stat = stat;
		_value = value;
	}
	
	/**
	 * Test impl.
	 * @return true, if successful
	 */
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Player player = effector.getActingPlayer();
		if (player == null)
		{
			return false;
		}

		return _stat switch
		{
			BaseStat.INT => player.getINT() >= _value,
			BaseStat.STR => player.getSTR() >= _value,
			BaseStat.CON => player.getCON() >= _value,
			BaseStat.DEX => player.getDEX() >= _value,
			BaseStat.MEN => player.getMEN() >= _value,
			BaseStat.WIT => player.getWIT() >= _value,
			_ => false
		};
	}
}
