using L2Dn.GameServer.AI;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Betray effect implementation.
/// </summary>
[HandlerName("Betray")]
public sealed class Betray: AbstractEffect
{
    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effector.isPlayer() && effected.isSummon();
    }

    public override EffectFlags EffectFlags => EffectFlags.BETRAYED;

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, effected.getActingPlayer());
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}