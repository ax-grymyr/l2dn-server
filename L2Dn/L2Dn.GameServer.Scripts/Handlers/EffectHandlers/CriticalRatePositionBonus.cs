using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class CriticalRatePositionBonus: AbstractEffect
{
    private readonly double _amount;
    private readonly Position _position;

    public CriticalRatePositionBonus(StatSet @params)
    {
        _amount = @params.getDouble("amount", 0);
        _position = @params.getEnum("position", Position.Front);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getStat().mergePositionTypeValue(Stat.CRITICAL_RATE, _position, _amount / 100 + 1, (a, b) => a * b);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getStat().mergePositionTypeValue(Stat.CRITICAL_RATE, _position, _amount / 100 + 1, (a, b) => a / b);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _position);

    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._position));
}