using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class StatUp: AbstractEffect
{
    private readonly double _amount;
    private readonly FrozenSet<Stat> _stats;

    public StatUp(StatSet @params)
    {
        _amount = @params.getDouble("amount", 0);
        string stats = @params.getString("stat", "STR");
        _stats = ParseUtil.ParseEnumSet<Stat>(stats, "STAT_", string.Empty, ',');
    }

    public override void pump(Creature effected, Skill skill)
    {
        foreach (Stat stat in _stats)
            effected.getStat().mergeAdd(stat, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _stats.GetSetHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._amount, x._stats.GetSetComparable()));
}