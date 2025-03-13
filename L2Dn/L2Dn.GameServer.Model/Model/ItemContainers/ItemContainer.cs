using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.ItemContainers;

public abstract class ItemContainer
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemContainer));

	protected readonly Set<Item> _items = [];

	protected ItemContainer()
	{
	}

	public abstract Creature? getOwner();

	public abstract ItemLocation getBaseLocation();

	public virtual string getName()
	{
		return "ItemContainer";
	}

	/**
	 * @return int the owner object Id
	 */
	public virtual int getOwnerId()
    {
        Creature? owner = getOwner();
		return owner == null ? 0 : owner.ObjectId;
	}

	/**
	 * @return the quantity of items in the inventory
	 */
	public int getSize()
	{
		return _items.size();
	}

	/**
	 * Gets the items in inventory.
	 * @return the items in inventory.
	 */
	public virtual ICollection<Item> getItems()
	{
		return _items;
	}

	/**
	 * @param itemId the item Id
	 * @return the item from inventory by itemId
	 */
	public Item? getItemByItemId(int itemId)
	{
		foreach (Item item in _items)
		{
			if (item.getId() == itemId)
			{
				return item;
			}
		}
		return null;
	}

	/**
	 * @param itemId the item Id
	 * @return the items list from inventory by using its itemId
	 */
	public ICollection<Item> getAllItemsByItemId(int itemId)
	{
		List<Item> result = new();
		foreach (Item item in _items)
		{
			if (itemId == item.getId())
			{
				result.Add(item);
			}
		}
		return result;
	}

	/**
	 * @param objectId the item object Id
	 * @return item from inventory by objectId
	 */
	public Item? getItemByObjectId(int objectId)
	{
		foreach (Item item in _items)
		{
			if (objectId == item.ObjectId)
			{
				return item;
			}
		}
		return null;
	}

	/**
	 * Gets the inventory item count by item Id and enchant level including equipped items.
	 * @param itemId the item Id
	 * @param enchantLevel the item enchant level, use -1 to match any enchant level
	 * @return the inventory item count
	 */
	public long getInventoryItemCount(int itemId, int enchantLevel)
	{
		return getInventoryItemCount(itemId, enchantLevel, true);
	}

	/**
	 * Gets the inventory item count by item Id and enchant level, may include equipped items.
	 * @param itemId the item Id
	 * @param enchantLevel the item enchant level, use -1 to match any enchant level
	 * @param includeEquipped if {@code true} includes equipped items in the result
	 * @return the inventory item count
	 */
	public long getInventoryItemCount(int itemId, int enchantLevel, bool includeEquipped)
	{
		long count = 0;
		foreach (Item item in _items)
		{
			if (item.getId() == itemId && (item.getEnchantLevel() == enchantLevel || enchantLevel < 0) && (includeEquipped || !item.isEquipped()))
			{
				if (item.isStackable())
				{
					return item.getCount();
				}
				count++;
			}
		}
		return count;
	}

	/**
	 * @return true if player got item for self resurrection
	 */
	public bool haveItemForSelfResurrection()
	{
		foreach (Item item in _items)
		{
			if (item.getTemplate().isAllowSelfResurrection())
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * Adds item to inventory
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be added
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public virtual Item? addItem(string process, Item item, Player? actor, object? reference)
	{
		Item newItem = item;
		Item? olditem = getItemByItemId(newItem.getId());

		// If stackable item is found in inventory just add to current quantity
		if (olditem != null && olditem.isStackable())
		{
			long count = newItem.getCount();
			olditem.changeCount(process, count, actor, reference);
			olditem.setLastChange(ItemChangeType.MODIFIED);

			// And destroys the item
			ItemData.getInstance().destroyItem(process, newItem, actor, reference);
			newItem.updateDatabase();
			newItem = olditem;
		}
		else // If item has not been found in inventory, create new one
		{
			newItem.setOwnerId(process, getOwnerId(), actor, reference);
			newItem.setItemLocation(getBaseLocation());
			newItem.setLastChange(ItemChangeType.ADDED);

			// Add item in inventory
			addItem(newItem);
		}

		refreshWeight();
		return newItem;
	}

	/**
	 * Adds item to inventory
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item Identifier of the item to be added
	 * @param count : long Quantity of items to be added
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public virtual Item? addItem(string process, int itemId, long count, Player? actor, object? reference)
	{
		Item? item = getItemByItemId(itemId);

		// If stackable item is found in inventory just add to current quantity
		if (item != null && item.isStackable())
		{
			item.changeCount(process, count, actor, reference);
			item.setLastChange(ItemChangeType.MODIFIED);
		}
		else // If item has not been found in inventory, create new one
		{
			List<ItemInfo> items = new List<ItemInfo>();
			for (int i = 0; i < count; i++)
			{
				ItemTemplate? template = ItemData.getInstance().getTemplate(itemId);
				if (template == null)
				{
					LOGGER.Warn("Invalid ItemId (" + itemId + ") requested by " + (actor != null ? actor : process));
					return null;
				}

				item = ItemData.getInstance().createItem(process, itemId, template.isStackable() ? count : 1, actor, reference);
				item.setOwnerId(getOwnerId());
				item.setItemLocation(getBaseLocation());
				item.setLastChange(ItemChangeType.ADDED);

				// Add item in inventory
				addItem(item);

				// Add additional items to InventoryUpdate.
				if (count > 1 && i < count - 1)
				{
					items.Add(new ItemInfo(item, ItemChangeType.ADDED));
				}

				// If stackable, end loop as entire count is included in 1 instance of item
				if (template.isStackable() || !Config.MULTIPLE_ITEM_DROP)
				{
					break;
				}
			}

			// If additional items where created send InventoryUpdate.
			if (actor != null && count > 1 && item != null && !item.isStackable() && item.getItemLocation() == ItemLocation.INVENTORY)
			{
				InventoryUpdatePacket iu = new InventoryUpdatePacket(items);
				actor.sendInventoryUpdate(iu);
			}
		}

		refreshWeight();
		return item;
	}

	/**
	 * Transfers item to another inventory
	 * @param process string Identifier of process triggering this action
	 * @param objectId Item Identifier of the item to be transfered
	 * @param countValue Quantity of items to be transfered
	 * @param target the item container where the item will be moved.
	 * @param actor Player requesting the item transfer
	 * @param reference Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public virtual Item? transferItem(string process, int objectId, long countValue, ItemContainer target, Player? actor, object? reference)
	{
		if (target == null)
		{
			return null;
		}

		Item? sourceitem = getItemByObjectId(objectId);
		if (sourceitem == null)
		{
			return null;
		}

		Item? targetitem = sourceitem.isStackable() ? target.getItemByItemId(sourceitem.getId()) : null;
		lock (sourceitem)
		{
			// check if this item still present in this container
			if (getItemByObjectId(objectId) != sourceitem)
			{
				return null;
			}

			// Check if requested quantity is available
			long count = countValue;
			if (count > sourceitem.getCount())
			{
				count = sourceitem.getCount();
			}

			// If possible, move entire item object
			if (sourceitem.getCount() == count && targetitem == null && !sourceitem.isStackable())
			{
				removeItem(sourceitem);
				target.addItem(process, sourceitem, actor, reference);
				targetitem = sourceitem;
			}
			else
			{
				if (sourceitem.getCount() > count) // If possible, only update counts
				{
					sourceitem.changeCount(process, -count, actor, reference);
				}
				else // Otherwise destroy old item
				{
					removeItem(sourceitem);
					ItemData.getInstance().destroyItem(process, sourceitem, actor, reference);
				}

				if (targetitem != null) // If possible, only update counts
				{
					targetitem.changeCount(process, count, actor, reference);
				}
				else // Otherwise add new item
				{
					targetitem = target.addItem(process, sourceitem.getId(), count, actor, reference);
				}
			}

			// Updates database
			sourceitem.updateDatabase(true);
			if (targetitem != sourceitem && targetitem != null)
			{
				targetitem.updateDatabase();
			}
			if (sourceitem.isAugmented() && actor != null)
			{
				sourceitem.getAugmentation()?.removeBonus(actor);
			}
			refreshWeight();
			target.refreshWeight();
		}
		return targetitem;
	}

	/**
	 * Detaches the item from this item container so it can be used as a single instance.
	 * @param process string Identifier of process triggering this action
	 * @param item the item instance to be detached
	 * @param count the count of items to be detached
	 * @param newLocation the new item location
	 * @param actor Player requesting the item detach
	 * @param reference Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return the detached item instance if operation completes successfully, {@code null} if the item does not exist in this container anymore or item count is not available
	 */
	public virtual Item? detachItem(string process, Item item, long count, ItemLocation newLocation, Player actor, object? reference)
	{
		if (item == null)
		{
			return null;
		}

		lock (item)
		{
			if (!_items.Contains(item))
			{
				return null;
			}

			if (count > item.getCount())
			{
				return null;
			}

			if (count == item.getCount())
			{
				removeItem(item);

				item.setItemLocation(newLocation);
				item.updateDatabase(true);
				refreshWeight();

				return item;
			}

			item.changeCount(process, -count, actor, reference);
			item.updateDatabase(true);

			Item newItem = ItemData.getInstance().createItem(process, item.getId(), count, actor, reference);
			newItem.setOwnerId(getOwnerId());
			newItem.setItemLocation(newLocation);
			newItem.updateDatabase(true);
			refreshWeight();

			return newItem;
		}
	}

	/**
	 * Detaches the item from this item container so it can be used as a single instance.
	 * @param process string Identifier of process triggering this action
	 * @param itemObjectId the item object id to be detached
	 * @param count the count of items to be detached
	 * @param newLocation the new item location
	 * @param actor Player requesting the item detach
	 * @param reference Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return the detached item instance if operation completes successfully, {@code null} if the item does not exist in this container anymore or item count is not available
	 */
	public Item? detachItem(string process, int itemObjectId, long count, ItemLocation newLocation, Player actor, object reference)
	{
		Item? item = getItemByObjectId(itemObjectId);
		if (item == null)
		{
			return null;
		}
		return detachItem(process, item, count, newLocation, actor, reference);
	}

	/**
	 * Destroy item from inventory and updates database
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be destroyed
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public virtual Item? destroyItem(string process, Item item, Player? actor, object? reference)
	{
		return destroyItem(process, item, item.getCount(), actor, reference);
	}

	/**
	 * Destroy item from inventory and updates database
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be destroyed
	 * @param count
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public virtual Item? destroyItem(string? process, Item item, long count, Player? actor, object? reference)
	{
		lock (item)
		{
			// Adjust item quantity
			if (item.getCount() > count)
			{
				item.changeCount(process, -count, actor, reference);
				item.setLastChange(ItemChangeType.MODIFIED);
				refreshWeight();
			}
			else
			{
				if (item.getCount() < count)
				{
					return null;
				}

				bool removed = removeItem(item);
				if (!removed)
				{
					return null;
				}

				ItemData.getInstance().destroyItem(process, item, actor, reference);
				item.updateDatabase();
				refreshWeight();

				item.stopAllTasks();
			}
		}
		return item;
	}

	/**
	 * Destroy item from inventory by using its <b>objectID</b> and updates database
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Instance identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public virtual Item? destroyItem(string process, int objectId, long count, Player? actor, object? reference)
	{
		Item? item = getItemByObjectId(objectId);
		return item == null ? null : destroyItem(process, item, count, actor, reference);
	}

	/**
	 * Destroy item from inventory by using its <b>itemId</b> and updates database
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public virtual Item? destroyItemByItemId(string process, int itemId, long count, Player? actor, object? reference)
	{
		Item? item = getItemByItemId(itemId);
		return item == null ? null : destroyItem(process, item, count, actor, reference);
	}

	/**
	 * Destroy all items from inventory and updates database
	 * @param process : String Identifier of process triggering this action
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 */
	public void destroyAllItems(string process, Player? actor, object? reference)
	{
		foreach (Item item in _items)
		{
			destroyItem(process, item, actor, reference);
		}
	}

	/**
	 * @return warehouse Adena.
	 */
	public virtual long getAdena()
	{
		foreach (Item item in _items)
		{
			if (item.getId() == Inventory.ADENA_ID)
			{
				return item.getCount();
			}
		}
		return 0;
	}

	public virtual long getBeautyTickets()
	{
		foreach (Item item in _items)
		{
			if (item.getId() == Inventory.BEAUTY_TICKET_ID)
			{
				return item.getCount();
			}
		}
		return 0;
	}

	/**
	 * Adds item to inventory for further adjustments.
	 * @param item : Item to be added from inventory
	 */
	protected virtual void addItem(Item item)
	{
		_items.add(item);
	}

	/**
	 * Removes item from inventory for further adjustments.
	 * @param item : Item to be removed from inventory
	 * @return
	 */
	protected virtual bool removeItem(Item item)
	{
		return _items.remove(item);
	}

	/**
	 * Refresh the weight of equipment loaded
	 */
	protected virtual void refreshWeight()
	{
	}

	/**
	 * Delete item object from world
	 */
	public virtual void deleteMe()
	{
		if (this is PlayerInventory || this is PlayerWarehouse || getOwner() != null)
		{
			foreach (Item item in _items)
			{
				item.updateDatabase(true);
				item.stopAllTasks();
			}
		}

		foreach (Item item in _items)
		{
			World.getInstance().removeObject(item);
		}

		_items.clear();
	}

	/**
	 * Update database with items in inventory
	 */
	public virtual void updateDatabase()
	{
		if (getOwner() != null)
		{
			foreach (Item item in _items)
			{
				item.updateDatabase(true);
			}
		}
	}

	/**
	 * Get back items in container from database
	 */
	public virtual void restore()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int ownerId = getOwnerId();
			ItemLocation location = getBaseLocation();
			var query = ctx.Items.Where(r => r.OwnerId == ownerId && r.Location == (int)location);
			foreach (var record in query)
			{
				Item item = new Item(record);
				World.getInstance().addObject(item);

				Player? owner = getOwner()?.getActingPlayer();

				// If stackable item is found in inventory just add to current quantity
				if (item.isStackable() && getItemByItemId(item.getId()) != null)
				{
					addItem("Restore", item, owner, null);
				}
				else
				{
					addItem(item);
				}
			}

			refreshWeight();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore container: " + e);
		}
	}

	public virtual bool validateCapacity(long slots)
	{
		return true;
	}

	public virtual bool validateWeight(long weight)
	{
		return true;
	}

	/**
	 * If the item is stackable validates 1 slot, if the item isn't stackable validates the item count.
	 * @param itemId the item Id to verify
	 * @param count amount of item's weight to validate
	 * @return {@code true} if the item doesn't exists or it validates its slot count
	 */
	public bool validateCapacityByItemId(int itemId, long count)
	{
		ItemTemplate? template = ItemData.getInstance().getTemplate(itemId);
		return template == null || (template.isStackable() ? validateCapacity(1) : validateCapacity(count));
	}

	/**
	 * @param itemId the item Id to verify
	 * @param count amount of item's weight to validate
	 * @return {@code true} if the item doesn't exist, or it validates its weight
	 */
	public bool validateWeightByItemId(int itemId, long count)
	{
		ItemTemplate? template = ItemData.getInstance().getTemplate(itemId);
		return template == null || validateWeight(template.getWeight() * count);
	}
}