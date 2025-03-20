using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public abstract class AbstractStatAddEffect: AbstractEffect
{
    private readonly Stat _stat;
    private readonly double _amount;

    protected AbstractStatAddEffect(EffectParameterSet parameters, Stat stat)
    {
        _stat = stat;
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        if (parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF) != StatModifierType.DIFF)
            Logger.Warn(GetType().Name + " can only use DIFF mode.");
    }

    public double Amount => _amount;

    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(_stat, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._stat, x._amount));
}