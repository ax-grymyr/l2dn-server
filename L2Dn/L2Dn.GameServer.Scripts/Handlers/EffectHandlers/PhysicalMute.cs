using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Physical Mute effect implementation.
/// </summary>
public sealed class PhysicalMute: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.PSYCHICAL_MUTED;

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getAI().notifyEvent(CtrlEvent.EVT_MUTED);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}