using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

/**
 * Get all information from Item to generate ItemInfo.
 */
public class ItemInfo
{
	/** Identifier of the Item */
	private int _objectId;
	
	/** The Item template of the Item */
	private ItemTemplate _item;
	
	/** The level of enchant on the Item */
	private int _enchantLevel;
	
	/** The augmentation of the item */
	private VariationInstance _augmentation;
	
	/** The quantity of Item */
	private long _count;
	
	/** The price of the Item */
	private int _price;
	
	/** The custom Item types (used loto, race tickets) */
	private int _type1;
	private int _type2;
	
	/** If True the Item is equipped */
	private int _equipped;
	
	/** The action to do clientside (1=ADD, 2=MODIFY, 3=REMOVE) */
	private ItemChangeType _change;
	
	/** The mana of this item */
	private int _mana;
	private int _time;
	private bool _isBlessed = false;
	private bool _available = true;
	
	private int _location;
	
	private sbyte _elemAtkType = -2;
	private int _elemAtkPower = 0;
	private readonly int[] _attributeDefence =
	{
		0,
		0,
		0,
		0,
		0,
		0
	};
	
	private int[] _option;
	private List<EnsoulOption> _soulCrystalOptions;
	private List<EnsoulOption> _soulCrystalSpecialOptions;
	private int _visualId;
	private long _visualExpiration;
	
	private int _reuseDelay;
	private Player _owner;
	
	/**
	 * Get all information from Item to generate ItemInfo.
	 * @param item
	 */
	public ItemInfo(Item item)
	{
		Objects.requireNonNull(item);
		
		// Get the Identifier of the Item
		_objectId = item.getObjectId();
		
		// Get the Item of the Item
		_item = item.getTemplate();
		
		// Get the enchant level of the Item
		_enchantLevel = item.getEnchantLevel();
		
		// Get the augmentation bonus
		_augmentation = item.getAugmentation();
		
		// Get the quantity of the Item
		_count = item.getCount();
		
		// Get custom item types (used loto, race tickets)
		_type1 = item.getCustomType1();
		_type2 = item.getCustomType2();
		
		// Verify if the Item is equipped
		_equipped = item.isEquipped() ? 1 : 0;
		
		// Get the action to do clientside
		switch (item.getLastChange())
		{
			case Item.ADDED:
			{
				_change = 1;
				break;
			}
			case Item.MODIFIED:
			{
				_change = 2;
				break;
			}
			case Item.REMOVED:
			{
				_change = 3;
				break;
			}
		}
		
		// Get shadow item mana
		_mana = item.getMana();
		_time = item.isTimeLimitedItem() ? (int) (item.getRemainingTime() / 1000) : -9999;
		_available = item.isAvailable();
		_location = item.getLocationSlot();
		_elemAtkType = item.getAttackAttributeType().getClientId();
		_elemAtkPower = item.getAttackAttributePower();
		foreach (AttributeType type in AttributeType.ATTRIBUTE_TYPES)
		{
			_attributeDefence[type.getClientId()] = item.getDefenceAttribute(type);
		}
		_isBlessed = item.isBlessed();
		_option = item.getEnchantOptions();
		_soulCrystalOptions = item.getSpecialAbilities();
		_soulCrystalSpecialOptions = item.getAdditionalSpecialAbilities();
		_visualId = item.getVisualId();
		_visualExpiration = item.getVisualLifeTime() > 0 ? (item.getVisualLifeTime() - System.currentTimeMillis()) / 1000 : 0;
		_reuseDelay = item.getReuseDelay();
		_owner = item.getActingPlayer();
	}
	
	public ItemInfo(Item item, ItemChangeType change): this(item)
	{
		_change = change;
		_visualExpiration = item.getVisualLifeTime() > 0 ? (item.getVisualLifeTime() - System.currentTimeMillis()) / 1000 : 0;
	}
	
	public ItemInfo(TradeItem item)
	{
		if (item == null)
		{
			return;
		}
		
		// Get the Identifier of the Item
		_objectId = item.getObjectId();
		
		// Get the Item of the Item
		_item = item.getItem();
		
		// Get the enchant level of the Item
		_enchantLevel = item.getEnchant();
		
		// Get the augmentation bonus
		if ((item.getAugmentationOption1() >= 0) && (item.getAugmentationOption2() >= 0))
		{
			_augmentation = new VariationInstance(0, item.getAugmentationOption1(), item.getAugmentationOption2());
		}
		
		// Get the quantity of the Item
		_count = item.getCount();
		
		// Get custom item types (used loto, race tickets)
		_type1 = item.getCustomType1();
		_type2 = item.getCustomType2();
		
		// Verify if the Item is equipped
		_equipped = 0;
		
		// Get the action to do clientside
		_change = 0;
		
		// Get shadow item mana
		_mana = -1;
		_time = -9999;
		_location = item.getLocationSlot();
		_elemAtkType = item.getAttackElementType();
		_elemAtkPower = item.getAttackElementPower();
		for (byte i = 0; i < 6; i++)
		{
			_attributeDefence[i] = item.getElementDefAttr(i);
		}
		
		_option = item.getEnchantOptions();
		_soulCrystalOptions = item.getSoulCrystalOptions();
		_soulCrystalSpecialOptions = item.getSoulCrystalSpecialOptions();
		_visualId = item.getVisualId();
		_isBlessed = item.isBlessed();
	}
	
