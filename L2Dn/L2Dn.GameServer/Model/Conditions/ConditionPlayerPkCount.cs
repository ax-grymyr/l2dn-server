using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerPkCount.
 */
public class ConditionPlayerPkCount : Condition
{
	private readonly int _pk;
	
	/**
	 * Instantiates a new condition player pk count.
	 * @param pk the pk
	 */
	public ConditionPlayerPkCount(int pk)
	{
		_pk = pk;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return false;
		}
		return effector.getActingPlayer().getPkKills() <= _pk;
	}
}
