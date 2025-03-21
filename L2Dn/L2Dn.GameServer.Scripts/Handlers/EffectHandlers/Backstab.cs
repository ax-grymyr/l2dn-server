using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Backstab effect implementation.
/// </summary>
public sealed class Backstab: AbstractEffect
{
    private readonly double _power;
    private readonly double _chanceBoost;
    private readonly double _criticalChance;
    private readonly bool _overHit;

    public Backstab(StatSet @params)
    {
        _power = @params.getDouble("power");
        _chanceBoost = @params.getDouble("chanceBoost");
        _criticalChance = @params.getDouble("criticalChance", 0);
        _overHit = @params.getBoolean("overHit", false);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !effector.IsInFrontOf(effected) &&
            !Formulas.calcSkillEvasion(effector, effected, skill) &&
            Formulas.calcBlowSuccess(effector, effected, skill, _chanceBoost);
    }

    public override EffectType getEffectType() => EffectType.PHYSICAL_ATTACK;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        if (_overHit && effected.isAttackable())
            ((Attackable)effected).overhitEnabled(true);

        bool ss = skill.useSoulShot() && (effector.isChargedShot(ShotType.SOULSHOTS) ||
            effector.isChargedShot(ShotType.BLESSED_SOULSHOTS));

        byte shld = Formulas.calcShldUse(effector, effected);
        double damage = Formulas.calcBlowDamage(effector, effected, skill, true, _power, shld, ss);

        if (Formulas.calcCrit(_criticalChance, effector, effected, skill))
            damage *= 2;

        effector.doAttack(damage, effected, skill, false, true, true, false);
    }

    public override int GetHashCode() => HashCode.Combine(_power, _chanceBoost, _criticalChance, _overHit);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._power, x._chanceBoost, x._criticalChance, x._overHit));
}