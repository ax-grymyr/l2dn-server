using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class DefenceAttribute: AbstractEffect
{
    private readonly double _amount;
    private readonly FrozenSet<Stat> _stats;

    public DefenceAttribute(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        string attributes = parameters.GetString(XmlSkillEffectParameterType.Attribute, "FIRE");
        _stats = ParseUtil.ParseEnumSet<Stat>(attributes, string.Empty, "_RES", ',');
        if (_stats.Count == 0)
            throw new ArgumentException("No stats defined for DefenceAttribute effect");
    }

    public override void Pump(Creature effected, Skill skill)
    {
        foreach (Stat stat in _stats)
            effected.getStat().mergeAdd(stat, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _stats.GetSetHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._amount, x._stats.GetSetComparable()));
}