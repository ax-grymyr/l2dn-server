using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author UnAfraid
 */
public class ConditionTargetLevelRange: Condition
{
	private readonly int[] _levels;
	
	/**
	 * Instantiates a new condition target levels range.
	 * @param levels the {@code levels} range.
	 */
	public ConditionTargetLevelRange(int[] levels)
	{
		_levels = levels;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effected == null)
		{
			return false;
		}
		int level = effected.getLevel();
		return (level >= _levels[0]) && (level <= _levels[1]);
	}
}
