using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerLevel.
 * @author mkizub
 */
public class ConditionPlayerLevel : Condition
{
	private readonly int _level;
	
	/**
	 * Instantiates a new condition player level.
	 * @param level the level
	 */
	public ConditionPlayerLevel(int level)
	{
		_level = level;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return effector.getLevel() >= _level;
	}
}
