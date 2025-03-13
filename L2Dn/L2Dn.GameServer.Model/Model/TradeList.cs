using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

public class TradeList
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(TradeList));

	private readonly Player _owner;
	private Player? _partner;
	private readonly Set<TradeItem> _items = [];
	private string _title = string.Empty;
	private bool _packaged;

	private bool _confirmed;
	private bool _locked;

	public TradeList(Player owner)
	{
		_owner = owner;
	}

	public Player getOwner()
	{
		return _owner;
	}

	public void setPartner(Player? partner)
	{
		_partner = partner;
	}

	public Player? getPartner()
	{
		return _partner;
	}

	public void setTitle(string title)
	{
		_title = title;
	}

	public string getTitle()
	{
		return _title;
	}

	public bool isLocked()
	{
		return _locked;
	}

	public bool isConfirmed()
	{
		return _confirmed;
	}

	public bool isPackaged()
	{
		return _packaged;
	}

	public void setPackaged(bool value)
	{
		_packaged = value;
	}

	/**
	 * @return all items from TradeList
	 */
	public ICollection<TradeItem> getItems()
	{
		return _items;
	}

	/**
	 * Returns the list of items in inventory available for transaction
	 * @param inventory
	 * @return Item : items in inventory
	 */
	public List<TradeItem> getAvailableItems(PlayerInventory inventory)
	{
		List<TradeItem> list = new List<TradeItem>();
		foreach (TradeItem item in _items)
		{
			TradeItem tradeItem = new TradeItem(item, item.getCount(), item.getPrice());
			inventory.adjustAvailableItem(tradeItem);
			list.Add(tradeItem);
		}
		return list;
	}

	/**
	 * @return Item List size
	 */
	public int getItemCount()
	{
		return _items.size();
	}

	/**
	 * Adjust available item from Inventory by the one in this list
	 * @param item : Item to be adjusted
	 * @return TradeItem representing adjusted item
	 */
	public TradeItem? adjustAvailableItem(Item item)
	{
		if (item.isStackable())
		{
			foreach (TradeItem exclItem in _items)
			{
				if (exclItem.getItem().getId() == item.getId())
                {
                    return item.getCount() <= exclItem.getCount()
                        ? null
                        : new TradeItem(item, item.getCount() - exclItem.getCount(), item.getReferencePrice());
                }
			}
		}
		return new TradeItem(item, item.getCount(), item.getReferencePrice());
	}

	/**
	 * Add simplified item to TradeList
	 * @param objectId : int
	 * @param count : int
	 * @return
	 */
	public TradeItem? addItem(int objectId, long count)
	{
		return addItem(objectId, count, 0);
	}

	/**
	 * Add item to TradeList
	 * @param objectId : int
	 * @param count : long
	 * @param price : long
	 * @return
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public TradeItem? addItem(int objectId, long count, long price)
	{
		if (_locked)
		{
			return null;
		}

		WorldObject? o = World.getInstance().findObject(objectId);
		if (!(o is Item))
		{
			return null;
		}

		Item item = (Item) o;
		if (!(item.isTradeable() || (_owner.isGM() && Config.GM_TRADE_RESTRICTED_ITEMS)) || item.isQuestItem())
		{
			return null;
		}

		if (!_owner.getInventory().canManipulateWithItemId(item.getId()))
		{
			return null;
		}

		if (count <= 0 || count > item.getCount())
		{
			return null;
		}

		if (!item.isStackable() && count > 1)
		{
			return null;
		}

		if (Inventory.MAX_ADENA / count < price)
		{
			return null;
		}

		foreach (TradeItem checkitem in _items)
		{
			if (checkitem.getObjectId() == objectId)
			{
				return null;
			}
		}

		TradeItem titem = new TradeItem(item, count, price);
		_items.add(titem);

		// If Player has already confirmed this trade, invalidate the confirmation
		invalidateConfirmation();
		return titem;
	}

	/**
	 * Add item to TradeList
	 * @param itemId
	 * @param count
	 * @param price
	 * @return
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public TradeItem? addItemByItemId(int itemId, long count, long price)
	{
		if (_locked)
		{
			LOGGER.Warn(_owner.getName() + ": Attempt to modify locked TradeList!");
			return null;
		}

		ItemTemplate? item = ItemData.getInstance().getTemplate(itemId);
		if (item == null)
		{
			LOGGER.Warn(_owner.getName() + ": Attempt to add invalid item to TradeList!");
			return null;
		}

		if (!item.isTradeable() || item.isQuestItem())
		{
			return null;
		}

		if (!item.isStackable() && count > 1)
		{
			LOGGER.Warn(_owner.getName() + ": Attempt to add non-stackable item to TradeList with count > 1!");
			return null;
		}

		if (Inventory.MAX_ADENA / count < price)
		{
			LOGGER.Warn(_owner.getName() + ": Attempt to overflow adena !");
			return null;
		}

		TradeItem titem = new TradeItem(item, count, price);
		_items.add(titem);

		// If Player has already confirmed this trade, invalidate the confirmation
		invalidateConfirmation();
		return titem;
	}

	/**
	 * Remove item from TradeList
	 * @param objectId : int
	 * @param itemId
	 * @param count : int
	 * @return
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	private TradeItem? removeItem(int objectId, int itemId, long count)
	{
		if (_locked)
		{
			LOGGER.Warn(_owner.getName() + ": Attempt to modify locked TradeList!");
			return null;
		}

		if (count < 0)
		{
			LOGGER.Warn(_owner.getName() + ": Attempt to remove " + count + " items from TradeList!");
			return null;
		}

		foreach (TradeItem titem in _items)
		{
			if (titem.getObjectId() == objectId || titem.getItem().getId() == itemId)
			{
				// If Partner has already confirmed this trade, invalidate the confirmation
				if (_partner != null)
				{
					TradeList? partnerList = _partner.getActiveTradeList();
					if (partnerList == null)
					{
						LOGGER.Warn(_partner.getName() + ": Trading partner (" + _partner.getName() + ") is invalid in this trade!");
						return null;
					}
					partnerList.invalidateConfirmation();
				}

				// Reduce item count or complete item
				if (count != -1 && titem.getCount() > count)
				{
					titem.setCount(titem.getCount() - count);
				}
				else
				{
					_items.remove(titem);
				}

				return titem;
			}
		}
		return null;
	}

	/**
	 * Update items in TradeList according their quantity in owner inventory
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void updateItems()
	{
		foreach (TradeItem titem in _items)
		{
			Item? item = _owner.getInventory().getItemByObjectId(titem.getObjectId());
			if (item == null || titem.getCount() < 1)
			{
				removeItem(titem.getObjectId(), -1, -1);
			}
			else if (item.getCount() < titem.getCount())
			{
				titem.setCount(item.getCount());
			}
		}
	}

	/**
	 * Lockes TradeList, no further changes are allowed
	 */
	public void @lock()
	{
		_locked = true;
	}

	/**
	 * Clears item list
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void clear()
	{
		_items.clear();
		_locked = false;
	}

	/**
	 * Confirms TradeList
	 * @return : bool
	 */
	public bool confirm()
	{
		if (_confirmed)
		{
			return true; // Already confirmed
		}

		// If Partner has already confirmed this trade, proceed exchange
		if (_partner != null)
		{
			TradeList? partnerList = _partner.getActiveTradeList();
			if (partnerList == null)
			{
				LOGGER.Warn(_partner.getName() + ": Trading partner (" + _partner.getName() + ") is invalid in this trade!");
				return false;
			}

			// Synchronization order to avoid deadlock
			TradeList sync1;
			TradeList sync2;
			if (getOwner().ObjectId > partnerList.getOwner().ObjectId)
			{
				sync1 = partnerList;
				sync2 = this;
			}
			else
			{
				sync1 = this;
				sync2 = partnerList;
			}

			lock (sync1)
			{
				lock (sync2)
				{
					_confirmed = true;
					if (partnerList.isConfirmed())
					{
						partnerList.@lock();
						@lock();
						if (!partnerList.validate() || !validate())
						{
							return false;
						}

						doExchange(partnerList);
					}
					else
					{
						_partner.onTradeConfirm(_owner);
					}
				}
			}
		}
		else
		{
			_confirmed = true;
		}

		return _confirmed;
	}

	/**
	 * Cancels TradeList confirmation
	 */
	private void invalidateConfirmation()
	{
		_confirmed = false;
	}

	/**
	 * Validates TradeList with owner inventory
	 * @return
	 */
	private bool validate()
	{
		// Check for Owner validity
		if (_owner == null || World.getInstance().getPlayer(_owner.ObjectId) == null)
		{
			LOGGER.Warn("Invalid owner of TradeList");
			return false;
		}

		// Check for Item validity
		foreach (TradeItem titem in _items)
		{
			Item? item = _owner.checkItemManipulation(titem.getObjectId(), titem.getCount(), "transfer");
			if (item == null || item.getCount() < 1)
			{
				LOGGER.Warn(_owner.getName() + ": Invalid Item in TradeList");
				return false;
			}
		}

		return true;
	}

	/**
	 * Transfers all TradeItems from inventory to partner
	 * @param partner
	 * @param ownerIU
	 * @param partnerIU
	 * @return
	 */
	private bool TransferItems(Player partner, InventoryUpdatePacket ownerIU, InventoryUpdatePacket partnerIU)
	{
		foreach (TradeItem titem in _items)
		{
			Item? oldItem = _owner.getInventory().getItemByObjectId(titem.getObjectId());
			if (oldItem == null)
			{
				return false;
			}

			Item? newItem = _owner.getInventory().transferItem("Trade", titem.getObjectId(), titem.getCount(), partner.getInventory(), _owner, _partner);
			if (newItem == null)
			{
				return false;
			}

			// Add changes to inventory update packets
			if (oldItem.getCount() > 0 && oldItem != newItem)
			{
				ownerIU.addModifiedItem(oldItem);
			}
			else
			{
				ownerIU.addRemovedItem(oldItem);
			}

			if (newItem.getCount() > titem.getCount())
			{
				partnerIU.addModifiedItem(newItem);
			}
			else
			{
				partnerIU.addNewItem(newItem);
			}
		}
		return true;
	}

	/**
	 * @param partner
	 * @return items slots count
	 */
	private long countItemsSlots(Player partner)
	{
		long slots = 0;
		foreach (TradeItem item in _items)
		{
			if (item == null)
			{
				continue;
			}

			ItemTemplate? template = ItemData.getInstance().getTemplate(item.getItem().getId());
			if (template == null)
			{
				continue;
			}
			if (!template.isStackable())
			{
				slots += item.getCount();
			}
			else if (partner.getInventory().getItemByItemId(item.getItem().getId()) == null)
			{
				slots++;
			}
		}
		return slots;
	}

	/**
	 * @return the weight of items in tradeList
	 */
	private int calcItemsWeight()
	{
		long weight = 0;
		foreach (TradeItem item in _items)
		{
			if (item == null)
			{
				continue;
			}
			ItemTemplate? template = ItemData.getInstance().getTemplate(item.getItem().getId());
			if (template == null)
			{
				continue;
			}
			weight += item.getCount() * template.getWeight();
		}
		return (int) Math.Min(weight, int.MaxValue);
	}

	/**
	 * Proceeds with trade
	 * @param partnerList
	 */
	private void doExchange(TradeList partnerList)
	{
        Player partner = partnerList.getOwner();
        if (partner != _partner)
            return; // TODO: check added

		bool success = false;


		// check weight and slots
		if (!_owner.getInventory().validateWeight(partnerList.calcItemsWeight()) || !partnerList.getOwner().getInventory().validateWeight(calcItemsWeight()))
		{
			partnerList.getOwner().sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
			_owner.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
		}
		else if (!_owner.getInventory().validateCapacity(partnerList.countItemsSlots(getOwner())) || !partnerList.getOwner().getInventory().validateCapacity(countItemsSlots(partnerList.getOwner())))
		{
			partnerList.getOwner().sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			_owner.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
		}
		else
		{
			// Prepare inventory update packet
			InventoryUpdatePacket ownerIU = new InventoryUpdatePacket();
			InventoryUpdatePacket partnerIU = new InventoryUpdatePacket();

			// Transfer items
			partnerList.TransferItems(_owner, partnerIU, ownerIU);
			TransferItems(partnerList.getOwner(), ownerIU, partnerIU);

			// Send inventory update packets
			_owner.sendInventoryUpdate(ownerIU);
            partner.sendInventoryUpdate(partnerIU);

			success = true;
		}

		// Visual inconsistencies fix.
		_owner.sendItemList();
        partner.sendItemList();

		// Finish the trade
		partnerList.getOwner().onTradeFinish(success);
		_owner.onTradeFinish(success);
	}

	/**
	 * Buy items from this PrivateStore list
	 * @param player
	 * @param items
	 * @return int: result of trading. 0 - ok, 1 - canceled (no adena), 2 - failed (item error)
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public int privateStoreBuy(Player player, Set<ItemRequest> items)
	{
		if (_locked)
		{
			return 1;
		}

		if (!validate())
		{
			@lock();
			return 1;
		}

		if (!_owner.isOnline() || !player.isOnline())
		{
			return 1;
		}

		long slots = 0;
		long weight = 0;
		long totalPrice = 0;

		PlayerInventory ownerInventory = _owner.getInventory();
		PlayerInventory playerInventory = player.getInventory();
		foreach (ItemRequest item in items)
		{
			bool found = false;
			foreach (TradeItem ti in _items)
			{
				if (ti.getObjectId() == item.getObjectId())
				{
					if (ti.getPrice() == item.getPrice())
					{
						if (ti.getCount() < item.getCount())
						{
							item.setCount(ti.getCount());
						}
						found = true;
					}
					break;
				}
			}
			// item with this objectId and price not found in tradelist
			if (!found)
			{
				if (_packaged)
				{
					Util.handleIllegalPlayerAction(player, "[TradeList.privateStoreBuy()] " + player + " tried to cheat the package sell and buy only a part of the package! Ban this player for bot usage!", Config.DEFAULT_PUNISH);
					return 2;
				}

				item.setCount(0);
				continue;
			}

			// check for overflow in the single item
			if (Inventory.MAX_ADENA / item.getCount() < item.getPrice())
			{
				// private store attempting to overflow - disable it
				@lock();
				return 1;
			}

			totalPrice += item.getCount() * item.getPrice();
			// check for overflow of the total price
			if (Inventory.MAX_ADENA < totalPrice || totalPrice < 0)
			{
				// private store attempting to overflow - disable it
				@lock();
				return 1;
			}

			// Check if requested item is available for manipulation
			Item? oldItem = _owner.checkItemManipulation(item.getObjectId(), item.getCount(), "sell");
			if (oldItem == null || !oldItem.isTradeable())
			{
				// private store sell invalid item - disable it
				@lock();
				return 2;
			}

			ItemTemplate? template = ItemData.getInstance().getTemplate(item.getItemId());
			if (template == null)
			{
				continue;
			}
			weight += item.getCount() * template.getWeight();
			if (!template.isStackable())
			{
				slots += item.getCount();
			}
			else if (playerInventory.getItemByItemId(item.getItemId()) == null)
			{
				slots++;
			}
		}

		if (totalPrice > playerInventory.getAdena())
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			return 1;
		}

		if (!playerInventory.validateWeight(weight))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
			return 1;
		}

		if (!playerInventory.validateCapacity(slots))
		{
			player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			return 1;
		}

		// Prepare inventory update packets
		InventoryUpdatePacket ownerIU = new InventoryUpdatePacket();
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket();
        Item? adenaItem = playerInventory.getAdenaInstance();
		if (!playerInventory.reduceAdena("PrivateStore", totalPrice, player, _owner))
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			return 1;
		}

        if (adenaItem != null)
		    playerIU.addItem(adenaItem);

		ownerInventory.addAdena("PrivateStore", totalPrice, _owner, player);
		// ownerIU.addItem(ownerInventory.getAdenaInstance());
		bool ok = true;

		// Transfer items
		foreach (ItemRequest item in items)
		{
			if (item.getCount() == 0)
			{
				continue;
			}

			// Check if requested item is available for manipulation
			Item? oldItem = _owner.checkItemManipulation(item.getObjectId(), item.getCount(), "sell");
			if (oldItem == null)
			{
				// should not happen - validation already done
				@lock();
				ok = false;
				break;
			}

			// Proceed with item transfer
			Item? newItem = ownerInventory.transferItem("PrivateStore", item.getObjectId(), item.getCount(), playerInventory, _owner, player);
			if (newItem == null)
			{
				ok = false;
				break;
			}
			removeItem(item.getObjectId(), -1, item.getCount());

			PrivateStoreHistoryManager.getInstance().registerTransaction(PrivateStoreType.SELL, newItem, item.getCount(), item.getCount() * item.getPrice());

			// Add changes to inventory update packets
			if (oldItem.getCount() > 0 && oldItem != newItem)
			{
				ownerIU.addModifiedItem(oldItem);
			}
			else
			{
				ownerIU.addRemovedItem(oldItem);
			}
			if (newItem.getCount() > item.getCount())
			{
				playerIU.addModifiedItem(newItem);
			}
			else
			{
				playerIU.addNewItem(newItem);
			}

			// Send messages about the transaction to both players
			if (newItem.isStackable())
			{
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_PURCHASED_S3_S2_S);
				msg.Params.addString(player.getName());
				msg.Params.addItemName(newItem);
				msg.Params.addLong(item.getCount());
				_owner.sendPacket(msg);

				msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_PURCHASED_S3_S2_S_FROM_C1);
				msg.Params.addString(_owner.getName());
				msg.Params.addItemName(newItem);
				msg.Params.addLong(item.getCount());
				player.sendPacket(msg);
			}
			else
			{
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_PURCHASED_S2);
				msg.Params.addString(player.getName());
				msg.Params.addItemName(newItem);
				_owner.sendPacket(msg);

				msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_PURCHASED_S2_FROM_C1);
				msg.Params.addString(_owner.getName());
				msg.Params.addItemName(newItem);
				player.sendPacket(msg);
			}

			_owner.sendPacket(new ExPrivateStoreSellingResultPacket(item.getObjectId(), item.getCount(), player.getAppearance().getVisibleName()));
		}

		// Send inventory update packet
		_owner.sendInventoryUpdate(ownerIU);
		player.sendInventoryUpdate(playerIU);

		// Visual inconsistencies fix.
		_owner.sendItemList();
		player.sendItemList();

		return ok ? 0 : 2;
	}

	/**
	 * Sell items to this PrivateStore list
	 * @param player
	 * @param requestedItems
	 * @return : bool true if success
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public bool privateStoreSell(Player player, ItemRequest[] requestedItems)
	{
		if (_locked || !_owner.isOnline() || !player.isOnline())
		{
			return false;
		}

		bool ok = false;

		PlayerInventory ownerInventory = _owner.getInventory();
		PlayerInventory playerInventory = player.getInventory();

		// Prepare inventory update packet
		InventoryUpdatePacket ownerIU = new InventoryUpdatePacket();
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket();
		long totalPrice = 0;

		TradeItem[] sellerItems = _items.ToArray();
		foreach (ItemRequest item in requestedItems)
		{
			// searching item in tradelist using itemId
			bool found = false;
			foreach (TradeItem ti in sellerItems)
			{
				if (ti.getItem().getId() == item.getItemId())
				{
					// price should be the same
					if (ti.getPrice() == item.getPrice())
					{
						// if requesting more than available - decrease count
						if (ti.getCount() < item.getCount())
						{
							item.setCount(ti.getCount());
						}
						found = item.getCount() > 0;
					}
					break;
				}
			}
			// not found any item in the tradelist with same itemId and price
			// maybe another player already sold this item ?
			if (!found)
			{
				continue;
			}

			// check for overflow in the single item
			if (Inventory.MAX_ADENA / item.getCount() < item.getPrice())
			{
				@lock();
				break;
			}

			long _totalPrice = totalPrice + item.getCount() * item.getPrice();
			// check for overflow of the total price
			if (Inventory.MAX_ADENA < _totalPrice || _totalPrice < 0)
			{
				@lock();
				break;
			}

			if (ownerInventory.getAdena() < _totalPrice)
			{
				continue;
			}

			if (item.getObjectId() < 1 || item.getObjectId() > sellerItems.Length)
			{
				continue;
			}

			TradeItem tradeItem = sellerItems[item.getObjectId() - 1];
			if (tradeItem == null || tradeItem.getItem().getId() != item.getItemId())
			{
				continue;
			}

			// Check if requested item is available for manipulation
			int objectId = tradeItem.getObjectId();
			Item? oldItem = player.checkItemManipulation(objectId, item.getCount(), "sell");
			// private store - buy use same objectId for buying several non-stackable items
			if (oldItem == null)
			{
				// searching other items using same itemId
				oldItem = playerInventory.getItemByItemId(item.getItemId());
				if (oldItem == null)
				{
					continue;
				}
				objectId = oldItem.ObjectId;
				oldItem = player.checkItemManipulation(objectId, item.getCount(), "sell");
				if (oldItem == null)
				{
					continue;
				}
			}
			if (oldItem.getId() != item.getItemId())
			{
				Util.handleIllegalPlayerAction(player, player + " is cheating with sell items", Config.DEFAULT_PUNISH);
				return false;
			}

			if (!oldItem.isTradeable())
			{
				continue;
			}

			// Proceed with item transfer
			Item? newItem = playerInventory.transferItem("PrivateStore", objectId, item.getCount(), ownerInventory, player, _owner);
			if (newItem == null)
			{
				continue;
			}

			removeItem(-1, item.getItemId(), item.getCount());
			ok = true;

			PrivateStoreHistoryManager.getInstance().registerTransaction(PrivateStoreType.BUY, newItem, item.getCount(), item.getCount() * item.getPrice());

			// increase total price only after successful transaction
			totalPrice = _totalPrice;

			// Add changes to inventory update packets
			if (oldItem.getCount() > 0 && oldItem != newItem)
			{
				playerIU.addModifiedItem(oldItem);
			}
			else
			{
				playerIU.addRemovedItem(oldItem);
			}
			if (newItem.getCount() > item.getCount())
			{
				ownerIU.addModifiedItem(newItem);
			}
			else
			{
				ownerIU.addNewItem(newItem);
			}

			// Send messages about the transaction to both players
			if (newItem.isStackable())
			{
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_PURCHASED_S3_S2_S_FROM_C1);
				msg.Params.addString(player.getName());
				msg.Params.addItemName(newItem);
				msg.Params.addLong(item.getCount());
				_owner.sendPacket(msg);

				msg = new SystemMessagePacket(SystemMessageId.C1_PURCHASED_S3_S2_S);
				msg.Params.addString(_owner.getName());
				msg.Params.addItemName(newItem);
				msg.Params.addLong(item.getCount());
				player.sendPacket(msg);
			}
			else
			{
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_PURCHASED_S2_FROM_C1);
				msg.Params.addString(player.getName());
				msg.Params.addItemName(newItem);
				_owner.sendPacket(msg);

				msg = new SystemMessagePacket(SystemMessageId.C1_PURCHASED_S2);
				msg.Params.addString(_owner.getName());
				msg.Params.addItemName(newItem);
				player.sendPacket(msg);
			}

			_owner.sendPacket(new ExPrivateStoreBuyingResultPacket(item.getObjectId(), item.getCount(), player.getAppearance().getVisibleName()));
		}

		if (totalPrice > 0)
		{
			// Transfer adena
			if (totalPrice > ownerInventory.getAdena())
			{
				// should not happen, just a precaution
				return false;
			}

            Item? adenaItem = ownerInventory.getAdenaInstance();
			ownerInventory.reduceAdena("PrivateStore", totalPrice, _owner, player);
            if (adenaItem != null)
			    ownerIU.addItem(adenaItem);

			playerInventory.addAdena("PrivateStore", totalPrice, player, _owner);
            Item? playerAdena = playerInventory.getAdenaInstance();
            if (playerAdena != null)
			    playerIU.addItem(playerAdena);
		}

		if (ok)
		{
			// Send inventory update packet
			_owner.sendInventoryUpdate(ownerIU);
			player.sendInventoryUpdate(playerIU);

			// Visual inconsistencies fix.
			_owner.sendItemList();
			player.sendItemList();
		}
		return ok;
	}
}