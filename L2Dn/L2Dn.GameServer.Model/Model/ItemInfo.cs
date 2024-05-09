using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

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
	private int? _mana;
	private TimeSpan? _time;
	private bool _isBlessed = false;
	private bool _available = true;
	
	private int _location;
	
	private AttributeType _elemAtkType = AttributeType.NONE;
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
	
	private ImmutableArray<int> _option;
	private ICollection<EnsoulOption> _soulCrystalOptions;
	private ICollection<EnsoulOption> _soulCrystalSpecialOptions;
	private int _visualId;
	private TimeSpan? _visualExpiration;
	
	private TimeSpan _reuseDelay;
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
			case ItemChangeType.ADDED:
			{
				_change = ItemChangeType.ADDED;
				break;
			}
			case ItemChangeType.MODIFIED:
			{
				_change = ItemChangeType.MODIFIED;
				break;
			}
			case ItemChangeType.REMOVED:
			{
				_change = ItemChangeType.REMOVED;
				break;
			}
		}
		
		// Get shadow item mana
		_mana = item.getMana();
		_time = item.isTimeLimitedItem() ? item.getRemainingTime() : null;
		_available = item.isAvailable();
		_location = item.getLocationSlot();
		_elemAtkType = item.getAttackAttributeType();
		_elemAtkPower = item.getAttackAttributePower();
		foreach (AttributeType type in AttributeTypeUtil.AttributeTypes)
		{
			_attributeDefence[(int)type] = item.getDefenceAttribute(type);
		}
		_isBlessed = item.isBlessed();
		_option = item.getEnchantOptions();
		_soulCrystalOptions = item.getSpecialAbilities();
		_soulCrystalSpecialOptions = item.getAdditionalSpecialAbilities();
		_visualId = item.getVisualId();
		_visualExpiration = item.getVisualLifeTime() != null ? (item.getVisualLifeTime().Value - DateTime.UtcNow) : null;
		_reuseDelay = item.getReuseDelay();
		_owner = item.getActingPlayer();
	}
	
	public ItemInfo(Item item, ItemChangeType change): this(item)
	{
		_change = change;
		_visualExpiration = item.getVisualLifeTime() != null ? item.getVisualLifeTime().Value - DateTime.UtcNow : null;
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
		_mana = null;
		_time = null;
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
		_mana = null;
		_time = null;
		_location = 0;
		_soulCrystalOptions = new List<EnsoulOption>();
		_soulCrystalSpecialOptions = new List<EnsoulOption>();
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
		_soulCrystalOptions = item.getSoulCrystalOptions().ToList();
		_soulCrystalSpecialOptions = item.getSoulCrystalSpecialOptions().ToList();
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
	
	public ItemChangeType getChange()
	{
		return _change;
	}
	
	public int? getMana()
	{
		return _mana;
	}
	
	public TimeSpan? getTime()
	{
		return _time ?? _visualExpiration;
	}
	
	public bool isAvailable()
	{
		return _available;
	}
	
	public int getLocation()
	{
		return _location;
	}
	
	public AttributeType getAttackElementType()
	{
		return _elemAtkType;
	}
	
	public int getAttackElementPower()
	{
		return _elemAtkPower;
	}
	
	public int getAttributeDefence(AttributeType attribute)
	{
		return _attributeDefence[(int)attribute];
	}
	
	public ImmutableArray<int> getEnchantOptions()
	{
		return _option;
	}
	
	public int getVisualId()
	{
		return _visualId;
	}
	
	public ICollection<EnsoulOption> getSoulCrystalOptions()
	{
		return _soulCrystalOptions != null ? _soulCrystalOptions : new List<EnsoulOption>();
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
				if (soulCrystalOption1.Equals(soulCrystalOption2))
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
	
	public ICollection<EnsoulOption> getSoulCrystalSpecialOptions()
	{
		return _soulCrystalSpecialOptions != null ? _soulCrystalSpecialOptions : new List<EnsoulOption>();
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
				if (soulCrystalSpecialOption1.Equals(soulCrystalSpecialOption2))
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
	
	public TimeSpan? getVisualExpiration()
	{
		return _visualExpiration;
	}
	
	public bool isBlessed()
	{
		return _isBlessed;
	}
	
	public TimeSpan getReuseDelay()
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
