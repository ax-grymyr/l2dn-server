using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Refuel Airship condition implementation.
 * @author Adry_85
 */
public class ConditionPlayerCanRefuelAirship: Condition
{
	private readonly int _value;

	public ConditionPlayerCanRefuelAirship(int value)
	{
		_value = value;
	}

	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool canRefuelAirship = true;
		Player player = effector.getActingPlayer();
		if ((player == null) || (player.getAirShip() == null) || !(player.getAirShip() is ControllableAirShip) ||
		    ((player.getAirShip().getFuel() + _value) > player.getAirShip().getMaxFuel()))
		{
			canRefuelAirship = false;
		}

		return canRefuelAirship;
	}
}