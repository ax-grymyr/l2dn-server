using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCheckFlag")]
public sealed class OpCheckFlagSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (!caster.isPlayer() || player == null)
        {
            return false;
        }

        bool canTakeFort = player.isClanLeader() && !player.isAlikeDead() && !player.isCursedWeaponEquipped();

        Fort? fort = FortManager.getInstance().getFort(player);
        if (fort == null || fort.getResidenceId() <= 0 || !fort.getSiege().isInProgress() ||
            fort.getSiege().getAttackerClan(player.getClan()) == null)
        {
            var sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canTakeFort = false;
        }
        else if (fort.getFlagPole() != target)
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            canTakeFort = false;
        }
        else if (!Util.checkIfInRange(200, player, target, true))
        {
            player.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
            canTakeFort = false;
        }

        return canTakeFort;
    }
}