using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Set Skill effect implementation.
/// </summary>
public sealed class SetSkill: AbstractEffect
{
    private readonly int _skillId;
    private readonly int _skillLevel;

    public SetSkill(EffectParameterSet parameters)
    {
        _skillId = parameters.GetInt32(XmlSkillEffectParameterType.SkillId, 0);
        _skillLevel = parameters.GetInt32(XmlSkillEffectParameterType.SkillLevel, 1);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isPlayer())
            return;

        Skill? setSkill = SkillData.Instance.GetSkill(_skillId, _skillLevel);
        if (setSkill == null)
            return;

        effected.getActingPlayer()?.addSkill(setSkill, true);
    }

    public override int GetHashCode() => HashCode.Combine(_skillId, _skillLevel);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._skillId, x._skillLevel));
}