using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Physical Attack Mute effect implementation.
/// </summary>
public sealed class PhysicalAttackMute: AbstractEffect
{
    public PhysicalAttackMute(StatSet @params)
    {
    }

    public override long getEffectFlags() => EffectFlag.PSYCHICAL_ATTACK_MUTED.getMask();

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.startPhysicalAttackMuted();
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}