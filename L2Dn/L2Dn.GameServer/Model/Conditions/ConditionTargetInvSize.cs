using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetInvSize.
 * @author Zoey76
 */
public class ConditionTargetInvSize : Condition
{
	private readonly int _size;
	
	/**
	 * Instantiates a new condition player inv size.
	 * @param size the size
	 */
	public ConditionTargetInvSize(int size)
	{
		_size = size;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effected != null) && effected.isPlayer())
		{
			Player target = effected.getActingPlayer();
			return target.getInventory().getNonQuestSize() <= (target.getInventoryLimit() - _size);
		}
		return false;
	}
}
