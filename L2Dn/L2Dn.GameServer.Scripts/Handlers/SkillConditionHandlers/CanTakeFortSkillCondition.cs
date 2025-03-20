using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Serenitty
 */
public class CanTakeFortSkillCondition: ISkillCondition
{
    public CanTakeFortSkillCondition(StatSet @params)
    {
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (!caster.isPlayer() || player == null)
            return false;

        bool canTakeFort = !player.isAlikeDead() && !player.isCursedWeaponEquipped();

        Clan? clan = caster.getClan();

        Fort? fort = FortManager.getInstance().getFortById(FortManager.ORC_FORTRESS);
        if (fort == null || !fort.getSiege().isInProgress() || clan == null ||
            clan.getLevel() < FortSiegeManager.getInstance().getSiegeClanMinLevel())
        {
            var sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canTakeFort = false;
        }
        else if (target != null && target.Id != FortManager.ORC_FORTRESS_FLAGPOLE_ID)
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            canTakeFort = false;
        }

        return canTakeFort;
    }
}