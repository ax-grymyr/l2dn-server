using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Index
 */
public class WorldExchangeManager: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(WorldExchangeManager));
	private readonly Map<long, WorldExchangeHolder> _itemBids = new();
	private readonly Map<int, WorldExchangeItemSubType> _itemCategories = new();
	private readonly Map<String, Map<int, String>> _localItemNames = new();
	private int _lastWorldExchangeId;
	
	private ScheduledFuture _checkStatus = null;
	
	public WorldExchangeManager()
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		firstLoad();
		if (_checkStatus == null)
		{
			TimeSpan interval = TimeSpan.FromMilliseconds(Config.WORLD_EXCHANGE_SAVE_INTERVAL);
			_checkStatus = ThreadPool.scheduleAtFixedRate(checkBidStatus, interval, interval);
		}
	}
	
	public void load()
	{
		// if (Config.MULTILANG_ENABLE)
		// {
		// 	_localItemNames.clear();
		// 	foreach (String lang in Config.MULTILANG_ALLOWED)
		// 	{
		// 		File file = new File("data/lang/" + lang + "/ItemNameLocalisation.xml");
		// 		if (!file.isFile())
		// 		{
		// 			continue;
		// 		}
		// 		
		// 		parseDatapackFile("data/lang/" + lang + "/ItemNameLocalisation.xml");
		// 		int size = _localItemNames.get(lang).size();
		// 		if (size == 0)
		// 		{
		// 			_localItemNames.remove(lang);
		// 		}
		// 		else
		// 		{
		// 			LOGGER.Info(GetType().Name + ": Loaded ItemName localisations for [" + lang + "].");
		// 		}
		// 	}
		// }
		//
		// if (!Config.MULTILANG_DEFAULT.equals(Config.WORLD_EXCHANGE_DEFAULT_LANG) && !_localItemNames.containsKey(Config.WORLD_EXCHANGE_DEFAULT_LANG))
		// {
		// 	parseDatapackFile("data/lang/" + Config.WORLD_EXCHANGE_DEFAULT_LANG + "/ItemNameLocalisation.xml");
		// }
	}
	
	// public void parseDocument(Document doc, File f)
	// {
	// 	Map<int, String> local = new();
	// 	forEach(doc, "list", listNode =>
	// 	{
	// 		forEach(listNode, "blessed", itemNode =>
	// 		{
	// 			StatSet itemSet = new StatSet(parseAttributes(itemNode));
	// 			local.put(-1, itemSet.getString("name"));
	// 		});
	// 		forEach(listNode, "item", itemNode =>
	// 		{
	// 			StatSet itemSet = new StatSet(parseAttributes(itemNode));
	// 			local.put(itemSet.getInt("id"), itemSet.getString("name"));
	// 		});
	// 	});
	// 	_localItemNames.put(doc.getDocumentURI().split("data/lang/")[1].split("/")[0], local);
	// }
	
	public Map<int, String> getItemLocalByLang(String lang)
	{
		return _localItemNames.get(lang);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	void firstLoad()
	{
		Map<int, Item> itemInstances = loadItemInstances();
		loadItemBids(itemInstances);
		load();
	}
	
	/**
	 * Little task which check and update bid items if it needs.
	 */
	private void checkBidStatus()
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		foreach (var entry in _itemBids)
		{
			WorldExchangeHolder holder = entry.Value;
			DateTime currentTime = DateTime.UtcNow;
			DateTime endTime = holder.getEndTime();
			if (endTime > currentTime)
			{
				continue;
			}
			
			switch (holder.getStoreType())
			{
				case WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE:
				{
					_itemBids.remove(entry.Key);
					continue;
				}
				case WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED:
				{
					holder.setEndTime(calculateDate(Config.WORLD_EXCHANGE_ITEM_BACK_PERIOD));
					holder.setStoreType(WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME);
					_itemBids.replace(entry.Key, holder);
					insert(entry.Key, false);
					break;
				}
				case WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD:
				case WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME:
				{
					holder.setStoreType(WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE);
					insert(entry.Key, true);
					Item item = holder.getItemInstance();
					item.setItemLocation(ItemLocation.VOID);
					item.updateDatabase(!Config.WORLD_EXCHANGE_LAZY_UPDATE);
					break;
				}
			}
		}
	}
	
	/**
	 * Load items from database for make proper holders
	 * @return
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	Map<int, Item> loadItemInstances()
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return new();
		}
		
		Map<int, Item> itemInstances = new();
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (DbItem record in ctx.Items.Where(i => i.Location == (int)ItemLocation.EXCHANGE))
			{
				Item itemInstance = new Item(record);
				itemInstances.put(itemInstance.getObjectId(), itemInstance);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed loading items instances." + e);
		}
		
		return itemInstances;
	}
	
	/**
	 * Loading all items, which used or using in World Exchange.
	 * @param itemInstances
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	private void loadItemBids(Map<int, Item> itemInstances)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (WorldExchangeItem record in ctx.WorldExchangeItems)
			{
				bool needChange = false;
				int worldExchangeId = record.Id;
				_lastWorldExchangeId = Math.Max(worldExchangeId, _lastWorldExchangeId);
				Item itemInstance = itemInstances.get(record.ItemObjectId);
				WorldExchangeItemStatusType storeType = (WorldExchangeItemStatusType)record.ItemStatus;
				
				if (storeType == WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE)
				{
					continue;
				}
				
				if (itemInstance == null)
				{
					LOGGER.Warn(GetType().Name + ": Failed loading commission item with world exchange id " + worldExchangeId + " because item instance does not exist or failed to load.");
					continue;
				}
				
				WorldExchangeItemSubType categoryId = (WorldExchangeItemSubType)record.CategoryId;
				long price = record.Price;
				int bidPlayerObjectId = record.OldOwnerId;
				DateTime startTime = record.StartTime;
				DateTime endTime = record.EndTime;
				if (endTime < DateTime.UtcNow)
				{
					if ((storeType == WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME) ||
					    (storeType == WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD))
					{
						itemInstance.setItemLocation(ItemLocation.VOID);
						itemInstance.updateDatabase(true);
						continue;
					}

					endTime = calculateDate(Config.WORLD_EXCHANGE_ITEM_BACK_PERIOD);
					storeType = WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME;
					needChange = true;
				}
				_itemBids.put(worldExchangeId, new WorldExchangeHolder(worldExchangeId, itemInstance, new ItemInfo(itemInstance), price, bidPlayerObjectId, storeType, categoryId, startTime, endTime, needChange));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed loading bid items." + e);
		}
	}
	
	private long calculateFeeForRegister(Player player, int objectId, long amount, long priceForEach)
	{
		Item itemToRemove = player.getInventory().getItemByObjectId(objectId);
		if (itemToRemove.getId() == Inventory.ADENA_ID)
		{
			return priceForEach * 100L;
		}
		return (long)(priceForEach * (itemToRemove.getId() == Inventory.ADENA_ID ? 1 : amount) * Config.WORLD_EXCHANGE_ADENA_FEE);
	}
	
	/**
	 * Forwarded from client packet "ExWorldExchangeRegisterItem" for check ops and register item if it can in World Exchange system
	 * @param player
	 * @param itemObjectId
	 * @param amount
	 * @param priceForEach
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void registerItemBid(Player player, int itemObjectId, long amount, long priceForEach)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		Map<WorldExchangeItemStatusType, List<WorldExchangeHolder>> playerBids = getPlayerBids(player.getObjectId());
		if (playerBids.size() >= 10)
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.NO_SLOTS_AVAILABLE));
			player.sendPacket(WorldExchangeRegisterItemPacket.FAIL);
			return;
		}
		if (player.getInventory().getItemByObjectId(itemObjectId) == null)
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeRegisterItemPacket.FAIL);
			return;
		}
		if ((amount < 1) || (priceForEach < 1) || ((amount * priceForEach) < 1))
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.INCORRECT_ITEM_COUNT_2));
			player.sendPacket(WorldExchangeRegisterItemPacket.FAIL);
			return;
		}
		
		Item item = player.getInventory().getItemByObjectId(itemObjectId);
		long feePrice = calculateFeeForRegister(player, itemObjectId, amount, priceForEach);
		if ((Config.WORLD_EXCHANGE_MAX_ADENA_FEE != -1) && (feePrice > Config.WORLD_EXCHANGE_MAX_ADENA_FEE))
		{
			feePrice = Config.WORLD_EXCHANGE_MAX_ADENA_FEE;
		}
		if (feePrice > player.getAdena())
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.NOT_ENOUGH_ADENA));
			player.sendPacket(WorldExchangeRegisterItemPacket.FAIL);
			return;
		}
		if (feePrice < 1)
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.INCORRECT_ITEM_COUNT_2));
			player.sendPacket(WorldExchangeRegisterItemPacket.FAIL);
			return;
		}
		
		int freeId = getNextId();
		InventoryUpdatePacket iu;
		if (item.isStackable() && (player.getInventory().getInventoryItemCount(item.getId(), -1) > amount))
		{
			iu = new InventoryUpdatePacket(new ItemInfo(item, ItemChangeType.MODIFIED));
		}
		else
		{
			iu = new InventoryUpdatePacket(new ItemInfo(item, ItemChangeType.REMOVED));
		}
		
		Item itemInstance = player.getInventory().detachItem("World Exchange Registration", item, amount, ItemLocation.EXCHANGE, player, null);
		if (itemInstance == null)
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeRegisterItemPacket.FAIL);
			return;
		}
		
		WorldExchangeItemSubType category = _itemCategories.get(itemInstance.getId());
		if (category == null)
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_YOU_REGISTERED_HAS_BEEN_SOLD));
			player.sendPacket(WorldExchangeRegisterItemPacket.FAIL);
			return;
		}
		
		player.sendPacket(iu);
		player.getInventory().reduceAdena("World Exchange Registration", feePrice, player, null);
		DateTime endTime = calculateDate(Config.WORLD_EXCHANGE_ITEM_SELL_PERIOD);
		_itemBids.put(freeId,
			new WorldExchangeHolder(freeId, itemInstance, new ItemInfo(itemInstance), priceForEach,
				player.getObjectId(), WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED, category,
				DateTime.UtcNow, endTime, true));
		
		player.sendPacket(new WorldExchangeRegisterItemPacket(itemObjectId, amount, 1));
		if (!Config.WORLD_EXCHANGE_LAZY_UPDATE)
		{
			insert(freeId, false);
		}
	}
	
	private int getNextId()
	{
		return Interlocked.Increment(ref _lastWorldExchangeId);
	}
	
	private DateTime calculateDate(int days)
	{
		return DateTime.UtcNow.AddDays(days);
	}
	
	/**
	 * Forwarded from ExWorldExchangeSettleRecvResult for make Action, because client send only WORLD EXCHANGE Index without anu addition info.
	 * @param player
	 * @param worldExchangeIndex
	 */
	public void getItemStatusAndMakeAction(Player player, long worldExchangeIndex)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		WorldExchangeHolder worldExchangeItem = _itemBids.get(worldExchangeIndex);
		if (worldExchangeItem == null)
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		WorldExchangeItemStatusType storeType = worldExchangeItem.getStoreType();
		switch (storeType)
		{
			case WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED:
			{
				cancelBid(player, worldExchangeItem);
				break;
			}
			case WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD:
			{
				takeBidMoney(player, worldExchangeItem);
				break;
			}
			case WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME:
			{
				returnItem(player, worldExchangeItem);
				break;
			}
		}
	}
	
	/**
	 * Forwarded from getItemStatusAndMakeAction / remove item and holder from active bid and take it back to owner.
	 * @param player
	 * @param worldExchangeItem
	 */
	private void cancelBid(Player player, WorldExchangeHolder worldExchangeItem)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		if (worldExchangeItem.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (!_itemBids.containsKey(worldExchangeItem.getWorldExchangeId()))
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (_itemBids.get(worldExchangeItem.getWorldExchangeId()) != worldExchangeItem)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (player.getObjectId() != worldExchangeItem.getOldOwnerId())
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.ITEM_OUT_OF_STOCK));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (worldExchangeItem.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_YOU_REGISTERED_HAS_BEEN_SOLD));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		player.sendPacket(new WorldExchangeSettleRecvResultPacket(worldExchangeItem.getItemInstance().getObjectId(), worldExchangeItem.getItemInstance().getCount(), (byte) 1));
		player.getInventory().addItem("World Exchange Cancellation", worldExchangeItem.getItemInstance(), player, player);
		worldExchangeItem.setStoreType(WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE);
		worldExchangeItem.setHasChanges(true);
		_itemBids.replace(worldExchangeItem.getWorldExchangeId(), worldExchangeItem);
		if (!Config.WORLD_EXCHANGE_LAZY_UPDATE)
		{
			insert(worldExchangeItem.getWorldExchangeId(), true);
		}
	}
	
	/**
	 * Forwarded from getItemStatusAndMakeAction / takes money from bid.
	 * @param player
	 * @param worldExchangeItem
	 */
	private void takeBidMoney(Player player, WorldExchangeHolder worldExchangeItem)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		if (worldExchangeItem.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (!_itemBids.containsKey(worldExchangeItem.getWorldExchangeId()))
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (_itemBids.get(worldExchangeItem.getWorldExchangeId()) != worldExchangeItem)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (player.getObjectId() != worldExchangeItem.getOldOwnerId())
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (worldExchangeItem.getStoreType() != WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_YOU_REGISTERED_HAS_BEEN_SOLD));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (worldExchangeItem.getEndTime() < DateTime.UtcNow)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_REGISTRATION_PERIOD_FOR_THE_ITEM_YOU_REGISTERED_HAS_EXPIRED));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		player.sendPacket(new WorldExchangeSettleRecvResultPacket(worldExchangeItem.getItemInstance().getObjectId(), worldExchangeItem.getItemInstance().getCount(), (byte) 1));
		long fee = (long)(worldExchangeItem.getPrice() * Config.WORLD_EXCHANGE_LCOIN_TAX * 100 / 100);
		long returnPrice = worldExchangeItem.getPrice() - Math.Min(fee, (Config.WORLD_EXCHANGE_MAX_LCOIN_TAX != -1 ? Config.WORLD_EXCHANGE_MAX_LCOIN_TAX : long.MaxValue)); // floating-point accuracy workaround :D
		player.getInventory().addItem("World Exchange Took Money", Inventory.LCOIN_ID, (returnPrice), player, null);
		worldExchangeItem.setStoreType(WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE);
		Item item = worldExchangeItem.getItemInstance();
		item.setItemLocation(ItemLocation.VOID);
		item.updateDatabase(!Config.WORLD_EXCHANGE_LAZY_UPDATE);
		worldExchangeItem.setHasChanges(true);
		_itemBids.replace(worldExchangeItem.getWorldExchangeId(), worldExchangeItem);
		if (!Config.WORLD_EXCHANGE_LAZY_UPDATE)
		{
			insert(worldExchangeItem.getWorldExchangeId(), true);
		}
	}
	
	/**
	 * Forwarded from getItemStatusAndMakeAction / take back item which placed on World Exchange.
	 * @param player
	 * @param worldExchangeItem
	 */
	private void returnItem(Player player, WorldExchangeHolder worldExchangeItem)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		if (worldExchangeItem.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (!_itemBids.containsKey(worldExchangeItem.getWorldExchangeId()))
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (_itemBids.get(worldExchangeItem.getWorldExchangeId()) != worldExchangeItem)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.ITEM_OUT_OF_STOCK));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (player.getObjectId() != worldExchangeItem.getOldOwnerId())
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.ITEM_TO_BE_TRADED_DOES_NOT_EXIST));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (worldExchangeItem.getStoreType() != WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.ITEM_OUT_OF_STOCK));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		if (worldExchangeItem.getEndTime() < DateTime.UtcNow)
		{
			player.sendPacket(new WorldExchangeSettleListPacket(player));
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_REGISTRATION_PERIOD_FOR_THE_ITEM_YOU_REGISTERED_HAS_EXPIRED));
			player.sendPacket(WorldExchangeSettleRecvResultPacket.FAIL);
			return;
		}
		
		player.sendPacket(new WorldExchangeSettleRecvResultPacket(worldExchangeItem.getItemInstance().getObjectId(), worldExchangeItem.getItemInstance().getCount(), (byte) 1));
		player.getInventory().addItem("World Exchange Took Out Time Item Back", worldExchangeItem.getItemInstance(), player, null);
		worldExchangeItem.setStoreType(WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE);
		worldExchangeItem.setHasChanges(true);
		_itemBids.replace(worldExchangeItem.getWorldExchangeId(), worldExchangeItem);
		if (!Config.WORLD_EXCHANGE_LAZY_UPDATE)
		{
			insert(worldExchangeItem.getWorldExchangeId(), true);
		}
	}
	
	/**
	 * Forwarded from ExWorldExchangeBuyItem / request for but item and create a visible clone for old owner.
	 * @param player
	 * @param worldExchangeId
	 */
	public void buyItem(Player player, int worldExchangeId)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		if (!_itemBids.containsKey(worldExchangeId))
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeBuyItemPacket.FAIL);
			return;
		}
		
		WorldExchangeHolder worldExchangeItem = _itemBids.get(worldExchangeId);
		if (worldExchangeItem.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE)
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.THE_ITEM_IS_NOT_FOUND));
			player.sendPacket(WorldExchangeBuyItemPacket.FAIL);
			return;
		}
		
		if (worldExchangeItem.getStoreType() != WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED)
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.ITEM_OUT_OF_STOCK));
			player.sendPacket(WorldExchangeBuyItemPacket.FAIL);
			return;
		}
		
		Item lcoin = player.getInventory().getItemByItemId(Inventory.LCOIN_ID);
		if ((lcoin == null) || (lcoin.getCount() < worldExchangeItem.getPrice()))
		{
			player.sendPacket(new SystemMessagePacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_L2_COINS_ADD_MORE_L2_COINS_AND_TRY_AGAIN));
			player.sendPacket(WorldExchangeBuyItemPacket.FAIL);
			return;
		}
		
		player.getInventory().destroyItem("World Exchange Buying", lcoin, worldExchangeItem.getPrice(), player, null);
		Item newItem = createItem(worldExchangeItem.getItemInstance(), player);
		DateTime destroyTime = calculateDate(Config.WORLD_EXCHANGE_PAYMENT_TAKE_PERIOD);
		WorldExchangeHolder newHolder = new WorldExchangeHolder(worldExchangeId, newItem, new ItemInfo(newItem), worldExchangeItem.getPrice(), worldExchangeItem.getOldOwnerId(), WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD, worldExchangeItem.getCategory(), worldExchangeItem.getStartTime(), destroyTime, true);
		_itemBids.replace(worldExchangeId, worldExchangeItem, newHolder);
		if (!Config.WORLD_EXCHANGE_LAZY_UPDATE)
		{
			insert(worldExchangeItem.getWorldExchangeId(), false);
		}
		Item receivedItem = player.getInventory().addItem("World Exchange Buying", worldExchangeItem.getItemInstance(), player, null);
		player.sendPacket(new WorldExchangeBuyItemPacket(receivedItem.getObjectId(), receivedItem.getCount(), (byte) 1));
		SystemMessagePacket sm;
		if (receivedItem.getEnchantLevel() > 0)
		{
			if (receivedItem.getCount() < 2)
			{
				sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_S2_4);
				sm.Params.addByte(receivedItem.getEnchantLevel());
				sm.Params.addItemName(receivedItem);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_S2_X_S3);
				sm.Params.addItemName(receivedItem);
				sm.Params.addLong(receivedItem.getCount());
				sm.Params.addByte(receivedItem.getEnchantLevel());
			}
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
			sm.Params.addItemName(receivedItem);
			sm.Params.addLong(receivedItem.getCount());
		}
		
		player.sendPacket(sm);
		foreach (Player oldOwner in World.getInstance().getPlayers())
		{
			if (oldOwner.getObjectId() == newHolder.getOldOwnerId())
			{
				oldOwner.sendPacket(new WorldExchangeSellCompleteAlarmPacket(newItem.getId(), newItem.getCount()));
				break;
			}
		}
	}
	
	/**
	 * Create a new item for make it visible in UI for old owner.
	 * @param oldItem item from holder which will be "cloned"
	 * @param requestor
	 * @return cloned item
	 */
	private Item createItem(Item oldItem, Player requestor)
	{
		Item newItem = new Item(oldItem.getId());
		newItem.setOwnerId(requestor.getObjectId());
		newItem.setEnchantLevel(oldItem.getEnchantLevel() < 1 ? 0 : oldItem.getEnchantLevel());
		newItem.setItemLocation(ItemLocation.EXCHANGE);
		newItem.setCount(oldItem.getCount());
		newItem.setVisualId(oldItem.getVisualId(), false);
		newItem.setBlessed(oldItem.isBlessed());
		newItem.setOwnerId(oldItem.getOwnerId());
		newItem.updateDatabase(true); // in any case it will be store in database
		VariationInstance vi = oldItem.getAugmentation();
		if (vi != null)
		{
			newItem.setAugmentation(vi, true);
		}
		
		InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(newItem, ItemChangeType.REMOVED));
		requestor.sendInventoryUpdate(iu);
		return newItem;
	}
	
	/**
	 * @param ownerId
	 * @param type
	 * @param sortType
	 * @param lang
	 * @return items, which player can buy
	 */
	public List<WorldExchangeHolder> getItemBids(int ownerId, WorldExchangeItemSubType type, WorldExchangeSortType sortType, String lang)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return new();
		}
		
		List<WorldExchangeHolder> returnList = new();
		foreach (WorldExchangeHolder holder in _itemBids.values())
		{
			if (holder.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE)
			{
				continue;
			}
			
			if ((holder.getOldOwnerId() == ownerId) || (holder.getCategory() != type))
			{
				continue;
			}
			
			if (holder.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED)
			{
				returnList.add(holder);
			}
		}
		
		return sortList(returnList, sortType, lang);
	}
	
	/**
	 * @param ids
	 * @param sortType
	 * @param lang
	 * @return items with the same id (used in registration, where shows similar items with price)
	 */
	public List<WorldExchangeHolder> getItemBids(List<int> ids, WorldExchangeSortType sortType, String lang)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return new();
		}
		
		List<WorldExchangeHolder> returnList = new();
		foreach (WorldExchangeHolder holder in _itemBids.values())
		{
			if (holder.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE)
			{
				continue;
			}
			
			if (ids.Contains(holder.getItemInstance().getId()) && (holder.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED))
			{
				returnList.add(holder);
			}
		}
		
		return sortList(returnList, sortType, lang);
	}
	
	/**
	 * @param unsortedList
	 * @param sortType
	 * @param lang
	 * @return sort items by type if it needs 399 - that max value which can been in list buffer size - 32768 - list has 11 + cycle of 82 bytes - 32768 / 82 = 399.6 = 32718 for item info + 50 reserved = 32729 item info and initial data + 39 reserved
	 */
	private List<WorldExchangeHolder> sortList(List<WorldExchangeHolder> unsortedList, WorldExchangeSortType sortType, String lang)
	{
		List<WorldExchangeHolder> sortedList = new(unsortedList);
		switch (sortType)
		{
			case WorldExchangeSortType.PRICE_ASCE:
			{
				sortedList.Sort((a, b) => a.getPrice().CompareTo(b.getPrice()));
				break;
			}
			case WorldExchangeSortType.PRICE_DESC:
			{
				sortedList.Sort((a, b) => -a.getPrice().CompareTo(b.getPrice()));
				break;
			}
			case WorldExchangeSortType.ITEM_NAME_ASCE:
			{
				if ((lang == null) || (!lang.equals("en") && _localItemNames.containsKey(lang)))
				{
					// TODO extract to comparer classes
					sortedList.Sort((a, b) =>
						getItemName(lang, a.getItemInstance().getId(), a.getItemInstance().isBlessed())
							.CompareTo(getItemName(lang, b.getItemInstance().getId(),
								b.getItemInstance().isBlessed())));
				}
				else
				{
					sortedList.Sort((a, b) => 
						((a.getItemInstance().isBlessed() ? "Blessed " : "") + a.getItemInstance().getItemName()).CompareTo(
							((b.getItemInstance().isBlessed() ? "Blessed " : "") + b.getItemInstance().getItemName())));
				}
				break;
			}
			case WorldExchangeSortType.ITEM_NAME_DESC:
			{
				if ((lang == null) || (!lang.equals("en") && _localItemNames.containsKey(lang)))
				{
					// TODO extract to comparer classes
					sortedList.Sort((a, b) =>
						-getItemName(lang, a.getItemInstance().getId(), a.getItemInstance().isBlessed())
							.CompareTo(getItemName(lang, b.getItemInstance().getId(),
								b.getItemInstance().isBlessed())));
				}
				else
				{
					sortedList.Sort((a, b) => 
						-((a.getItemInstance().isBlessed() ? "Blessed " : "") + a.getItemInstance().getItemName()).CompareTo(
							((b.getItemInstance().isBlessed() ? "Blessed " : "") + b.getItemInstance().getItemName())));
				}
				break;
			}
			case WorldExchangeSortType.PRICE_PER_PIECE_ASCE:
			{
				sortedList.Sort((a, b) => a.getPrice().CompareTo(b.getPrice()));
				break;
			}
			case WorldExchangeSortType.PRICE_PER_PIECE_DESC:
			{
				sortedList.Sort((a, b) => -a.getPrice().CompareTo(b.getPrice()));
				break;
			}
		}
		
		if (sortedList.size() > 399)
		{
			return sortedList[..399];
		}
		
		return sortedList;
	}
	
	private String getItemName(String lang, int id, bool isBlessed)
	{
		if (!_localItemNames.containsKey(lang))
		{
			return "";
		}
		
		Map<int, String> names = _localItemNames.get(lang);
		String name = names.get(id);
		if (name == null)
		{
			return "";
		}
		
		if (isBlessed)
		{
			return names.get(-1) + " " + name;
		}
		
		return name;
	}
	
	/**
	 * @param ownerId
	 * @return items which will bid player
	 */
	public Map<WorldExchangeItemStatusType, List<WorldExchangeHolder>> getPlayerBids(int ownerId)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return new();
		}
		
		List<WorldExchangeHolder> registered = new();
		List<WorldExchangeHolder> sold = new();
		List<WorldExchangeHolder> outTime = new();
		foreach (WorldExchangeHolder holder in _itemBids.values())
		{
			if (holder.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_NONE)
			{
				continue;
			}
			
			if (holder.getOldOwnerId() != ownerId)
			{
				continue;
			}
			
			switch (holder.getStoreType())
			{
				case WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED:
				{
					registered.add(holder);
					break;
				}
				case WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD:
				{
					sold.add(holder);
					break;
				}
				case WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME:
				{
					outTime.add(holder);
					break;
				}
			}
		}
		
		Map<WorldExchangeItemStatusType, List<WorldExchangeHolder>> returnMap = new();
		returnMap.put(WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED, registered);
		returnMap.put(WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD, sold);
		returnMap.put(WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME, outTime);
		return returnMap;
	}
	
	public void addCategoryType(List<int> itemIds, int category)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		foreach (int itemId in itemIds)
		{
			_itemCategories.putIfAbsent(itemId, (WorldExchangeItemSubType)category);
		}
	}
	
	/**
	 * Will send player alarm on WorldEnter if player has success sold items or items, if time is out
	 * @param player
	 */
	public void checkPlayerSellAlarm(Player player)
	{
		if (!Config.ENABLE_WORLD_EXCHANGE)
		{
			return;
		}
		
		foreach (WorldExchangeHolder holder in _itemBids.values())
		{
			if ((holder.getOldOwnerId() == player.getObjectId()) && ((holder.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD) || (holder.getStoreType() == WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME)))
			{
				player.sendPacket(new WorldExchangeSellCompleteAlarmPacket(holder.getItemInstance().getId(), holder.getItemInstance().getCount()));
				break;
			}
		}
	}
	
	public void storeMe()
	{
		if (!Config.ENABLE_WORLD_EXCHANGE || !Config.WORLD_EXCHANGE_LAZY_UPDATE)
		{
			return;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (WorldExchangeHolder holder in _itemBids.values())
			{
				if (!holder.hasChanges())
				{
					continue;
				}
				
				ctx.WorldExchangeItems.Add(new WorldExchangeItem() // TODO: it was REPLACE statement
				{
					Id = holder.getWorldExchangeId(),
					ItemObjectId = holder.getItemInstance().getObjectId(),
					ItemStatus = (int)holder.getStoreType(),
					CategoryId = (int)holder.getCategory(),
					Price = holder.getPrice(),
					OldOwnerId = holder.getOldOwnerId(),
					StartTime = holder.getStartTime(),
					EndTime = holder.getEndTime()
				});

			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Error while saving World Exchange item bids:\n" + e);
		}
	}
	
	public void insert(long worldExchangeId, bool remove)
	{
		if (Config.WORLD_EXCHANGE_LAZY_UPDATE)
		{
			return;
		}
		
		try 
		{
			WorldExchangeHolder holder = _itemBids.get(worldExchangeId);

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.WorldExchangeItems.Add(new WorldExchangeItem() // TODO: it was REPLACE statement
			{
				Id = holder.getWorldExchangeId(),
				ItemObjectId = holder.getItemInstance().getObjectId(),
				ItemStatus = (int)holder.getStoreType(),
				CategoryId = (int)holder.getCategory(),
				Price = holder.getPrice(),
				OldOwnerId = holder.getOldOwnerId(),
				StartTime = holder.getStartTime(),
				EndTime = holder.getEndTime()
			});

			ctx.SaveChanges();

			if (remove)
			{
				_itemBids.remove(worldExchangeId);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error while saving World Exchange item bid " + worldExchangeId + "\n" + e);
		}
	}
	
	/**
	 * Returns the average price of the specified item.
	 * @param itemId the ID of the item
	 * @return the average price, or 0 if there are no items with the specified ID
	 */
	public long getAveragePriceOfItem(int itemId)
	{
		long totalPrice = 0;
		long totalItemCount = 0;
		foreach (WorldExchangeHolder holder in _itemBids.values())
		{
			if (holder.getItemInstance().getTemplate().getId() != itemId)
			{
				continue;
			}
			
			totalItemCount++;
			totalPrice += holder.getPrice();
		}
		return totalItemCount == 0 ? 0 : totalPrice / totalItemCount;
	}
	
	public static WorldExchangeManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly WorldExchangeManager INSTANCE = new WorldExchangeManager();
	}
}