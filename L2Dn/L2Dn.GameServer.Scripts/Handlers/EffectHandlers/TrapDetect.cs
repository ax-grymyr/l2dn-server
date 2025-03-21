using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trap Detect effect implementation.
/// </summary>
[HandlerStringKey("TrapDetect")]
public sealed class TrapDetect: AbstractEffect
{
    private readonly int _power;

    public TrapDetect(EffectParameterSet parameters)
    {
        _power = parameters.GetInt32(XmlSkillEffectParameterType.Power);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isTrap() || effected.isAlikeDead())
            return;

        Trap trap = (Trap)effected;
        if (trap.getLevel() <= _power)
            trap.setDetected(effector);
    }

    public override int GetHashCode() => _power;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}