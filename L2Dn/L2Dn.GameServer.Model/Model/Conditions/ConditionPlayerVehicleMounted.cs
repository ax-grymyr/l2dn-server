using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Nyaran
 */
public class ConditionPlayerVehicleMounted : Condition
{
	private readonly bool _value;
	
	/**
	 * @param value the value
	 */
	public ConditionPlayerVehicleMounted(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return true;
		}
		return (effector.getActingPlayer().isInVehicle() == _value);
	}
}
