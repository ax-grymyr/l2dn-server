using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerIsClanLeader.
 */
public class ConditionPlayerIsClanLeader : Condition
{
	private readonly bool _value;
	
	/**
	 * Instantiates a new condition player is clan leader.
	 * @param value the value
	 */
	public ConditionPlayerIsClanLeader(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return false;
		}
		return (effector.getActingPlayer().isClanLeader() == _value);
	}
}
