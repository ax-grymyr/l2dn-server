using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.ItemContainers;

public class PlayerInventory: Inventory
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PlayerInventory));
	
	private readonly Player _owner;
	private Item _adena;
	private Item _ancientAdena;
	private Item _beautyTickets;
	private ICollection<int> _blockItems = null;
	private InventoryBlockType _blockMode = InventoryBlockType.NONE;
	private int _questItemSize;
	
	public PlayerInventory(Player owner)
	{
		_owner = owner;
	}
	
	public override Player getOwner()
	{
		return _owner;
	}
	
	protected override ItemLocation getBaseLocation()
	{
		return ItemLocation.INVENTORY;
	}
	
	protected override ItemLocation getEquipLocation()
	{
		return ItemLocation.PAPERDOLL;
	}
	
	public Item getAdenaInstance()
	{
		return _adena;
	}
	
	public override long getAdena()
	{
		return _adena != null ? _adena.getCount() : 0;
	}
	
	public Item getAncientAdenaInstance()
	{
		return _ancientAdena;
	}
	
	public long getAncientAdena()
	{
		return (_ancientAdena != null) ? _ancientAdena.getCount() : 0;
	}
	
	public Item getBeautyTicketsInstance()
	{
		return _beautyTickets;
	}
	
	public override long getBeautyTickets()
	{
		return _beautyTickets != null ? _beautyTickets.getCount() : 0;
	}
	
	/**
	 * Returns the list of items in inventory available for transaction
	 * @param allowAdena
	 * @param allowAncientAdena
	 * @return Item : items in inventory
	 */
	public ICollection<Item> getUniqueItems(bool allowAdena, bool allowAncientAdena)
	{
		return getUniqueItems(allowAdena, allowAncientAdena, true);
	}
	
	public ICollection<Item> getUniqueItems(bool allowAdena, bool allowAncientAdena, bool onlyAvailable)
	{
		List<Item> list = new();
		foreach (Item item in _items)
		{
			if (!allowAdena && (item.getId() == ADENA_ID))
			{
				continue;
			}
			if (!allowAncientAdena && (item.getId() == ANCIENT_ADENA_ID))
			{
				continue;
			}
			bool isDuplicate = false;
			foreach (Item litem in list)
			{
				if (litem.getId() == item.getId())
				{
					isDuplicate = true;
					break;
				}
			}
			if (!isDuplicate && (!onlyAvailable || (item.isSellable() && item.isAvailable(_owner, false, false))))
			{
				list.Add(item);
			}
		}
		return list;
	}
	
	/**
	 * Returns the list of all items in inventory that have a given item id.
	 * @param itemId : ID of item
	 * @param includeEquipped : include equipped items
	 * @return Collection<Item> : matching items from inventory
	 */
	public ICollection<Item> getAllItemsByItemId(int itemId, bool includeEquipped)
	{
		List<Item> result = new();
		foreach (Item item in _items)
		{
			if ((itemId == item.getId()) && (includeEquipped || !item.isEquipped()))
			{
				result.Add(item);
			}
		}
		return result;
	}
	
	/**
	 * @param itemId
	 * @param enchantment
	 * @return
	 */
	public ICollection<Item> getAllItemsByItemId(int itemId, int enchantment)
	{
		return getAllItemsByItemId(itemId, enchantment, true);
	}
	
	/**
	 * Returns the list of all items in inventory that have a given item id AND a given enchantment level.
	 * @param itemId : ID of item
	 * @param enchantment : enchant level of item
	 * @param includeEquipped : include equipped items
	 * @return Collection<Item> : matching items from inventory
	 */
	public ICollection<Item> getAllItemsByItemId(int itemId, int enchantment, bool includeEquipped)
	{
		List<Item> result = new();
		foreach (Item item in _items)
		{
			if ((itemId == item.getId()) && (item.getEnchantLevel() == enchantment) && (includeEquipped || !item.isEquipped()))
			{
				result.Add(item);
			}
		}
		return result;
	}
	
	/**
	 * @param allowAdena
	 * @param allowNonTradeable
	 * @param feightable
	 * @return the list of items in inventory available for transaction
	 */
	public ICollection<Item> getAvailableItems(bool allowAdena, bool allowNonTradeable, bool feightable)
	{
		List<Item> result = new();
		foreach (Item item in _items)
		{
			if (!item.isAvailable(_owner, allowAdena, allowNonTradeable) || !canManipulateWithItemId(item.getId()))
			{
				continue;
			}
			else if (feightable)
			{
				if ((item.getItemLocation() == ItemLocation.INVENTORY) && item.isFreightable())
				{
					result.Add(item);
				}
				continue;
			}
			result.Add(item);
		}
		return result;
	}
	
	/**
	 * Returns the list of items in inventory available for transaction adjusted by tradeList
	 * @param tradeList
	 * @return Item : items in inventory
	 */
	public ICollection<TradeItem> getAvailableItems(TradeList tradeList)
	{
		List<TradeItem> result = new();
		foreach (Item item in _items)
		{
			if ((item != null) && item.isAvailable(_owner, false, false))
			{
				TradeItem adjItem = tradeList.adjustAvailableItem(item);
				if (adjItem != null)
				{
					result.Add(adjItem);
				}
			}
		}
		return result;
	}
	
	/**
	 * Adjust TradeItem according his status in inventory
	 * @param item : Item to be adjusted
	 */
	public void adjustAvailableItem(TradeItem item)
	{
		bool notAllEquipped = false;
		foreach (Item adjItem in getAllItemsByItemId(item.getItem().getId()))
		{
			if (adjItem.isEquipable())
			{
				if (!adjItem.isEquipped())
				{
					notAllEquipped |= true;
				}
			}
			else
			{
				notAllEquipped |= true;
				break;
			}
		}
		if (notAllEquipped)
		{
			Item adjItem = getItemByItemId(item.getItem().getId());
			item.setObjectId(adjItem.getObjectId());
			item.setEnchant(adjItem.getEnchantLevel());
			
			if (adjItem.getCount() < item.getCount())
			{
				item.setCount(adjItem.getCount());
			}
			
			return;
		}
		
		item.setCount(0);
	}
	
	/**
	 * Adds adena to PcInventory
	 * @param process : String Identifier of process triggering this action
	 * @param count : int Quantity of adena to be added
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 */
	public void addAdena(String process, long count, Player actor, Object reference)
	{
		if (count > 0)
		{
			addItem(process, ADENA_ID, count, actor, reference);
		}
	}
	
	/**
	 * Adds Beauty Tickets to PcInventory
	 * @param process : String Identifier of process triggering this action
	 * @param count : int Quantity of Beauty Tickets to be added
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 */
	public void addBeautyTickets(String process, long count, Player actor, Object reference)
	{
		if (count > 0)
		{
			addItem(process, BEAUTY_TICKET_ID, count, actor, reference);
		}
	}
	
	/**
	 * Removes adena to PcInventory
	 * @param process : String Identifier of process triggering this action
	 * @param count : int Quantity of adena to be removed
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return bool : true if adena was reduced
	 */
	public bool reduceAdena(String process, long count, Player actor, Object reference)
	{
		if (count > 0)
		{
			return destroyItemByItemId(process, ADENA_ID, count, actor, reference) != null;
		}
		return false;
	}
	
	/**
	 * Removes Beauty Tickets to PcInventory
	 * @param process : String Identifier of process triggering this action
	 * @param count : int Quantity of Beauty Tickets to be removed
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return bool : true if adena was reduced
	 */
	public bool reduceBeautyTickets(String process, long count, Player actor, Object reference)
	{
		if (count > 0)
		{
			return destroyItemByItemId(process, BEAUTY_TICKET_ID, count, actor, reference) != null;
		}
		return false;
	}
	
	/**
	 * Adds specified amount of ancient adena to player inventory.
	 * @param process : String Identifier of process triggering this action
	 * @param count : int Quantity of adena to be added
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 */
	public void addAncientAdena(String process, long count, Player actor, Object reference)
	{
		if (count > 0)
		{
			addItem(process, ANCIENT_ADENA_ID, count, actor, reference);
		}
	}
	
	/**
	 * Removes specified amount of ancient adena from player inventory.
	 * @param process : String Identifier of process triggering this action
	 * @param count : int Quantity of adena to be removed
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return bool : true if adena was reduced
	 */
	public bool reduceAncientAdena(String process, long count, Player actor, Object reference)
	{
		return (count > 0) && (destroyItemByItemId(process, ANCIENT_ADENA_ID, count, actor, reference) != null);
	}
	
	/**
	 * Adds item in inventory and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be added
	 * @param actor : Player Player requesting the item add
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public override Item addItem(String process, Item item, Player actor, Object reference)
	{
		Item addedItem = base.addItem(process, item, actor, reference);
		if (addedItem != null)
		{
			if ((addedItem.getId() == ADENA_ID) && !addedItem.Equals(_adena))
			{
				_adena = addedItem;
			}
			else if ((addedItem.getId() == ANCIENT_ADENA_ID) && !addedItem.Equals(_ancientAdena))
			{
				_ancientAdena = addedItem;
			}
			else if ((addedItem.getId() == BEAUTY_TICKET_ID) && !addedItem.Equals(_beautyTickets))
			{
				_beautyTickets = addedItem;
			}
			
			if (actor != null)
			{
				// Send inventory update packet
				InventoryUpdate playerIU = new InventoryUpdate();
				playerIU.addItem(addedItem);
				actor.sendInventoryUpdate(playerIU);
				
				// Notify to scripts
				if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_ITEM_ADD, actor, addedItem.getTemplate()))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnPlayerItemAdd(actor, addedItem), actor, addedItem.getTemplate());
				}
			}
		}
		return addedItem;
	}
	
	/**
	 * Adds item in inventory and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item Identifier of the item to be added
	 * @param count : int Quantity of items to be added
	 * @param actor : Player Player requesting the item creation
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public override Item addItem(String process, int itemId, long count, Player actor, Object reference)
	{
		return addItem(process, itemId, count, actor, reference, true);
	}
	
	/**
	 * Adds item in inventory and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item Identifier of the item to be added
	 * @param count : int Quantity of items to be added
	 * @param actor : Player Player requesting the item creation
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @param update : Update inventory (not used by MultiSellChoose packet / it sends update after finish)
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public Item addItem(String process, int itemId, long count, Player actor, Object reference, bool update)
	{
		Item item = base.addItem(process, itemId, count, actor, reference);
		if (item != null)
		{
			if ((item.getId() == ADENA_ID) && !item.Equals(_adena))
			{
				_adena = item;
			}
			else if ((item.getId() == ANCIENT_ADENA_ID) && !item.Equals(_ancientAdena))
			{
				_ancientAdena = item;
			}
			else if ((item.getId() == BEAUTY_TICKET_ID) && !item.Equals(_beautyTickets))
			{
				_beautyTickets = item;
			}
			
			if (actor != null)
			{
				// Send inventory update packet
				if (update)
				{
					InventoryUpdate playerIU = new InventoryUpdate();
					if (item.isStackable() && (item.getCount() > count))
					{
						playerIU.addModifiedItem(item);
					}
					else
					{
						playerIU.addNewItem(item);
					}
					actor.sendInventoryUpdate(playerIU);
					
					// Adena UI update.
					if (item.getId() == Inventory.ADENA_ID)
					{
						actor.sendPacket(new ExAdenaInvenCount(actor));
					}
					// LCoin UI update.
					else if (item.getId() == Inventory.LCOIN_ID)
					{
						actor.sendPacket(new ExBloodyCoinCount(actor));
					}
				}
				
				// Notify to scripts
				if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_ITEM_ADD, actor, item.getTemplate()))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnPlayerItemAdd(actor, item), actor, item.getTemplate());
				}
			}
		}
		return item;
	}
	
	/**
	 * Transfers item to another inventory and checks _adena and _ancientAdena
	 * @param process string Identifier of process triggering this action
	 * @param objectId Item Identifier of the item to be transfered
	 * @param count Quantity of items to be transfered
	 * @param target the item container for the item to be transfered.
	 * @param actor the player requesting the item transfer
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public override Item transferItem(String process, int objectId, long count, ItemContainer target, Player actor, Object reference)
	{
		Item item = base.transferItem(process, objectId, count, target, actor, reference);
		
		if ((_adena != null) && ((_adena.getCount() <= 0) || (_adena.getOwnerId() != getOwnerId())))
		{
			_adena = null;
		}
		
		if ((_ancientAdena != null) && ((_ancientAdena.getCount() <= 0) || (_ancientAdena.getOwnerId() != getOwnerId())))
		{
			_ancientAdena = null;
		}
		
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_ITEM_TRANSFER, item.getTemplate()))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerItemTransfer(actor, item, target), item.getTemplate());
		}
		
		return item;
	}
	
	public override Item detachItem(String process, Item item, long count, ItemLocation newLocation, Player actor, Object reference)
	{
		Item detachedItem = base.detachItem(process, item, count, newLocation, actor, reference);
		if ((detachedItem != null) && (actor != null))
		{
			actor.sendItemList();
		}
		return detachedItem;
	}
	
	/**
	 * Destroy item from inventory and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be destroyed
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public override Item destroyItem(String process, Item item, Player actor, Object reference)
	{
		return destroyItem(process, item, item.getCount(), actor, reference);
	}
	
	/**
	 * Destroy item from inventory and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be destroyed
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public override Item destroyItem(String process, Item item, long count, Player actor, Object reference)
	{
		Item destroyedItem = base.destroyItem(process, item, count, actor, reference);
		
		if ((_adena != null) && (_adena.getCount() <= 0))
		{
			_adena = null;
		}
		
		if ((_ancientAdena != null) && (_ancientAdena.getCount() <= 0))
		{
			_ancientAdena = null;
		}
		
		if (destroyedItem != null)
		{
			// Adena UI update.
			if (destroyedItem.getId() == Inventory.ADENA_ID)
			{
				actor.sendPacket(new ExAdenaInvenCount(actor));
			}
			// LCoin UI update.
			else if (destroyedItem.getId() == Inventory.LCOIN_ID)
			{
				actor.sendPacket(new ExBloodyCoinCount(actor));
			}
			
			// Notify to scripts
			if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_ITEM_DESTROY, destroyedItem.getTemplate()))
			{
				EventDispatcher.getInstance().notifyEventAsync(new OnPlayerItemDestroy(actor, destroyedItem), destroyedItem.getTemplate());
			}
		}
		
		return destroyedItem;
	}
	
	/**
	 * Destroys item from inventory and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Instance identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public override Item destroyItem(String process, int objectId, long count, Player actor, Object reference)
	{
		Item item = getItemByObjectId(objectId);
		return item == null ? null : destroyItem(process, item, count, actor, reference);
	}
	
	/**
	 * Destroy item from inventory by using its <b>itemId</b> and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param actor : Player Player requesting the item destroy
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public override Item destroyItemByItemId(String process, int itemId, long count, Player actor, Object reference)
	{
		// Attempt to find non equipped items.
		Item destroyItem = null;
		ICollection<Item> items = getAllItemsByItemId(itemId);
		foreach (Item item in items)
		{
			destroyItem = item;
			if (!destroyItem.isEquipped())
			{
				break;
			}
		}
		
		// No item found.
		if (destroyItem == null)
		{
			return null;
		}
		
		// Support destroying multiple non stackable items.
		if (!destroyItem.isStackable() && (count > 1))
		{
			if (getInventoryItemCount(itemId, -1, false) >= count)
			{
				InventoryUpdate iu = new InventoryUpdate();
				long destroyed = 0;
				foreach (Item item in items)
				{
					if (!item.isEquipped() && (destroyItem(process, item, 1, actor, reference) != null))
					{
						iu.addRemovedItem(item);
						if (++destroyed == count)
						{
							_owner.sendInventoryUpdate(iu);
							refreshWeight();
							return item;
						}
					}
				}
			}
			else
			{
				return null;
			}
		}
		
		// Single item or stackable.
		return destroyItem(process, destroyItem, count, actor, reference);
	}
	
	/**
	 * Drop item from inventory and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be dropped
	 * @param actor : Player Player requesting the item drop
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public override Item dropItem(String process, Item item, Player actor, Object reference)
	{
		Item droppedItem = base.dropItem(process, item, actor, reference);
		
		if ((_adena != null) && ((_adena.getCount() <= 0) || (_adena.getOwnerId() != getOwnerId())))
		{
			_adena = null;
		}
		
		if ((_ancientAdena != null) && ((_ancientAdena.getCount() <= 0) || (_ancientAdena.getOwnerId() != getOwnerId())))
		{
			_ancientAdena = null;
		}
		
		// Notify to scripts
		if ((droppedItem != null) && EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_ITEM_DROP, droppedItem.getTemplate()))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerItemDrop(actor, droppedItem, droppedItem.getLocation()), droppedItem.getTemplate());
		}
		
		return droppedItem;
	}
	
	/**
	 * Drop item from inventory by using its <b>objectID</b> and checks _adena and _ancientAdena
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Instance identifier of the item to be dropped
	 * @param count : int Quantity of items to be dropped
	 * @param actor : Player Player requesting the item drop
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public override Item dropItem(String process, int objectId, long count, Player actor, Object reference)
	{
		Item item = base.dropItem(process, objectId, count, actor, reference);
		
		if ((_adena != null) && ((_adena.getCount() <= 0) || (_adena.getOwnerId() != getOwnerId())))
		{
			_adena = null;
		}
		
		if ((_ancientAdena != null) && ((_ancientAdena.getCount() <= 0) || (_ancientAdena.getOwnerId() != getOwnerId())))
		{
			_ancientAdena = null;
		}
		
		// Notify to scripts
		if ((item != null) && EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_ITEM_DROP, item.getTemplate()))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerItemDrop(actor, item, item.getLocation()), item.getTemplate());
		}
		
		return item;
	}
	
	/**
	 * Adds item to inventory for further adjustments.
	 * @param item : Item to be added from inventory
	 */
	protected override void addItem(Item item)
	{
		if (item.isQuestItem())
		{
			Interlocked.Increment(ref _questItemSize);
		}
		
		base.addItem(item);
	}
	
	/**
	 * <b>Overloaded</b>, when removes item from inventory, remove also owner shortcuts.
	 * @param item : Item to be removed from inventory
	 */
	protected override bool removeItem(Item item)
	{
		// Removes any reference to the item from Shortcut bar
		_owner.removeItemFromShortCut(item.getObjectId());
		
		// Removes active Enchant Scroll
		if (_owner.isProcessingItem(item.getObjectId()))
		{
			_owner.removeRequestsThatProcessesItem(item.getObjectId());
		}
		
		if (item.getId() == ADENA_ID)
		{
			_adena = null;
		}
		else if (item.getId() == ANCIENT_ADENA_ID)
		{
			_ancientAdena = null;
		}
		else if (item.getId() == BEAUTY_TICKET_ID)
		{
			_beautyTickets = null;
		}
		
		if (item.isQuestItem())
		{
			Interlocked.Decrement(ref _questItemSize);
		}
		
		return base.removeItem(item);
	}
	
	/**
	 * @return the quantity of quest items in the inventory
	 */
	public int getQuestSize()
	{
		return _questItemSize;
	}
	
	/**
	 * @return the quantity of items in the inventory
	 */
	public int getNonQuestSize()
	{
		return _items.size() - _questItemSize;
	}
	
	/**
	 * Refresh the weight of equipment loaded
	 */
	protected override void refreshWeight()
	{
		base.refreshWeight();
		
		_owner.refreshOverloaded(true);
		
		StatusUpdate su = new StatusUpdate(_owner);
		su.addUpdate(StatusUpdateType.CUR_LOAD, _owner.getCurrentLoad());
		_owner.sendPacket(su);
	}
	
	/**
	 * Get back items in inventory from database
	 */
	public override void restore()
	{
		base.restore();
		_adena = getItemByItemId(ADENA_ID);
		_ancientAdena = getItemByItemId(ANCIENT_ADENA_ID);
		_beautyTickets = getItemByItemId(BEAUTY_TICKET_ID);
	}
	
	public static int[][] restoreVisibleInventory(int objectId)
	{
		int[][] paperdoll = new int[Inventory.PAPERDOLL_TOTALSLOTS][];
		for (int i = 0; i < paperdoll.Length; i++)
			paperdoll[i] = new int[4];
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(
				"SELECT object_id,item_id,loc_data,enchant_level FROM items WHERE owner_id=? AND loc='PAPERDOLL'");
			ps.setInt(1, objectId);
			{
				ResultSet invdata = ps.executeQuery();
				while (invdata.next())
				{
					int slot = invdata.getInt("loc_data");
					ItemVariables vars = new ItemVariables(invdata.getInt("object_id"));
					paperdoll[slot][0] = invdata.getInt("object_id");
					paperdoll[slot][1] = invdata.getInt("item_id");
					paperdoll[slot][2] = invdata.getInt("enchant_level");
					paperdoll[slot][3] = vars.getInt(ItemVariables.VISUAL_ID, 0);
					if (paperdoll[slot][3] > 0) // fix for hair appearance conflicting with original model
					{
						paperdoll[slot][1] = paperdoll[slot][3];
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore inventory: " + e);
		}
		return paperdoll;
	}
	
	/**
	 * @param itemList the items that needs to be validated.
	 * @param sendMessage if {@code true} will send a message of inventory full.
	 * @param sendSkillMessage if {@code true} will send a message of skill not available.
	 * @return {@code true} if the inventory isn't full after taking new items and items weight add to current load doesn't exceed max weight load.
	 */
	public bool checkInventorySlotsAndWeight(List<ItemTemplate> itemList, bool sendMessage, bool sendSkillMessage)
	{
		int lootWeight = 0;
		int requiredSlots = 0;
		if (itemList != null)
		{
			foreach (ItemTemplate item in itemList)
			{
				// If the item is not stackable or is stackable and not present in inventory, will need a slot.
				if (!item.isStackable() || (getInventoryItemCount(item.getId(), -1) <= 0))
				{
					requiredSlots++;
				}
				lootWeight += item.getWeight();
			}
		}
		
		bool inventoryStatusOK = validateCapacity(requiredSlots) && validateWeight(lootWeight);
		if (!inventoryStatusOK && sendMessage)
		{
			_owner.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			if (sendSkillMessage)
			{
				_owner.sendPacket(SystemMessageId.WEIGHT_AND_VOLUME_LIMIT_HAVE_BEEN_EXCEEDED_THAT_SKILL_IS_CURRENTLY_UNAVAILABLE);
			}
		}
		return inventoryStatusOK;
	}
	
	/**
	 * If the item is not stackable or is stackable and not present in inventory, will need a slot.
	 * @param item the item to validate.
	 * @return {@code true} if there is enough room to add the item inventory.
	 */
	public bool validateCapacity(Item item)
	{
		int slots = 0;
		if (!item.isStackable() || ((getInventoryItemCount(item.getId(), -1) <= 0) && !item.getTemplate().hasExImmediateEffect()))
		{
			slots++;
		}
		return validateCapacity(slots, item.isQuestItem());
	}
	
	/**
	 * If the item is not stackable or is stackable and not present in inventory, will need a slot.
	 * @param itemId the item Id for the item to validate.
	 * @return {@code true} if there is enough room to add the item inventory.
	 */
	public bool validateCapacityByItemId(int itemId)
	{
		int slots = 0;
		Item invItem = getItemByItemId(itemId);
		if ((invItem == null) || !invItem.isStackable())
		{
			slots++;
		}
		return validateCapacity(slots, ItemData.getInstance().getTemplate(itemId).isQuestItem());
	}
	
	public override bool validateCapacity(long slots)
	{
		return validateCapacity(slots, false);
	}
	
	public bool validateCapacity(long slots, bool questItem)
	{
		return ((slots == 0) && !Config.AUTO_LOOT_SLOT_LIMIT) || questItem ? (getQuestSize() + slots) <= _owner.getQuestInventoryLimit() : (getNonQuestSize() + slots) <= _owner.getInventoryLimit();
	}
	
	public override bool validateWeight(long weight)
	{
		// Disable weight check for GMs.
		if (_owner.isGM() && _owner.getDietMode() && _owner.getAccessLevel().allowTransaction())
		{
			return true;
		}
		return ((_totalWeight + weight) <= _owner.getMaxLoad());
	}
	
	/**
	 * Set inventory block for specified IDs<br>
	 * array reference is used for {@link PlayerInventory#_blockItems}
	 * @param items array of Ids to block/allow
	 * @param mode blocking mode {@link PlayerInventory#_blockMode}
	 */
	public void setInventoryBlock(ICollection<int> items, InventoryBlockType mode)
	{
		_blockMode = mode;
		_blockItems = items;
		_owner.sendItemList();
	}
	
	/**
	 * Unblock blocked itemIds
	 */
	public void unblock()
	{
		_blockMode = InventoryBlockType.NONE;
		_blockItems = null;
		_owner.sendItemList();
	}
	
	/**
	 * Check if player inventory is in block mode.
	 * @return true if some itemIds blocked
	 */
	public bool hasInventoryBlock()
	{
		return ((_blockMode != InventoryBlockType.NONE) && (_blockItems != null) && !_blockItems.isEmpty());
	}
	
	/**
	 * Block all items except adena
	 */
	public void blockAllItems()
	{
		setInventoryBlock([ADENA_ID], InventoryBlockType.WHITELIST);
	}
	
	/**
	 * Return block mode
	 * @return int {@link PlayerInventory#_blockMode}
	 */
	public InventoryBlockType getBlockMode()
	{
		return _blockMode;
	}
	
	/**
	 * Return Collection<int> with blocked item ids
	 * @return Collection<int>
	 */
	public ICollection<int> getBlockItems()
	{
		return _blockItems;
	}
	
	/**
	 * Check if player can use item by itemid
	 * @param itemId int
	 * @return true if can use
	 */
	public bool canManipulateWithItemId(int itemId)
	{
		ICollection<int> blockedItems = _blockItems;
		if (blockedItems != null)
		{
			switch (_blockMode)
			{
				case InventoryBlockType.NONE:
				{
					return true;
				}
				case InventoryBlockType.WHITELIST:
				{
					foreach (int id in blockedItems)
					{
						if (id == itemId)
						{
							return true;
						}
					}
					return false;
				}
				case InventoryBlockType.BLACKLIST:
				{
					foreach (int id in blockedItems)
					{
						if (id == itemId)
						{
							return false;
						}
					}
					return true;
				}
			}
		}
		return true;
	}
	
	public override String ToString()
	{
		return GetType().Name + "[" + _owner + "]";
	}
	
	/**
	 * Apply skills of inventory items
	 */
	public void applyItemSkills()
	{
		foreach (Item item in _items)
		{
			item.giveSkillsToOwner();
			item.applyEnchantStats();
			if (item.isEquipped())
			{
				item.applySpecialAbilities();
				
				if (((item.getLocationSlot() >= Inventory.PAPERDOLL_AGATHION1) && (item.getLocationSlot() <= Inventory.PAPERDOLL_AGATHION5)))
				{
					AgathionSkillHolder agathionSkills = AgathionData.getInstance().getSkills(item.getId());
					if (agathionSkills != null)
					{
						// Remove old skills.
						foreach (Skill skill in agathionSkills.getMainSkills(item.getEnchantLevel()))
						{
							_owner.removeSkill(skill, false, skill.isPassive());
						}
						foreach (Skill skill in agathionSkills.getSubSkills(item.getEnchantLevel()))
						{
							_owner.removeSkill(skill, false, skill.isPassive());
						}
						// Add new skills.
						if (item.getLocationSlot() == Inventory.PAPERDOLL_AGATHION1)
						{
							foreach (Skill skill in agathionSkills.getMainSkills(item.getEnchantLevel()))
							{
								if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, _owner, _owner))
								{
									continue;
								}
								_owner.addSkill(skill, false);
							}
						}
						foreach (Skill skill in agathionSkills.getSubSkills(item.getEnchantLevel()))
						{
							if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, _owner, _owner))
							{
								continue;
							}
							_owner.addSkill(skill, false);
						}
					}
				}
			}
		}
	}
	
	/**
	 * Reduce the number of arrows/bolts owned by the Player and send it Server->Client Packet InventoryUpdate or ItemList (to unequip if the last arrow was consumed).
	 * @param type
	 */
	public override void reduceAmmunitionCount(EtcItemType type)
	{
		if ((type != EtcItemType.ARROW) && (type != EtcItemType.BOLT))
		{
			LOGGER.Warn(type.ToString(), " which is not ammo type.");
			return;
		}
		
		Weapon weapon = _owner.getActiveWeaponItem();
		if (weapon == null)
		{
			return;
		}
		
		Item ammunition = null;
		switch (weapon.getItemType())
		{
			case WeaponType.BOW:
			{
				ammunition = findArrowForBow(weapon);
				break;
			}
			case WeaponType.CROSSBOW:
			case WeaponType.TWOHANDCROSSBOW:
			{
				ammunition = findBoltForCrossBow(weapon);
				break;
			}
			default:
			{
				return;
			}
		}
		
		if ((ammunition == null) || (ammunition.getItemType() != type))
		{
			return;
		}
		
		if (ammunition.getEtcItem().isInfinite())
		{
			return;
		}
		
		// Reduce item count.
		updateItemCountNoDbUpdate(null, ammunition, -1, _owner, null);
	}
	
	/**
	 * Reduces item count in the stack, destroys item when count reaches 0.
	 * @param process
	 * @param item
	 * @param countDelta Adds items to stack if positive, reduces if negative. If stack count reaches 0 item is destroyed.
	 * @param creator
	 * @param reference
	 * @return Amount of items left.
	 */
	public bool updateItemCountNoDbUpdate(String process, Item item, long countDelta, Player creator, Object reference)
	{
		InventoryUpdate iu = new InventoryUpdate();
		long left = item.getCount() + countDelta;
		try
		{
			if (left > 0)
			{
				lock (item)
				{
					if ((process != null) && (process.Length > 0))
					{
						item.changeCount(process, countDelta, creator, reference);
					}
					else
					{
						item.changeCountWithoutTrace(-1, creator, reference);
					}
					item.setLastChange(Item.MODIFIED);
					refreshWeight();
					iu.addModifiedItem(item);
					return true;
				}
			}
			else if (left == 0)
			{
				iu.addRemovedItem(item);
				destroyItem(process, item, _owner, null);
				return true;
			}
			else
			{
				return false;
			}
		}
		finally
		{
			_owner.sendInventoryUpdate(iu);
		}
	}
	
	/**
	 * Reduces item count in the stack, destroys item when count reaches 0.
	 * @param process
	 * @param item
	 * @param countDelta Adds items to stack if positive, reduces if negative. If stack count reaches 0 item is destroyed.
	 * @param creator
	 * @param reference
	 * @return Amount of items left.
	 */
	public bool updateItemCount(String process, Item item, long countDelta, Player creator, Object reference)
	{
		if (item != null)
		{
			try
			{
				return updateItemCountNoDbUpdate(process, item, countDelta, creator, reference);
			}
			finally
			{
				if (item.getCount() > 0)
				{
					item.updateDatabase();
				}
			}
		}
		return false;
	}
}