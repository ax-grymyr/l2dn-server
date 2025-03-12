using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Hide effect implementation.
/// </summary>
public sealed class Hide: AbstractEffect
{
    public Hide(StatSet @params)
    {
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isPlayer())
        {
            effected.setInvisible(true);

            if (effected.getAI().getNextIntention() != null &&
                effected.getAI().getNextIntention()?.getCtrlIntention() == CtrlIntention.AI_INTENTION_ATTACK)
            {
                effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
            }

            World.getInstance().forEachVisibleObject<Creature>(effected, target =>
            {
                if (target.getTarget() == effected)
                {
                    target.setTarget(null);
                    target.abortAttack();
                    target.abortCast();
                    target.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
                }
            });
        }
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null)
        {
            if (!player.inObserverMode())
                player.setInvisible(false);
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}