using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class EnergySavedSkillCondition: ISkillCondition
{
	private readonly int _amount;
	
	public EnergySavedSkillCondition(StatSet @params)
	{
		_amount = @params.getInt("amount");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return caster.getActingPlayer().getCharges() >= _amount;
	}
}