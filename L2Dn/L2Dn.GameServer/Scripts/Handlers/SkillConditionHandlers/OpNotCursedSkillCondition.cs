using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpNotCursedSkillCondition: ISkillCondition
{
	public OpNotCursedSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return caster.isPlayer() && !caster.getActingPlayer().isCursedWeaponEquipped();
	}
}