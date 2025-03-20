using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// An effect that changes damage taken from an attack.
/// The retail implementation seems to be altering whatever damage is taken after the attack has been done and
/// not when attack is being done. Exceptions for this effect appears to be DOT effects and terrain damage,
/// they are unaffected by this stat. As for example in retail this effect does reduce reflected damage taken
/// (because it is received damage), as well as it does not decrease reflected damage done,
/// because reflected damage is being calculated with the original attack damage and not this altered one.
/// Multiple values of this effect add-up to each other rather than multiplying with each other. Be careful,
/// there were cases in retail where damage is deacreased to 0.
/// </summary>
public sealed class DamageByAttack: AbstractEffect
{
    private readonly double _value;
    private readonly DamageByAttackType _type;

    public DamageByAttack(EffectParameterSet parameters)
    {
        _value = parameters.GetDouble(XmlSkillEffectParameterType.Amount);
        _type = parameters.GetEnum(XmlSkillEffectParameterType.Type, DamageByAttackType.NONE);
        if (parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF) != StatModifierType.DIFF)
        {
            Logger.Warn(GetType().Name + " can only use DIFF mode.");
        }
    }

    public override void Pump(Creature target, Skill skill)
    {
        switch (_type)
        {
            case DamageByAttackType.PK:
            {
                target.getStat().mergeAdd(Stat.PVP_DAMAGE_TAKEN, _value);
                break;
            }
            case DamageByAttackType.ENEMY_ALL:
            {
                target.getStat().mergeAdd(Stat.PVE_DAMAGE_TAKEN, _value);
                break;
            }
            case DamageByAttackType.MOB:
            {
                target.getStat().mergeAdd(Stat.PVE_DAMAGE_TAKEN_MONSTER, _value);
                break;
            }
            case DamageByAttackType.BOSS:
            {
                target.getStat().mergeAdd(Stat.PVE_DAMAGE_TAKEN_RAID, _value);
                break;
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_value, _type);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._value, x._type));
}