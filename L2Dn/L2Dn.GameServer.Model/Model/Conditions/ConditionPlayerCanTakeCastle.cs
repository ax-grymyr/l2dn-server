using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Take Castle condition implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCanTakeCastle(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player is null || skill is null)
            return false;

        Clan? clan = player.getClan();

        bool canTakeCastle = player.isClanLeader() && !player.isAlikeDead() && !player.isCursedWeaponEquipped();

        Castle? castle = CastleManager.getInstance().getCastle(player);
        if (castle == null || castle.getResidenceId() <= 0 || !castle.getSiege().isInProgress() ||
            clan == null || castle.getSiege().getAttackerClan(clan) == null)
        {
            SystemMessagePacket sm = new(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canTakeCastle = false;
        }
        else if (effected == null || !castle.getArtefacts().Contains(effected))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            canTakeCastle = false;
        }
        else if (!Util.checkIfInRange(200, player, effected, true) || player.getZ() < effected.getZ() ||
                 Math.Abs(player.getZ() - effected.getZ()) > 40)
        {
            player.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
            canTakeCastle = false;
        }

        return value == canTakeCastle;
    }
}