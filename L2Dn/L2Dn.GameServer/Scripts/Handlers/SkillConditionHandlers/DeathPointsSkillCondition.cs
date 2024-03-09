using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class DeathPointsSkillCondition: ISkillCondition
{
	private readonly int _amount;
	private readonly bool _less;
	
	public DeathPointsSkillCondition(StatSet @params)
	{
		_amount = @params.getInt("amount");
		_less = @params.getBoolean("less", false);
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if (_less)
		{
			return caster.getActingPlayer().getDeathPoints() <= _amount;
		}
		return caster.getActingPlayer().getDeathPoints() >= _amount;
	}
}