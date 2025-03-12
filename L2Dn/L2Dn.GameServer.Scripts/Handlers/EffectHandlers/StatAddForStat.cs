using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class StatAddForStat: AbstractEffect
{
    private readonly Stat _stat;
    private readonly int _min;
    private readonly int _max;
    private readonly Stat _addStat;
    private readonly double _amount;

    public StatAddForStat(StatSet @params)
    {
        _stat = @params.getEnum<Stat>("stat");
        _min = @params.getInt("min", 0);
        _max = @params.getInt("max", 2147483647);
        _addStat = @params.getEnum<Stat>("addStat");
        _amount = @params.getDouble("amount", 0);
        if (@params.getEnum("mode", StatModifierType.DIFF) != StatModifierType.DIFF)
            throw new ArgumentException(nameof(StatAddForStat) + " can only use DIFF mode.");
    }

    public override void pump(Creature effected, Skill skill)
    {
        int currentValue = (int)effected.getStat().getValue(_stat);
        if (currentValue >= _min && currentValue <= _max)
            effected.getStat().mergeAdd(_addStat, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _min, _max, _addStat, _amount);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._stat, x._min, x._max, x._addStat, x._amount));
}