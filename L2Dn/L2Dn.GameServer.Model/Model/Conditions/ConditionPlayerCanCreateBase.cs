using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Create Base condition implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCanCreateBase(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player is null || skill is null)
            return false;

        Clan? clan = player.getClan();
        if (clan is null)
            return false;

        bool canCreateBase = !(player.isAlikeDead() || player.isCursedWeaponEquipped() || player.getClan() == null);

        Castle? castle = CastleManager.getInstance().getCastle(player);
        Fort? fort = FortManager.getInstance().getFort(player);
        SystemMessagePacket sm;
        if (castle == null && fort == null)
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canCreateBase = false;
        }
        else if ((castle != null && !castle.getSiege().isInProgress()) ||
                 (fort != null && !fort.getSiege().isInProgress()))
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canCreateBase = false;
        }
        else if ((castle != null && castle.getSiege().getAttackerClan(clan) == null) ||
                 (fort != null && fort.getSiege().getAttackerClan(clan) == null))
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canCreateBase = false;
        }
        else if (!player.isClanLeader())
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canCreateBase = false;
        }
        else if ((castle != null && castle.getSiege().getAttackerClan(clan)?.getNumFlags() >=
                     SiegeManager.getInstance().getFlagMaxCount()) || (fort != null &&
                     fort.getSiege().getAttackerClan(clan)?.getNumFlags() >=
                     FortSiegeManager.getInstance().getFlagMaxCount()))
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canCreateBase = false;
        }
        else if (!player.isInsideZone(ZoneId.HQ))
        {
            player.sendPacket(SystemMessageId.YOU_CAN_T_BUILD_HEADQUARTERS_HERE);
            canCreateBase = false;
        }

        return value == canCreateBase;
    }
}