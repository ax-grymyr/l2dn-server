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
/// Delete Hate effect implementation.
/// </summary>
public sealed class DeleteHate: AbstractEffect
{
    private readonly int _chance;

    public DeleteHate(StatSet @params)
    {
        _chance = @params.getInt("chance", 100);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcProbability(_chance, effector, effected, skill);
    }

    public override EffectType getEffectType() => EffectType.HATE;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isAttackable())
            return;

        Attackable target = (Attackable)effected;
        target.clearAggroList();
        target.setWalking();
        target.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
    }

    public override int GetHashCode() => _chance;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._chance);
}