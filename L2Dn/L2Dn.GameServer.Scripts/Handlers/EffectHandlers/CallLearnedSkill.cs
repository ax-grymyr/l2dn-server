using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Call Learned Skill by Level effect implementation.
/// </summary>
public sealed class CallLearnedSkill: AbstractEffect
{
    private readonly int _skillId;

    public CallLearnedSkill(EffectParameterSet parameters)
    {
        _skillId = parameters.GetInt32(XmlSkillEffectParameterType.SkillId);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Skill? knownSkill = effector.getKnownSkill(_skillId);
        if (knownSkill != null)
            SkillCaster.triggerCast(effector, effected, knownSkill);
    }

    public override int GetHashCode() => _skillId;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._skillId);
}