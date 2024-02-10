using L2Dn.GameServer.Model.Actor;

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
		
		switch (_stat)
		{
			case INT:
			{
				return player.getINT() >= _value;
			}
			case STR:
			{
				return player.getSTR() >= _value;
			}
			case CON:
			{
				return player.getCON() >= _value;
			}
			case DEX:
			{
				return player.getDEX() >= _value;
			}
			case MEN:
			{
				return player.getMEN() >= _value;
			}
			case WIT:
			{
				return player.getWIT() >= _value;
			}
		}
		
		return false;
	}
}
