using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionWithSkill.
 * @author Steuf
 */
public class ConditionWithSkill: Condition
{
	private readonly bool _skill;
	
	/**
	 * Instantiates a new condition with skill.
	 * @param skill the skill
	 */
	public ConditionWithSkill(bool skill)
	{
		_skill = skill;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (skill != null) == _skill;
	}
}
