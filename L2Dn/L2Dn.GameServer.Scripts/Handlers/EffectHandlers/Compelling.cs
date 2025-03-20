using L2Dn.GameServer.AI;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("Compelling")]
public sealed class Compelling: AbstractEffect
{
    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcProbability(100, effector, effected, skill);
    }

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        if (effected == null || effected.isRaid() || (effected.isNpc() && !effected.isAttackable()))
            return false;

        return effected.isPlayable() || effected.isAttackable();
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getAI().notifyEvent(CtrlEvent.EVT_AFRAID);
        CompellingAction(effector, effected);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        CompellingAction(effector, effected); // TODO: effector parameter was null in Java code
        return false;
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (!effected.isPlayer())
            effected.getAI().notifyEvent(CtrlEvent.EVT_THINK);
    }

    private static void CompellingAction(Creature effector, Creature effected)
    {
        effected.setRunning();
        effected.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, effector.Location.Location3D);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}