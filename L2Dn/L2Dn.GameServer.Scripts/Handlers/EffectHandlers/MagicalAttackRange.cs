using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Magical Attack effect implementation.
/// </summary>
public sealed class MagicalAttackRange: AbstractEffect
{
    private readonly double _power;
    private readonly double _shieldDefPercent;

    public MagicalAttackRange(StatSet @params)
    {
        _power = @params.getDouble("power");
        _shieldDefPercent = @params.getDouble("shieldDefPercent", 0);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill);
    }

    public override EffectType getEffectType() => EffectType.MAGICAL_ATTACK;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayer() && effectedPlayer != null && effectedPlayer.isFakeDeath() &&
            Config.FAKE_DEATH_DAMAGE_STAND)
        {
            effected.stopFakeDeath(true);
        }

        double mDef = effected.getMDef();
        switch (Formulas.calcShldUse(effector, effected))
        {
            case Formulas.SHIELD_DEFENSE_SUCCEED:
            {
                mDef += effected.getShldDef() * _shieldDefPercent / 100;
                break;
            }
            case Formulas.SHIELD_DEFENSE_PERFECT_BLOCK:
            {
                mDef = -1;
                break;
            }
        }

        double damage = 1;
        bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);

        if (mDef != -1)
        {
            bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
            bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);

            damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power, mDef, sps, bss,
                mcrit);
        }

        effector.doAttack(damage, effected, skill, false, false, mcrit, false);
    }

    public override int GetHashCode() => HashCode.Combine(_power, _shieldDefPercent);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x._shieldDefPercent));
}