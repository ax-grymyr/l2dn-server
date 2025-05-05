using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Events;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats.Functions;
using L2Dn.GameServer.StaticData.Xml.Items;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.Templates;

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

    private readonly int _id;
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

    protected int _type1; // needed for the item list (inventory)
    protected int _type2; // different lists for armor, weapon, etc.
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
        _id = xmlItem.Id;
        _eventContainer = new EventContainer($"Item template {_id}", GlobalEvents.Global);

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

        _common = _id >= 11605 && _id <= 12361;
        _heroItem = (_id >= 6611 && _id <= 6621) || (_id >= 9388 && _id <= 9390) || _id == 6842;
        _pvpItem = (_id >= 10667 && _id <= 10835) || (_id >= 12852 && _id <= 12977) ||
            (_id >= 14363 && _id <= 14525) || _id == 14528 || _id == 14529 || _id == 14558 ||
            (_id >= 15913 && _id <= 16024) || (_id >= 16134 && _id <= 16147) || _id == 16149 ||
            _id == 16151 || _id == 16153 || _id == 16155 || _id == 16157 || _id == 16159 ||
            (_id >= 16168 && _id <= 16176) || (_id >= 16179 && _id <= 16220);

        // Sealed item checks
        if (_additionalName == "Sealed")
        {
            if (_tradeable)
                Logger.Warn($"{nameof(ItemTemplate)}: Found tradeable [Sealed] item {_id}");

            if (_dropable)
                Logger.Warn($"{nameof(ItemTemplate)}: Found dropable [Sealed] item {_id}");

            if (_sellable)
                Logger.Warn($"{nameof(ItemTemplate)}: Found sellable [Sealed] item {_id}");
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

    /// <summary>
    /// Returns the itemType.
    /// </summary>
    public abstract ItemType getItemType();

    public ItemTypeMask getItemMask() => getItemType().GetMask();

    /// <summary>
    /// Verifies if the item is an etc item.
    /// </summary>
    /// <returns><c>true</c> if the item is an etc item, <c>false</c> otherwise.</returns>
    public bool isEtcItem() => getItemType().IsEtcItem();

    /// <summary>
    /// Verifies if the item is an armor.
    /// </summary>
    /// <returns><c>true</c> if the item is an armor, <c>false</c> otherwise.</returns>
    public bool isArmor() => getItemType().IsArmor();

    /// <summary>
    /// Verifies if the item is a weapon.
    /// </summary>
    /// <returns><c>true</c> if the item is a weapon, <c>false</c> otherwise.</returns>
    public bool isWeapon() => getItemType().IsWeapon();

    /// <summary>
    /// Verifies if the item is a magic weapon.
    /// </summary>
    /// <returns><c>true</c> if the weapon is magic, <c>false</c> otherwise.</returns>
    public virtual bool isMagicWeapon() => false;

    /// <summary>
    /// The equip reuse delay.
    /// </summary>
    public TimeSpan getEquipReuseDelay() => _equipReuseDelay;

    /// <summary>
    /// Returns the duration of the item.
    /// </summary>
    public int? getDuration() => _duration;

    /// <summary>
    /// Returns the time of the item.
    /// </summary>
    public TimeSpan? getTime() => _time;

    /// <summary>
    /// Gets the auto destroy time of the item in seconds: 0 or less - default.
    /// </summary>
    public TimeSpan? getAutoDestroyTime() => _autoDestroyTime;

    /// <summary>
    /// Returns the ID of the item.
    /// </summary>
    public int Id => _id;

    /// <summary>
    /// Returns the display ID of the item.
    /// </summary>
    public int getDisplayId() => _displayId;

    /// <summary>
    /// Returns the type of material of the item.
    /// </summary>
    public MaterialType getMaterialType() => _materialType;

    /// <summary>
    /// Returns the type 2 of the item.
    /// </summary>
    public int getType2() => _type2;

    /// <summary>
    /// Returns the weight of the item.
    /// </summary>
    public int getWeight() => _weight;

    /// <summary>
    /// Returns if the item is crystallizable.
    /// </summary>
    /// <returns><c>true</c> if the item is crystallizable; otherwise, <c>false</c>.</returns>
    public bool isCrystallizable() => _crystalType != CrystalType.NONE && _crystalCount > 0;

    /// <summary>
    /// Gets the general item grade (No S80, S84, R95, R99).
    /// </summary>
    public ItemGrade getItemGrade() => _crystalType.GetItemGrade();

    /// <summary>
    /// Returns the type of crystal if item is crystallizable.
    /// </summary>
    public CrystalType getCrystalType() => _crystalType;

    /// <summary>
    /// Return the ID of crystal if item is crystallizable.
    /// </summary>
    public int getCrystalItemId() => _crystalType.getCrystalId();

    /// <summary>
    /// The grade of the item.
    /// For grades S80 and S84 return S, R95, and R99 return R.
    /// </summary>
    public CrystalType getCrystalTypePlus() =>
        _crystalType switch
        {
            CrystalType.S80 or CrystalType.S84 => CrystalType.S,
            CrystalType.R95 or CrystalType.R99 => CrystalType.R,
            _ => _crystalType,
        };

    /// <summary>
    /// Gets the quantity of crystals for crystallization.
    /// </summary>
    public int getCrystalCount() => _crystalCount;

    /// <summary>
    /// The quantity of crystals for crystallization on specific enchant level.
    /// </summary>
    /// <param name="enchantLevel">Enchant level.</param>
    /// <returns>The quantity of crystals.</returns>
    public int getCrystalCount(int enchantLevel) =>
        enchantLevel switch
        {
            > 3 => _type2 switch
            {
                TYPE2_SHIELD_ARMOR or TYPE2_ACCESSORY => _crystalCount +
                    _crystalType.getCrystalEnchantBonusArmor() * (3 * enchantLevel - 6),
                TYPE2_WEAPON => _crystalCount + _crystalType.getCrystalEnchantBonusWeapon() * (2 * enchantLevel - 3),
                _ => _crystalCount,
            },
            > 0 => _type2 switch
            {
                TYPE2_SHIELD_ARMOR or TYPE2_ACCESSORY => _crystalCount +
                    _crystalType.getCrystalEnchantBonusArmor() * enchantLevel,
                TYPE2_WEAPON => _crystalCount + _crystalType.getCrystalEnchantBonusWeapon() * enchantLevel,
                _ => _crystalCount,
            },
            _ => _crystalCount,
        };

    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    public string getName() => _name;

    /// <summary>
    /// Gets the item's additional name.
    /// </summary>
    public string getAdditionalName() => _additionalName;

    public ImmutableArray<AttributeHolder> getAttributes() => _elementals.Values;

    public AttributeHolder? getAttribute(AttributeType type) => _elementals.GetValueOrDefault(type);

    /// <summary>
    /// Gets the part of the body used with the item.
    /// </summary>
    public long getBodyPart() => _bodyPart;

    /// <summary>
    /// Gets the type 1 of the item.
    /// </summary>
    public int getType1() => _type1;

    /// <summary>
    /// Determines whether the item is stackable.
    /// </summary>
    /// <returns><c>true</c> if the item is stackable; otherwise, <c>false</c>.</returns>
    public bool isStackable() => _stackable;

    /// <summary>
    /// Determines whether the item can be equipped.
    /// </summary>
    /// <returns><c>true</c> if the item can be equipped; otherwise, <c>false</c>.</returns>
    public bool isEquipable() => _bodyPart != 0 && !getItemType().IsEtcItem();

    /// <summary>
    /// Gets the reference price of the item.
    /// </summary>
    /// <returns>The reference price.</returns>
    public int getReferencePrice() => _referencePrice;

    /// <summary>
    /// Determines whether the item can be sold.
    /// </summary>
    /// <returns><c>true</c> if the item can be sold; otherwise, <c>false</c>.</returns>
    public bool isSellable() => _sellable;

    /// <summary>
    /// Determines whether the item can be dropped.
    /// </summary>
    /// <returns><c>true</c> if the item can be dropped; otherwise, <c>false</c>.</returns>
    public bool isDropable() => _dropable;

    /// <summary>
    /// Determines whether the item can be destroyed.
    /// </summary>
    /// <returns><c>true</c> if the item can be destroyed; otherwise, <c>false</c>.</returns>
    public bool isDestroyable() => _destroyable;

    /// <summary>
    /// Determines whether the item can be traded.
    /// </summary>
    /// <returns><c>true</c> if the item can be traded; otherwise, <c>false</c>.</returns>
    public bool isTradeable() => _tradeable;

    /// <summary>
    /// Determines whether the item can be put into warehouse.
    /// </summary>
    /// <returns><c>true</c> if the item can be put into warehouse; otherwise, <c>false</c>.</returns>
    public bool isDepositable() => _depositable;

    /// <summary>
    /// Determines whether the item can be enchanted. This method also checks the enchant blacklist.
    /// </summary>
    /// <returns><c>true</c> if the item can be enchanted; otherwise, <c>false</c>.</returns>
    public bool isEnchantable() => _enchantable && !Config.Character.ENCHANT_BLACKLIST.Contains(_id);

    /// <summary>
    /// Returns the enchantment limit of the item.
    /// </summary>
    /// <returns>The enchantment limit.</returns>
    public int getEnchantLimit() => _enchantLimit > 0 ? _enchantLimit : 0;

    /**
     * @return the available ensoul slot count.
     */
    public int getEnsoulSlots() => _ensoulNormalSlots;

    /// <summary>
    /// Gets the available special ensoul slot count.
    /// </summary>
    /// <returns>The available special ensoul slot count.</returns>
    public int getSpecialEnsoulSlots() => _ensoulSpecialSlots;

    /// <summary>
    /// Determines whether the item can be elemented.
    /// </summary>
    /// <returns><c>true</c> if the item can be elemented; otherwise, <c>false</c>.</returns>
    public bool isElementable() => _elementable;

    /// <summary>
    /// Determines whether the item is common.
    /// </summary>
    /// <returns><c>true</c> if the item is common; otherwise, <c>false</c>.</returns>
    public bool isCommon() => _common;

    /**
     * Returns if item is hero-only
     * @return <c>true</c> if the item is hero-only; otherwise, <c>false</c>.
     */
    public bool isHeroItem() => _heroItem;

    /// <summary>
    /// Determines whether the item is a PvP item.
    /// </summary>
    /// <returns><c>true</c> if the item is a PvP item; otherwise, <c>false</c>.</returns>
    public bool isPvpItem() => _pvpItem;

    public bool isPotion() => getItemType() == EtcItemType.POTION;

    public bool isElixir() => getItemType() == EtcItemType.ELIXIR;

    public bool isScroll() => getItemType() == EtcItemType.SCROLL;

    public ImmutableArray<IConditionBase> getConditions() => _conditions;

    public bool hasSkills() => !_skills.IsDefaultOrEmpty;

    /// <summary>
    /// Gets the extractable items list.
    /// </summary>
    public ImmutableArray<ExtractableProduct> getExtractableItems() => _extractableItems;

    /// <summary>
    /// Gets all skills linked to this item (armor, weapon, etc.).
    /// </summary>
    /// <returns>Skills linked to this item.</returns>
    public ImmutableArray<ItemSkillHolder> getAllSkills() => _skills;

    public ImmutableArray<ItemSkillHolder> UnequipSkills => _unequipSkills;

    /// <summary>
    /// Gets the skills that match the specified condition.
    /// </summary>
    /// <param name="condition">The condition to match.</param>
    /// <returns>A list of ItemSkillHolder if the item has skills and matches the condition; otherwise, <c>null</c>.</returns>
    public List<ItemSkillHolder>? getSkills(Predicate<ItemSkillHolder> condition)
    {
        if (_skills.IsDefaultOrEmpty)
            return null;

        List<ItemSkillHolder>? result = null;
        foreach (ItemSkillHolder skill in _skills)
        {
            if (condition(skill))
            {
                result ??= [];
                result.Add(skill);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the skills of the specified type.
    /// </summary>
    /// <param name="type">The type of skills to get.</param>
    /// <returns>A list of ItemSkillHolder if the item has skills of the specified type; otherwise, <c>null</c>.</returns>
    public List<ItemSkillHolder>? getSkills(ItemSkillType type)
    {
        if (_skills.IsDefaultOrEmpty)
            return null;

        List<ItemSkillHolder>? result = null;
        foreach (ItemSkillHolder skill in _skills)
        {
            if (skill.getType() == type)
            {
                result ??= [];
                result.Add(skill);
            }
        }

        return result;
    }

    /// <summary>
    /// Executes the action on each item skill with the specified type (if there are skills at all).
    /// </summary>
    /// <param name="type">The type of skills to process.</param>
    /// <param name="action">The action to execute on each skill.</param>
    public void forEachSkill(ItemSkillType type, Action<ItemSkillHolder> action)
    {
        if (!_skills.IsDefaultOrEmpty)
        {
            foreach (ItemSkillHolder skill in _skills)
            {
                if (skill.getType() == type)
                    action(skill);
            }
        }
    }

    public bool isConditionAttached() => !_conditions.IsDefaultOrEmpty;

    public bool isQuestItem() => _questItem;

    public bool isFreightable() => _freightable;

    public bool isAllowSelfResurrection() => _allowSelfResurrection;

    public bool isOlyRestrictedItem() => _isOlyRestricted || Config.Olympiad.LIST_OLY_RESTRICTED_ITEMS.Contains(_id);

    /// <summary>
    /// Determines whether the item cannot be used in event games.
    /// </summary>
    /// <returns><c>true</c> if the item cannot be used in event games; otherwise, <c>false</c>.</returns>
    public bool isEventRestrictedItem() => _isEventRestricted;

    /// <summary>
    /// Determines whether the item is for NPCs.
    /// </summary>
    /// <returns><c>true</c> if the item is for NPCs; otherwise, <c>false</c>.</returns>
    public bool isForNpc() => _forNpc;

    public bool isAppearanceable() => _isAppearanceable;

    /**
     * @return {@code true} if the item is blessed, {@code false} otherwise.
     */
    public bool isBlessed() => _isBlessed;

    /// <summary>
    /// Gets the artifact slot.
    /// </summary>
    /// <returns>The artifact slot.</returns>
    public int getArtifactSlot() => _artifactSlot;

    /**
     * Verifies if the item has effects immediately.
     * Used for herbs mostly.
     * @return <c>true</c> if the item applies effects immediately; otherwise, <c>false</c>.
     */
    public bool hasExImmediateEffect() => _exImmediateEffect;

    /**
     * Determines whether the item has immediate effects.
     * @return <c>true</c> if the item applies effects immediately; otherwise, <c>false</c>.
     */
    public bool hasImmediateEffect() => _immediateEffect;

    /// <summary>
    /// Gets the default action of the item.
    /// </summary>
    /// <returns>The default action.</returns>
    public ActionType getDefaultAction() => _defaultAction;

    public int useSkillDisTime() => _useSkillDisTime;

    /// <summary>
    /// Gets the item reuse delay time.
    /// </summary>
    /// <returns>The reuse delay time.</returns>
    public TimeSpan getReuseDelay() => _reuseDelay;

    /**
     * Gets the shared reuse group.<br>
     * Items with the same reuse group will render reuse delay upon those items when used.
     * @return the shared reuse group
     */
    public int getSharedReuseGroup() => _sharedReuseGroup;

    public CommissionItemType getCommissionItemType() => _commissionItemType;

    /// <summary>
    /// Gets the icon of the item. Usable in HTML windows.
    /// </summary>
    /// <returns>The icon link in client files.</returns>
    public string getIcon() => _icon;

    public int getDefaultEnchantLevel() => _defaultEnchantLevel;

    public bool isPetItem() => getItemType() == EtcItemType.PET_COLLAR;

    public double getStats(Stat stat, double defaultValue)
    {
        StatFuncParameters? parameters = _statFuncParameters.GetValueOrDefault(stat);
        return parameters is { FuncType: StatFuncType.ADD or StatFuncType.SET } ? parameters.Value : defaultValue;
    }

    /// <summary>
    /// Returns the name of the item followed by the item ID.
    /// </summary>
    public override string ToString() => $"{_name} ({_id})";
}