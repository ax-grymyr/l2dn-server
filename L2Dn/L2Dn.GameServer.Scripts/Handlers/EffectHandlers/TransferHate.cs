using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Transfer Hate effect implementation.
/// </summary>
public sealed class TransferHate: AbstractEffect
{
    private readonly int _chance;

    public TransferHate(StatSet @params)
    {
        _chance = @params.getInt("chance", 100);
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcProbability(_chance, effector, effected, skill);
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return Util.checkIfInRange(skill.getEffectRange(), effector, effected, true);
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        World.getInstance().forEachVisibleObjectInRange<Attackable>(effector, skill.getAffectRange(), hater =>
        {
            if (hater.isDead())
                return;

            long hate = hater.getHating(effector);
            if (hate <= 0)
                return;

            hater.reduceHate(effector, -hate);
            hater.addDamageHate(effected, 0, hate);
        });
    }

    public override int GetHashCode() => _chance;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._chance);
}