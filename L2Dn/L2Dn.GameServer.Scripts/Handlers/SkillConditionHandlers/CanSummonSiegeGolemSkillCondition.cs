using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("CanSummonSiegeGolem")]
public sealed class CanSummonSiegeGolemSkillCondition: ISkillCondition
{
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

        if ((castle != null && castle.getSiege().getAttackerClan(clan.Id) == null) ||
            (fort != null && fort.getSiege().getAttackerClan(clan.Id) == null))
        {
            player.sendPacket(SystemMessageId.INVALID_TARGET);
            return false;
        }

        return true;
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}