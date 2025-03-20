using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// MagicalAttack-damage over time effect implementation.
/// </summary>
public sealed class MagicalDamOverTime: AbstractEffect
{
    private readonly double _power;
    private readonly bool _canKill;

    public MagicalDamOverTime(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        _canKill = parameters.GetBoolean(XmlSkillEffectParameterType.CanKill, false);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override EffectTypes EffectTypes => EffectTypes.MAGICAL_DMG_OVER_TIME;

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Creature creature = effector;
        Creature target = effected;

        if (target.isDead())
            return false;

        double damage = Formulas.calcMagicDam(creature, target, skill, creature.getMAtk(), _power, target.getMDef(),
            false, false, false); // In retail spiritshots change nothing.

        damage *= TicksMultiplier;

        if (damage >= target.getCurrentHp() - 1)
        {
            if (skill.IsToggle)
            {
                target.sendPacket(SystemMessageId.YOUR_SKILL_HAS_BEEN_CANCELED_DUE_TO_LACK_OF_HP);
                return false;
            }

            // For DOT skills that will not kill effected player.
            if (!_canKill)
            {
                // Fix for players dying by DOTs if HP < 1 since reduceCurrentHP method will kill them
                if (target.getCurrentHp() <= 1)
                    return skill.IsToggle;

                damage = target.getCurrentHp() - 1;
            }
        }

        effector.doAttack(damage, effected, skill, true, false, false, false);
        return skill.IsToggle;
    }

    public override int GetHashCode() => HashCode.Combine(_power, _canKill, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x._canKill, x.Ticks));
}