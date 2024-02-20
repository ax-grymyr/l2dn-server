using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerMp.
 */
public class ConditionPlayerMp : Condition
{
	private readonly int _mp;
	
	/**
	 * Instantiates a new condition player mp.
	 * @param mp the mp
	 */
	public ConditionPlayerMp(int mp)
	{
		_mp = mp;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return ((effector.getCurrentMp() * 100) / effector.getMaxMp()) <= _mp;
	}
}
