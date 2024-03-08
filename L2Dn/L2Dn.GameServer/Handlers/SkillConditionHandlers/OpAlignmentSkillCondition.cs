using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpAlignmentSkillCondition: ISkillCondition
{
	private readonly SkillConditionAffectType _affectType;
	private readonly SkillConditionAlignment _alignment;

	public OpAlignmentSkillCondition(StatSet @params)
	{
		_affectType = @params.getEnum<SkillConditionAffectType>("affectType");
		_alignment = @params.getEnum<SkillConditionAlignment>("alignment");
	}

	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		switch (_affectType)
		{
			case SkillConditionAffectType.CASTER:
			{
				return _alignment.test(caster.getActingPlayer());
			}
			
			case SkillConditionAffectType.TARGET:
			{
				if ((target != null) && target.isPlayer())
				{
					return _alignment.test(target.getActingPlayer());
				}

				break;
			}
		}

		return false;
	}
}