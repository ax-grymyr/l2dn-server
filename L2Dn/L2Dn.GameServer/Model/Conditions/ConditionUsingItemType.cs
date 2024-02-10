using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionUsingItemType.
 * @author mkizub
 */
public class ConditionUsingItemType : Condition
{
	private readonly bool _armor;
	private readonly int _mask;
	
	/**
	 * Instantiates a new condition using item type.
	 * @param mask the mask
	 */
	public ConditionUsingItemType(int mask)
	{
		_mask = mask;
		_armor = (_mask & (ArmorType.MAGIC.mask() | ArmorType.LIGHT.mask() | ArmorType.HEAVY.mask())) != 0;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector == null)
		{
			return false;
		}
		
		if (!effector.isPlayer())
		{
			return !_armor && ((_mask & effector.getAttackType().mask()) != 0);
		}
		
		Inventory inv = effector.getInventory();
		// If ConditionUsingItemType is one between Light, Heavy or Magic
		if (_armor)
		{
			// Get the itemMask of the weared chest (if exists)
			Item chest = inv.getPaperdollItem(Inventory.PAPERDOLL_CHEST);
			if (chest == null)
			{
				return (ArmorType.NONE.mask() & _mask) == ArmorType.NONE.mask();
			}
			 int chestMask = chest.getTemplate().getItemMask();
			
			// If chest armor is different from the condition one return false
			if ((_mask & chestMask) == 0)
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
				return (ArmorType.NONE.mask() & _mask) == ArmorType.NONE.mask();
			}
			 int legMask = legs.getTemplate().getItemMask();
			// return true if legs armor matches too
			return (_mask & legMask) != 0;
		}
		return (_mask & inv.getWearedMask()) != 0;
	}
}
