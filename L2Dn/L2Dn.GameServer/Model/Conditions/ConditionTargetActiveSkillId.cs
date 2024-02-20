using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetActiveSkillId.
 */
public class ConditionTargetActiveSkillId : Condition
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	
	/**
	 * Instantiates a new condition target active skill id.
	 * @param skillId the skill id
	 */
	public ConditionTargetActiveSkillId(int skillId)
	{
		_skillId = skillId;
		_skillLevel = -1;
	}
	
	/**
	 * Instantiates a new condition target active skill id.
	 * @param skillId the skill id
	 * @param skillLevel the skill level
	 */
	public ConditionTargetActiveSkillId(int skillId, int skillLevel)
	{
		_skillId = skillId;
		_skillLevel = skillLevel;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Skill knownSkill = effected.getKnownSkill(_skillId);
		if (knownSkill != null)
		{
			return (_skillLevel == -1) || (_skillLevel <= knownSkill.getLevel());
		}
		return false;
	}
}
