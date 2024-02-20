using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerActiveSkillId.
 * @author DrHouse
 */
public class ConditionPlayerActiveSkillId: Condition
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	
	/**
	 * Instantiates a new condition player active skill id.
	 * @param skillId the skill id
	 */
	public ConditionPlayerActiveSkillId(int skillId)
	{
		_skillId = skillId;
		_skillLevel = -1;
	}
	
	/**
	 * Instantiates a new condition player active skill id.
	 * @param skillId the skill id
	 * @param skillLevel the skill level
	 */
	public ConditionPlayerActiveSkillId(int skillId, int skillLevel)
	{
		_skillId = skillId;
		_skillLevel = skillLevel;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Skill knownSkill = effector.getKnownSkill(_skillId);
		if (knownSkill != null)
		{
			return (_skillLevel == -1) || (_skillLevel <= knownSkill.getLevel());
		}
		return false;
	}
}
