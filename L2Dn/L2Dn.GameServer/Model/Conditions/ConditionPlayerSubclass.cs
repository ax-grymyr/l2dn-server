using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerSubclass.
 */
public class ConditionPlayerSubclass : Condition
{
	private readonly bool _value;
	
	/**
	 * Instantiates a new condition player subclass.
	 * @param value the value
	 */
	public ConditionPlayerSubclass(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return true;
		}
		return effector.getActingPlayer().isSubClassActive() == _value;
	}
}
