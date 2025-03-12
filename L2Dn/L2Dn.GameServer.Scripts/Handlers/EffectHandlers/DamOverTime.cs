using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Dam Over Time effect implementation.
/// </summary>
public sealed class DamOverTime: AbstractEffect
{
    private readonly bool _canKill;
    private readonly double _power;

    public DamOverTime(StatSet @params)
    {
        _canKill = @params.getBoolean("canKill", false);
        _power = @params.getDouble("power");
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

                if (!_canKill && damage >= effected.getCurrentHp() - 1)
                {
                    damage = effected.getCurrentHp() - 1;
                }

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
        if (damage >= effected.getCurrentHp() - 1)
        {
            if (skill.isToggle())
            {
                effected.sendPacket(SystemMessageId.YOUR_SKILL_HAS_BEEN_CANCELED_DUE_TO_LACK_OF_HP);
                return false;
            }

            // For DOT skills that will not kill effected player.
            if (!_canKill)
            {
                // Fix for players dying by DOTs if HP < 1 since reduceCurrentHP method will kill them
                if (effected.getCurrentHp() <= 1)
                    return skill.isToggle();

                damage = effected.getCurrentHp() - 1;
            }
        }

        effector.doAttack(damage, effected, skill, true, false, false, false);
        return skill.isToggle();
    }

    public override int GetHashCode() => HashCode.Combine(_canKill, _power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._canKill, x._power, x.Ticks));
}