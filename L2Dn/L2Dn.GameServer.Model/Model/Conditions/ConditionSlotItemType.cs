using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionSlotItemType.
 * @author mkizub
 */
public class ConditionSlotItemType : ConditionInventory
{
	private readonly ItemTypeMask _mask;
	
	/**
	 * Instantiates a new condition slot item type.
	 * @param slot the slot
	 * @param mask the mask
	 */
	public ConditionSlotItemType(int slot, ItemTypeMask mask): base(slot)
	{
		_mask = mask;
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
			return false;
		}
		return (itemSlot.getTemplate().getItemMask() & _mask) != ItemTypeMask.Zero;
	}
}