using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Detection effect implementation.
/// </summary>
public sealed class Detection: AbstractEffect
{
    public Detection(StatSet @params)
    {
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        Player? target = effected.getActingPlayer();
        if (!effector.isPlayer() || !effected.isPlayer() || player == null || target == null)
            return;

        bool hasParty = player.isInParty();
        bool hasClan = player.getClanId() > 0;
        bool hasAlly = player.getAllyId() > 0;

        if (target.isInvisible())
        {
            if (hasParty && target.isInParty() &&
                player.getParty()?.getLeaderObjectId() == target.getParty()?.getLeaderObjectId())
                return;

            if (hasClan && player.getClanId() == target.getClanId())
                return;

            if (hasAlly && player.getAllyId() == target.getAllyId())
                return;

            // Remove Hide.
            target.getEffectList().stopEffects(AbnormalType.HIDE);
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}