using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("CriticalDamagePosition")]
public sealed class CriticalDamagePosition: AbstractEffect
{
    private readonly double _amount;
    private readonly Position _position;

    public CriticalDamagePosition(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        _position = parameters.GetEnum(XmlSkillEffectParameterType.Position, Position.Front);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getStat().mergePositionTypeValue(Stat.CRITICAL_DAMAGE, _position, _amount / 100 + 1, (a, b) => a * b);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getStat().mergePositionTypeValue(Stat.CRITICAL_DAMAGE, _position, _amount / 100 + 1, (a, b) => a / b);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _position);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._position));
}