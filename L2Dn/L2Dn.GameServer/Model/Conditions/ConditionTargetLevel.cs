using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetLevel.
 * @author mkizub
 */
public class ConditionTargetLevel : Condition
{
	private readonly int _level;
	
	/**
	 * Instantiates a new condition target level.
	 * @param level the level
	 */
	public ConditionTargetLevel(int level)
	{
		_level = level;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effected == null)
		{
			return false;
		}
		return effected.getLevel() >= _level;
	}
}
