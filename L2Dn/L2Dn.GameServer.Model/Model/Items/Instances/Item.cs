using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using L2Dn.Events;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model.Items.Instances;

/**
 * This class manages items.
 * @version $Revision: 1.4.2.1.2.11 $ $Date: 2005/03/31 16:07:50 $
 */
public class Item: WorldObject
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Item));
	private static readonly Logger LOG_ITEMS = LogManager.GetLogger("item");

	/** Owner */
	private int _ownerId;
	private Player _owner;

	/** ID of who dropped the item last, used for knownlist */
	private int _dropperObjectId = 0;

	/** Quantity of the item */
	private long _count = 1;
	/** Initial Quantity of the item */
	private long _initCount;
	/** Remaining time (in milliseconds) */
	private DateTime? _time;
	/** Quantity of the item can decrease */
	private bool _decrease = false;

	/** ID of the item */
	private readonly int _itemId;

	/** ItemTemplate associated to the item */
	private readonly ItemTemplate _itemTemplate;

	/** Location of the item : Inventory, PaperDoll, WareHouse */
	private ItemLocation _loc;

	/** Slot where item is stored : Paperdoll slot, inventory order ... */
	private int _locData;

	/** Level of enchantment of the item */
	private int _enchantLevel;

	/** Wear Item */
	private bool _wear;

	/** Augmented Item */
	private VariationInstance? _augmentation;

	/** Shadow item */
	private int? _mana;
	private bool _consumingMana;

	/** Custom item types (used loto, race tickets) */
	private int _type1;
	private int _type2;

	private DateTime? _dropTime;

	private bool _published = false;

	private bool _protected;

	public const int UNCHANGED = 0;
	public const int ADDED = 1;
	public const int REMOVED = 3;
	public const int MODIFIED = 2;

	public static readonly ImmutableArray<int> DEFAULT_ENCHANT_OPTIONS = [];

	private ItemChangeType _lastChange = ItemChangeType.MODIFIED; // 1 added, 2 modified, 3 removed
	private bool _existsInDb; // if a record exists in DB.
	private bool _storedInDb; // if DB data is up-to-date.

	private readonly object _dbLock = new();

	private Map<AttributeType, AttributeHolder>? _elementals;

	private ScheduledFuture? _itemLootShedule;

	private readonly DropProtection _dropProtection = new();

	private readonly List<Options.Options> _enchantOptions = new();
	private readonly EnsoulOption[] _ensoulOptions = new EnsoulOption[2];
	private readonly EnsoulOption[] _ensoulSpecialOptions = new EnsoulOption[1];
	private bool _isBlessed;

	/**
	 * Constructor of the Item from the objectId and the itemId.
	 * @param objectId : int designating the ID of the object in the world
	 * @param itemId : int designating the ID of the item
	 */
	public Item(int objectId, int itemId): base(objectId)
	{
		InstanceType = InstanceType.Item;
		_itemId = itemId;
		_itemTemplate = ItemData.getInstance().getTemplate(itemId);
		if (_itemId == 0 || _itemTemplate == null)
			throw new ArgumentException();

		base.setName(_itemTemplate.getName());
		_loc = ItemLocation.VOID;
		_type1 = 0;
		_type2 = 0;
		_dropTime = DateTime.MinValue;
		_mana = _itemTemplate.getDuration();
		_time = _itemTemplate.getTime() == null ? null : DateTime.UtcNow + _itemTemplate.getTime();
		scheduleLifeTimeTask();
		scheduleVisualLifeTime();
	}

	/**
	 * Constructor of the Item from the objetId and the description of the item given by the Item.
	 * @param objectId : int designating the ID of the object in the world
	 * @param itemTemplate : Item containing information of the item
	 */
	public Item(int objectId, ItemTemplate itemTemplate): base(objectId)
	{
		InstanceType = InstanceType.Item;
		_itemTemplate = itemTemplate;
		_itemId = itemTemplate.getId();
		if (_itemId == 0)
			throw new ArgumentException();

		base.setName(_itemTemplate.getName());
		_loc = ItemLocation.VOID;
		_mana = _itemTemplate.getDuration();
		_time = _itemTemplate.getTime() == null ? null : DateTime.UtcNow + _itemTemplate.getTime();
		scheduleLifeTimeTask();
		scheduleVisualLifeTime();
	}

	/**
	 * @param rs
	 * @throws SQLException
	 */
	public Item(DbItem item): this(item.ObjectId, ItemData.getInstance().getTemplate(item.ItemId))
	{
		_count = item.Count;
		_ownerId = item.OwnerId;
		_loc = (ItemLocation)item.Location;
		_locData = item.LocationData;
		_enchantLevel = item.EnchantLevel;
		_type1 = item.CustomType1;
		_type2 = item.CustomType2;
		_mana = item.ManaLeft;
		_time = item.Time;
		_existsInDb = true;
		_storedInDb = true;

		if (isEquipable())
		{
			restoreAttributes();
			restoreSpecialAbilities();
		}

		_isBlessed = getVariables().getBoolean(ItemVariables.BLESSED, false);
	}

	/**
	 * Constructor overload.<br>
	 * Sets the next free object ID in the ID factory.
	 * @param itemId the item template ID
	 */
	public Item(int itemId): this(IdManager.getInstance().getNextId(), itemId)
	{
	}

	/**
	 * Remove a Item from the world and send server->client GetItem packets.<br>
	 * <br>
	 * <b><u>Actions</u>:</b><br>
	 * <li>Send a Server->Client Packet GetItem to player that pick up and its _knowPlayers member</li>
	 * <li>Remove the WorldObject from the world</li><br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T REMOVE the object from _allObjects of World </b></font><br>
	 * <br>
	 * <b><u>Example of use</u>:</b><br>
	 * <li>Do Pickup Item : Player and Pet</li><br>
	 * @param creature Character that pick up the item
	 */
	public void pickupMe(Creature creature)
	{
		WorldRegion oldregion = getWorldRegion();

		// Create a server->client GetItem packet to pick up the Item
		creature.broadcastPacket(new GetItemPacket(this, creature.ObjectId));

		lock (this)
		{
			setSpawned(false);
		}

		// if this item is a mercenary ticket, remove the spawns!
		Castle castle = CastleManager.getInstance().getCastle(this);
		if (castle != null && SiegeGuardManager.getInstance().getSiegeGuardByItem(castle.getResidenceId(), getId()) != null)
		{
			SiegeGuardManager.getInstance().removeTicket(this);
			ItemsOnGroundManager.getInstance().removeObject(this);
		}

		// outside of synchronized to avoid deadlocks
		// Remove the Item from the world
		World.getInstance().removeVisibleObject(this, oldregion);

		// Notify to scripts
		EventContainer events = getTemplate().Events;
		if (creature.isPlayer() && events.HasSubscribers<OnPlayerItemPickup>())
		{
			events.NotifyAsync(new OnPlayerItemPickup(creature.getActingPlayer(), this));
		}
	}

	/**
	 * Sets the ownerID of the item
	 * @param process : String Identifier of process triggering this action
	 * @param ownerId : int designating the ID of the owner
	 * @param creator : Player Player requesting the item creation
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 */
	public void setOwnerId(string process, int ownerId, Player creator, object? reference)
	{
		setOwnerId(ownerId);

		if ((Config.LOG_ITEMS && !Config.LOG_ITEMS_SMALL_LOG && !Config.LOG_ITEMS_IDS_ONLY) || (Config.LOG_ITEMS_SMALL_LOG && (_itemTemplate.isEquipable() || _itemTemplate.getId() == Inventory.ADENA_ID)) || (Config.LOG_ITEMS_IDS_ONLY && Config.LOG_ITEMS_IDS_LIST.Contains(_itemTemplate.getId())))
		{
			if (_enchantLevel > 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SETOWNER:");
				sb.Append(process);
				sb.Append(", item ");
				sb.Append(ObjectId);
				sb.Append(":+");
				sb.Append(_enchantLevel);
				sb.Append(" ");
				sb.Append(_itemTemplate.getName());
				sb.Append("(");
				sb.Append(_count);
				sb.Append("), ");
				sb.Append(creator);
				sb.Append(", ");
				sb.Append(reference);
				LOG_ITEMS.Info(sb.ToString());
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SETOWNER:");
				sb.Append(process);
				sb.Append(", item ");
				sb.Append(ObjectId);
				sb.Append(":");
				sb.Append(_itemTemplate.getName());
				sb.Append("(");
				sb.Append(_count);
				sb.Append("), ");
				sb.Append(creator);
				sb.Append(", ");
				sb.Append(reference);
				LOG_ITEMS.Info(sb.ToString());
			}
		}

		if (creator != null && creator.isGM() && Config.GMAUDIT)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(process);
			sb.Append("(id: ");
			sb.Append(_itemId);
			sb.Append(" name: ");
			sb.Append(getName());
			sb.Append(")");

			string targetName = creator.getTarget() != null ? creator.getTarget().getName() : "no-target";

			string referenceName = "no-reference";
			if (reference is WorldObject)
			{
				referenceName = ((WorldObject) reference).getName() != null ? ((WorldObject) reference).getName() : "no-name";
			}
			else if (reference is string)
			{
				referenceName = (string) reference;
			}

			// TODO: GM audit not implemented
			//GMAudit.auditGMAction(creator.ToString(), sb.ToString(), targetName, StringUtil.concat("Object referencing this action is: ", referenceName));
		}
	}

	/**
	 * Sets the ownerID of the item
	 * @param ownerId : int designating the ID of the owner
	 */
	public void setOwnerId(int ownerId)
	{
		if (ownerId == _ownerId)
		{
			return;
		}

		// Remove any inventory skills from the old owner.
		removeSkillsFromOwner();

		_owner = null;
		_ownerId = ownerId;
		_storedInDb = false;

		// Give any inventory skills to the new owner only if the item is in inventory
		// else the skills will be given when location is set to inventory.
		giveSkillsToOwner();
	}

	/**
	 * Returns the ownerID of the item
	 * @return int : ownerID of the item
	 */
	public int getOwnerId()
	{
		return _ownerId;
	}

	/**
	 * Sets the location of the item
	 * @param loc : ItemLocation (enumeration)
	 */
	public void setItemLocation(ItemLocation loc)
	{
		setItemLocation(loc, 0);
	}

	/**
	 * Sets the location of the item.<br>
	 * <u><i>Remark :</i></u> If loc and loc_data different from database, say datas not up-to-date
	 * @param loc : ItemLocation (enumeration)
	 * @param locData : int designating the slot where the item is stored or the village for freights
	 */
	public void setItemLocation(ItemLocation loc, int locData)
	{
		if (loc == _loc && locData == _locData)
		{
			return;
		}

		// Remove any inventory skills from the old owner.
		removeSkillsFromOwner();

		_loc = loc;
		_locData = locData;
		_storedInDb = false;

		// Give any inventory skills to the new owner only if the item is in inventory
		// else the skills will be given when location is set to inventory.
		giveSkillsToOwner();
	}

	public ItemLocation getItemLocation()
	{
		return _loc;
	}

	/**
	 * Sets the quantity of the item.
	 * @param count the new count to set
	 */
	public void setCount(long count)
	{
		if (_count == count)
		{
			return;
		}

		_count = count >= -1 ? count : 0;
		_storedInDb = false;
	}

	/**
	 * @return Returns the count.
	 */
	public long getCount()
	{
		return _count;
	}

	/**
	 * Sets the quantity of the item.<br>
	 * <u><i>Remark :</i></u> If loc and loc_data different from database, say datas not up-to-date
	 * @param process : String Identifier of process triggering this action
	 * @param count : int
	 * @param creator : Player Player requesting the item creation
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 */
	public void changeCount(string? process, long count, Player? creator, object? reference)
	{
		if (count == 0)
		{
			return;
		}

		long old = _count;
		long max = _itemId == Inventory.ADENA_ID ? Inventory.MAX_ADENA : long.MaxValue;

		if (count > 0 && _count > max - count)
		{
			setCount(max);
		}
		else
		{
			setCount(_count + count);
		}

		if (_count < 0)
		{
			setCount(0);
		}

		_storedInDb = false;

		if ((Config.LOG_ITEMS && process != null && !Config.LOG_ITEMS_SMALL_LOG && !Config.LOG_ITEMS_IDS_ONLY) || (Config.LOG_ITEMS_SMALL_LOG && (_itemTemplate.isEquipable() || _itemTemplate.getId() == Inventory.ADENA_ID)) || (Config.LOG_ITEMS_IDS_ONLY && Config.LOG_ITEMS_IDS_LIST.Contains(_itemTemplate.getId())))
		{
			if (_enchantLevel > 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("CHANGE:");
				sb.Append(process);
				sb.Append(", item ");
				sb.Append(ObjectId);
				sb.Append(":+");
				sb.Append(_enchantLevel);
				sb.Append(" ");
				sb.Append(_itemTemplate.getName());
				sb.Append("(");
				sb.Append(_count);
				sb.Append("), PrevCount(");
				sb.Append(old);
				sb.Append("), ");
				sb.Append(creator);
				sb.Append(", ");
				sb.Append(reference);
				LOG_ITEMS.Info(sb.ToString());
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("CHANGE:");
				sb.Append(process);
				sb.Append(", item ");
				sb.Append(ObjectId);
				sb.Append(":");
				sb.Append(_itemTemplate.getName());
				sb.Append("(");
				sb.Append(_count);
				sb.Append("), PrevCount(");
				sb.Append(old);
				sb.Append("), ");
				sb.Append(creator);
				sb.Append(", ");
				sb.Append(reference);
				LOG_ITEMS.Info(sb.ToString());
			}
		}

		if (creator != null && creator.isGM() && Config.GMAUDIT)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(process);
			sb.Append("(id: ");
			sb.Append(_itemId);
			sb.Append(" objId: ");
			sb.Append(ObjectId);
			sb.Append(" name: ");
			sb.Append(getName());
			sb.Append(" count: ");
			sb.Append(count);
			sb.Append(")");

			string targetName = creator.getTarget() != null ? creator.getTarget().getName() : "no-target";

			string referenceName = "no-reference";
			if (reference is WorldObject)
			{
				referenceName = ((WorldObject) reference).getName() != null ? ((WorldObject) reference).getName() : "no-name";
			}
			else if (reference is string)
			{
				referenceName = (string) reference;
			}

			//GMAudit.auditGMAction(creator.ToString(), sb.ToString(), targetName, StringUtil.concat("Object referencing this action is: ", referenceName));
		}
	}

	// No logging (function designed for shots only)
	public void changeCountWithoutTrace(int count, Player creator, object reference)
	{
		changeCount(null, count, creator, reference);
	}

	/**
	 * Return true if item can be enchanted
	 * @return bool
	 */
	public bool isEnchantable()
	{
		if (_loc == ItemLocation.INVENTORY || _loc == ItemLocation.PAPERDOLL)
		{
			return _itemTemplate.isEnchantable();
		}
		return false;
	}

	/**
	 * Returns if item is equipable
	 * @return bool
	 */
	public bool isEquipable()
	{
		return _itemTemplate.getBodyPart() != ItemTemplate.SLOT_NONE;
	}

	/**
	 * Returns if item is equipped
	 * @return bool
	 */
	public bool isEquipped()
	{
		return _loc == ItemLocation.PAPERDOLL || _loc == ItemLocation.PET_EQUIP;
	}

	/**
	 * Returns the slot where the item is stored
	 * @return int
	 */
	public int getLocationSlot()
	{
		return _locData;
	}

	/**
	 * Returns the characteristics of the item.
	 * @return ItemTemplate
	 */
	public ItemTemplate getTemplate()
	{
		return _itemTemplate;
	}

	public int getCustomType1()
	{
		return _type1;
	}

	public int getCustomType2()
	{
		return _type2;
	}

	public void setCustomType1(int newtype)
	{
		_type1 = newtype;
	}

	public void setCustomType2(int newtype)
	{
		_type2 = newtype;
	}

	public void setDropTime(DateTime? time)
	{
		_dropTime = time;
	}

	public DateTime? getDropTime()
	{
		return _dropTime;
	}

	/**
	 * @return the type of item.
	 */
	public ItemType getItemType()
	{
		return _itemTemplate.getItemType();
	}

	/**
	 * Gets the item ID.
	 * @return the item ID
	 */
	public override int getId()
	{
		return _itemId;
	}

	/**
	 * @return the display Id of the item.
	 */
	public int getDisplayId()
	{
		return _itemTemplate.getDisplayId();
	}

	/**
	 * @return {@code true} if item is an EtcItem, {@code false} otherwise.
	 */
	public bool isEtcItem()
	{
		return _itemTemplate is EtcItem;
	}

	/**
	 * @return {@code true} if item is a Weapon/Shield, {@code false} otherwise.
	 */
	public bool isWeapon()
	{
		return _itemTemplate is Weapon;
	}

	/**
	 * @return {@code true} if item is an Armor, {@code false} otherwise.
	 */
	public bool isArmor()
	{
		return _itemTemplate is Armor;
	}

	/**
	 * @return the characteristics of the EtcItem, {@code false} otherwise.
	 */
	public EtcItem getEtcItem()
	{
		if (_itemTemplate is EtcItem)
		{
			return (EtcItem) _itemTemplate;
		}
		return null;
	}

	/**
	 * @return the characteristics of the Weapon.
	 */
	public Weapon getWeaponItem()
	{
		if (_itemTemplate is Weapon)
		{
			return (Weapon) _itemTemplate;
		}
		return null;
	}

	/**
	 * @return the characteristics of the Armor.
	 */
	public Armor getArmorItem()
	{
		if (_itemTemplate is Armor)
		{
			return (Armor) _itemTemplate;
		}
		return null;
	}

	/**
	 * @return the quantity of crystals for crystallization.
	 */
	public int getCrystalCount()
	{
		return _itemTemplate.getCrystalCount(_enchantLevel);
	}

	/**
	 * @return the reference price of the item.
	 */
	public long getReferencePrice()
	{
		return _itemTemplate.getReferencePrice();
	}

	/**
	 * @return the name of the item.
	 */
	public string getItemName()
	{
		return _itemTemplate.getName();
	}

	/**
	 * @return the reuse delay of this item.
	 */
	public TimeSpan getReuseDelay()
	{
		return _itemTemplate.getReuseDelay();
	}

	/**
	 * @return the shared reuse item group.
	 */
	public int getSharedReuseGroup()
	{
		return _itemTemplate.getSharedReuseGroup();
	}

	/**
	 * @return the last change of the item
	 */
	public ItemChangeType getLastChange()
	{
		return _lastChange;
	}

	/**
	 * Sets the last change of the item
	 * @param lastChange : int
	 */
	public void setLastChange(ItemChangeType lastChange)
	{
		_lastChange = lastChange;
	}

	/**
	 * Returns if item is stackable
	 * @return bool
	 */
	public bool isStackable()
	{
		return _itemTemplate.isStackable();
	}

	/**
	 * Returns if item is dropable
	 * @return bool
	 */
	public bool isDropable()
	{
		if (Config.ALT_ALLOW_AUGMENT_TRADE && isAugmented())
		{
			return true;
		}
		return !isAugmented() && getVisualId() == 0 && _itemTemplate.isDropable();
	}

	/**
	 * Returns if item is destroyable
	 * @return bool
	 */
	public bool isDestroyable()
	{
		if (!Config.ALT_ALLOW_AUGMENT_DESTROY && isAugmented())
		{
			return false;
		}
		return _itemTemplate.isDestroyable();
	}

	/**
	 * Returns if item is tradeable
	 * @return bool
	 */
	public bool isTradeable()
	{
		if (Config.ALT_ALLOW_AUGMENT_TRADE && isAugmented())
		{
			return true;
		}
		return !isAugmented() && _itemTemplate.isTradeable();
	}

	/**
	 * Returns if item is sellable
	 * @return bool
	 */
	public bool isSellable()
	{
		if (Config.ALT_ALLOW_AUGMENT_TRADE && isAugmented())
		{
			return true;
		}
		return !isAugmented() && _itemTemplate.isSellable();
	}

	/**
	 * @param isPrivateWareHouse
	 * @return if item can be deposited in warehouse or freight
	 */
	public bool isDepositable(bool isPrivateWareHouse)
	{
		// equipped, hero and quest items
		if (isEquipped() || !_itemTemplate.isDepositable())
		{
			return false;
		}
		// augmented not tradeable
		if (!isPrivateWareHouse && (!isTradeable() || isShadowItem()))
		{
			return false;
		}
		return true;
	}

	public bool isPotion()
	{
		return _itemTemplate.isPotion();
	}

	public bool isElixir()
	{
		return _itemTemplate.isElixir();
	}

	public bool isScroll()
	{
		return _itemTemplate.isScroll();
	}

	public bool isHeroItem()
	{
		return _itemTemplate.isHeroItem();
	}

	public bool isCommonItem()
	{
		return _itemTemplate.isCommon();
	}

	/**
	 * Returns whether this item is pvp or not
	 * @return bool
	 */
	public bool isPvp()
	{
		return _itemTemplate.isPvpItem();
	}

	public bool isOlyRestrictedItem()
	{
		return _itemTemplate.isOlyRestrictedItem();
	}

	/**
	 * @param player
	 * @param allowAdena
	 * @param allowNonTradeable
	 * @return if item is available for manipulation
	 */
	public bool isAvailable(Player player, bool allowAdena, bool allowNonTradeable)
	{
		Summon pet = player.getPet();

		return !isEquipped() // Not equipped
			&& _itemTemplate.getType2() != ItemTemplate.TYPE2_QUEST // Not Quest Item
			&& (_itemTemplate.getType2() != ItemTemplate.TYPE2_MONEY ||
				_itemTemplate.getType1() != ItemTemplate.TYPE1_SHIELD_ARMOR) // not money, not shield
			&& (pet == null ||
				ObjectId != pet.getControlObjectId()) // Not Control item of currently summoned pet
			&& !player.isProcessingItem(ObjectId) // Not momentarily used enchant scroll
			&& (allowAdena || _itemId != Inventory.ADENA_ID) // Not Adena
			&& !player.isCastingNow(s => s.getSkill().getItemConsumeId() != _itemId) && (allowNonTradeable ||
				(isTradeable() && !(_itemTemplate.getItemType() == EtcItemType.PET_COLLAR &&
					player.havePetInvItems())));
	}

	/**
	 * Returns the level of enchantment of the item
	 * @return int
	 */
	public int getEnchantLevel()
	{
		return _enchantLevel;
	}

	/**
	 * @return {@code true} if item is enchanted, {@code false} otherwise
	 */
	public bool isEnchanted()
	{
		return _enchantLevel > 0;
	}

	/**
	 * @param level the enchant value to set
	 */
	public void setEnchantLevel(int level)
	{
		int newLevel = Math.Max(0, level);
		if (_enchantLevel == newLevel)
		{
			return;
		}

		clearEnchantStats();

		// Agathion skills.
		if (isEquipped() && _itemTemplate.getBodyPart() == ItemTemplate.SLOT_AGATHION)
		{
			AgathionSkillHolder agathionSkills = AgathionData.getInstance().getSkills(getId());
			if (agathionSkills != null)
			{
				bool update = false;
				// Remove old skills.
				foreach (Skill skill in agathionSkills.getMainSkills(_enchantLevel))
				{
					getActingPlayer().removeSkill(skill, false, skill.isPassive());
					update = true;
				}
				foreach (Skill skill in agathionSkills.getSubSkills(_enchantLevel))
				{
					getActingPlayer().removeSkill(skill, false, skill.isPassive());
					update = true;
				}
				// Add new skills.
				if (getLocationSlot() == Inventory.PAPERDOLL_AGATHION1)
				{
					foreach (Skill skill in agathionSkills.getMainSkills(newLevel))
					{
						if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, getActingPlayer(), getActingPlayer()))
						{
							continue;
						}
						getActingPlayer().addSkill(skill, false);
						update = true;
					}
				}
				foreach (Skill skill in agathionSkills.getSubSkills(newLevel))
				{
					if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, getActingPlayer(), getActingPlayer()))
					{
						continue;
					}
					getActingPlayer().addSkill(skill, false);
					update = true;
				}
				if (update)
				{
					getActingPlayer().sendSkillList();
				}
			}
		}

		_enchantLevel = newLevel;
		applyEnchantStats();
		_storedInDb = false;

		getActingPlayer().getInventory().getPaperdollCache().clearMaxSetEnchant();
	}

	/**
	 * Returns whether this item is augmented or not
	 * @return true if augmented
	 */
	public bool isAugmented()
	{
		return _augmentation != null;
	}

	/**
	 * Returns the augmentation object for this item
	 * @return augmentation
	 */
	public VariationInstance? getAugmentation()
	{
		return _augmentation;
	}

	/**
	 * Sets a new augmentation
	 * @param augmentation
	 * @param updateDatabase
	 * @return return true if successfully
	 */
	public bool setAugmentation(VariationInstance augmentation, bool updateDatabase)
	{
		// Remove previous augmentation.
		if (_augmentation != null)
		{
			if (isEquipped())
			{
				_augmentation.removeBonus(getActingPlayer());
			}
			removeAugmentation();
		}

		_augmentation = augmentation;
		if (updateDatabase)
		{
			updateItemOptions();
		}

		// Notify to scripts.
		EventContainer events = getTemplate().Events;
		if (events.HasSubscribers<OnPlayerItemAugment>())
		{
			events.NotifyAsync(new OnPlayerItemAugment(getActingPlayer(), this, augmentation, true));
		}

		return true;
	}

	/**
	 * Remove the augmentation
	 */
	public void removeAugmentation()
	{
		if (_augmentation == null)
		{
			return;
		}

		// Copy augmentation before removing it.
		VariationInstance augment = _augmentation;
		_augmentation = null;

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int itemId = ObjectId;
			ctx.ItemVariations.Where(r => r.ItemId == itemId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not remove augmentation for " + this + " from DB: " + e);
		}

		// Notify to scripts.
		EventContainer events = getTemplate().Events;
		if (events.HasSubscribers<OnPlayerItemAugment>())
		{
			events.NotifyAsync(new OnPlayerItemAugment(getActingPlayer(), this, augment, false));
		}
	}

	public void restoreAttributes()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int itemId = ObjectId;
			DbItemVariation? record = ctx.ItemVariations.SingleOrDefault(r => r.ItemId == itemId);
			if (record is not null)
			{
				int mineralId = record.MineralId;
				int option1 = record.Option1;
				int option2 = record.Option2;
				if (option1 != -1 || option2 != -1)
				{
					_augmentation = new VariationInstance(mineralId, option1, option2);
				}
			}

			foreach (var itemElem in ctx.ItemElementals.Where(r => r.ItemId == itemId))
			{
				AttributeType attributeType = (AttributeType)itemElem.Type;
				int attributeValue = itemElem.Value;
				if (attributeType != (AttributeType)(-1) && attributeValue != -1)
				{
					applyAttribute(new AttributeHolder(attributeType, attributeValue));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not restore augmentation and elemental data for " + this + " from DB: " + e);
		}
	}

	public void updateItemOptions()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			updateItemOptions(ctx);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Item could not update atributes for " + this + " from DB:" + e);
		}
	}

	private void updateItemOptions(GameServerDbContext ctx)
	{
		try
		{
			int itemId = ObjectId;
			DbItemVariation? record = ctx.ItemVariations.SingleOrDefault(r => r.ItemId == itemId);
			if (record is null)
			{
				record = new DbItemVariation();
				record.ItemId = itemId;
			}

			if (_augmentation != null)
			{
				record.MineralId = _augmentation.getMineralId();
				record.Option1 = _augmentation.getOption1Id();
				record.Option1 = _augmentation.getOption2Id();
			}
			else
			{
				record.MineralId = 0;
				record.Option1 = -1;
				record.Option1 = -1;
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not update atributes for " + this + " from DB: " + e);
		}
	}

	public void updateItemElementals()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			updateItemElements(ctx);
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not update elementals for " + this + " from DB: " + e);
		}
	}

	private void updateItemElements(GameServerDbContext ctx)
	{
		try
		{
			int itemId = ObjectId;
			ctx.ItemElementals.Where(r => r.ItemId == itemId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not update elementals for " + this + " from DB: " + e);
		}

		if (_elementals == null)
		{
			return;
		}

		try
		{
			foreach (AttributeHolder attribute in _elementals.Values)
			{
				ctx.ItemElementals.Add(new DbItemElemental()
				{
					ItemId = ObjectId,
					Type = (byte)attribute.getType(),
					Value = attribute.getValue()
				});
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not update elementals for " + this + " from DB: " + e);
		}
	}

	public ICollection<AttributeHolder> getAttributes()
	{
		return _elementals != null ? _elementals.Values : null;
	}

	public bool hasAttributes()
	{
		return _elementals != null && _elementals.Count != 0;
	}

	public AttributeHolder? getAttribute(AttributeType type)
	{
		return _elementals != null ? _elementals.get(type) : null;
	}

	public AttributeHolder? getAttackAttribute()
	{
		if (isWeapon())
		{
			if (_itemTemplate.getAttributes() != null)
			{
				if (_itemTemplate.getAttributes().Count != 0)
				{
					return _itemTemplate.getAttributes().First();
				}
			}
			else if (_elementals != null && _elementals.Count != 0)
			{
				return _elementals.Values.First();
			}
		}
		return null;
	}

	public AttributeType getAttackAttributeType()
	{
		AttributeHolder holder = getAttackAttribute();
		return holder != null ? holder.getType() : AttributeType.NONE;
	}

	public int getAttackAttributePower()
	{
		AttributeHolder holder = getAttackAttribute();
		return holder != null ? holder.getValue() : 0;
	}

	public int getDefenceAttribute(AttributeType element)
	{
		if (isArmor())
		{
			if (_itemTemplate.getAttributes() != null)
			{
				AttributeHolder attribute = _itemTemplate.getAttribute(element);
				if (attribute != null)
				{
					return attribute.getValue();
				}
			}
			else if (_elementals != null)
			{
				AttributeHolder attribute = getAttribute(element);
				if (attribute != null)
				{
					return attribute.getValue();
				}
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	private void applyAttribute(AttributeHolder holder)
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
	 * Add elemental attribute to item and save to db
	 * @param holder
	 * @param updateDatabase
	 */
	public void setAttribute(AttributeHolder holder, bool updateDatabase)
	{
		applyAttribute(holder);
		if (updateDatabase)
		{
			updateItemElementals();
		}
	}

	/**
	 * Remove elemental from item
	 * @param type byte element to remove
	 */
	public void clearAttribute(AttributeType type)
	{
		if (_elementals == null || getAttribute(type) == null)
		{
			return;
		}

		lock (_elementals)
		{
			_elementals.remove(type);
		}

		try
		{
			int itemId = ObjectId;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ItemElementals.Where(r => r.ItemId == itemId && r.Type == (byte)type).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not remove elemental enchant for " + this + " from DB: " + e);
		}
	}

	public void clearAllAttributes()
	{
		if (_elementals == null)
		{
			return;
		}

		lock (_elementals)
		{
			_elementals.Clear();
		}

		try
		{
			int itemId = ObjectId;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ItemElementals.Where(r => r.ItemId == itemId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not remove all elemental enchant for " + this + " from DB: " + e);
		}
	}

	/**
	 * Returns true if this item is a shadow item Shadow items have a limited life-time
	 * @return
	 */
	public bool isShadowItem()
	{
		return _mana != null;
	}

	/**
	 * Returns the remaining mana of this shadow item
	 * @return lifeTime
	 */
	public int? getMana()
	{
		return _mana;
	}

	/**
	 * Decreases the mana of this shadow item, sends a inventory update schedules a new consumption task if non is running optionally one could force a new task
	 * @param resetConsumingMana if true forces a new consumption task if item is equipped
	 */
	public void decreaseMana(bool resetConsumingMana)
	{
		decreaseMana(resetConsumingMana, 1);
	}

	/**
	 * Decreases the mana of this shadow item, sends a inventory update schedules a new consumption task if non is running optionally one could force a new task
	 * @param resetConsumingMana if forces a new consumption task if item is equipped
	 * @param count how much mana decrease
	 */
	public void decreaseMana(bool resetConsumingMana, int count)
	{
		if (!isShadowItem())
		{
			return;
		}

		if (_mana - count >= 0)
		{
			_mana -= count;
		}
		else
		{
			_mana = 0;
		}

		if (_storedInDb)
		{
			_storedInDb = false;
		}
		if (resetConsumingMana)
		{
			_consumingMana = false;
		}

		Player player = getActingPlayer();
		if (player != null)
		{
			SystemMessagePacket sm;
			switch (_mana)
			{
				case 10:
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S_REMAINING_MANA_IS_NOW_10);
					sm.Params.addItemName(_itemTemplate);
					player.sendPacket(sm);
					break;
				}
				case 5:
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S_REMAINING_MANA_IS_NOW_5);
					sm.Params.addItemName(_itemTemplate);
					player.sendPacket(sm);
					break;
				}
				case 1:
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S_REMAINING_MANA_IS_NOW_1_IT_WILL_DISAPPEAR_SOON);
					sm.Params.addItemName(_itemTemplate);
					player.sendPacket(sm);
					break;
				}
			}

			if (_mana == 0) // The life time has expired
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_S_REMAINING_MANA_IS_NOW_0_AND_THE_ITEM_HAS_DISAPPEARED);
				sm.Params.addItemName(_itemTemplate);
				player.sendPacket(sm);

				// unequip
				if (isEquipped())
				{
					List<ItemInfo> items = new List<ItemInfo>();
					foreach (Item item in player.getInventory().unEquipItemInSlotAndRecord(getLocationSlot()))
					{
						items.Add(new ItemInfo(item, ItemChangeType.MODIFIED));
					}
					InventoryUpdatePacket iu = new InventoryUpdatePacket(items);
					player.sendInventoryUpdate(iu);
					player.broadcastUserInfo();
				}

				if (_loc != ItemLocation.WAREHOUSE)
				{
					// destroy
					player.getInventory().destroyItem("Item", this, player, null);

					// send update
					InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(this, ItemChangeType.REMOVED));
					player.sendInventoryUpdate(iu);
				}
				else
				{
					player.getWarehouse().destroyItem("Item", this, player, null);
				}
			}
			else
			{
				// Reschedule if still equipped
				if (!_consumingMana && isEquipped())
				{
					scheduleConsumeManaTask();
				}
				if (_loc != ItemLocation.WAREHOUSE)
				{
					InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(this, ItemChangeType.MODIFIED));
					player.sendInventoryUpdate(iu);
				}
			}
		}
	}

	public void scheduleConsumeManaTask()
	{
		if (_consumingMana)
		{
			return;
		}
		_consumingMana = true;
		ItemManaTaskManager.getInstance().add(this);
	}

	/**
	 * Returns false cause item can't be attacked
	 * @return bool false
	 */
	public override bool isAutoAttackable(Creature attacker)
	{
		return false;
	}

	/**
	 * Updates the database.
	 */
	public void updateDatabase()
	{
		updateDatabase(false);
	}

	/**
	 * Updates the database.
	 * @param force if the update should necessarilly be done.
	 */
	public void updateDatabase(bool force)
	{
		lock (_dbLock)
		{
			if (_existsInDb)
			{
				if (_ownerId == 0 || _loc == ItemLocation.VOID || _loc == ItemLocation.REFUND || (_count == 0 && _loc != ItemLocation.LEASE))
				{
					removeFromDb();
				}
				else if (!Config.LAZY_ITEMS_UPDATE || force)
				{
					updateInDb();
				}
			}
			else
			{
				if (_ownerId == 0 || _loc == ItemLocation.VOID || _loc == ItemLocation.REFUND || (_count == 0 && _loc != ItemLocation.LEASE))
				{
					return;
				}
				insertIntoDb();
			}
		}
	}

	/**
	 * Init a dropped Item and add it in the world as a visible object.<br>
	 * <br>
	 * <b><u>Actions</u>:</b><br>
	 * <li>Set the x,y,z position of the Item dropped and update its _worldregion</li>
	 * <li>Add the Item dropped to _visibleObjects of its WorldRegion</li>
	 * <li>Add the Item dropped in the world as a <b>visible</b> object</li><br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T ADD the object to _allObjects of World </b></font><br>
	 * <br>
	 * <b><u>Example of use</u>:</b><br>
	 * <li>Drop item</li>
	 * <li>Call Pet</li>
	 * @param dropper
	 * @param locX
	 * @param locY
	 * @param locZ
	 */
	public void dropMe(Creature dropper, Location3D location)
	{
		Location3D loc = location;
		if (dropper != null)
		{
			Instance? instance = dropper.getInstanceWorld();
			loc = GeoEngine.getInstance().getValidLocation(dropper.Location.Location3D, loc, instance);
			setInstance(instance); // Inherit instancezone when dropped in visible world
		}
		else
		{
			setInstance(null); // No dropper? Make it a global item...
		}

		// Set the x,y,z position of the Item dropped and update its world region
		setSpawned(true);
		setXYZ(loc);

		setDropTime(DateTime.UtcNow);
		setDropperObjectId(dropper != null ? dropper.ObjectId : 0); // Set the dropper Id for the knownlist packets in sendInfo

		// Add the Item dropped in the world as a visible object
		WorldRegion region = getWorldRegion();
		region.addVisibleObject(this);
		World.getInstance().addVisibleObject(this, region);
		if (Config.SAVE_DROPPED_ITEM)
		{
			ItemsOnGroundManager.getInstance().save(this);
		}
		setDropperObjectId(0); // Set the dropper Id back to 0 so it no longer shows the drop packet

		if (dropper != null && dropper.isPlayer())
		{
			_owner = null;

			// Notify to scripts
			EventContainer events = getTemplate().Events;
			if (events.HasSubscribers<OnPlayerItemDrop>())
			{
				events.NotifyAsync(new OnPlayerItemDrop(dropper.getActingPlayer(), this, loc));
			}
		}
	}

	/**
	 * Update the database with values of the item
	 */
	private void updateInDb()
	{
		if (!_existsInDb || _wear || _storedInDb)
		{
			return;
		}

		try
		{
			int itemId = ObjectId;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			DbItem? item = ctx.Items.SingleOrDefault(r => r.ObjectId == itemId);
			if (item is null)
			{
				item = new DbItem();
				item.ObjectId = itemId;
				item.ItemId = _itemId;
			}

			item.OwnerId = _ownerId;
			item.Count = _count;
			item.Location = (int)_loc;
			item.LocationData = _locData;
			item.EnchantLevel = _enchantLevel;
			item.CustomType1 = _type1;
			item.CustomType2 = _type2;
			item.ManaLeft = _mana;
			item.Time = _time;
			ctx.SaveChanges();

			_existsInDb = true;
			_storedInDb = true;

			if (_augmentation != null)
			{
				updateItemOptions(ctx);
			}

			if (_elementals != null)
			{
				updateItemElements(ctx);
			}

			updateSpecialAbilities(ctx);
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not update " + this + " in DB: Reason: " + e);
		}
	}

	/**
	 * Insert the item in database
	 */
	private void insertIntoDb()
	{
		if (_existsInDb || ObjectId == 0 || _wear)
		{
			return;
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Items.Add(new DbItem()
			{
				OwnerId = _ownerId,
				ItemId = _itemId,
				Count = _count,
				Location = (int)_loc,
				LocationData = _locData,
				EnchantLevel = _enchantLevel,
				ObjectId = ObjectId,
				CustomType1 = _type1,
				CustomType2 = _type2,
				ManaLeft = _mana,
				Time = _time,
			});

			ctx.SaveChanges();

			_existsInDb = true;
			_storedInDb = true;

			if (_augmentation != null)
			{
				updateItemOptions(ctx);
			}

			if (_elementals != null)
			{
				updateItemElements(ctx);
			}

			updateSpecialAbilities(ctx);
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not insert " + this + " into DB: Reason: " + e);
		}
	}

	/**
	 * Delete item from database
	 */
	private void removeFromDb()
	{
		if (!_existsInDb || _wear)
		{
			return;
		}

		try
		{
			int itemId = ObjectId;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			ctx.ItemVariables.Where(r => r.ItemId == itemId).ExecuteDelete();
			ctx.ItemVariations.Where(r => r.ItemId == itemId).ExecuteDelete();
			ctx.ItemElementals.Where(r => r.ItemId == itemId).ExecuteDelete();
			ctx.ItemSpecialAbilities.Where(r => r.ItemId == itemId).ExecuteDelete();
			ctx.Items.Where(r => r.ObjectId == itemId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not delete " + this + " in DB " + e);
		}
		finally
		{
			_existsInDb = false;
			_storedInDb = false;
		}
	}

	public void resetOwnerTimer()
	{
		if (_itemLootShedule != null)
		{
			_itemLootShedule.cancel(true);
			_itemLootShedule = null;
		}
	}

	public void setItemLootShedule(ScheduledFuture sf)
	{
		_itemLootShedule = sf;
	}

	public ScheduledFuture getItemLootShedule()
	{
		return _itemLootShedule;
	}

	public void setProtected(bool isProtected)
	{
		_protected = isProtected;
	}

	public bool isProtected()
	{
		return _protected;
	}

	public bool isAvailable()
	{
		if (!_itemTemplate.isConditionAttached())
		{
			return true;
		}
		if (_loc == ItemLocation.PET || _loc == ItemLocation.PET_EQUIP)
		{
			return true;
		}

		Player player = getActingPlayer();
		if (player != null)
		{
			foreach (Condition condition in _itemTemplate.getConditions())
			{
				if (condition == null)
				{
					continue;
				}

				if (!condition.TestImpl(player, player, null, _itemTemplate))
				{
					return false;
				}
			}

			if (player.hasRequest<AutoPeelRequest>())
			{
				EtcItem etcItem = getEtcItem();
				if (etcItem != null && etcItem.getExtractableItems() != null)
				{
					return false;
				}
			}
		}

		return true;
	}

	public void setCountDecrease(bool decrease)
	{
		_decrease = decrease;
	}

	public bool getCountDecrease()
	{
		return _decrease;
	}

	public void setInitCount(int initCount)
	{
		_initCount = initCount;
	}

	public long getInitCount()
	{
		return _initCount;
	}

	public void restoreInitCount()
	{
		if (_decrease)
		{
			setCount(_initCount);
		}
	}

	public bool isTimeLimitedItem()
	{
		return _time != null;
	}

	/**
	 * Returns (current system time + time) of this time limited item
	 * @return Time
	 */
	public DateTime? getTime()
	{
		return _time;
	}

	public TimeSpan? getRemainingTime()
	{
		return _time is null ? null : _time.Value - DateTime.UtcNow;
	}

	public void endOfLife()
	{
		Player player = getActingPlayer();
		if (player != null)
		{
			if (isEquipped())
			{
				List<ItemInfo> items = new List<ItemInfo>();
				foreach (Item item in player.getInventory().unEquipItemInSlotAndRecord(getLocationSlot()))
				{
					items.Add(new ItemInfo(item, ItemChangeType.MODIFIED));
				}

				InventoryUpdatePacket iu = new InventoryUpdatePacket(items);
				player.sendInventoryUpdate(iu);
			}

			if (_loc != ItemLocation.WAREHOUSE)
			{
				// destroy
				player.getInventory().destroyItem("Item", this, player, null);

				// send update
				InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(this, ItemChangeType.REMOVED));
				player.sendInventoryUpdate(iu);
			}
			else
			{
				player.getWarehouse().destroyItem("Item", this, player, null);
			}

			var sm = new SystemMessagePacket(SystemMessageId.S1_HAS_EXPIRED);
			sm.Params.addItemName(_itemId);
			player.sendPacket(sm);
		}
	}

	public void scheduleLifeTimeTask()
	{
		if (!isTimeLimitedItem())
		{
			return;
		}
		if (getRemainingTime() <= TimeSpan.Zero)
		{
			endOfLife();
		}
		else
		{
			ItemLifeTimeTaskManager.getInstance().add(this, getTime().Value);
		}
	}

	public void setDropperObjectId(int id)
	{
		_dropperObjectId = id;
	}

	public override void sendInfo(Player player)
	{
		if (_dropperObjectId != 0)
		{
			player.sendPacket(new DropItemPacket(this, _dropperObjectId));
		}
		else
		{
			player.sendPacket(new SpawnItemPacket(this));
		}
	}

	public DropProtection getDropProtection()
	{
		return _dropProtection;
	}

	public bool isPublished()
	{
		return _published;
	}

	public void publish()
	{
		_published = true;
	}

	public override bool decayMe()
	{
		if (Config.SAVE_DROPPED_ITEM)
		{
			ItemsOnGroundManager.getInstance().removeObject(this);
		}

		return base.decayMe();
	}

	public bool isQuestItem()
	{
		return _itemTemplate.isQuestItem();
	}

	public bool isElementable()
	{
		if (_loc == ItemLocation.INVENTORY || _loc == ItemLocation.PAPERDOLL)
		{
			return _itemTemplate.isElementable();
		}
		return false;
	}

	public bool isFreightable()
	{
		return _itemTemplate.isFreightable();
	}

	public int useSkillDisTime()
	{
		return _itemTemplate.useSkillDisTime();
	}

	public int getOlyEnchantLevel()
	{
		Player player = getActingPlayer();
		int enchant = _enchantLevel;

		if (player == null)
		{
			return enchant;
		}

		if (player.isInOlympiadMode())
		{
			if (_itemTemplate.isWeapon())
			{
				if (Config.ALT_OLY_WEAPON_ENCHANT_LIMIT >= 0 && enchant > Config.ALT_OLY_WEAPON_ENCHANT_LIMIT)
				{
					enchant = Config.ALT_OLY_WEAPON_ENCHANT_LIMIT;
				}
			}
			else
			{
				if (Config.ALT_OLY_ARMOR_ENCHANT_LIMIT >= 0 && enchant > Config.ALT_OLY_ARMOR_ENCHANT_LIMIT)
				{
					enchant = Config.ALT_OLY_ARMOR_ENCHANT_LIMIT;
				}
			}
		}

		return enchant;
	}

	public bool hasPassiveSkills()
	{
		return _itemTemplate.getItemType() == EtcItemType.ENCHT_ATTR_RUNE && _loc == ItemLocation.INVENTORY && _ownerId > 0 && _itemTemplate.getSkills(ItemSkillType.NORMAL) != null;
	}

	public void giveSkillsToOwner()
	{
		if (!hasPassiveSkills())
		{
			return;
		}

		Player player = getActingPlayer();
		if (player != null)
		{
			_itemTemplate.forEachSkill(ItemSkillType.NORMAL, holder =>
			{
				Skill skill = holder.getSkill();
				if (skill.isPassive())
				{
					player.addSkill(skill, false);
				}
			});
		}
	}

	public void removeSkillsFromOwner()
	{
		if (!hasPassiveSkills())
		{
			return;
		}

		Player player = getActingPlayer();
		if (player != null)
		{
			_itemTemplate.forEachSkill(ItemSkillType.NORMAL, holder =>
			{
				Skill skill = holder.getSkill();
				if (skill.isPassive())
				{
					player.removeSkill(skill, false, skill.isPassive());
				}
			});
		}
	}

	public override bool isItem()
	{
		return true;
	}

	public override Player getActingPlayer()
	{
		if (_owner == null && _ownerId != 0)
		{
			_owner = World.getInstance().getPlayer(_ownerId);
		}
		return _owner;
	}

	public TimeSpan getEquipReuseDelay()
	{
		return _itemTemplate.getEquipReuseDelay();
	}

	/**
	 * @param player
	 * @param command
	 */
	public void onBypassFeedback(Player player, string command)
	{
		if (command.StartsWith("Quest"))
		{
			string questName = command.Substring(6);
			string @event = null;
			int idx = questName.IndexOf(' ');
			if (idx > 0)
			{
				@event = questName.Substring(idx).Trim();
			}

			EventContainer events = getTemplate().Events;
			if (@event != null)
			{
				if (events.HasSubscribers<OnItemBypassEvent>())
				{
					events.NotifyAsync(new OnItemBypassEvent(this, player, @event));
				}
			}
			else if (events.HasSubscribers<OnItemTalk>())
			{
				events.NotifyAsync(new OnItemTalk(this, player));
			}
		}
	}

	/**
	 * Returns enchant effect object for this item
	 * @return enchanteffect
	 */
	public ImmutableArray<int> getEnchantOptions()
	{
		return EnchantItemOptionsData.getInstance().getOptions(this);
	}

	public ICollection<EnsoulOption> getSpecialAbilities()
	{
		List<EnsoulOption> result = new();
		foreach (EnsoulOption ensoulOption in _ensoulOptions)
		{
			if (ensoulOption != null)
			{
				result.Add(ensoulOption);
			}
		}
		return result;
	}

	public EnsoulOption getSpecialAbility(int index)
	{
		return _ensoulOptions[index];
	}

	public ICollection<EnsoulOption> getAdditionalSpecialAbilities()
	{
		List<EnsoulOption> result = new();
		foreach (EnsoulOption ensoulSpecialOption in _ensoulSpecialOptions)
		{
			if (ensoulSpecialOption != null)
			{
				result.Add(ensoulSpecialOption);
			}
		}
		return result;
	}

	public EnsoulOption getAdditionalSpecialAbility(int index)
	{
		return _ensoulSpecialOptions[index];
	}

	public void addSpecialAbility(EnsoulOption option, int position, int type, bool updateInDB)
	{
		if (type == 1 && (position < 0 || position > 1)) // two first slots
		{
			return;
		}
		if (type == 2 && position != 0) // third slot
		{
			return;
		}

		if (type == 1) // Adding regular ability
		{
			EnsoulOption oldOption = _ensoulOptions[position];
			if (oldOption != null)
			{
				removeSpecialAbility(oldOption);
			}
			if (position < _itemTemplate.getEnsoulSlots())
			{
				_ensoulOptions[position] = option;
			}
		}
		else if (type == 2) // Adding special ability
		{
			EnsoulOption oldOption = _ensoulSpecialOptions[position];
			if (oldOption != null)
			{
				removeSpecialAbility(oldOption);
			}
			if (position < _itemTemplate.getSpecialEnsoulSlots())
			{
				_ensoulSpecialOptions[position] = option;
			}
		}

		if (updateInDB)
		{
			updateSpecialAbilities();
		}
	}

	public void removeSpecialAbility(int position, int type)
	{
		if (type == 1)
		{
			EnsoulOption option = _ensoulOptions[position];
			if (option != null)
			{
				removeSpecialAbility(option);
				_ensoulOptions[position] = null;

				// Rearrange.
				if (position == 0)
				{
					EnsoulOption secondEnsoul = _ensoulOptions[1];
					if (secondEnsoul != null)
					{
						removeSpecialAbility(secondEnsoul);
						_ensoulOptions[1] = null;
						addSpecialAbility(secondEnsoul, 0, type, true);
					}
				}
			}
		}
		else if (type == 2)
		{
			EnsoulOption option = _ensoulSpecialOptions[position];
			if (option != null)
			{
				removeSpecialAbility(option);
				_ensoulSpecialOptions[position] = null;
			}
		}
	}

	public void clearSpecialAbilities()
	{
		foreach (EnsoulOption ensoulOption in _ensoulOptions)
		{
			clearSpecialAbility(ensoulOption);
		}
		foreach (EnsoulOption ensoulSpecialOption in _ensoulSpecialOptions)
		{
			clearSpecialAbility(ensoulSpecialOption);
		}
	}

	public void applySpecialAbilities()
	{
		if (!isEquipped())
		{
			return;
		}

		foreach (EnsoulOption ensoulOption in _ensoulOptions)
		{
			applySpecialAbility(ensoulOption);
		}
		foreach (EnsoulOption ensoulSpecialOption in _ensoulSpecialOptions)
		{
			applySpecialAbility(ensoulSpecialOption);
		}
	}

	private void removeSpecialAbility(EnsoulOption option)
	{
		try
		{
			int itemObjectId = ObjectId;
			int optionId = option.getId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ItemSpecialAbilities.Where(r => r.ItemId == itemObjectId && r.OptionId == optionId).ExecuteDelete();

			Skill skill = option.getSkill();
			if (skill != null)
			{
				Player player = getActingPlayer();
				if (player != null)
				{
					player.removeSkill(skill.getId());
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not remove special ability for " + this + ": " + e);
		}
	}

	private void applySpecialAbility(EnsoulOption option)
	{
		if (option == null)
		{
			return;
		}

		Skill skill = option.getSkill();
		if (skill != null)
		{
			Player player = getActingPlayer();
			if (player != null && player.getSkillLevel(skill.getId()) != skill.getLevel())
			{
				player.addSkill(skill, false);
			}
		}
	}

	private void clearSpecialAbility(EnsoulOption option)
	{
		if (option == null)
		{
			return;
		}

		Skill skill = option.getSkill();
		if (skill != null)
		{
			Player player = getActingPlayer();
			if (player != null)
			{
				player.removeSkill(skill, false, true);
			}
		}
	}

	private void restoreSpecialAbilities()
	{
		try
		{
			int itemObjectId = ObjectId;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = ctx.ItemSpecialAbilities.Where(r => r.ItemId == itemObjectId).OrderBy(r => r.Position);
			foreach (var record in query)
			{
				int optionId = record.OptionId;
				int type = record.Type;
				int position = record.Position;
				EnsoulOption option = EnsoulData.getInstance().getOption(optionId);
				if (option != null)
				{
					addSpecialAbility(option, position, type, false);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not restore special abilities for " + this + ": " + e);
		}
	}

	public void updateSpecialAbilities()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			updateSpecialAbilities(ctx);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Item could not update item special abilities: " + e);
		}
	}

	private void updateSpecialAbilities(GameServerDbContext ctx)
	{
		try
		{
			static DbItemSpecialAbility GetOrCreateSpecialAbility(GameServerDbContext ctx, int itemObjectId, int optionId)
			{
				var record =
					ctx.ItemSpecialAbilities.SingleOrDefault(r => r.ItemId == itemObjectId && r.OptionId == optionId);

				if (record is null)
				{
					record = new DbItemSpecialAbility();
					record.ItemId = itemObjectId;
					record.OptionId = optionId;
					ctx.ItemSpecialAbilities.Add(record);
				}

				return record;
			}

			int itemObjectId = ObjectId;
			for (int i = 0; i < _ensoulOptions.Length; i++)
			{
				if (_ensoulOptions[i] == null)
				{
					continue;
				}

				DbItemSpecialAbility record = GetOrCreateSpecialAbility(ctx, itemObjectId, _ensoulOptions[i].getId());
				record.Type = 1;
				record.Position = (byte)i;
			}

			for (int i = 0; i < _ensoulSpecialOptions.Length; i++)
			{
				if (_ensoulSpecialOptions[i] == null)
				{
					continue;
				}

				DbItemSpecialAbility record =
					GetOrCreateSpecialAbility(ctx, itemObjectId, _ensoulSpecialOptions[i].getId());

				record.Type = 2;
				record.Position = (byte)i;
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Item could not update item special abilities: " + e);
		}
	}

	/**
	 * Clears all the enchant bonuses if item is enchanted and containing bonuses for enchant value.
	 */
	public void clearEnchantStats()
	{
		Player player = getActingPlayer();
		if (player == null)
		{
			_enchantOptions.Clear();
			return;
		}

		foreach (Options.Options op in _enchantOptions)
		{
			op.remove(player);
		}
		_enchantOptions.Clear();
	}

	/**
	 * Clears and applies all the enchant bonuses if item is enchanted and containing bonuses for enchant value.
	 */
	public void applyEnchantStats()
	{
		Player player = getActingPlayer();
		ImmutableArray<int> enchantOptions = getEnchantOptions();
		if (!isEquipped() || player == null || enchantOptions.IsDefaultOrEmpty)
		{
			return;
		}

		foreach (int id in enchantOptions)
		{
			Options.Options? options = OptionData.getInstance().getOptions(id);
			if (options != null)
			{
				options.apply(player);
				_enchantOptions.Add(options);
			}
			else if (id != 0)
			{
				LOGGER.Info("Item applyEnchantStats could not find option " + id + " " + this + " " + player);
			}
		}
	}

	public override void setHeading(int heading)
	{
	}

	public void stopAllTasks()
	{
		ItemLifeTimeTaskManager.getInstance().remove(this);
		ItemAppearanceTaskManager.getInstance().remove(this);
	}

	public ItemVariables getVariables()
	{
		ItemVariables vars = getScript<ItemVariables>();
		return vars != null ? vars : addScript(new ItemVariables(ObjectId));
	}

	public int getVisualId()
	{
		int visualId = getVariables().getInt(ItemVariables.VISUAL_ID, 0);
		if (visualId > 0)
		{
			int appearanceStoneId = getVariables().getInt(ItemVariables.VISUAL_APPEARANCE_STONE_ID, 0);
			if (appearanceStoneId > 0)
			{
				AppearanceStone stone = AppearanceItemData.getInstance().getStone(appearanceStoneId);
				if (stone != null)
				{
					Player player = getActingPlayer();
					if (player != null)
					{
						if (!stone.getRaces().isEmpty() && !stone.getRaces().Contains(player.getRace()))
						{
							return 0;
						}
						if (!stone.getRacesNot().isEmpty() && stone.getRacesNot().Contains(player.getRace()))
						{
							return 0;
						}
					}
				}
			}
		}
		return visualId;
	}

	public void setVisualId(int visualId)
	{
		setVisualId(visualId, true);
	}

	public void setVisualId(int visualId, bool announce)
	{
		getVariables().set(ItemVariables.VISUAL_ID, visualId);

		// When removed, cancel existing lifetime task.
		if (visualId == 0)
		{
			ItemAppearanceTaskManager.getInstance().remove(this);
			onVisualLifeTimeEnd(announce);
		}
	}

	public int getAppearanceStoneId()
	{
		return getVariables().getInt(ItemVariables.VISUAL_APPEARANCE_STONE_ID, 0);
	}

	public DateTime? getVisualLifeTime()
	{
		DateTime time = getVariables().getDateTime(ItemVariables.VISUAL_APPEARANCE_LIFE_TIME, DateTime.MinValue);
		return time == DateTime.MinValue ? null : time;
	}

	public void scheduleVisualLifeTime()
	{
		ItemAppearanceTaskManager.getInstance().remove(this);
		if (getVisualLifeTime() != null)
		{
			DateTime endTime = getVisualLifeTime().Value;
			if (endTime - DateTime.UtcNow > TimeSpan.Zero)
			{
				ItemAppearanceTaskManager.getInstance().add(this, endTime);
			}
			else
			{
				onVisualLifeTimeEnd();
			}
		}
	}

	public void onVisualLifeTimeEnd()
	{
		onVisualLifeTimeEnd(true);
	}

	public void onVisualLifeTimeEnd(bool announce)
	{
		removeVisualSetSkills();

		ItemVariables vars = getVariables();
		vars.remove(ItemVariables.VISUAL_ID);
		vars.remove(ItemVariables.VISUAL_APPEARANCE_STONE_ID);
		vars.remove(ItemVariables.VISUAL_APPEARANCE_LIFE_TIME);
		vars.storeMe();

		Player player = getActingPlayer();
		if (player != null)
		{
			InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(this, ItemChangeType.MODIFIED));
			player.broadcastUserInfo(UserInfoType.APPAREANCE);
			player.sendInventoryUpdate(iu);

			if (announce)
			{
				if (isEnchanted())
				{
					var sm = new SystemMessagePacket(SystemMessageId.S1_S2_THE_ITEM_S_TEMPORARY_APPEARANCE_HAS_BEEN_RESET);
					sm.Params.addInt(_enchantLevel).addItemName(this);
					player.sendPacket(sm);
				}
				else
				{
					var sm = new SystemMessagePacket(SystemMessageId.S1_THE_ITEM_S_TEMPORARY_APPEARANCE_HAS_BEEN_RESET);
					sm.Params.addItemName(this);
					player.sendPacket(sm);
				}
			}
		}
	}

	public bool isBlessed()
	{
		return _isBlessed;
	}

	public void setBlessed(bool blessed)
	{
		_isBlessed = blessed;

		ItemVariables vars = getVariables();
		if (!blessed)
		{
			vars.remove(ItemVariables.BLESSED);
		}
		else
		{
			vars.set(ItemVariables.BLESSED, true);
		}
		vars.storeMe();
	}

	public void removeVisualSetSkills()
	{
		if (!isEquipped())
		{
			return;
		}

		int appearanceStoneId = getAppearanceStoneId();
		if (appearanceStoneId > 0)
		{
			AppearanceStone stone = AppearanceItemData.getInstance().getStone(appearanceStoneId);
			if (stone != null && stone.getType() == AppearanceType.FIXED)
			{
				Player player = getActingPlayer();
				if (player != null)
				{
					bool update = false;
					foreach (ArmorSet armorSet in ArmorSetData.getInstance().getSets(stone.getVisualId()))
					{
						if (armorSet.getPiecesCount(player, x => x.getVisualId()) - 1 /* not removed yet */ < armorSet.getMinimumPieces())
						{
							foreach (ArmorsetSkillHolder holder in armorSet.getSkills())
							{
								Skill skill = holder.getSkill();
								if (skill != null)
								{
									player.removeSkill(skill, false, skill.isPassive());
									update = true;
								}
							}
						}
					}

					if (update)
					{
						player.sendSkillList();
					}
				}
			}
		}
	}

	public void applyVisualSetSkills()
	{
		if (!isEquipped())
		{
			return;
		}

		int appearanceStoneId = getAppearanceStoneId();
		if (appearanceStoneId > 0)
		{
			AppearanceStone stone = AppearanceItemData.getInstance().getStone(appearanceStoneId);
			if (stone != null && stone.getType() == AppearanceType.FIXED)
			{
				Player player = getActingPlayer();
				if (player != null)
				{
					bool update = false;
					bool updateTimeStamp = false;
					foreach (ArmorSet armorSet in ArmorSetData.getInstance().getSets(stone.getVisualId()))
					{
						if (armorSet.getPiecesCount(player, x => x.getVisualId()) >= armorSet.getMinimumPieces())
						{
							foreach (ArmorsetSkillHolder holder in armorSet.getSkills())
							{
								if (player.getSkillLevel(holder.getSkillId()) >= holder.getSkillLevel())
								{
									continue;
								}

								Skill skill = holder.getSkill();
								if (skill == null || (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, player, player)))
								{
									continue;
								}

								player.addSkill(skill, false);
								update = true;

								if (skill.isActive())
								{
									if (!player.hasSkillReuse(skill.getReuseHashCode()))
									{
										TimeSpan equipDelay = getEquipReuseDelay();
										if (equipDelay > TimeSpan.Zero)
										{
											player.addTimeStamp(skill, equipDelay);
											player.disableSkill(skill, equipDelay);
										}
									}

									// Active, non offensive, skills start with reuse on equip.
									if (!skill.isBad() && !skill.isTransformation() && Config.ARMOR_SET_EQUIP_ACTIVE_SKILL_REUSE > 0 && player.hasEnteredWorld())
									{
										player.addTimeStamp(skill,
											skill.getReuseDelay() > TimeSpan.Zero
												? skill.getReuseDelay()
												: TimeSpan.FromMilliseconds(Config.ARMOR_SET_EQUIP_ACTIVE_SKILL_REUSE));
									}

									updateTimeStamp = true;
								}
							}
						}
					}

					if (updateTimeStamp)
					{
						player.sendPacket(new SkillCoolTimePacket(player));
					}

					if (update)
					{
						player.sendSkillList();
					}
				}
			}
		}
	}

	/**
	 * Returns the item in String format
	 * @return String
	 */
	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append(_itemTemplate);
		sb.Append('[');
		sb.Append(ObjectId);
		sb.Append(']');
		return sb.ToString();
	}
}