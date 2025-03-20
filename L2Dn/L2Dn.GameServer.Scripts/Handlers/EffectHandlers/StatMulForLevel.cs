using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class StatMulForLevel: AbstractEffect
{
    private readonly Stat _stat;
    private readonly FrozenDictionary<int, double> _values;

    public StatMulForLevel(StatSet @params)
    {
        _stat = @params.getEnum<Stat>("stat");

        List<int> amount = @params.getIntegerList("amount");
        _values = @params.getIntegerList("level").
            Select((level, index) => new KeyValuePair<int, double>(level, amount[index])).ToFrozenDictionary();

        if (@params.getEnum("mode", StatModifierType.PER) != StatModifierType.PER)
            throw new ArgumentException(nameof(StatMulForLevel) + " can only use PER mode.");
    }

    public override void Pump(Creature effected, Skill skill)
    {
        if (_values.TryGetValue(effected.getLevel(), out double amount))
            effected.getStat().mergeMul(_stat, amount / 100 + 1);
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _values.GetDictionaryHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._stat, x._values.GetDictionaryComparable()));
}