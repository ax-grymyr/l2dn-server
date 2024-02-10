using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author DS
 */
public class ConditionTargetPlayable: Condition
{
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effected != null) && effected.isPlayable();
	}
}
