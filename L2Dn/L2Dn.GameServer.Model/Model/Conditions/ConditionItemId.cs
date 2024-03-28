using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionItemId.
 * @author mkizub
 */
public class ConditionItemId: Condition
{
	private readonly int _itemId;
	
	/**
	 * Instantiates a new condition item id.
	 * @param itemId the item id
	 */
	public ConditionItemId(int itemId)
	{
		_itemId = itemId;
	}
	
	/**
	 * Test impl.
	 * @return true, if successful
	 */
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (item != null) && (item.getId() == _itemId);
	}
}
