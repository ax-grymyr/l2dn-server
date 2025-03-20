using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Take Fort condition implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCanTakeFort(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player is null || skill is null)
            return false;

        bool canTakeFort = !(player.isAlikeDead() || player.isCursedWeaponEquipped() || !player.isClanLeader());

        Fort? fort = FortManager.getInstance().getFort(player);
        if (fort == null || fort.getResidenceId() <= 0 || !fort.getSiege().isInProgress() ||
            fort.getSiege().getAttackerClan(player.getClan()) == null)
        {
            SystemMessagePacket sm = new(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canTakeFort = false;
        }
        else if (fort.getFlagPole() != effected)
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            canTakeFort = false;
        }
        else if (!Util.checkIfInRange(200, player, effected, true))
        {
            player.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
            canTakeFort = false;
        }

        return value == canTakeFort;
    }
}