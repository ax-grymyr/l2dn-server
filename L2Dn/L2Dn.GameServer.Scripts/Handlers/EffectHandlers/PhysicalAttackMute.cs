using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Physical Attack Mute effect implementation.
/// </summary>
[AbstractEffectName("PhysicalAttackMute")]
public sealed class PhysicalAttackMute: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.PSYCHICAL_ATTACK_MUTED;

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.startPhysicalAttackMuted();
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}