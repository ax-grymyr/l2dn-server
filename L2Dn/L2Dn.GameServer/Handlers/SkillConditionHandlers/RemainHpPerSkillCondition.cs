using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class RemainHpPerSkillCondition: ISkillCondition
{
	private readonly int _amount;
	private readonly SkillConditionPercentType _percentType;
	private readonly SkillConditionAffectType _affectType;
	
	public RemainHpPerSkillCondition(StatSet @params)
	{
		_amount = @params.getInt("amount");
		_percentType = @params.getEnum<SkillConditionPercentType>("percentType");
		_affectType = @params.getEnum<SkillConditionAffectType>("affectType");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		switch (_affectType)
		{
			case SkillConditionAffectType.CASTER:
			{
				return _percentType.test(caster.getCurrentHpPercent(), _amount);
			}
			case SkillConditionAffectType.TARGET:
			{
				if ((target != null) && target.isCreature())
				{
					return _percentType.test(((Creature) target).getCurrentHpPercent(), _amount);
				}
				break;
			}
			case SkillConditionAffectType.SUMMON:
			{
				if (caster.hasServitors())
				{
					return _percentType.test(caster.getActingPlayer().getAnyServitor().getCurrentHpPercent(), _amount);
				}
				break;
			}
		}
		return false;
	}
}
