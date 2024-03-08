using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpTargetMyPledgeAcademySkillCondition: ISkillCondition
{
	public OpTargetMyPledgeAcademySkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if ((caster.getClan() == null) || (target == null) || !target.isPlayer())
		{
			return false;
		}
		Player targetPlayer = target.getActingPlayer();
		return targetPlayer.isAcademyMember() && (targetPlayer.getClan() == caster.getClan());
	}
}