using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetNone.
 * @author mkizub
 */
public class ConditionTargetNone: Condition
{
	/**
	 * Instantiates a new condition target none.
	 */
	public ConditionTargetNone()
	{
		//
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return effected == null;
	}
}
