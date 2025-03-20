using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// StatByMoveType effect implementation.
/// </summary>
public sealed class StatByMoveType: AbstractEffect
{
    private readonly Stat _stat;
    private readonly MoveType _type;
    private readonly double _value;

    public StatByMoveType(EffectParameterSet parameters)
    {
        _stat = parameters.GetEnum<Stat>(XmlSkillEffectParameterType.Stat);
        _type = parameters.GetEnum<MoveType>(XmlSkillEffectParameterType.Type);
        _value = parameters.GetDouble(XmlSkillEffectParameterType.Value);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getStat().mergeMoveTypeValue(_stat, _type, _value);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getStat().mergeMoveTypeValue(_stat, _type, -_value);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        return skill.IsPassive || skill.IsToggle;
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _type, _value);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._stat, x._type, x._value));
}