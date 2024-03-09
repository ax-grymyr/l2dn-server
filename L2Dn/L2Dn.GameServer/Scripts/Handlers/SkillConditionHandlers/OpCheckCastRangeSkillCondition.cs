using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class OpCheckCastRangeSkillCondition: ISkillCondition
{
	private readonly int _distance;
	
	public OpCheckCastRangeSkillCondition(StatSet @params)
	{
		_distance = @params.getInt("distance");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return (target != null) //
			&& (caster.calculateDistance3D(target) >= _distance) //
			&& GeoEngine.getInstance().canSeeTarget(caster, target);
	}
}