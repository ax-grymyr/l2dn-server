using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Mobius
 */
public class ConditionMinDistance: Condition
{
	private readonly int _distance;
	
	public ConditionMinDistance(int distance)
	{
		_distance = distance;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effected != null) //
			&& (effector.Distance3D(effected) >= _distance) //
			&& GeoEngine.getInstance().canSeeTarget(effector, effected);
	}
}