using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public class ConditionTargetPlayer : Condition
{
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return effected.isPlayer();
	}
}
