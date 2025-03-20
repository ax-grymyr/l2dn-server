using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Fatal Blow effect implementation.
/// </summary>
public sealed class FatalBlow: AbstractEffect
{
    private readonly double _power;
    private readonly double _chanceBoost;
    private readonly double _criticalChance;
    private readonly FrozenSet<AbnormalType> _abnormals;
    private readonly double _abnormalPower;
    private readonly bool _overHit;

    public FatalBlow(StatSet @params)
    {
        _power = @params.getDouble("power");
        _chanceBoost = @params.getDouble("chanceBoost");
        _criticalChance = @params.getDouble("criticalChance", 0);
        _overHit = @params.getBoolean("overHit", false);

        string abnormals = @params.getString("abnormalType", string.Empty);
        _abnormals = ParseUtil.ParseEnumSet<AbnormalType>(abnormals);
        _abnormalPower = @params.getDouble("abnormalPower", 1);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill) &&
            Formulas.calcBlowSuccess(effector, effected, skill, _chanceBoost);
    }

    public override EffectTypes EffectType => EffectTypes.PHYSICAL_ATTACK;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        if (_overHit && effected.isAttackable())
            ((Attackable)effected).overhitEnabled(true);

        double power = _power;

        // Check if we apply an abnormal modifier.
        if (_abnormals.Count != 0)
        {
            foreach (AbnormalType abnormal in _abnormals)
            {
                if (effected.hasAbnormalType(abnormal))
                {
                    power += _abnormalPower;
                    break;
                }
            }
        }

        bool ss = skill.UseSoulShot && (effector.isChargedShot(ShotType.SOULSHOTS) ||
            effector.isChargedShot(ShotType.BLESSED_SOULSHOTS));

        byte shld = Formulas.calcShldUse(effector, effected);
        double damage = Formulas.calcBlowDamage(effector, effected, skill, false, power, shld, ss);
        bool crit = Formulas.calcCrit(_criticalChance, effector, effected, skill);

        if (crit)
            damage *= 2;

        effector.doAttack(damage, effected, skill, false, false, true, false);
    }

    public override int GetHashCode() =>
        HashCode.Combine(_power, _chanceBoost, _criticalChance, _abnormals.GetSetHashCode(), _abnormalPower, _overHit);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._power, x._chanceBoost, x._criticalChance, x._abnormals.GetSetComparable(), x._abnormalPower,
                x._overHit));
}