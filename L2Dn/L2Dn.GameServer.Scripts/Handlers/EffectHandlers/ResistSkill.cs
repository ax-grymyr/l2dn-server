using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Resist Skill effect implementaion.
/// </summary>
[HandlerName("ResistSkill")]
public sealed class ResistSkill: AbstractEffect
{
    private readonly FrozenSet<SkillHolder> _skills;

    public ResistSkill(EffectParameterSet parameters)
    {
        string skillIdStr = parameters.GetString(XmlSkillEffectParameterType.SkillIds, string.Empty);
        ImmutableArray<int> skillIds = ParseUtil.ParseList<int>(skillIdStr);

        string skillLevelStr = parameters.GetString(XmlSkillEffectParameterType.SkillLevels, string.Empty);
        ImmutableArray<int> skillLevels = ParseUtil.ParseList<int>(skillLevelStr);

        List<SkillHolder> skillHolders = new(16);
        for (int i = 0; i < skillIds.Length; i++)
        {
            int skillLevel = skillLevels.Length > i ? skillLevels[i] : 0;
            skillHolders.Add(new SkillHolder(skillIds[i], skillLevel));
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