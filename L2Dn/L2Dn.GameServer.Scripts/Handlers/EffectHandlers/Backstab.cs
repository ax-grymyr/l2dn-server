using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
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

    public Backstab(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power);
        _chanceBoost = parameters.GetDouble(XmlSkillEffectParameterType.ChanceBoost);
        _criticalChance = parameters.GetDouble(XmlSkillEffectParameterType.CriticalChance, 0);
        _overHit = parameters.GetBoolean(XmlSkillEffectParameterType.OverHit, false);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !effector.IsInFrontOf(effected) &&
            !Formulas.calcSkillEvasion(effector, effected, skill) &&
            Formulas.calcBlowSuccess(effector, effected, skill, _chanceBoost);
    }

    public override EffectTypes EffectTypes => EffectTypes.PHYSICAL_ATTACK;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        if (_overHit && effected.isAttackable())
            ((Attackable)effected).overhitEnabled(true);

        bool ss = skill.UseSoulShot && (effector.isChargedShot(ShotType.SOULSHOTS) ||
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