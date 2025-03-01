using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Mobius
 */
public sealed class ConditionMinimumVitalityPoints(int count): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player is null)
            return false;

        return player.getVitalityPoints() >= count;
    }
}