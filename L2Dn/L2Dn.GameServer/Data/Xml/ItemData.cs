using System.Text;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class serves as a container for all item templates in the game.
 */
public class ItemData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemData));
	private static readonly Logger LOGGER_ITEMS = LogManager.GetLogger("item");
	
	private ItemTemplate[] _allTemplates;
	private readonly Map<int, EtcItem> _etcItems = new();
	private readonly Map<int, Armor> _armors = new();
	private readonly Map<int, Weapon> _weapons = new();
	private readonly List<File> _itemFiles = new();
	
	public static readonly Map<String, long> SLOTS = new();
	static
	{
		SLOTS.put("shirt", (long) ItemTemplate.SLOT_UNDERWEAR);
		SLOTS.put("lbracelet", (long) ItemTemplate.SLOT_L_BRACELET);
		SLOTS.put("rbracelet", (long) ItemTemplate.SLOT_R_BRACELET);
		SLOTS.put("talisman", (long) ItemTemplate.SLOT_DECO);
		SLOTS.put("chest", (long) ItemTemplate.SLOT_CHEST);
		SLOTS.put("fullarmor", (long) ItemTemplate.SLOT_FULL_ARMOR);
		SLOTS.put("head", (long) ItemTemplate.SLOT_HEAD);
		SLOTS.put("hair", (long) ItemTemplate.SLOT_HAIR);
		SLOTS.put("hairall", (long) ItemTemplate.SLOT_HAIRALL);
		SLOTS.put("underwear", (long) ItemTemplate.SLOT_UNDERWEAR);
		SLOTS.put("back", (long) ItemTemplate.SLOT_BACK);
		SLOTS.put("neck", (long) ItemTemplate.SLOT_NECK);
		SLOTS.put("legs", (long) ItemTemplate.SLOT_LEGS);
		SLOTS.put("feet", (long) ItemTemplate.SLOT_FEET);
		SLOTS.put("gloves", (long) ItemTemplate.SLOT_GLOVES);
		SLOTS.put("chest,legs", (long) ItemTemplate.SLOT_CHEST | ItemTemplate.SLOT_LEGS);
		SLOTS.put("belt", (long) ItemTemplate.SLOT_BELT);
		SLOTS.put("rhand", (long) ItemTemplate.SLOT_R_HAND);
		SLOTS.put("lhand", (long) ItemTemplate.SLOT_L_HAND);
		SLOTS.put("lrhand", (long) ItemTemplate.SLOT_LR_HAND);
		SLOTS.put("rear;lear", (long) ItemTemplate.SLOT_R_EAR | ItemTemplate.SLOT_L_EAR);
		SLOTS.put("rfinger;lfinger", (long) ItemTemplate.SLOT_R_FINGER | ItemTemplate.SLOT_L_FINGER);
		SLOTS.put("wolf", (long) ItemTemplate.SLOT_WOLF);
		SLOTS.put("greatwolf", (long) ItemTemplate.SLOT_GREATWOLF);
		SLOTS.put("hatchling", (long) ItemTemplate.SLOT_HATCHLING);
		SLOTS.put("strider", (long) ItemTemplate.SLOT_STRIDER);
		SLOTS.put("babypet", (long) ItemTemplate.SLOT_BABYPET);
		SLOTS.put("brooch", (long) ItemTemplate.SLOT_BROOCH);
		SLOTS.put("brooch_jewel", (long) ItemTemplate.SLOT_BROOCH_JEWEL);
		SLOTS.put("agathion", ItemTemplate.SLOT_AGATHION);
		SLOTS.put("artifactbook", ItemTemplate.SLOT_ARTIFACT_BOOK);
		SLOTS.put("artifact", ItemTemplate.SLOT_ARTIFACT);
		SLOTS.put("none", (long) ItemTemplate.SLOT_NONE);
		
		// retail compatibility
		SLOTS.put("onepiece", (long) ItemTemplate.SLOT_FULL_ARMOR);
		SLOTS.put("hair2", (long) ItemTemplate.SLOT_HAIR2);
		SLOTS.put("dhair", (long) ItemTemplate.SLOT_HAIRALL);
		SLOTS.put("alldress", (long) ItemTemplate.SLOT_ALLDRESS);
		SLOTS.put("deco1", (long) ItemTemplate.SLOT_DECO);
		SLOTS.put("waist", (long) ItemTemplate.SLOT_BELT);
	}
	
	protected ItemData()
	{
		processDirectory("data/stats/items", _itemFiles);
		if (Config.CUSTOM_ITEMS_LOAD)
		{
			processDirectory("data/stats/items/custom", _itemFiles);
		}
		
		load();
	}
	
	private void processDirectory(String dirName, List<File> list)
	{
		File dir = new File(Config.DATAPACK_ROOT, dirName);
		if (!dir.exists())
		{
			LOGGER.Warn("Dir " + dir.getAbsolutePath() + " does not exist.");
			return;
		}
		File[] files = dir.listFiles(new XMLFilter());
		foreach (File file in files)
		{
			list.add(file);
		}
	}
	
	private ICollection<ItemTemplate> loadItems()
	{
		Collection<ItemTemplate> list = ConcurrentHashMap.newKeySet();
		if (Config.THREADS_FOR_LOADING)
		{
			Collection<ScheduledFuture<?>> jobs = ConcurrentHashMap.newKeySet();
			foreach (File file in _itemFiles)
			{
				jobs.add(ThreadPool.schedule(() =>
				{
					DocumentItem document = new DocumentItem(file);
					document.parse();
					list.addAll(document.getItemList());
				}, 0));
			}
			while (!jobs.isEmpty())
			{
				foreach (ScheduledFuture<?> job in jobs)
				{
					if ((job == null) || job.isDone() || job.isCancelled())
					{
						jobs.remove(job);
					}
				}
			}
		}
		else
		{
			foreach (File file in _itemFiles)
			{
				DocumentItem document = new DocumentItem(file);
				document.parse();
				list.addAll(document.getItemList());
			}
		}
		return list;
	}
	
	private void load()
	{
		int highest = 0;
		_armors.clear();
		_etcItems.clear();
		_weapons.clear();
		foreach (ItemTemplate item in loadItems())
		{
			if (highest < item.getId())
			{
				highest = item.getId();
			}
			if (item is EtcItem)
			{
				_etcItems.put(item.getId(), (EtcItem) item);
				
				if ((item.getItemType() == EtcItemType.ARROW) || (item.getItemType() == EtcItemType.BOLT) || (item.getItemType() == EtcItemType.ELEMENTAL_ORB))
				{
					List<ItemSkillHolder> skills = item.getAllSkills();
					if (skills != null)
					{
						AmmunitionSkillList.add(skills);
					}
				}
			}
			else if (item is Armor)
			{
				_armors.put(item.getId(), (Armor) item);
			}
			else
			{
				_weapons.put(item.getId(), (Weapon) item);
			}
		}
		buildFastLookupTable(highest);
		LOGGER.Info(GetType().Name + ": Loaded " + _etcItems.size() + " Etc Items");
		LOGGER.Info(GetType().Name + ": Loaded " + _armors.size() + " Armor Items");
		LOGGER.Info(GetType().Name + ": Loaded " + _weapons.size() + " Weapon Items");
		LOGGER.Info(GetType().Name + ": Loaded " + (_etcItems.size() + _armors.size() + _weapons.size()) + " Items in total.");
	}
	
	/**
	 * Builds a variable in which all items are putting in in function of their ID.
	 * @param size
	 */
	private void buildFastLookupTable(int size)
	{
		// Create a FastLookUp Table called _allTemplates of size : value of the highest item ID
		LOGGER.Info(GetType().Name + ": Highest item id used: " + size);
		_allTemplates = new ItemTemplate[size + 1];
		
		// Insert armor item in Fast Look Up Table
		foreach (Armor item in _armors.values())
		{
			_allTemplates[item.getId()] = item;
		}
		
		// Insert weapon item in Fast Look Up Table
		foreach (Weapon item in _weapons.values())
		{
			_allTemplates[item.getId()] = item;
		}
		
		// Insert etcItem item in Fast Look Up Table
		foreach (EtcItem item in _etcItems.values())
		{
			_allTemplates[item.getId()] = item;
		}
	}
	
	/**
	 * Returns the item corresponding to the item ID
	 * @param id : int designating the item
	 * @return Item
	 */
	public ItemTemplate getTemplate(int id)
	{
		if ((id >= _allTemplates.Length) || (id < 0))
		{
			return null;
		}
		return _allTemplates[id];
	}
	
	/**
	 * Create the Item corresponding to the Item Identifier and quantitiy add logs the activity. <b><u>Actions</u>:</b>
	 * <li>Create and Init the Item corresponding to the Item Identifier and quantity</li>
	 * <li>Add the Item object to _allObjects of L2world</li>
	 * <li>Logs Item creation according to log settings</li><br>
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item Identifier of the item to be created
	 * @param count : int Quantity of items to be created for stackable items
	 * @param actor : Creature requesting the item creation
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item
	 */
	public Item createItem(String process, int itemId, long count, Creature actor, Object reference)
	{
		// Create and Init the Item corresponding to the Item Identifier
		Item item = new Item(IdManager.getInstance().getNextId(), itemId);
		if (process.equalsIgnoreCase("loot") && !Config.AUTO_LOOT_ITEM_IDS.Contains(itemId))
		{
			ScheduledFuture<?> itemLootShedule;
			if ((reference is Attackable) && ((Attackable) reference).isRaid()) // loot privilege for raids
			{
				Attackable raid = (Attackable) reference;
				// if in CommandChannel and was killing a World/RaidBoss
				if ((raid.getFirstCommandChannelAttacked() != null) && !Config.AUTO_LOOT_RAIDS)
				{
					item.setOwnerId(raid.getFirstCommandChannelAttacked().getLeaderObjectId());
					itemLootShedule = ThreadPool.schedule(new ResetOwner(item), Config.LOOT_RAIDS_PRIVILEGE_INTERVAL);
					item.setItemLootShedule(itemLootShedule);
				}
			}
			else if (!Config.AUTO_LOOT || ((reference is EventMonster) && ((EventMonster) reference).eventDropOnGround()))
			{
				item.setOwnerId(actor.getObjectId());
				itemLootShedule = ThreadPool.schedule(new ResetOwner(item), 15000);
				item.setItemLootShedule(itemLootShedule);
			}
		}
		
		// Add the Item object to _allObjects of L2world
		World.getInstance().addObject(item);
		
		// Set Item parameters
		if (item.isStackable() && (count > 1))
		{
			item.setCount(count);
		}
		
		if ((Config.LOG_ITEMS && !process.equals("Reset") && ((!Config.LOG_ITEMS_SMALL_LOG) && (!Config.LOG_ITEMS_IDS_ONLY))) || (Config.LOG_ITEMS_SMALL_LOG && (item.isEquipable() || (item.getId() == ADENA_ID))) || (Config.LOG_ITEMS_IDS_ONLY && Config.LOG_ITEMS_IDS_LIST.contains(item.getId())))
		{
			if (item.getEnchantLevel() > 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("CREATE:");
				sb.Append(process);
				sb.Append(", item ");
				sb.Append(item.getObjectId());
				sb.Append(":+");
				sb.Append(item.getEnchantLevel());
				sb.Append(" ");
				sb.Append(item.getTemplate().getName());
				sb.Append("(");
				sb.Append(item.getCount());
				sb.Append("), ");
				sb.Append(actor);
				sb.Append(", ");
				sb.Append(reference);
				LOGGER_ITEMS.Info(sb.ToString());
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("CREATE:");
				sb.Append(process);
				sb.Append(", item ");
				sb.Append(item.getObjectId());
				sb.Append(":");
				sb.Append(item.getTemplate().getName());
				sb.Append("(");
				sb.Append(item.getCount());
				sb.Append("), ");
				sb.Append(actor);
				sb.Append(", ");
				sb.Append(reference);
				LOGGER_ITEMS.Info(sb.ToString());
			}
		}
		
		if ((actor != null) && actor.isGM() && Config.GMAUDIT)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(process);
			sb.Append("(id: ");
			sb.Append(itemId);
			sb.Append(" count: ");
			sb.Append(count);
			sb.Append(" name: ");
			sb.Append(item.getItemName());
			sb.Append(" objId: ");
			sb.Append(item.getObjectId());
			sb.Append(")");
			
			String targetName = (actor.getTarget() != null ? actor.getTarget().getName() : "no-target");
			
			String referenceName = "no-reference";
			if (reference is WorldObject)
			{
				referenceName = (((WorldObject) reference).getName() != null ? ((WorldObject) reference).getName() : "no-name");
			}
			else if (reference is String)
			{
				referenceName = (String) reference;
			}
			
			GMAudit.auditGMAction(actor.ToString(), sb.ToString(), targetName, StringUtil.concat("Object referencing this action is: ", referenceName));
		}
		
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_ITEM_CREATE, item.getTemplate()))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnItemCreate(process, item, actor, reference), item.getTemplate());
		}
		
		return item;
	}
	
	public Item createItem(String process, int itemId, long count, Player actor)
	{
		return createItem(process, itemId, count, actor, null);
	}
	
	/**
	 * Destroys the Item.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Sets Item parameters to be unusable</li>
	 * <li>Removes the Item object to _allObjects of L2world</li>
	 * <li>Logs Item deletion according to log settings</li>
	 * </ul>
	 * @param process a string identifier of process triggering this action.
	 * @param item the item instance to be destroyed.
	 * @param actor the player requesting the item destroy.
	 * @param reference the object referencing current action like NPC selling item or previous item in transformation.
	 */
	public void destroyItem(String process, Item item, Player actor, Object reference)
	{
		lock (item)
		{
			long old = item.getCount();
			item.setCount(0);
			item.setOwnerId(0);
			item.setItemLocation(ItemLocation.VOID);
			item.setLastChange(Item.REMOVED);
			
			World.getInstance().removeObject(item);
			IdManager.getInstance().releaseId(item.getObjectId());
			
			if ((Config.LOG_ITEMS && ((!Config.LOG_ITEMS_SMALL_LOG) && (!Config.LOG_ITEMS_IDS_ONLY))) || (Config.LOG_ITEMS_SMALL_LOG && (item.isEquipable() || (item.getId() == ADENA_ID))) || (Config.LOG_ITEMS_IDS_ONLY && Config.LOG_ITEMS_IDS_LIST.contains(item.getId())))
			{
				if (item.getEnchantLevel() > 0)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("DELETE:");
					sb.Append(process);
					sb.Append(", item ");
					sb.Append(item.getObjectId());
					sb.Append(":+");
					sb.Append(item.getEnchantLevel());
					sb.Append(" ");
					sb.Append(item.getTemplate().getName());
					sb.Append("(");
					sb.Append(item.getCount());
					sb.Append("), PrevCount(");
					sb.Append(old);
					sb.Append("), ");
					sb.Append(actor);
					sb.Append(", ");
					sb.Append(reference);
					LOGGER_ITEMS.Info(sb.ToString());
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("DELETE:");
					sb.Append(process);
					sb.Append(", item ");
					sb.Append(item.getObjectId());
					sb.Append(":");
					sb.Append(item.getTemplate().getName());
					sb.Append("(");
					sb.Append(item.getCount());
					sb.Append("), PrevCount(");
					sb.Append(old);
					sb.Append("), ");
					sb.Append(actor);
					sb.Append(", ");
					sb.Append(reference);
					LOGGER_ITEMS.Info(sb.ToString());
				}
			}
			
			if ((actor != null) && actor.isGM() && Config.GMAUDIT)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(process);
				sb.Append("(id: ");
				sb.Append(item.getId());
				sb.Append(" count: ");
				sb.Append(item.getCount());
				sb.Append(" itemObjId: ");
				sb.Append(item.getObjectId());
				sb.Append(")");
				
				String targetName = (actor.getTarget() != null ? actor.getTarget().getName() : "no-target");
				
				String referenceName = "no-reference";
				if (reference is WorldObject)
				{
					referenceName = (((WorldObject) reference).getName() != null ? ((WorldObject) reference).getName() : "no-name");
				}
				else if (reference is String)
				{
					referenceName = (String) reference;
				}
				
				GMAudit.auditGMAction(actor.ToString(), sb.ToString(), targetName, StringUtil.concat("Object referencing this action is: ", referenceName));
			}
			
			// if it's a pet control item, delete the pet as well
			if (item.getTemplate().isPetItem())
			{
				try 
				{
					Connection con = DatabaseFactory.getConnection();
					PreparedStatement statement = con.prepareStatement("DELETE FROM pets WHERE item_obj_id=?");
					// Delete the pet in db
					statement.setInt(1, item.getObjectId());
					statement.execute();
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Could not delete pet objectid:", e);
				}
			}
		}
	}
	
	public void reload()
	{
		load();
		EnchantItemHPBonusData.getInstance().load();
	}
	
	public Set<int> getAllArmorsId()
	{
		return _armors.Keys;
	}
	
	public ICollection<Armor> getAllArmors()
	{
		return _armors.values();
	}
	
	public Set<int> getAllWeaponsId()
	{
		return _weapons.Keys;
	}
	
	public ICollection<Weapon> getAllWeapons()
	{
		return _weapons.values();
	}
	
	public Set<int> getAllEtcItemsId()
	{
		return _etcItems.Keys;
	}
	
	public ICollection<EtcItem> getAllEtcItems()
	{
		return _etcItems.values();
	}
	
	public ItemTemplate[] getAllItems()
	{
		return _allTemplates;
	}
	
	public int getArraySize()
	{
		return _allTemplates.Length;
	}
	
	protected class ResetOwner : Runnable
	{
		Item _item;
		
		public ResetOwner(Item item)
		{
			_item = item;
		}
		
		public void run()
		{
			// Set owner id to 0 only when location is VOID.
			if (_item.getItemLocation() == ItemLocation.VOID)
			{
				_item.setOwnerId(0);
			}
			_item.setItemLootShedule(null);
		}
	}
	
	public static ItemData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ItemData INSTANCE = new();
	}
}