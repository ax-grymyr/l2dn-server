using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AttackAttribute: AbstractEffect
{
    private readonly FrozenSet<Stat> _stats;
    private readonly double _amount;

    public AttackAttribute(StatSet @params)
    {
        _amount = @params.getDouble("amount", 0);
        string attributes = @params.getString("attribute", "FIRE");
        _stats = ParseUtil.ParseEnumSet<Stat>(attributes, string.Empty, "_POWER", ',');
    }

    public override void Pump(Creature effected, Skill skill)
    {
        foreach (Stat stat in _stats)
            effected.getStat().mergeAdd(stat, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_stats.GetSetHashCode(), _amount);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._stats.GetSetComparable(), x._amount));
}