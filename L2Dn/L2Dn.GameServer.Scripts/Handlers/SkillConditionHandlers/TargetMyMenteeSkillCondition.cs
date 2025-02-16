using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class TargetMyMenteeSkillCondition: ISkillCondition
{
	public TargetMyMenteeSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if ((target == null) || !target.isPlayer())
		{
			return false;
		}
		return MentorManager.getInstance().getMentee(caster.ObjectId, target.ObjectId) != null;
	}
}