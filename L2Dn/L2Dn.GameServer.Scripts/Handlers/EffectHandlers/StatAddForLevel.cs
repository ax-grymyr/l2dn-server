using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("StatAddForLevel")]
public sealed class StatAddForLevel: AbstractEffect
{
    private readonly FrozenDictionary<int, double> _values;
    private readonly Stat _stat;

    public StatAddForLevel(EffectParameterSet parameters)
    {
        _stat = parameters.GetEnum<Stat>(XmlSkillEffectParameterType.Stat);

        string amountStr = parameters.GetString(XmlSkillEffectParameterType.Amount);
        ImmutableArray<int> amounts = ParseUtil.ParseList<int>(amountStr, ',');

        string levelStr = parameters.GetString(XmlSkillEffectParameterType.Level);
        ImmutableArray<int> levels = ParseUtil.ParseList<int>(levelStr, ',');

        _values = levels.Select((level, index) => new KeyValuePair<int, double>(level, amounts[index])).
            ToFrozenDictionary();

        if (parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF) != StatModifierType.DIFF)
            throw new ArgumentException(nameof(StatAddForLevel) + " can only use DIFF mode.");
    }

    public override void Pump(Creature effected, Skill skill)
    {
        if (_values.TryGetValue(effected.getLevel(), out double amount))
            effected.getStat().mergeAdd(_stat, amount);
    }

    public override int GetHashCode() => HashCode.Combine(_values.GetDictionaryHashCode(), _stat);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._values.GetDictionaryComparable(), x._stat));
}