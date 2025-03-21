using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("StatAddForStat")]
public sealed class StatAddForStat: AbstractEffect
{
    private readonly Stat _stat;
    private readonly int _min;
    private readonly int _max;
    private readonly Stat _addStat;
    private readonly double _amount;

    public StatAddForStat(EffectParameterSet parameters)
    {
        _stat = parameters.GetEnum<Stat>(XmlSkillEffectParameterType.Stat);
        _min = parameters.GetInt32(XmlSkillEffectParameterType.Min, 0);
        _max = parameters.GetInt32(XmlSkillEffectParameterType.Max, 2147483647);
        _addStat = parameters.GetEnum<Stat>(XmlSkillEffectParameterType.AddStat);
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        if (parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF) != StatModifierType.DIFF)
            throw new ArgumentException(nameof(StatAddForStat) + " can only use DIFF mode.");
    }

    public override void Pump(Creature effected, Skill skill)
    {
        int currentValue = (int)effected.getStat().getValue(_stat);
        if (currentValue >= _min && currentValue <= _max)
            effected.getStat().mergeAdd(_addStat, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _min, _max, _addStat, _amount);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._stat, x._min, x._max, x._addStat, x._amount));
}