using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author Mode
 */
public class CanUseVitalityIncreaseItemSkillCondition: ISkillCondition
{
	private readonly int _amount;
	
	public CanUseVitalityIncreaseItemSkillCondition(StatSet @params)
	{
		_amount = @params.getInt("amount", 0);
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return caster.isPlayer() && ((caster.getActingPlayer().getVitalityPoints() + _amount) <= PlayerStat.MAX_VITALITY_POINTS);
	}
}