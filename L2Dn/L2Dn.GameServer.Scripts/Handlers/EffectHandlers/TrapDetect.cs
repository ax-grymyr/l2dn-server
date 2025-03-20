using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trap Detect effect implementation.
/// </summary>
public sealed class TrapDetect: AbstractEffect
{
    private readonly int _power;

    public TrapDetect(StatSet @params)
    {
        _power = @params.getInt("power");
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