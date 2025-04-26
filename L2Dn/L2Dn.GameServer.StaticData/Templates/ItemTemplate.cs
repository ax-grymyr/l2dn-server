using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text;
using L2Dn.Events;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats.Functions;
using L2Dn.GameServer.StaticData.Xml.Items;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.Model.Items;

/// <summary>
/// This class contains all information about an item (Weapon, Armor, or EtcItem).
/// </summary>
public abstract class ItemTemplate: IIdentifiable, IEventContainerProvider
{
    protected static readonly Logger Logger = LogManager.GetLogger(nameof(ItemTemplate));

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

    private readonly EventContainer _eventContainer;

    private readonly int _itemId;
    private readonly int _displayId;
    private readonly string _name;
    private readonly string _additionalName;
    private readonly string _icon;
    private readonly int _weight;
    private readonly bool _stackable;
    private readonly MaterialType _materialType;
    private readonly CrystalType _crystalType;
    private readonly TimeSpan _equipReuseDelay;
    private readonly int? _duration; // mana
    private readonly TimeSpan? _time;
    private readonly TimeSpan? _autoDestroyTime;
    private readonly long _bodyPart;
    private readonly int _referencePrice;
    private readonly int _crystalCount;
    private readonly bool _sellable;
    private readonly bool _dropable;
    private readonly bool _destroyable;
    private readonly bool _tradeable;
    private readonly bool _depositable;
    private readonly bool _enchantable;
    private readonly int _enchantLimit;
    private readonly int _ensoulNormalSlots;
    private readonly int _ensoulSpecialSlots;
    private readonly bool _elementable;
    private readonly bool _questItem;
    private readonly bool _freightable;
    private readonly bool _allowSelfResurrection;
    private readonly bool _isOlyRestricted;
    private readonly bool _isEventRestricted;
    private readonly bool _forNpc;
    private readonly bool _common;
    private readonly bool _heroItem;
    private readonly bool _pvpItem;
    private readonly bool _immediateEffect;
    private readonly bool _exImmediateEffect;
    private readonly int _defaultEnchantLevel;
    private readonly ActionType _defaultAction;

    protected int _type1; // needed for item list (inventory)
    protected int _type2; // different lists for armor, weapon, etc
    private readonly ImmutableArray<ItemSkillHolder> _unequipSkills;
    private readonly ImmutableArray<ExtractableProduct> _extractableItems;
    private readonly ImmutableArray<ItemSkillHolder> _skills;
    private readonly FrozenDictionary<Stat, StatFuncParameters> _statFuncParameters;
    private readonly FrozenDictionary<AttributeType, AttributeHolder> _elementals;
    private readonly ImmutableArray<IConditionBase> _conditions;

    private readonly int _useSkillDisTime;
    protected TimeSpan _reuseDelay;
    private readonly int _sharedReuseGroup;

    private readonly CommissionItemType _commissionItemType;

    private readonly bool _isAppearanceable;
    private readonly bool _isBlessed;

    private readonly int _artifactSlot;

