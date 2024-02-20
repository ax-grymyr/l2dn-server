using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetAggro.
 * @author mkizub
 */
public class ConditionTargetAggro: Condition
{
	private readonly bool _isAggro;
	
	/**
	 * Instantiates a new condition target aggro.
	 * @param isAggro the is aggro
	 */
	public ConditionTargetAggro(bool isAggro)
	{
		_isAggro = isAggro;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effected != null)
		{
			if (effected.isMonster())
			{
				return ((Monster) effected).isAggressive() == _isAggro;
			}
			if (effected.isPlayer())
			{
				return ((Player) effected).getReputation() < 0;
			}
		}
		return false;
	}
}
