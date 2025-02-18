using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Nyaran
 */
public sealed class ConditionPlayerVehicleMounted(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player is null)
            return true;

        return player.isInVehicle() == value;
    }
}