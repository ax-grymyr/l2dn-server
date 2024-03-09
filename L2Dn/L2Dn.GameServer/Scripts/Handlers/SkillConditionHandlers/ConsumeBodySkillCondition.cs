using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class ConsumeBodySkillCondition: ISkillCondition
{
	public ConsumeBodySkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if ((target != null) && (target.isMonster() || target.isSummon()))
		{
			Creature creature = (Creature) target;
			if (creature.isDead() && creature.isSpawned())
			{
				return true;
			}
		}
		
		if (caster.isPlayer())
		{
			caster.sendPacket(SystemMessageId.INVALID_TARGET);
		}
		return false;
	}
}