    private protected ItemTemplate(XmlItem xmlItem, ItemParameterSet parameters)
    {
        _eventContainer = new EventContainer($"Item template {_itemId}", GlobalEvents.Global);

        _itemId = xmlItem.Id;
        _name = xmlItem.Name;
        _additionalName = xmlItem.AdditionalName;

        _displayId = parameters.GetInt32(XmlItemParameterType.displayId, xmlItem.Id);
        _icon = parameters.GetString(XmlItemParameterType.icon, string.Empty);
        _weight = parameters.GetInt32(XmlItemParameterType.weight, 0);
        _materialType = parameters.GetEnum(XmlItemParameterType.material, MaterialType.STEEL);
        _equipReuseDelay = parameters.GetTimeSpanSeconds(XmlItemParameterType.equip_reuse_delay, TimeSpan.Zero);

        int duration = parameters.GetInt32(XmlItemParameterType.duration, -1);
        _duration = duration < 0 ? null : duration;

        int time = parameters.GetInt32(XmlItemParameterType.time, -1);
        _time = time < 0 ? null : TimeSpan.FromMinutes(time);

        int autoDestroyTime = parameters.GetInt32(XmlItemParameterType.auto_destroy_time, -1);
        _autoDestroyTime = autoDestroyTime < 0 ? null : TimeSpan.FromSeconds(autoDestroyTime);

        _bodyPart = ItemData.SlotNameMap.GetValueOrDefault(parameters.GetString(XmlItemParameterType.bodypart, "none"));
        _referencePrice = parameters.GetInt32(XmlItemParameterType.price, 0);
        _crystalType = parameters.GetEnum(XmlItemParameterType.crystal_type, CrystalType.NONE);
        _crystalCount = parameters.GetInt32(XmlItemParameterType.crystal_count, 0);
        _stackable = parameters.GetBoolean(XmlItemParameterType.is_stackable, false);
        _sellable = parameters.GetBoolean(XmlItemParameterType.is_sellable, true);
        _dropable = parameters.GetBoolean(XmlItemParameterType.is_dropable, true);
        _destroyable = parameters.GetBoolean(XmlItemParameterType.is_destroyable, true);
        _tradeable = parameters.GetBoolean(XmlItemParameterType.is_tradable, true);
        _questItem = parameters.GetBoolean(XmlItemParameterType.is_questitem, false);
        if (Config.CustomDepositableItems.CUSTOM_DEPOSITABLE_ENABLED)
        {
            _depositable = !_questItem || Config.CustomDepositableItems.CUSTOM_DEPOSITABLE_QUEST_ITEMS;
        }
        else
        {
            _depositable = parameters.GetBoolean(XmlItemParameterType.is_depositable, true);
        }

        _ensoulNormalSlots = parameters.GetInt32(XmlItemParameterType.ensoulNormalSlots, 0);
        _ensoulSpecialSlots = parameters.GetInt32(XmlItemParameterType.ensoulSpecialSlots, 0);

        _elementable = parameters.GetBoolean(XmlItemParameterType.element_enabled, false);
        _enchantable = parameters.GetBoolean(XmlItemParameterType.enchant_enabled, false);
        _enchantLimit = parameters.GetInt32(XmlItemParameterType.enchant_limit, 0);
        _freightable = parameters.GetBoolean(XmlItemParameterType.is_freightable, false);
        _allowSelfResurrection = parameters.GetBoolean(XmlItemParameterType.allow_self_resurrection, false);
        _isOlyRestricted = parameters.GetBoolean(XmlItemParameterType.is_oly_restricted, false);
        _isEventRestricted = parameters.GetBoolean(XmlItemParameterType.is_event_restricted, false);
        _forNpc = parameters.GetBoolean(XmlItemParameterType.for_npc, false);
        _isAppearanceable = parameters.GetBoolean(XmlItemParameterType.isAppearanceable, false);
        _isBlessed = parameters.GetBoolean(XmlItemParameterType.blessed, false);
        _artifactSlot = parameters.GetInt32(XmlItemParameterType.artifactSlot, 0);
        _immediateEffect = parameters.GetBoolean(XmlItemParameterType.immediate_effect, false);
        _exImmediateEffect = parameters.GetBoolean(XmlItemParameterType.ex_immediate_effect, false);
        _defaultAction = parameters.GetEnum(XmlItemParameterType.default_action, ActionType.NONE);
        _useSkillDisTime = parameters.GetInt32(XmlItemParameterType.useSkillDisTime, 0);
        _defaultEnchantLevel = parameters.GetInt32(XmlItemParameterType.enchanted, 0);
        _reuseDelay = parameters.GetTimeSpanMilliSeconds(XmlItemParameterType.reuse_delay, TimeSpan.Zero);
        _sharedReuseGroup = parameters.GetInt32(XmlItemParameterType.shared_reuse_group, 0);
        _commissionItemType =
            parameters.GetEnum(XmlItemParameterType.commissionItemType, CommissionItemType.OTHER_ITEM);

        _common = _itemId >= 11605 && _itemId <= 12361;
        _heroItem = (_itemId >= 6611 && _itemId <= 6621) || (_itemId >= 9388 && _itemId <= 9390) || _itemId == 6842;
        _pvpItem = (_itemId >= 10667 && _itemId <= 10835) || (_itemId >= 12852 && _itemId <= 12977) ||
            (_itemId >= 14363 && _itemId <= 14525) || _itemId == 14528 || _itemId == 14529 || _itemId == 14558 ||
            (_itemId >= 15913 && _itemId <= 16024) || (_itemId >= 16134 && _itemId <= 16147) || _itemId == 16149 ||
            _itemId == 16151 || _itemId == 16153 || _itemId == 16155 || _itemId == 16157 || _itemId == 16159 ||
            (_itemId >= 16168 && _itemId <= 16176) || (_itemId >= 16179 && _itemId <= 16220);

        // Sealed item checks
        if (_additionalName == "Sealed")
        {
            if (_tradeable)
                Logger.Warn($"{nameof(ItemTemplate)}: Found tradeable [Sealed] item {_itemId}");

            if (_dropable)
                Logger.Warn($"{nameof(ItemTemplate)}: Found dropable [Sealed] item {_itemId}");

            if (_sellable)
                Logger.Warn($"{nameof(ItemTemplate)}: Found sellable [Sealed] item {_itemId}");
        }

        // Uequip skills
        _unequipSkills = xmlItem.UnequipSkills.
            Select(x => new ItemSkillHolder(x.Id, x.Level, x.Type, x.Chance, x.Value)).
            ToImmutableArray();

        // Extractable items
        _extractableItems = xmlItem.CapsuledItems.
            Select(x => new ExtractableProduct(x.Id, x.MinCount, x.MaxCount, x.Chance, x.MinEnchant, x.MaxEnchant)).
            ToImmutableArray();

        // Skills
        _skills = xmlItem.Skills.
            Select(x => new ItemSkillHolder(x.Id, x.Level, x.Type, x.Chance, x.Value)).
            ToImmutableArray();

        // Stats
        _statFuncParameters = xmlItem.Stats is null
            ? FrozenDictionary<Stat, StatFuncParameters>.Empty
            : xmlItem.Stats.Stats.Select(x => new StatFuncParameters(StatFuncType.ADD, 0, x.Type, x.Value)).
                ToFrozenDictionary(t => t.Stat);

        // Conditions
        _conditions = xmlItem.Conditions.Select(ConditionFactory.Instance.Create).ToImmutableArray();

        // Attributes
        _elementals = _statFuncParameters.Select(p => (Attribute: p.Key switch
            {
                Stat.FIRE_RES or Stat.FIRE_POWER => AttributeType.FIRE,
                Stat.WATER_RES or Stat.WATER_POWER => AttributeType.WATER,
                Stat.WIND_RES or Stat.WIND_POWER => AttributeType.WIND,
                Stat.EARTH_RES or Stat.EARTH_POWER => AttributeType.EARTH,
                Stat.HOLY_RES or Stat.HOLY_POWER => AttributeType.HOLY,
                Stat.DARK_RES or Stat.DARK_POWER => AttributeType.DARK,
                _ => AttributeType.NONE,
            }, Value: (int)p.Value.Value)).
            Where(p => p.Attribute != AttributeType.NONE).
            ToFrozenDictionary(p => p.Attribute, p => new AttributeHolder(p.Attribute, p.Value));
    }

