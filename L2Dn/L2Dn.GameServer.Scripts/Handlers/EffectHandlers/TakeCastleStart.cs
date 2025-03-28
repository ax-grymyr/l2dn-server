using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Take Castle Start effect implementation.
/// </summary>
public sealed class TakeCastleStart: AbstractEffect
{
    public TakeCastleStart(StatSet @params)
    {
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.isPlayer())
            return;

        Castle? castle = CastleManager.getInstance().getCastle(effected);
        if (castle != null && castle.getSiege().isInProgress())
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_OPPOSING_CLAN_HAS_STARTED_S1);
            sm.Params.addSkillName(skill.getId());
            castle.getSiege().announceToPlayer(sm, false);
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}