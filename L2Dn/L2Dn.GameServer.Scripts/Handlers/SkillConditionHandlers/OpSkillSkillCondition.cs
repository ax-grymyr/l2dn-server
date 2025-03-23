using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpSkill")]
public sealed class OpSkillSkillCondition: ISkillCondition
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly bool _hasLearned;

	public OpSkillSkillCondition(SkillConditionParameterSet parameters)
	{
		_skillId = parameters.GetInt32(XmlSkillConditionParameterType.SkillId);
		_skillLevel = parameters.GetInt32(XmlSkillConditionParameterType.SkillLevel);
		_hasLearned = parameters.GetBoolean(XmlSkillConditionParameterType.HasLearned);
	}

	public bool canUse(Creature caster, Skill skill, WorldObject? target)
	{
		Skill? requestedSkill = caster.getKnownSkill(_skillId);
		if (_hasLearned)
		{
			return requestedSkill != null && requestedSkill.Level == _skillLevel;
		}
		return requestedSkill == null || requestedSkill.Level != _skillLevel;
	}

    public override int GetHashCode() => HashCode.Combine(_skillId, _skillLevel, _hasLearned);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._skillId, x._skillLevel, x._hasLearned));
}