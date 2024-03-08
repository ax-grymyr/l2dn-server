using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

public class OpRestartPointSkillCondition: ISkillCondition
{
	public OpRestartPointSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return true; // TODO:
	}
}