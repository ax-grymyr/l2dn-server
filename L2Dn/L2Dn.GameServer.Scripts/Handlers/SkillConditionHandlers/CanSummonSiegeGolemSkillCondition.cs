using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class CanSummonSiegeGolemSkillCondition: ISkillCondition
{
    public CanSummonSiegeGolemSkillCondition(StatSet @params)
    {
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (!caster.isPlayer() || player == null)
            return false;

        Clan? clan = player.getClan();
        if (player.isAlikeDead() || player.isCursedWeaponEquipped() || clan == null)
            return false;

        Castle? castle = CastleManager.getInstance().getCastle(player);
        Fort? fort = FortManager.getInstance().getFort(player);
        if (castle == null && fort == null)
            return false;

        if ((fort != null && fort.getResidenceId() == 0) || (castle != null && castle.getResidenceId() == 0))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            return false;
        }

        if ((castle != null && !castle.getSiege().isInProgress()) || (fort != null && !fort.getSiege().isInProgress()))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            return false;
        }

        if ((castle != null && castle.getSiege().getAttackerClan(clan.getId()) == null) ||
            (fort != null && fort.getSiege().getAttackerClan(clan.getId()) == null))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            return false;
        }

        return true;
    }
}