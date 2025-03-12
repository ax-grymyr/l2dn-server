using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Magical Attack MP effect.
/// </summary>
public sealed class MagicalAttackMp: AbstractEffect
{
    private readonly double _power;
    private readonly bool _critical;
    private readonly double _criticalLimit;

    public MagicalAttackMp(StatSet @params)
    {
        _power = @params.getDouble("power");
        _critical = @params.getBoolean("critical");
        _criticalLimit = @params.getDouble("criticalLimit");
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isMpBlocked())
            return false;

        if (effector.isPlayer() && effected.isPlayer() && effected.isAffected(EffectFlag.DUELIST_FURY) &&
            !effector.isAffected(EffectFlag.DUELIST_FURY))
            return false;

        if (!Formulas.calcMagicAffected(effector, effected, skill))
        {
            if (effector.isPlayer())
                effector.sendPacket(SystemMessageId.YOUR_ATTACK_HAS_FAILED);

            if (effected.isPlayer())
            {
                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_RESISTED_C2_S_DRAIN);
                sm.Params.addString(effected.getName());
                sm.Params.addString(effector.getName());
                effected.sendPacket(sm);
            }

            return false;
        }

        return true;
    }

    public override EffectType getEffectType() => EffectType.MAGICAL_ATTACK;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
        bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
        byte shld = Formulas.calcShldUse(effector, effected);
        bool mcrit = _critical && Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
        double damage = Formulas.calcManaDam(effector, effected, skill, _power, shld, sps, bss, mcrit, _criticalLimit);
        double mp = Math.Min(effected.getCurrentMp(), damage);

        if (damage > 0)
        {
            effected.stopEffectsOnDamage();
            effected.setCurrentMp(effected.getCurrentMp() - mp);
        }

        if (effected.isPlayer())
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S2_S_MP_HAS_BEEN_DRAINED_BY_C1);
            sm.Params.addString(effector.getName());
            sm.Params.addInt((int)mp);
            effected.sendPacket(sm);
        }

        if (effector.isPlayer())
        {
            SystemMessagePacket sm2 = new SystemMessagePacket(SystemMessageId.YOUR_OPPONENT_S_MP_WAS_REDUCED_BY_S1);
            sm2.Params.addInt((int)mp);
            effector.sendPacket(sm2);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_power, _critical, _criticalLimit);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._power, x._critical, x._criticalLimit));
}