	public ItemInfo(Product item)
	{
		if (item == null)
		{
			return;
		}
		
		// Get the Identifier of the Item
		_objectId = 0;
		
		// Get the Item of the Item
		_item = item.getItem();
		
		// Get the enchant level of the Item
		_enchantLevel = 0;
		
		// Get the augmentation bonus
		_augmentation = null;
		
		// Get the quantity of the Item
		_count = item.getCount();
		
		// Get custom item types (used loto, race tickets)
		_type1 = item.getItem().getType1();
		_type2 = item.getItem().getType2();
		
		// Verify if the Item is equipped
		_equipped = 0;
		
		// Get the action to do clientside
		_change = 0;
		
		// Get shadow item mana
		_mana = -1;
		_time = -9999;
		_location = 0;
		_soulCrystalOptions = new();
		_soulCrystalSpecialOptions = new();
	}
	
	public ItemInfo(WarehouseItem item)
	{
		if (item == null)
		{
			return;
		}
		
		// Get the Identifier of the Item
		_objectId = item.getObjectId();
		
		// Get the Item of the Item
		_item = item.getItem();
		
		// Get the enchant level of the Item
		_enchantLevel = item.getEnchantLevel();
		
		// Get the augmentation bonus
		_augmentation = item.getAugmentation();
		
		// Get the quantity of the Item
		_count = item.getCount();
		
		// Get custom item types (used loto, race tickets)
		_type1 = item.getCustomType1();
		_type2 = item.getCustomType2();
		
		// Verify if the Item is equipped
		_equipped = 0;
		
		// Get shadow item mana
		_mana = item.getMana();
		_time = item.getTime();
		_location = item.getLocationSlot();
		_elemAtkType = item.getAttackElementType();
		_elemAtkPower = item.getAttackElementPower();
		for (byte i = 0; i < 6; i++)
		{
			_attributeDefence[i] = item.getElementDefAttr(i);
		}
		_option = item.getEnchantOptions();
		_soulCrystalOptions = item.getSoulCrystalOptions();
		_soulCrystalSpecialOptions = item.getSoulCrystalSpecialOptions();
		_isBlessed = item.isBlessed();
	}
	
	public int getObjectId()
	{
		return _objectId;
	}
	
	public ItemTemplate getItem()
	{
		return _item;
	}
	
	public int getEnchantLevel()
	{
		return _enchantLevel;
	}
	
	public VariationInstance getAugmentation()
	{
		return _augmentation;
	}
	
	public long getCount()
	{
		return _count;
	}
	
	public int getPrice()
	{
		return _price;
	}
	
	public int getCustomType1()
	{
		return _type1;
	}
	
	public int getCustomType2()
	{
		return _type2;
	}
	
	public int getEquipped()
	{
		return _equipped;
	}
	
	public int getChange()
	{
		return _change;
	}
	
	public int getMana()
	{
		return _mana;
	}
	
	public int getTime()
	{
		return _time > 0 ? _time : _visualExpiration > 0 ? (int) _visualExpiration : -9999;
	}
	
	public bool isAvailable()
	{
		return _available;
	}
	
	public int getLocation()
	{
		return _location;
	}
	
	public int getAttackElementType()
	{
		return _elemAtkType;
	}
	
	public int getAttackElementPower()
	{
		return _elemAtkPower;
	}
	
	public int getAttributeDefence(AttributeType attribute)
	{
		return _attributeDefence[attribute.getClientId()];
	}
	
	public int[] getEnchantOptions()
	{
		return _option;
	}
	
	public int getVisualId()
	{
		return _visualId;
	}
	
	public Collection<EnsoulOption> getSoulCrystalOptions()
	{
		return _soulCrystalOptions != null ? _soulCrystalOptions : Collections.emptyList();
	}
	
	public bool soulCrystalOptionsMatch(EnsoulOption[] soulCrystalOptions)
	{
		if ((_soulCrystalOptions == null))
		{
			return false;
		}
		
		foreach (EnsoulOption soulCrystalOption1 in _soulCrystalOptions)
		{
			bool found = false;
			foreach (EnsoulOption soulCrystalOption2 in soulCrystalOptions)
			{
				if (soulCrystalOption1.equals(soulCrystalOption2))
				{
					found = true;
					break;
				}
			}
			if (!found)
			{
				return false;
			}
		}
		
		return true;
	}
	
	public Collection<EnsoulOption> getSoulCrystalSpecialOptions()
	{
		return _soulCrystalSpecialOptions != null ? _soulCrystalSpecialOptions : Collections.emptyList();
	}
	
	public bool soulCrystalSpecialOptionsMatch(EnsoulOption[] soulCrystalSpecialOptions)
	{
		if (_soulCrystalSpecialOptions == null)
		{
			return false;
		}
		
		foreach (EnsoulOption soulCrystalSpecialOption1 in _soulCrystalSpecialOptions)
		{
			bool found = false;
			foreach (EnsoulOption soulCrystalSpecialOption2 in soulCrystalSpecialOptions)
			{
				if (soulCrystalSpecialOption1.equals(soulCrystalSpecialOption2))
				{
					found = true;
					break;
				}
			}
			if (!found)
			{
				return false;
			}
		}
		
		return true;
	}
	
	public long getVisualExpiration()
	{
		return _visualExpiration;
	}
	
	public bool isBlessed()
	{
		return _isBlessed;
	}
	
	public int getReuseDelay()
	{
		return _reuseDelay;
	}
	
	public Player getOwner()
	{
		return _owner;
	}
	
	public override String ToString()
	{
		return _item + "[objId: " + _objectId + ", count: " + _count + "]";
	}
}
