using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Call Learned Skill by Level effect implementation.
/// </summary>
public sealed class CallLearnedSkill: AbstractEffect
{
    private readonly int _skillId;

    public CallLearnedSkill(StatSet @params)
    {
        _skillId = @params.getInt("skillId");
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Skill? knownSkill = effector.getKnownSkill(_skillId);
        if (knownSkill != null)
            SkillCaster.triggerCast(effector, effected, knownSkill);
    }

    public override int GetHashCode() => _skillId;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._skillId);
}