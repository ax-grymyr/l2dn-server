using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class TwoHandedStance: AbstractEffect
{
    private readonly double _amount;

    public TwoHandedStance(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.PHYSICAL_ATTACK, _amount * effected.getShldDef() / 100);
    }

    public override int GetHashCode() => HashCode.Combine(_amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}