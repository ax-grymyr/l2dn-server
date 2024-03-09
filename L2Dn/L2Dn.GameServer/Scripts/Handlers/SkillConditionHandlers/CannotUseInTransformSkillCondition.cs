using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class CannotUseInTransformSkillCondition: ISkillCondition
{
	private readonly int _transformId;
	
	public CannotUseInTransformSkillCondition(StatSet @params)
	{
		_transformId = @params.getInt("transformId", -1);
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return (_transformId > 0) ? caster.getTransformationId() != _transformId : !caster.isTransformed();
	}
}