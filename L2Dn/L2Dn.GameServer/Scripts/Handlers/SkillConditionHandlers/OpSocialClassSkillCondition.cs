using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpSocialClassSkillCondition: ISkillCondition
{
	private readonly int _socialClass;
	
	public OpSocialClassSkillCondition(StatSet @params)
	{
		_socialClass = @params.getInt("socialClass");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		Player player = caster.getActingPlayer();
		if ((player == null) || (player.getClan() == null))
		{
			return false;
		}
		
		bool isClanLeader = player.isClanLeader();
		if ((_socialClass == -1) && !isClanLeader)
		{
			return false;
		}
		
		return isClanLeader || (player.getPledgeType() >= _socialClass);
	}
}