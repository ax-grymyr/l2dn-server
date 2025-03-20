using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Traps;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trap Remove effect implementation.
/// </summary>
public sealed class TrapRemove: AbstractEffect
{
    private readonly int _power;

    public TrapRemove(EffectParameterSet parameters)
    {
        _power = parameters.GetInt32(XmlSkillEffectParameterType.Power);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isTrap())
            return;

        if (effected.isAlikeDead())
            return;

        Trap trap = (Trap)effected;
        if (!trap.canBeSeen(effector))
        {
            if (effector.isPlayer())
                effector.sendPacket(SystemMessageId.INVALID_TARGET);

            return;
        }

        if (trap.getLevel() > _power)
            return;

        // Notify to scripts
        if (trap.Events.HasSubscribers<OnTrapAction>())
            trap.Events.NotifyAsync(new OnTrapAction(trap, effector, TrapAction.TRAP_DISARMED));

        trap.unSummon();
        if (effector.isPlayer())
            effector.sendPacket(SystemMessageId.THE_TRAP_DEVICE_HAS_BEEN_STOPPED);
    }

    public override int GetHashCode() => _power;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}