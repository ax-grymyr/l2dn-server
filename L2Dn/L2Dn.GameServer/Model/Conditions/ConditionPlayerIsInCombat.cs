using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public class ConditionPlayerIsInCombat : Condition
{
	private readonly bool _value;
	
	public ConditionPlayerIsInCombat(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool isInCombat = !AttackStanceTaskManager.getInstance().hasAttackStanceTask(effector);
		return _value == isInCombat;
	}
}
