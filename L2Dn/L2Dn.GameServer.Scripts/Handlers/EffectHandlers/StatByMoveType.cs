using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
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

    public StatByMoveType(StatSet @params)
    {
        _stat = @params.getEnum<Stat>("stat");
        _type = @params.getEnum<MoveType>("type");
        _value = @params.getDouble("value");
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getStat().mergeMoveTypeValue(_stat, _type, _value);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getStat().mergeMoveTypeValue(_stat, _type, -_value);
    }

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        return skill.isPassive() || skill.isToggle();
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _type, _value);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._stat, x._type, x._value));
}