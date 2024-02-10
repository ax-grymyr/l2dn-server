using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerFlyMounted.
 * @author kerberos
 */
public class ConditionPlayerFlyMounted: Condition
{
	private readonly bool _value;
	
	/**
	 * Instantiates a new condition player fly mounted.
	 * @param value the value
	 */
	public ConditionPlayerFlyMounted(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector.getActingPlayer() == null) || (effector.getActingPlayer().isFlyingMounted() == _value);
	}
}
