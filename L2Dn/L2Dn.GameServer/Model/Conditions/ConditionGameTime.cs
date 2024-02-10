using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionGameTime.
 * @author mkizub
 */
public class ConditionGameTime: Condition
{
	private readonly bool _required;
	
	/**
	 * Instantiates a new condition game time.
	 * @param required the required
	 */
	public ConditionGameTime(bool required)
	{
		_required = required;
	}
	
	/**
	 * Test impl.
	 * @return true, if successful
	 */
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return GameTimeTaskManager.getInstance().isNight() == _required;
	}
}
