using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerRace.
 * @author mkizub, Zoey76
 */
public sealed class ConditionPlayerRace(Set<Race> races): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (effector == null || !effector.isPlayer() || player is null)
            return false;

        return races.Contains(player.getRace());
    }
}