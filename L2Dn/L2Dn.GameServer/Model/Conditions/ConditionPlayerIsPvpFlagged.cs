using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerIsPvpFlagged.
 * @author Mobius
 */
public class ConditionPlayerIsPvpFlagged : Condition
{
	private readonly bool _value;
	
	/**
	 * Instantiates a new condition player is PvP flagged.
	 * @param value the value
	 */
	public ConditionPlayerIsPvpFlagged(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector.getActingPlayer() != null) && ((effector.getActingPlayer().getPvpFlag()) == _value);
	}
}
