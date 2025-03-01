using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasClanHall.
 * @author MrPoke
 */
public sealed class ConditionPlayerHasClanHall(List<int> clanHall): Condition
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
            return clanHall is [0];

        // All Clan Halls
        if (clanHall is [-1])
            return clan.getHideoutId() > 0;

        return clanHall.Contains(clan.getHideoutId());
    }
}