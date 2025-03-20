using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Soul Blow effect implementation.
/// </summary>
[AbstractEffectName("SoulBlow")]
public sealed class SoulBlow: AbstractEffect
{
    private readonly double _power;
    private readonly double _chanceBoost;
    private readonly bool _overHit;

    public SoulBlow(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power);
        _chanceBoost = parameters.GetDouble(XmlSkillEffectParameterType.ChanceBoost);
        _overHit = parameters.GetBoolean(XmlSkillEffectParameterType.OverHit, false);
    }

    /// <summary>
    /// If is not evaded and blow lands.
    /// </summary>
    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill) &&
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
        double damage = Formulas.calcBlowDamage(effector, effected, skill, false, _power, shld, ss);

        Player? player = effector.getActingPlayer();
        if (effector.isPlayer() && player != null)
        {
            if (skill.MaxLightSoulConsumeCount > 0)
            {
                // Souls Formula (each soul increase +4%)
                int chargedSouls = player.getChargedSouls(SoulType.LIGHT) <= skill.MaxLightSoulConsumeCount
                    ? player.getChargedSouls(SoulType.LIGHT)
                    : skill.MaxLightSoulConsumeCount;

                damage *= 1 + chargedSouls * 0.04;
            }

            if (skill.MaxShadowSoulConsumeCount > 0)
            {
                // Souls Formula (each soul increase +4%)
                int chargedSouls = player.getChargedSouls(SoulType.SHADOW) <= skill.MaxShadowSoulConsumeCount
                    ? player.getChargedSouls(SoulType.SHADOW)
                    : skill.MaxShadowSoulConsumeCount;

                damage *= 1 + chargedSouls * 0.04;
            }
        }

        effector.doAttack(damage, effected, skill, false, false, true, false);
    }

    public override int GetHashCode() => HashCode.Combine(_power, _chanceBoost, _overHit);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x._chanceBoost, x._overHit));
}