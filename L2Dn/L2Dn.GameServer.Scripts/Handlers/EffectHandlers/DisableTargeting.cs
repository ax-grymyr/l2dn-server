using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Targeting disable effect implementation. When affected,
/// player will lose target and be unable to target for the duration.
/// </summary>
public sealed class DisableTargeting: AbstractEffect
{
    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.setTarget(null);
        effected.abortAttack();
        effected.abortCast();
        effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
    }

    public override EffectFlags EffectFlags => EffectFlags.TARGETING_DISABLED;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}