using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerIsHero.
 */
public class ConditionPlayerIsHero : Condition
{
	private readonly bool _value;
	
	/**
	 * Instantiates a new condition player is hero.
	 * @param value the value
	 */
	public ConditionPlayerIsHero(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return false;
		}
		return (effector.getActingPlayer().isHero() == _value);
	}
}
