using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerSiegeSide.
 */
public sealed class ConditionPlayerSiegeSide(int side): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? actingPlayer = effector.getActingPlayer();
        if (actingPlayer is null)
            return false;

        return actingPlayer.getSiegeSide() == side;
    }
}