using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Mana Heal Percent effect implementation.
/// </summary>
public sealed class ManaHealPercent: AbstractEffect
{
    private readonly double _power;

    public ManaHealPercent(StatSet @params)
    {
        _power = @params.getDouble("power", 0);
    }

    public override EffectType getEffectType() => EffectType.MANAHEAL_PERCENT;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null || effected.isDead() || effected.isDoor() || effected.isMpBlocked())
            return;

        double power = _power;
        bool full = power == 100.0;

        double amount = full ? effected.getMaxMp() : effected.getMaxMp() * power / 100.0;
        if (item != null && (item.isPotion() || item.isElixir()))
            amount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_MP, 0);

        // Prevents overheal
        amount = Math.Min(amount, Math.Max(0, effected.getMaxRecoverableMp() - effected.getCurrentMp()));
        if (amount != 0)
        {
            double newMp = amount + effected.getCurrentMp();
            effected.setCurrentMp(newMp, false);
            effected.broadcastStatusUpdate(effector);
        }

        SystemMessagePacket sm;
        if (effector.ObjectId != effected.ObjectId)
        {
            sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S2_MP_WITH_C1_S_HELP);
            sm.Params.addString(effector.getName());
        }
        else
        {
            sm = new SystemMessagePacket(SystemMessageId.S1_MP_HAS_BEEN_RESTORED);
        }

        sm.Params.addInt((int)amount);
        effected.sendPacket(sm);
    }

    public override int GetHashCode() => HashCode.Combine(_power);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}