using L2Dn.Extensions;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Confuse effect implementation.
/// </summary>
public sealed class Confuse: AbstractEffect
{
    private readonly int _chance;

    public Confuse(StatSet @params)
    {
        _chance = @params.getInt("chance", 100);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcProbability(_chance, effector, effected, skill);
    }

    public override EffectFlags getEffectFlags() => EffectFlags.CONFUSED;

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getAI().notifyEvent(CtrlEvent.EVT_CONFUSED);

        List<Creature> targetList = [];
        // Getting the possible targets

        World.getInstance().forEachVisibleObject<Creature>(effected, x => targetList.Add(x));

        // if there is no target, exit function
        if (targetList.Count != 0)
        {
            // Choosing randomly a new target
            Creature target = targetList.GetRandomElement();
            // Attacking the target
            effected.setTarget(target);
            effected.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
        }
    }

    public override int GetHashCode() => _chance;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._chance);
}