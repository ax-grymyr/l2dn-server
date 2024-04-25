using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionGameChance.
 * @author Advi
 */
public class ConditionGameChance: Condition
{
	private readonly int _chance;
	
	/**
	 * Instantiates a new condition game chance.
	 * @param chance the chance
	 */
	public ConditionGameChance(int chance)
	{
		_chance = chance;
	}
	
	/**
	 * Test impl.
	 * @return true, if successful
	 */
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return Rnd.get(100) < _chance;
	}
}
