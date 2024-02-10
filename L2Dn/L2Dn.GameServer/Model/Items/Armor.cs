using L2Dn.GameServer.Model.Items.Types;

namespace L2Dn.GameServer.Model.Items;

/**
 * This class is dedicated to the management of armors.
 */
public class Armor: ItemTemplate
{
	private ArmorType _type;
	
	/**
	 * Constructor for Armor.
	 * @param set the StatSet designating the set of couples (key,value) characterizing the armor.
	 */
	public Armor(StatSet set): base(set)
	{
	}
	
	public override void set(StatSet set)
	{
		base.set(set);
		_type = set.getEnum("armor_type", ArmorType.NONE);
		
		long bodyPart = getBodyPart();
		if ((bodyPart == ItemTemplate.SLOT_ARTIFACT) || (bodyPart == ItemTemplate.SLOT_AGATHION))
		{
			_type1 = ItemTemplate.TYPE1_SHIELD_ARMOR;
			_type2 = ItemTemplate.TYPE2_ACCESSORY;
		}
		else if ((bodyPart == ItemTemplate.SLOT_NECK) || ((bodyPart & ItemTemplate.SLOT_L_EAR) != 0) || ((bodyPart & ItemTemplate.SLOT_L_FINGER) != 0) || ((bodyPart & ItemTemplate.SLOT_R_BRACELET) != 0) || ((bodyPart & ItemTemplate.SLOT_L_BRACELET) != 0) || ((bodyPart & ItemTemplate.SLOT_ARTIFACT_BOOK) != 0))
		{
			_type1 = ItemTemplate.TYPE1_WEAPON_RING_EARRING_NECKLACE;
			_type2 = ItemTemplate.TYPE2_ACCESSORY;
		}
		else
		{
			if ((_type == ArmorType.NONE) && (getBodyPart() == ItemTemplate.SLOT_L_HAND))
			{
				_type = ArmorType.SHIELD;
			}
			_type1 = ItemTemplate.TYPE1_SHIELD_ARMOR;
			_type2 = ItemTemplate.TYPE2_SHIELD_ARMOR;
		}
	}
	
	/**
	 * @return the type of the armor.
	 */
	public override ArmorType getItemType()
	{
		return _type;
	}
	
	/**
	 * @return the ID of the item after applying the mask.
	 */
	public override int getItemMask()
	{
		return _type.mask();
	}
	
	/**
	 * @return {@code true} if the item is an armor, {@code false} otherwise
	 */
	public override bool isArmor()
	{
		return true;
	}
}