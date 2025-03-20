using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("StatAddForMp")]
public sealed class StatAddForMp: AbstractEffect
{
    private readonly int _mp;
    private readonly Stat _stat;
    private readonly double _amount;

    public StatAddForMp(EffectParameterSet parameters)
    {
        _mp = parameters.GetInt32(XmlSkillEffectParameterType.Mp, 0);
        _stat = parameters.GetEnum<Stat>(XmlSkillEffectParameterType.Stat);
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        if (effected.getMaxMp() >= _mp)
            effected.getStat().mergeAdd(_stat, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_mp, _stat, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._mp, x._stat, x._amount));
}