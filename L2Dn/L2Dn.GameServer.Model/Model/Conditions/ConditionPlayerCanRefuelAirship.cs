using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Refuel Airship condition implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCanRefuelAirship(int value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        AirShip? airShip = player?.getAirShip();

        bool canRefuelAirship =
            !(airShip is not ControllableAirShip || airShip.getFuel() + value > airShip.getMaxFuel());

        return canRefuelAirship;
    }
}