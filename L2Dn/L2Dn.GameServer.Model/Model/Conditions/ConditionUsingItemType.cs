using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionUsingItemType.
 * @author mkizub
 */
public class ConditionUsingItemType : Condition
{
	private readonly bool _armor;
	private readonly ItemTypeMask _mask;
	
	/**
	 * Instantiates a new condition using item type.
	 * @param mask the mask
	 */
	public ConditionUsingItemType(ItemTypeMask mask)
	{
		_mask = mask;
		_armor = (_mask & ((ItemTypeMask)ArmorType.MAGIC | ArmorType.LIGHT | ArmorType.HEAVY)) != ItemTypeMask.Zero;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector == null)
		{
			return false;
		}
		
		if (!effector.isPlayer())
		{
			return !_armor && (_mask & effector.getAttackType()) != ItemTypeMask.Zero;
		}
		
		Inventory inv = effector.getInventory();
		// If ConditionUsingItemType is one between Light, Heavy or Magic
		if (_armor)
		{
			// Get the itemMask of the weared chest (if exists)
			Item chest = inv.getPaperdollItem(Inventory.PAPERDOLL_CHEST);
			if (chest == null)
			{
				return (ArmorType.NONE & _mask) == ArmorType.NONE;
			}
			
			ItemTypeMask chestMask = chest.getTemplate().getItemMask();
			
			// If chest armor is different from the condition one return false
			if ((_mask & chestMask) == ItemTypeMask.Zero)
			{
				return false;
			}
			
			// So from here, chest armor matches conditions
			
			 long chestBodyPart = chest.getTemplate().getBodyPart();
			// return True if chest armor is a Full Armor
			if (chestBodyPart == ItemTemplate.SLOT_FULL_ARMOR)
			{
				return true;
			}
			
			// check legs armor
			Item legs = inv.getPaperdollItem(Inventory.PAPERDOLL_LEGS);
			if (legs == null)
			{
				return (ArmorType.NONE & _mask) == ArmorType.NONE;
			}
			
			ItemTypeMask legMask = legs.getTemplate().getItemMask();
			
			// return true if legs armor matches too
			return (_mask & legMask) != ItemTypeMask.Zero;
		}
		
		return (_mask & inv.getWearedMask()) != ItemTypeMask.Zero;
	}
}