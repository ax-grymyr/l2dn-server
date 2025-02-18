using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasFort.
 * @author MrPoke
 */
public sealed class ConditionPlayerHasFort(int fort): Condition
{
    /**
     * Test impl.
     * @return true, if successful
     */
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player is null)
            return false;

        Clan? clan = player.getClan();
        if (clan is null)
            return fort == 0;

        // Any fortress
        if (fort == -1)
            return clan.getFortId() > 0;

        return clan.getFortId() == fort;
    }
}