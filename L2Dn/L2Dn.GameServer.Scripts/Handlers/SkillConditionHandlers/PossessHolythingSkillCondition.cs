using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("PossessHolything")]
public sealed class PossessHolythingSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (!caster.isPlayer() || player == null)
        {
            return false;
        }

        bool canTakeCastle = player.isClanLeader() && !player.isAlikeDead() && !player.isCursedWeaponEquipped();

        Clan? clan = player.getClan();

        Castle? castle = CastleManager.getInstance().getCastle(player);
        if (castle == null || castle.getResidenceId() <= 0 || !castle.getSiege().isInProgress() || clan == null ||
            castle.getSiege().getAttackerClan(clan) == null)
        {
            var sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canTakeCastle = false;
        }
        else if (!castle.getArtefacts().Contains(target))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            canTakeCastle = false;
        }

        return canTakeCastle;
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}