using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Stats.Functions;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Items;

/**
 * This class contains all informations concerning the item (weapon, armor, etc).<br>
 * Mother class of :
 * <ul>
 * <li>Armor</li>
 * <li>EtcItem</li>
 * <li>Weapon</li>
 * </ul>
 */
public abstract class ItemTemplate: ListenersContainer, IIdentifiable
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemTemplate));
	
	public const int TYPE1_WEAPON_RING_EARRING_NECKLACE = 0;
	public const int TYPE1_SHIELD_ARMOR = 1;
	public const int TYPE1_ITEM_QUESTITEM_ADENA = 4;
	
	public const int TYPE2_WEAPON = 0;
	public const int TYPE2_SHIELD_ARMOR = 1;
	public const int TYPE2_ACCESSORY = 2;
	public const int TYPE2_QUEST = 3;
	public const int TYPE2_MONEY = 4;
	public const int TYPE2_OTHER = 5;
	
	public const int SLOT_NONE = 0x0000;
	public const int SLOT_UNDERWEAR = 0x0001;
	public const int SLOT_R_EAR = 0x0002;
	public const int SLOT_L_EAR = 0x0004;
	public const int SLOT_LR_EAR = 0x00006;
	public const int SLOT_NECK = 0x0008;
	public const int SLOT_R_FINGER = 0x0010;
	public const int SLOT_L_FINGER = 0x0020;
	public const int SLOT_LR_FINGER = 0x0030;
	public const int SLOT_HEAD = 0x0040;
	public const int SLOT_R_HAND = 0x0080;
	public const int SLOT_L_HAND = 0x0100;
	public const int SLOT_GLOVES = 0x0200;
	public const int SLOT_CHEST = 0x0400;
	public const int SLOT_LEGS = 0x0800;
	public const int SLOT_FEET = 0x1000;
	public const int SLOT_BACK = 0x2000;
	public const int SLOT_LR_HAND = 0x4000;
	public const int SLOT_FULL_ARMOR = 0x8000;
	public const int SLOT_HAIR = 0x010000;
	public const int SLOT_ALLDRESS = 0x020000;
	public const int SLOT_HAIR2 = 0x040000;
	public const int SLOT_HAIRALL = 0x080000;
	public const int SLOT_R_BRACELET = 0x100000;
	public const int SLOT_L_BRACELET = 0x200000;
	public const int SLOT_DECO = 0x400000;
	public const int SLOT_BELT = 0x10000000;
	public const int SLOT_BROOCH = 0x20000000;
	public const int SLOT_BROOCH_JEWEL = 0x40000000;
	public const long SLOT_AGATHION = 0x3000000000L;
	public const long SLOT_ARTIFACT_BOOK = 0x20000000000L;
	public const long SLOT_ARTIFACT = 0x40000000000L;
	
	public const int SLOT_WOLF = -100;
	public const int SLOT_HATCHLING = -101;
	public const int SLOT_STRIDER = -102;
	public const int SLOT_BABYPET = -103;
	public const int SLOT_GREATWOLF = -104;
	
	public const int SLOT_MULTI_ALLWEAPON = SLOT_LR_HAND | SLOT_R_HAND;
	
	private int _itemId;
	private int _displayId;
	private String _name;
	private String _additionalName;
	private String _icon;
	private int _weight;
	private bool _stackable;
	private MaterialType _materialType;
	private CrystalType _crystalType;
	private TimeSpan _equipReuseDelay;
	private int? _duration; // mana
	private TimeSpan? _time;
	private TimeSpan? _autoDestroyTime;
	private long _bodyPart;
	private int _referencePrice;
	private int _crystalCount;
	private bool _sellable;
	private bool _dropable;
	private bool _destroyable;
	private bool _tradeable;
	private bool _depositable;
	private bool _enchantable;
	private int _enchantLimit;
	private int _ensoulNormalSlots;
	private int _ensoulSpecialSlots;
	private bool _elementable;
	private bool _questItem;
	private bool _freightable;
	private bool _allowSelfResurrection;
	private bool _isOlyRestricted;
	private bool _isEventRestricted;
	private bool _forNpc;
	private bool _common;
	private bool _heroItem;
	private bool _pvpItem;
	private bool _immediateEffect;
	private bool _exImmediateEffect;
	private int _defaultEnchantLevel;
	private ActionType _defaultAction;
	
	protected int _type1; // needed for item list (inventory)
	protected int _type2; // different lists for armor, weapon, etc
	private Map<AttributeType, AttributeHolder> _elementals;
	protected Map<Stat, FuncTemplate> _funcTemplates;
	protected List<Condition> _preConditions;
	private List<ItemSkillHolder> _skills;
	
	private int _useSkillDisTime;
	protected int _reuseDelay;
	private int _sharedReuseGroup;
	
	private CommissionItemType _commissionItemType;
	
	private bool _isAppearanceable;
	private bool _isBlessed;
	
	private int _artifactSlot;

	/**
	 * Constructor of the Item that fill class variables.
	 * @param set : StatSet corresponding to a set of couples (key,value) for description of the item
	 */
	protected ItemTemplate(StatSet set)
	{
		this.set(set);
	}
	
	public virtual void set(StatSet set)
	{
		_itemId = set.getInt("item_id");
		_displayId = set.getInt("displayId", _itemId);
		_name = set.getString("name");
		_additionalName = set.getString("additionalName", null);
		_icon = set.getString("icon", null);
		_weight = set.getInt("weight", 0);
		_materialType = set.getEnum("material", MaterialType.STEEL);
		_equipReuseDelay = TimeSpan.FromSeconds(set.getInt("equip_reuse_delay", 0));
		_duration = set.getInt("duration", -1);
		
		int time = set.getInt("time", -1);
		_time = time < 0 ? null : TimeSpan.FromMilliseconds(time);

		int autoDestroyTime = set.getInt("auto_destroy_time", -1); 
		_autoDestroyTime = autoDestroyTime < 0 ? null : TimeSpan.FromSeconds(autoDestroyTime);
		_bodyPart = ItemData.SLOTS.get(set.getString("bodypart", "none"));
		_referencePrice = set.getInt("price", 0);
		_crystalType = set.getEnum("crystal_type", CrystalType.NONE);
		_crystalCount = set.getInt("crystal_count", 0);
		_stackable = set.getBoolean("is_stackable", false);
		_sellable = set.getBoolean("is_sellable", true);
		_dropable = set.getBoolean("is_dropable", true);
		_destroyable = set.getBoolean("is_destroyable", true);
		_tradeable = set.getBoolean("is_tradable", true);
		_questItem = set.getBoolean("is_questitem", false);
		if (Config.CUSTOM_DEPOSITABLE_ENABLED)
		{
			_depositable = !_questItem || Config.CUSTOM_DEPOSITABLE_QUEST_ITEMS;
		}
		else
		{
			_depositable = set.getBoolean("is_depositable", true);
		}
		
		_ensoulNormalSlots = set.getInt("ensoulNormalSlots", 0);
		_ensoulSpecialSlots = set.getInt("ensoulSpecialSlots", 0);
		
		_elementable = set.getBoolean("element_enabled", false);
		_enchantable = set.getBoolean("enchant_enabled", false);
		_enchantLimit = set.getInt("enchant_limit", 0);
		_freightable = set.getBoolean("is_freightable", false);
		_allowSelfResurrection = set.getBoolean("allow_self_resurrection", false);
		_isOlyRestricted = set.getBoolean("is_oly_restricted", false);
		_isEventRestricted = set.getBoolean("is_event_restricted", false);
		_forNpc = set.getBoolean("for_npc", false);
		_isAppearanceable = set.getBoolean("isAppearanceable", false);
		_isBlessed = set.getBoolean("blessed", false);
		_artifactSlot = set.getInt("artifactSlot", 0);
		_immediateEffect = set.getBoolean("immediate_effect", false);
		_exImmediateEffect = set.getBoolean("ex_immediate_effect", false);
		_defaultAction = set.getEnum("default_action", ActionType.NONE);
		_useSkillDisTime = set.getInt("useSkillDisTime", 0);
		_defaultEnchantLevel = set.getInt("enchanted", 0);
		_reuseDelay = set.getInt("reuse_delay", 0);
		_sharedReuseGroup = set.getInt("shared_reuse_group", 0);
		_commissionItemType = set.getEnum("commissionItemType", CommissionItemType.OTHER_ITEM);
		_common = ((_itemId >= 11605) && (_itemId <= 12361));
		_heroItem = ((_itemId >= 6611) && (_itemId <= 6621)) || ((_itemId >= 9388) && (_itemId <= 9390)) || (_itemId == 6842);
		_pvpItem = ((_itemId >= 10667) && (_itemId <= 10835)) || ((_itemId >= 12852) && (_itemId <= 12977)) || ((_itemId >= 14363) && (_itemId <= 14525)) || (_itemId == 14528) || (_itemId == 14529) || (_itemId == 14558) || ((_itemId >= 15913) && (_itemId <= 16024)) || ((_itemId >= 16134) && (_itemId <= 16147)) || (_itemId == 16149) || (_itemId == 16151) || (_itemId == 16153) || (_itemId == 16155) || (_itemId == 16157) || (_itemId == 16159) || ((_itemId >= 16168) && (_itemId <= 16176)) || ((_itemId >= 16179) && (_itemId <= 16220));
		
		// Sealed item checks.
		if ((_additionalName != null) && _additionalName.Equals("Sealed"))
		{
			if (_tradeable)
			{
				LOGGER.Warn("Found tradeable [Sealed] item " + _itemId);
			}
			if (_dropable)
			{
				LOGGER.Warn("Found dropable [Sealed] item " + _itemId);
			}
			if (_sellable)
			{
				LOGGER.Warn("Found sellable [Sealed] item " + _itemId);
			}
		}
	}
	
	/**
	 * Returns the itemType.
	 * @return Enum
	 */
	public abstract ItemType getItemType();

	public ItemTypeMask getItemMask() => getItemType().GetMask();

	/**
	 * Verifies if the item is an etc item.
	 * @return {@code true} if the item is an etc item, {@code false} otherwise.
	 */
	public bool isEtcItem() => getItemType().IsEtcItem();

	/**
	 * Verifies if the item is an armor.
	 * @return {@code true} if the item is an armor, {@code false} otherwise.
	 */
	public bool isArmor() => getItemType().IsArmor();

	/**
	 * Verifies if the item is a weapon.
	 * @return {@code true} if the item is a weapon, {@code false} otherwise.
	 */
	public bool isWeapon() => getItemType().IsWeapon();
	
	/**
	 * Verifies if the item is a magic weapon.
	 * @return {@code true} if the weapon is magic, {@code false} otherwise.
	 */
	public virtual bool isMagicWeapon()
	{
		return false;
	}
	
	/**
	 * @return the _equipReuseDelay
	 */
	public TimeSpan getEquipReuseDelay()
	{
		return _equipReuseDelay;
	}
	
	/**
	 * Returns the duration of the item
	 * @return int
	 */
	public int? getDuration()
	{
		return _duration;
	}
	
	/**
	 * Returns the time of the item
	 * @return long
	 */
	public TimeSpan? getTime()
	{
		return _time;
	}
	
	/**
	 * @return the auto destroy time of the item in seconds: 0 or less - default
	 */
	public TimeSpan? getAutoDestroyTime()
	{
		return _autoDestroyTime;
	}
	
	/**
	 * Returns the ID of the item
	 * @return int
	 */
	public int getId()
	{
		return _itemId;
	}
	
	/**
	 * Returns the ID of the item
	 * @return int
	 */
	public int getDisplayId()
	{
		return _displayId;
	}
	
	/**
	 * Return the type of material of the item
	 * @return MaterialType
	 */
	public MaterialType getMaterialType()
	{
		return _materialType;
	}
	
	/**
	 * Returns the type 2 of the item
	 * @return int
	 */
	public int getType2()
	{
		return _type2;
	}
	
	/**
	 * Returns the weight of the item
	 * @return int
	 */
	public int getWeight()
	{
		return _weight;
	}
	
	/**
	 * Returns if the item is crystallizable
	 * @return bool
	 */
	public bool isCrystallizable()
	{
		return (_crystalType != CrystalType.NONE) && (_crystalCount > 0);
	}
	
	/**
	 * @return return General item grade (No S80, S84, R95, R99)
	 */
	public ItemGrade getItemGrade()
	{
		return _crystalType.GetItemGrade();
	}
	
	/**
	 * Return the type of crystal if item is crystallizable
	 * @return CrystalType
	 */
	public CrystalType getCrystalType()
	{
		return _crystalType;
	}
	
	/**
	 * Return the ID of crystal if item is crystallizable
	 * @return int
	 */
	public int getCrystalItemId()
	{
		return CrystalTypeInfo.Get(_crystalType).getCrystalId();
	}
	
	/**
	 * For grades S80 and S84 return S, R95, and R99 return R
	 * @return the grade of the item.
	 */
	public CrystalType getCrystalTypePlus()
	{
		switch (_crystalType)
		{
			case CrystalType.S80:
			case CrystalType.S84:
			{
				return CrystalType.S;
			}
			case CrystalType.R95:
			case CrystalType.R99:
			{
				return CrystalType.R;
			}
			default:
			{
				return _crystalType;
			}
		}
	}
	
	/**
	 * @return the quantity of crystals for crystallization.
	 */
	public int getCrystalCount()
	{
		return _crystalCount;
	}
	
	/**
	 * @param enchantLevel
	 * @return the quantity of crystals for crystallization on specific enchant level
	 */
	public int getCrystalCount(int enchantLevel)
	{
		if (enchantLevel > 3)
		{
			switch (_type2)
			{
				case TYPE2_SHIELD_ARMOR:
				case TYPE2_ACCESSORY:
				{
					return _crystalCount + (CrystalTypeInfo.Get(_crystalType).getCrystalEnchantBonusArmor() * ((3 * enchantLevel) - 6));
				}
				case TYPE2_WEAPON:
				{
					return _crystalCount + (CrystalTypeInfo.Get(_crystalType).getCrystalEnchantBonusWeapon() * ((2 * enchantLevel) - 3));
				}
				default:
				{
					return _crystalCount;
				}
			}
		}
		else if (enchantLevel > 0)
		{
			switch (_type2)
			{
				case TYPE2_SHIELD_ARMOR:
				case TYPE2_ACCESSORY:
				{
					return _crystalCount + (CrystalTypeInfo.Get(_crystalType).getCrystalEnchantBonusArmor() * enchantLevel);
				}
				case TYPE2_WEAPON:
				{
					return _crystalCount + (CrystalTypeInfo.Get(_crystalType).getCrystalEnchantBonusWeapon() * enchantLevel);
				}
				default:
				{
					return _crystalCount;
				}
			}
		}
		else
		{
			return _crystalCount;
		}
	}
	
	/**
	 * @return the name of the item.
	 */
	public String getName()
	{
		return _name;
	}
	
	/**
	 * @return the item's additional name.
	 */
	public String getAdditionalName()
	{
		return _additionalName;
	}
	
	public ICollection<AttributeHolder> getAttributes()
	{
		return _elementals != null ? _elementals.values() : null;
	}
	
	public AttributeHolder getAttribute(AttributeType type)
	{
		return _elementals != null ? _elementals.get(type) : null;
	}
	
	/**
	 * Sets the base elemental of the item.
	 * @param holder the element to set.
	 */
	public void setAttributes(AttributeHolder holder)
	{
		if (_elementals == null)
		{
			_elementals = new();
			_elementals.put(holder.getType(), holder);
		}
		else
		{
			AttributeHolder attribute = getAttribute(holder.getType());
			if (attribute != null)
			{
				attribute.setValue(holder.getValue());
			}
			else
			{
				_elementals.put(holder.getType(), holder);
			}
		}
	}
	
	/**
	 * @return the part of the body used with the item.
	 */
	public long getBodyPart()
	{
		return _bodyPart;
	}
	
	/**
	 * @return the type 1 of the item.
	 */
	public int getType1()
	{
		return _type1;
	}
	
	/**
	 * @return {@code true} if the item is stackable, {@code false} otherwise.
	 */
	public bool isStackable()
	{
		return _stackable;
	}
	
	/**
	 * @return {@code true} if the item can be equipped, {@code false} otherwise.
	 */
	public bool isEquipable()
	{
		return (_bodyPart != 0) && !(getItemType() is EtcItemType);
	}
	
	/**
	 * @return the price of reference of the item.
	 */
	public int getReferencePrice()
	{
		return _referencePrice;
	}
	
	/**
	 * @return {@code true} if the item can be sold, {@code false} otherwise.
	 */
	public bool isSellable()
	{
		return _sellable;
	}
	
	/**
	 * @return {@code true} if the item can be dropped, {@code false} otherwise.
	 */
	public bool isDropable()
	{
		return _dropable;
	}
	
	/**
	 * @return {@code true} if the item can be destroyed, {@code false} otherwise.
	 */
	public bool isDestroyable()
	{
		return _destroyable;
	}
	
	/**
	 * @return {@code true} if the item can be traded, {@code false} otherwise.
	 */
	public bool isTradeable()
	{
		return _tradeable;
	}
	
	/**
	 * @return {@code true} if the item can be put into warehouse, {@code false} otherwise.
	 */
	public bool isDepositable()
	{
		return _depositable;
	}
	
	/**
	 * This method also check the enchant blacklist.
	 * @return {@code true} if the item can be enchanted, {@code false} otherwise.
	 */
	public bool isEnchantable()
	{
		return _enchantable && !Config.ENCHANT_BLACKLIST.Contains(_itemId);
	}
	
	/**
	 * Returns the enchantment limit of the item
	 * @return int
	 */
	public int getEnchantLimit()
	{
		return _enchantLimit > 0 ? _enchantLimit : 0;
	}
	
	/**
	 * @return the available ensoul slot count.
	 */
	public int getEnsoulSlots()
	{
		return _ensoulNormalSlots;
	}
	
	/**
	 * @return the available special ensoul slot count.
	 */
	public int getSpecialEnsoulSlots()
	{
		return _ensoulSpecialSlots;
	}
	
	/**
	 * @return {@code true} if the item can be elemented, {@code false} otherwise.
	 */
	public bool isElementable()
	{
		return _elementable;
	}
	
	/**
	 * Returns if item is common
	 * @return bool
	 */
	public bool isCommon()
	{
		return _common;
	}
	
	/**
	 * Returns if item is hero-only
	 * @return
	 */
	public bool isHeroItem()
	{
		return _heroItem;
	}
	
	/**
	 * Returns if item is pvp
	 * @return
	 */
	public bool isPvpItem()
	{
		return _pvpItem;
	}

	public bool isPotion() => getItemType() == EtcItemType.POTION;
	
	public bool isElixir() => getItemType() == EtcItemType.ELIXIR;
	
	public bool isScroll() => getItemType() == EtcItemType.SCROLL;
	
	/**
	 * Add the FuncTemplate f to the list of functions used with the item
	 * @param template : FuncTemplate to add
	 */
	public void addFunctionTemplate(FuncTemplate template)
	{
		switch (template.getStat())
		{
			case Stat.FIRE_RES:
			case Stat.FIRE_POWER:
			{
				setAttributes(new AttributeHolder(AttributeType.FIRE, (int) template.getValue()));
				break;
			}
			case Stat.WATER_RES:
			case Stat.WATER_POWER:
			{
				setAttributes(new AttributeHolder(AttributeType.WATER, (int) template.getValue()));
				break;
			}
			case Stat.WIND_RES:
			case Stat.WIND_POWER:
			{
				setAttributes(new AttributeHolder(AttributeType.WIND, (int) template.getValue()));
				break;
			}
			case Stat.EARTH_RES:
			case Stat.EARTH_POWER:
			{
				setAttributes(new AttributeHolder(AttributeType.EARTH, (int) template.getValue()));
				break;
			}
			case Stat.HOLY_RES:
			case Stat.HOLY_POWER:
			{
				setAttributes(new AttributeHolder(AttributeType.HOLY, (int) template.getValue()));
				break;
			}
			case Stat.DARK_RES:
			case Stat.DARK_POWER:
			{
				setAttributes(new AttributeHolder(AttributeType.DARK, (int) template.getValue()));
				break;
			}
		}
		
		if (_funcTemplates == null)
		{
			_funcTemplates = new();
		}
		if (_funcTemplates.put(template.getStat(), template) != null)
		{
			LOGGER.Warn("Item with id " + _itemId + " has 2 func templates with same stat: " + template.getStat());
		}
	}
	
	public void attachCondition(Condition c)
	{
		if (_preConditions == null)
		{
			_preConditions = new();
		}
		_preConditions.Add(c);
	}
	
	public List<Condition> getConditions()
	{
		return _preConditions;
	}
	
	public bool hasSkills()
	{
		return _skills != null;
	}
	
	/**
	 * Method to retrieve skills linked to this item armor and weapon: passive skills etcitem: skills used on item use <-- ???
	 * @return Skills linked to this item as SkillHolder[]
	 */
	public List<ItemSkillHolder> getAllSkills()
	{
		return _skills;
	}
	
	/**
	 * @param condition
	 * @return {@code List} of {@link ItemSkillHolder} if item has skills and matches the condition, {@code null} otherwise
	 */
	public List<ItemSkillHolder> getSkills(Predicate<ItemSkillHolder> condition)
	{
		if (_skills == null)
		{
			return null;
		}
		
		List<ItemSkillHolder> result = new();
		foreach (ItemSkillHolder skill in _skills)
		{
			if (condition(skill))
			{
				result.Add(skill);
			}
		}
		return result;
	}
	
	/**
	 * @param type
	 * @return {@code List} of {@link ItemSkillHolder} if item has skills, {@code null} otherwise
	 */
	public List<ItemSkillHolder> getSkills(ItemSkillType type)
	{
		if (_skills == null)
		{
			return null;
		}
		
		List<ItemSkillHolder> result = new();
		foreach (ItemSkillHolder skill in _skills)
		{
			if (skill.getType() == type)
			{
				result.Add(skill);
			}
		}
		return result;
	}
	
	/**
	 * Executes the action on each item skill with the specified type (If there are skills at all)
	 * @param type
	 * @param action
	 */
	public void forEachSkill(ItemSkillType type, Action<ItemSkillHolder> action)
	{
		if (_skills != null)
		{
			foreach (ItemSkillHolder skill in _skills)
			{
				if (skill.getType() == type)
				{
					action(skill);
				}
			}
		}
	}
	
	public void addSkill(ItemSkillHolder holder)
	{
		// Agathion skills managed by AgathionData.
		// if ((getBodyPart() == SLOT_AGATHION) && (holder.getType() != ItemSkillType.ON_EQUIP) && (holder.getType() != ItemSkillType.ON_UNEQUIP))
		// {
		// LOGGER.warning("Remove from agathion " + _itemId + " " + holder + "!");
		// return;
		// }
		
		if (_skills == null)
		{
			_skills = new();
		}
		_skills.Add(holder);
	}
	
	public bool checkCondition(Creature creature, WorldObject @object, bool sendMessage)
	{
		if (creature.canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !Config.GM_ITEM_RESTRICTION)
		{
			return true;
		}
		
		// Don't allow hero equipment and restricted items during Olympiad
		if ((isOlyRestrictedItem() || _heroItem) && (creature.isPlayer() && creature.getActingPlayer().isInOlympiadMode()))
		{
			if (isEquipable())
			{
				creature.sendPacket(SystemMessageId.THE_ITEM_CANNOT_BE_EQUIPPED_IN_THE_OLYMPIAD);
			}
			else
			{
				creature.sendPacket(SystemMessageId.THE_ITEM_CANNOT_BE_USED_IN_THE_OLYMPIAD);
			}
			return false;
		}
		
		if (_isEventRestricted && (creature.isPlayer() && (creature.getActingPlayer().isOnEvent())))
		{
			creature.sendMessage("You cannot use this item in the event.");
			return false;
		}
		
		if (!isConditionAttached())
		{
			return true;
		}
		
		Creature target = @object.isCreature() ? (Creature) @object : null;
		foreach (Condition preCondition in _preConditions)
		{
			if (preCondition == null)
			{
				continue;
			}
			
			if (!preCondition.test(creature, target, null, null))
			{
				if (creature.isSummon())
				{
					creature.sendPacket(SystemMessageId.THIS_PET_CANNOT_USE_THIS_ITEM);
					return false;
				}
				
				if (sendMessage)
				{
					String msg = preCondition.getMessage();
					SystemMessageId msgId = preCondition.getMessageId();
					if (msg != null)
					{
						creature.sendMessage(msg);
					}
					else if (msgId != 0)
					{
						SystemMessagePacket sm = new SystemMessagePacket(msgId);
						if (preCondition.isAddName())
						{
							sm.Params.addItemName(_itemId);
						}
						creature.sendPacket(sm);
					}
				}
				return false;
			}
		}
		return true;
	}
	
	public bool isConditionAttached()
	{
		return (_preConditions != null) && _preConditions.Count != 0;
	}
	
	public bool isQuestItem()
	{
		return _questItem;
	}
	
	public bool isFreightable()
	{
		return _freightable;
	}
	
	public bool isAllowSelfResurrection()
	{
		return _allowSelfResurrection;
	}
	
	public bool isOlyRestrictedItem()
	{
		return _isOlyRestricted || Config.LIST_OLY_RESTRICTED_ITEMS.Contains(_itemId);
	}
	
	/**
	 * @return {@code true} if item cannot be used in event games.
	 */
	public bool isEventRestrictedItem()
	{
		return _isEventRestricted;
	}
	
	public bool isForNpc()
	{
		return _forNpc;
	}
	
	public bool isAppearanceable()
	{
		return _isAppearanceable;
	}
	
	/**
	 * @return {@code true} if the item is blessed, {@code false} otherwise.
	 */
	public bool isBlessed()
	{
		return _isBlessed;
	}
	
	public int getArtifactSlot()
	{
		return _artifactSlot;
	}
	
	/**
	 * Verifies if the item has effects immediately.<br>
	 * <i>Used for herbs mostly.</i>
	 * @return {@code true} if the item applies effects immediately, {@code false} otherwise
	 */
	public bool hasExImmediateEffect()
	{
		return _exImmediateEffect;
	}
	
	/**
	 * Verifies if the item has effects immediately.
	 * @return {@code true} if the item applies effects immediately, {@code false} otherwise
	 */
	public bool hasImmediateEffect()
	{
		return _immediateEffect;
	}
	
	/**
	 * @return the _default_action
	 */
	public ActionType getDefaultAction()
	{
		return _defaultAction;
	}
	
	public int useSkillDisTime()
	{
		return _useSkillDisTime;
	}
	
	/**
	 * Gets the item reuse delay time in seconds.
	 * @return the reuse delay time
	 */
	public int getReuseDelay()
	{
		return _reuseDelay;
	}
	
	/**
	 * Gets the shared reuse group.<br>
	 * Items with the same reuse group will render reuse delay upon those items when used.
	 * @return the shared reuse group
	 */
	public int getSharedReuseGroup()
	{
		return _sharedReuseGroup;
	}
	
	public CommissionItemType getCommissionItemType()
	{
		return _commissionItemType;
	}
	
	/**
	 * Usable in HTML windows.
	 * @return the icon link in client files
	 */
	public String getIcon()
	{
		return _icon;
	}
	
	public int getDefaultEnchantLevel()
	{
		return _defaultEnchantLevel;
	}

	public bool isPetItem() => getItemType() == EtcItemType.PET_COLLAR;
	
	/**
	 * @param extractableProduct
	 */
	public virtual void addCapsuledItem(ExtractableProduct extractableProduct)
	{
	}
	
	public double getStats(Stat stat, double defaultValue)
	{
		if (_funcTemplates != null)
		{
			FuncTemplate template = _funcTemplates.get(stat);
			if ((template != null) && ((template.getFunctionClass() == typeof(FuncAdd)) || (template.getFunctionClass() == typeof(FuncSet))))
			{
				return template.getValue();
			}
		}
		return defaultValue;
	}
	
	/**
	 * Returns the name of the item followed by the item ID.
	 * @return the name and the ID of the item
	 */
	public override String ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(_name);
		sb.Append("(");
		sb.Append(_itemId);
		sb.Append(")");
		return sb.ToString();
	}
}