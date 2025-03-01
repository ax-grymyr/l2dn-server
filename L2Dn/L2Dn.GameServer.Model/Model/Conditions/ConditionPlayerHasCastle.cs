using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasCastle.
 * @author MrPoke
 */
public sealed class ConditionPlayerHasCastle(int castle): Condition
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
            return castle == 0;

        // Any castle
        if (castle == -1)
            return clan.getCastleId() > 0;

        return clan.getCastleId() == castle;
    }
}