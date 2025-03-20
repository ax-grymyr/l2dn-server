using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Damage Over Time Percent effect implementation.
/// </summary>
public sealed class DamOverTimePercent: AbstractEffect
{
    private readonly bool _canKill;
    private readonly double _power;

    public DamOverTimePercent(EffectParameterSet parameters)
    {
        _canKill = parameters.GetBoolean(XmlSkillEffectParameterType.CanKill, false);
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override EffectTypes EffectTypes => EffectTypes.DMG_OVER_TIME_PERCENT;

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        double damage = effected.getCurrentHp() * _power * TicksMultiplier;
        if (damage >= effected.getCurrentHp() - 1)
        {
            if (skill.IsToggle)
            {
                effected.sendPacket(SystemMessageId.YOUR_SKILL_HAS_BEEN_CANCELED_DUE_TO_LACK_OF_HP);
                return false;
            }

            // For DOT skills that will not kill effected player.
            if (!_canKill)
            {
                // Fix for players dying by DOTs if HP < 1 since reduceCurrentHP method will kill them
                if (effected.getCurrentHp() <= 1)
                    return skill.IsToggle;

                damage = effected.getCurrentHp() - 1;
            }
        }

        effector.doAttack(damage, effected, skill, true, false, false, false);
        return skill.IsToggle;
    }

    public override int GetHashCode() => HashCode.Combine(_canKill, _power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._canKill, x._power, x.Ticks));
}