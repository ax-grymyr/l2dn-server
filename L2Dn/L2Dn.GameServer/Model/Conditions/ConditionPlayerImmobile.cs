using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public class ConditionPlayerImmobile : Condition
{
	private readonly bool _value;
	
	public ConditionPlayerImmobile(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool isImmobile = !effector.isMovementDisabled();
		return _value == isImmobile;
	}
}
