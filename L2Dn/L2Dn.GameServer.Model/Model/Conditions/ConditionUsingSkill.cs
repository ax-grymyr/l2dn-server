using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionUsingSkill.
 * @author mkizub
 */
public class ConditionUsingSkill : Condition
{
	private readonly int _skillId;
	
	/**
	 * Instantiates a new condition using skill.
	 * @param skillId the skill id
	 */
	public ConditionUsingSkill(int skillId)
	{
		_skillId = skillId;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (skill == null)
		{
			return false;
		}
		return skill.getId() == _skillId;
	}
}
