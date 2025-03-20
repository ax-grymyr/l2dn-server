using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Summon Siege Golem implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCanSummonSiegeGolem(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player is null)
        {
            return !value;
        }

        Clan? playerClan = player.getClan();
        bool canSummonSiegeGolem = !(player.isAlikeDead() || player.isCursedWeaponEquipped() || playerClan == null);

        Castle? castle = CastleManager.getInstance().getCastle(player);
        Fort? fort = FortManager.getInstance().getFort(player);
        if (castle == null && fort == null)
            canSummonSiegeGolem = false;

        if ((fort != null && fort.getResidenceId() == 0) || (castle != null && castle.getResidenceId() == 0))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            canSummonSiegeGolem = false;
        }
        else if ((castle != null && !castle.getSiege().isInProgress()) ||
                 (fort != null && !fort.getSiege().isInProgress()))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            canSummonSiegeGolem = false;
        }
        else if (playerClan != null &&
                 ((castle != null && castle.getSiege().getAttackerClan(playerClan.Id) == null) ||
                     (fort != null && fort.getSiege().getAttackerClan(playerClan.Id) == null)))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            canSummonSiegeGolem = false;
        }

        return value == canSummonSiegeGolem;
    }
}