using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Zoey76
 */
public class ConditionPlayerLevelRange : Condition
{
	private readonly int[] _levels;
	
	/**
	 * Instantiates a new condition player levels range.
	 * @param levels the {@code levels} range.
	 */
	public ConditionPlayerLevelRange(int[] levels)
	{
		_levels = levels;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		int level = effector.getLevel();
		return ((level >= _levels[0]) && (level <= _levels[1]));
	}
}
