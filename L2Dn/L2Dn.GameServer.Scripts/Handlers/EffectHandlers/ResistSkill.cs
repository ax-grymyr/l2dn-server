using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Resist Skill effect implementaion.
/// </summary>
public sealed class ResistSkill: AbstractEffect
{
    private readonly FrozenSet<SkillHolder> _skills;

    public ResistSkill(StatSet @params)
    {
        List<SkillHolder> skillHolders = new(16);
        for (int i = 1;; i++)
        {
            int skillId = @params.getInt("skillId" + i, 0);
            int skillLevel = @params.getInt("skillLevel" + i, 0);
            if (skillId == 0)
                break;

            skillHolders.Add(new SkillHolder(skillId, skillLevel));
        }

        if (skillHolders.Count == 0)
            throw new ArgumentException(nameof(ResistSkill) + ": no parameters!");

        _skills = skillHolders.ToFrozenSet();
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        foreach (SkillHolder holder in _skills)
            effected.addIgnoreSkillEffects(holder);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        foreach (SkillHolder holder in _skills)
            effected.removeIgnoreSkillEffects(holder);
    }

    public override int GetHashCode() => _skills.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._skills.GetSetComparable());
}