using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionSlotItemId.
 * @author mkizub
 */
public class ConditionSlotItemId : ConditionInventory
{
	private readonly int _itemId;
	private readonly int _enchantLevel;
	
	/**
	 * Instantiates a new condition slot item id.
	 * @param slot the slot
	 * @param itemId the item id
	 * @param enchantLevel the enchant level
	 */
	public ConditionSlotItemId(int slot, int itemId, int enchantLevel) :base(slot)
	{
		_itemId = itemId;
		_enchantLevel = enchantLevel;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effector == null) || !effector.isPlayer())
		{
			return false;
		}
		
		Item itemSlot = effector.getInventory().getPaperdollItem(_slot);
		if (itemSlot == null)
		{
			return _itemId == 0;
		}
		return (itemSlot.getId() == _itemId) && (itemSlot.getEnchantLevel() >= _enchantLevel);
	}
}
