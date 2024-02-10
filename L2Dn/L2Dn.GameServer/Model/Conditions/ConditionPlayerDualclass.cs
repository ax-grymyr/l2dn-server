using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

public class ConditionPlayerDualclass: Condition
{
	private readonly bool _value;
	
	public ConditionPlayerDualclass(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return true;
		}
		return effector.getActingPlayer().isDualClassActive() == _value;
	}
}