    public EventContainer Events => _eventContainer;

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
    public int Id => _itemId;

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
        return _crystalType != CrystalType.NONE && _crystalCount > 0;
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
        return _crystalType.getCrystalId();
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
                    return _crystalCount + _crystalType.getCrystalEnchantBonusArmor() * (3 * enchantLevel - 6);
                }
                case TYPE2_WEAPON:
                {
                    return _crystalCount + _crystalType.getCrystalEnchantBonusWeapon() * (2 * enchantLevel - 3);
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
                    return _crystalCount + _crystalType.getCrystalEnchantBonusArmor() * enchantLevel;
                }
                case TYPE2_WEAPON:
                {
                    return _crystalCount + _crystalType.getCrystalEnchantBonusWeapon() * enchantLevel;
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
    public string getName()
    {
        return _name;
    }

    /**
     * @return the item's additional name.
     */
    public string getAdditionalName()
    {
        return _additionalName;
    }

    public ImmutableArray<AttributeHolder> getAttributes() => _elementals.Values;

    public AttributeHolder? getAttribute(AttributeType type) => _elementals.GetValueOrDefault(type);

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
        return _bodyPart != 0 && !getItemType().IsEtcItem();
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
        return _enchantable && !Config.Character.ENCHANT_BLACKLIST.Contains(_itemId);
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

    public ImmutableArray<IConditionBase> getConditions() => _conditions;

    public bool hasSkills()
    {
        return _skills != null;
    }

    /**
 * @return the extractable items list.
 */
    public ImmutableArray<ExtractableProduct> getExtractableItems()
    {
        return _extractableItems;
    }


    /**
     * Method to retrieve skills linked to this item armor and weapon: passive skills etcitem: skills used on item use <-- ???
     * @return Skills linked to this item as SkillHolder[]
     */
    public ImmutableArray<ItemSkillHolder> getAllSkills()
    {
        return _skills;
    }

    public ImmutableArray<ItemSkillHolder> UnequipSkills => _unequipSkills;

    /**
     * @param condition
     * @return {@code List} of {@link ItemSkillHolder} if item has skills and matches the condition, {@code null} otherwise
     */
    public List<ItemSkillHolder>? getSkills(Predicate<ItemSkillHolder> condition)
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
    public List<ItemSkillHolder>? getSkills(ItemSkillType type)
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

    public bool isConditionAttached() => !_conditions.IsDefaultOrEmpty;

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
        return _isOlyRestricted || Config.Olympiad.LIST_OLY_RESTRICTED_ITEMS.Contains(_itemId);
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
    public TimeSpan getReuseDelay()
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
    public string getIcon()
    {
        return _icon;
    }

    public int getDefaultEnchantLevel()
    {
        return _defaultEnchantLevel;
    }

    public bool isPetItem() => getItemType() == EtcItemType.PET_COLLAR;

    public double getStats(Stat stat, double defaultValue)
    {
        StatFuncParameters? parameters = _statFuncParameters.GetValueOrDefault(stat);
        return parameters is { FuncType: StatFuncType.ADD or StatFuncType.SET } ? parameters.Value : defaultValue;
    }

    /// <summary>
    /// Returns the name of the item followed by the item ID.
    /// </summary>
    public override string ToString() => $"{_name} ({_itemId})";
}