using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author
 */
public class OpCheckAccountTypeSkillCondition: ISkillCondition
{
	public OpCheckAccountTypeSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return false;
	}
}