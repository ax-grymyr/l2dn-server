using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// HpToOwner effect implementation.
/// </summary>
public sealed class HpToOwner: AbstractEffect
{
    private readonly double _power;
    private readonly int _stealAmount;

    public HpToOwner(StatSet @params)
    {
        _power = @params.getDouble("power");
        _stealAmount = @params.getInt("stealAmount");
        Ticks = @params.getInt("ticks");
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!skill.isToggle() && skill.isMagic())
        {
            // TODO: M.Crit can occur even if this skill is resisted. Only then m.crit damage is applied and not debuff
            bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
            if (mcrit)
            {
                double damage = _power * 10; // Tests show that 10 times HP DOT is taken during magic critical.
                effected.reduceCurrentHp(damage, effector, skill, true, false, true, false);
            }
        }
    }

    public override EffectType getEffectType() => EffectType.DMG_OVER_TIME;

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        double damage = _power * TicksMultiplier;

        effector.doAttack(damage, effected, skill, true, false, false, false);
        if (_stealAmount > 0)
        {
            double amount = damage * _stealAmount / 100;
            effector.setCurrentHp(effector.getCurrentHp() + amount);
            effector.setCurrentMp(effector.getCurrentMp() + amount);
        }

        return skill.isToggle();
    }

    public override int GetHashCode() => HashCode.Combine(_power, _stealAmount, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x._stealAmount, x.Ticks));
}