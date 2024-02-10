using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Mobius
 */
public class ConditionMinimumVitalityPoints: Condition
{
	private readonly int _count;
	
	public ConditionMinimumVitalityPoints(int count)
	{
		_count = count;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Player player = effector.getActingPlayer();
		if (player != null)
		{
			return player.getVitalityPoints() >= _count;
		}
		return false;
	}
}
