using System.Collections.Frozen;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasClanHall.
 * @author MrPoke
 */
public sealed class ConditionPlayerHasClanHall(FrozenSet<int> clanHalls): Condition
{
    /**
     * Test impl.
     * @return true, if successful
     */
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player is null)
            return false;

        Clan? clan = player.getClan();
        if (clan is null)
            return clanHalls.Count == 1 && clanHalls.Contains(0);

        // All Clan Halls
        if (clanHalls.Count == 1 && clanHalls.Contains(-1))
            return clan.getHideoutId() > 0;

        return clanHalls.Contains(clan.getHideoutId());
    }
}