using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Double Casting effect implementation.
/// </summary>
public sealed class DoubleCast: AbstractEffect
{
    private static readonly SkillHolder[] _toggleSkills = [new(11007, 1), new(11009, 1), new(11008, 1), new(11010, 1),];
    private readonly Map<int, List<SkillHolder>> _addedToggles;

    public DoubleCast(StatSet @params)
    {
        _addedToggles = [];
    }

    public override EffectFlags EffectFlags => EffectFlags.DOUBLE_CAST;

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isPlayer())
        {
            foreach (SkillHolder holder in _toggleSkills)
            {
                Skill s = holder.getSkill();
                if (s != null && !effected.isAffectedBySkill(holder))
                {
                    _addedToggles.GetOrAdd(effected.ObjectId, _ => []).Add(holder);
                    s.ApplyEffects(effected, effected);
                }
            }
        }

        base.onStart(effector, effected, skill, item);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isPlayer())
        {
            _addedToggles.computeIfPresent(effected.ObjectId, (_, v) =>
            {
                v.ForEach(h => effected.stopSkillEffects(h.getSkill()));
                return (object?)null; // TODO: !!!!!!!!!
            });
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}