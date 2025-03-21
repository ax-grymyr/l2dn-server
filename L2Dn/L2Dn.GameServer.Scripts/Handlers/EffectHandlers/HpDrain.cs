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
/// HP Drain effect implementation.
/// </summary>
public sealed class HpDrain: AbstractEffect
{
    private readonly double _power;
    private readonly double _percentage;

    public HpDrain(StatSet @params)
    {
        _power = @params.getDouble("power", 0);
        _percentage = @params.getDouble("percentage", 0);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !Formulas.calcSkillEvasion(effector, effected, skill);
    }

    public override EffectType getEffectType() => EffectType.HP_DRAIN;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isAlikeDead())
            return;

        bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
        bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
        bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
        double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power, effected.getMDef(),
            sps, bss, mcrit);

        double drain = 0;
        int cp = (int)effected.getCurrentCp();
        int hp = (int)effected.getCurrentHp();

        if (cp > 0)
            drain = damage < cp ? 0 : damage - cp;
        else if (damage > hp)
            drain = hp;
        else
            drain = damage;

        double hpAdd = _percentage / 100 * drain;
        double hpFinal = effector.getCurrentHp() + hpAdd > effector.getMaxHp()
            ? effector.getMaxHp()
            : effector.getCurrentHp() + hpAdd;

        effector.setCurrentHp(hpFinal);

        effector.doAttack(damage, effected, skill, false, false, mcrit, false);
    }

    public override int GetHashCode() => HashCode.Combine(_power, _percentage);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x._percentage));
}