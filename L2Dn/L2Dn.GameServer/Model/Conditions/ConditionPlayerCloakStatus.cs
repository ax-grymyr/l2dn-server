using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerCloakStatus.
 */
public class ConditionPlayerCloakStatus: Condition
{
	private readonly bool _value;
	
	/**
	 * Instantiates a new condition player cloak status.
	 * @param value the value
	 */
	public ConditionPlayerCloakStatus(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector.getActingPlayer() != null) && (effector.getActingPlayer().getInventory().canEquipCloak() == _value);
	}
}
