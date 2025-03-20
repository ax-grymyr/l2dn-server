using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpSkillSkillCondition: ISkillCondition
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly bool _hasLearned;

	public OpSkillSkillCondition(StatSet @params)
	{
		_skillId = @params.getInt("skillId");
		_skillLevel = @params.getInt("skillLevel");
		_hasLearned = @params.getBoolean("hasLearned");
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
}