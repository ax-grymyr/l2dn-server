using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerCharges.
 */
public class ConditionPlayerCharges: Condition
{
	private readonly int _charges;
	
	/**
	 * Instantiates a new condition player charges.
	 * @param charges the charges
	 */
	public ConditionPlayerCharges(int charges)
	{
		_charges = charges;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector.getActingPlayer() != null) && (effector.getActingPlayer().getCharges() >= _charges);
	}
}
