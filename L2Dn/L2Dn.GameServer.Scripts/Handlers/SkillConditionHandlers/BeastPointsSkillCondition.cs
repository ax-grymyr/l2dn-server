using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class BeastPointsSkillCondition: ISkillCondition
{
	private readonly int _amount;
	
	public BeastPointsSkillCondition(StatSet @params)
	{
		_amount = @params.getInt("amount");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return caster.getActingPlayer().getBeastPoints() >= _amount;
	}
}