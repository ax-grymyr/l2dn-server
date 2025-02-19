using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class BuildCampSkillCondition: ISkillCondition
{
    public BuildCampSkillCondition(StatSet @params)
    {
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (!caster.isPlayer() || player is null)
            return false;

        if (player.isAlikeDead() || player.isCursedWeaponEquipped() || player.getClan() == null)
            return false;

        Castle? castle = CastleManager.getInstance().getCastle(player);
        Fort? fort = FortManager.getInstance().getFort(player);
        SystemMessagePacket sm;
        if (castle == null && fort == null)
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            return false;
        }

        if ((castle != null && !castle.getSiege().isInProgress()) ||
            (fort != null && !fort.getSiege().isInProgress()))
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            return false;
        }

        Clan? clan = player.getClan();
        if (clan == null || !player.isClanLeader())
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            return false;
        }

        SiegeClan? castleAttackerClan = castle?.getSiege().getAttackerClan(clan);
        SiegeClan? fortAttackerClan = fort?.getSiege().getAttackerClan(clan);

        if ((castle != null && castleAttackerClan == null) || (fort != null && fortAttackerClan == null))
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            return false;
        }

        if ((castle != null && castleAttackerClan != null && castleAttackerClan.getNumFlags() >=
                SiegeManager.getInstance().getFlagMaxCount()) || (fort != null && fortAttackerClan != null &&
                fortAttackerClan.getNumFlags() >= FortSiegeManager.getInstance().getFlagMaxCount()))
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            return false;
        }

        if (!player.isInsideZone(ZoneId.HQ))
        {
            player.sendPacket(SystemMessageId.YOU_CAN_T_BUILD_HEADQUARTERS_HERE);
            return false;
        }

        return true;
    }
}