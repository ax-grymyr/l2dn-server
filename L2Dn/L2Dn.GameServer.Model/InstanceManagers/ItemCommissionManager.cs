using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Commission;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author NosBit, Ren
 */
public class ItemCommissionManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemCommissionManager));

	private const int INTERACTION_DISTANCE = 250;
	private const int ITEMS_LIMIT_PER_REQUEST = 999;
	private const int MAX_ITEMS_REGISTRED_PER_PLAYER = 10;
	private static readonly long MIN_REGISTRATION_AND_SALE_FEE = 1000;
	private static readonly double REGISTRATION_FEE_PER_DAY = 0.0001;
	private static readonly double SALE_FEE_PER_DAY = 0.005;
	private static readonly int[] DURATION = [1, 3, 5, 7, 15, 30];

	private readonly Map<long, CommissionItem> _commissionItems = new();

	protected ItemCommissionManager()
	{
		Map<int, Item> itemInstances = new();
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (DbItem item in ctx.Items.Where(r => r.Location == (int)ItemLocation.COMMISSION))
			{
				Item itemInstance = new Item(item);
				itemInstances.put(itemInstance.ObjectId, itemInstance);
			}

			foreach (DbCommissionItem item in ctx.CommissionItems)
			{
				int commissionId = item.Id;
				Item? itemInstance = itemInstances.get(item.ItemObjectId);
				if (itemInstance == null)
				{
					LOGGER.Error(GetType().Name + ": Failed loading commission item with commission id " + commissionId + " because item instance does not exist or failed to load.");
					continue;
				}

				CommissionItem commissionItem = new CommissionItem(commissionId, itemInstance, item.PricePerUnit,
					item.StartTime, item.DurationInDays, item.DiscountInPercentage);

				_commissionItems.put(commissionItem.getCommissionId(), commissionItem);
				if (commissionItem.getEndTime() < DateTime.UtcNow)
				{
					expireSale(commissionItem);
				}
				else
				{
					TimeSpan delay = commissionItem.getEndTime() - DateTime.UtcNow;
					if (delay < TimeSpan.Zero)
						delay = TimeSpan.Zero;

					commissionItem.setSaleEndTask(ThreadPool.schedule(() => expireSale(commissionItem), delay));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Failed loading commission items." + e);
		}
	}

	/**
	 * Shows the player the auctions filtered by filter.
	 * @param player the player
	 * @param filter the filter
	 */
	public void showAuctions(Player player, Predicate<ItemTemplate> filter)
	{
		List<CommissionItem> commissionItems = new();
		foreach (CommissionItem item in _commissionItems.Values)
		{
			if (filter(item.getItemInfo().getItem()))
			{
				commissionItems.Add(item);
				if (commissionItems.Count >= ITEMS_LIMIT_PER_REQUEST)
				{
					break;
				}
			}
		}

		if (commissionItems.Count == 0)
		{
			player.sendPacket(new ExResponseCommissionListPacket(CommissionListReplyType.ITEM_DOES_NOT_EXIST));
			return;
		}

		int chunks = commissionItems.Count / ExResponseCommissionListPacket.MAX_CHUNK_SIZE;
		if (commissionItems.Count > chunks * ExResponseCommissionListPacket.MAX_CHUNK_SIZE)
		{
			chunks++;
		}

		for (int i = chunks - 1; i >= 0; i--)
		{
			player.sendPacket(new ExResponseCommissionListPacket(CommissionListReplyType.AUCTIONS, commissionItems, i,
				i * ExResponseCommissionListPacket.MAX_CHUNK_SIZE));
		}
	}

	/**
	 * Shows the player his auctions.
	 * @param player the player
	 */
	public void showPlayerAuctions(Player player)
	{
		List<CommissionItem> commissionItems = new();
		foreach (CommissionItem c in _commissionItems.Values)
		{
			if (c.getItemInstance().getOwnerId() == player.ObjectId)
			{
				commissionItems.Add(c);
				if (commissionItems.Count == MAX_ITEMS_REGISTRED_PER_PLAYER)
				{
					break;
				}
			}
		}

		if (commissionItems.Count != 0)
		{
			player.sendPacket(new ExResponseCommissionListPacket(CommissionListReplyType.PLAYER_AUCTIONS, commissionItems));
		}
		else
		{
			player.sendPacket(new ExResponseCommissionListPacket(CommissionListReplyType.PLAYER_AUCTIONS_EMPTY));
		}
	}

	/**
	 * Registers an item for the given player.
	 * @param player the player
	 * @param itemObjectId the item object id
	 * @param itemCount the item count
	 * @param pricePerUnit the price per unit
	 * @param durationType the duration type
	 * @param discountInPercentage the discount type
	 */
	public void registerItem(Player player, int itemObjectId, long itemCount, long pricePerUnit, int durationType, byte discountInPercentage)
	{
		if (itemCount < 1)
		{
			player.sendPacket(SystemMessageId.THE_ITEM_HAS_FAILED_TO_BE_REGISTERED);
			player.sendPacket(ExResponseCommissionRegisterPacket.FAILED);
			return;
		}

		long totalPrice = itemCount * pricePerUnit;
		if (totalPrice <= MIN_REGISTRATION_AND_SALE_FEE)
		{
			player.sendPacket(SystemMessageId.THE_ITEM_CANNOT_BE_REGISTERED_BECAUSE_REQUIREMENTS_ARE_NOT_MET);
			player.sendPacket(ExResponseCommissionRegisterPacket.FAILED);
			return;
		}

		Item? itemInstance = player.getInventory().getItemByObjectId(itemObjectId);
		if (itemInstance == null || !itemInstance.isAvailable(player, false, false) || itemInstance.getCount() < itemCount)
		{
			player.sendPacket(SystemMessageId.THE_ITEM_HAS_FAILED_TO_BE_REGISTERED);
			player.sendPacket(ExResponseCommissionRegisterPacket.FAILED);
			return;
		}

		byte durationInDays = (byte) DURATION[durationType];

		lock (this)
		{
			long playerRegisteredItems = 0;
			foreach (CommissionItem item in _commissionItems.Values)
			{
				if (item.getItemInstance().getOwnerId() == player.ObjectId)
				{
					playerRegisteredItems++;
				}
			}

			if (playerRegisteredItems >= MAX_ITEMS_REGISTRED_PER_PLAYER)
			{
				player.sendPacket(SystemMessageId.THE_MAXIMUM_NUMBER_OF_AUCTION_HOUSE_ITEMS_FOR_REGISTRATION_IS_10);
				player.sendPacket(ExResponseCommissionRegisterPacket.FAILED);
				return;
			}

			long registrationFee = (long)Math.Max(MIN_REGISTRATION_AND_SALE_FEE,
				totalPrice * REGISTRATION_FEE_PER_DAY * Math.Min(durationInDays, (short)7));
			if (!player.getInventory().reduceAdena("Commission Registration Fee", registrationFee, player, null))
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ADENA_TO_REGISTER_THE_ITEM);
				player.sendPacket(ExResponseCommissionRegisterPacket.FAILED);
				return;
			}

			itemInstance = player.getInventory().detachItem("Commission Registration", itemInstance, itemCount,
				ItemLocation.COMMISSION, player, null);
			if (itemInstance == null)
			{
				player.getInventory().addAdena("Commission error refund", registrationFee, player, null);
				player.sendPacket(SystemMessageId.THE_ITEM_HAS_FAILED_TO_BE_REGISTERED);
				player.sendPacket(ExResponseCommissionRegisterPacket.FAILED);
				return;
			}

			switch (Math.Max(durationType, discountInPercentage))
			{
				case 4:
				{
					player.destroyItemByItemId("Consume", 22353, 1, player, true); // 15 days
					break;
				}
				case 5:
				{
					player.destroyItemByItemId("Consume", 22354, 1, player, true); // 30 days
					break;
				}
				case 30:
				{
					player.destroyItemByItemId("Consume", 22351, 1, player, true); // 30% discount
					break;
				}
				case 100:
				{
					player.destroyItemByItemId("Consume", 22352, 1, player, true); // 100% discount
					break;
				}
			}

			try
			{
				DateTime startTime = DateTime.UtcNow;
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				DbCommissionItem dbCommissionItem = new DbCommissionItem()
				{
					ItemObjectId = itemInstance.ObjectId,
					PricePerUnit = pricePerUnit,
					StartTime = startTime,
					DurationInDays = durationInDays,
					DiscountInPercentage = discountInPercentage
				};

				ctx.CommissionItems.Add(dbCommissionItem);

				ctx.SaveChanges();

				int commissionId = dbCommissionItem.Id;

				CommissionItem commissionItem = new CommissionItem(commissionId, itemInstance, pricePerUnit, startTime,
					durationInDays, discountInPercentage);

				TimeSpan delay = commissionItem.getEndTime() - DateTime.UtcNow;
				if (delay < TimeSpan.Zero)
					delay = TimeSpan.Zero;

				ScheduledFuture saleEndTask = ThreadPool.schedule(() => expireSale(commissionItem), delay);
				commissionItem.setSaleEndTask(saleEndTask);
				_commissionItems.put(commissionItem.getCommissionId(), commissionItem);
				player.getLastCommissionInfos().put(itemInstance.Id,
					new ExResponseCommissionInfoPacket(itemInstance.Id, pricePerUnit, itemCount,
						(byte)((durationInDays - 1) / 2)));
				player.sendPacket(SystemMessageId.THE_ITEM_HAS_BEEN_REGISTERED);
				player.sendPacket(ExResponseCommissionRegisterPacket.SUCCEED);
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Failed inserting commission item. ItemInstance: " + itemInstance, e);
				player.sendPacket(SystemMessageId.THE_ITEM_HAS_FAILED_TO_BE_REGISTERED);
				player.sendPacket(ExResponseCommissionRegisterPacket.FAILED);
			}
		}
	}

	/**
	 * Deletes an item and returns it to the player.
	 * @param player the player
	 * @param commissionId the commission id
	 */
	public void deleteItem(Player player, long commissionId)
	{
		CommissionItem? commissionItem = getCommissionItem(commissionId);
		if (commissionItem == null)
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CANCEL_THE_SALE);
			player.sendPacket(ExResponseCommissionDeletePacket.FAILED);
			return;
		}

		if (commissionItem.getItemInstance().getOwnerId() != player.ObjectId)
		{
			player.sendPacket(ExResponseCommissionDeletePacket.FAILED);
			return;
		}

		if (!player.isInventoryUnder80(false) || player.getWeightPenalty() >= 3)
		{
			player.sendPacket(SystemMessageId.TO_BUY_CANCEL_YOU_NEED_TO_FREE_20_OF_WEIGHT_AND_10_OF_SLOTS_IN_YOUR_INVENTORY);
			player.sendPacket(SystemMessageId.FAILED_TO_CANCEL_THE_SALE);
			player.sendPacket(ExResponseCommissionDeletePacket.FAILED);
			return;
		}

		if (_commissionItems.remove(commissionId) == null || commissionItem.getSaleEndTask()?.cancel(false) != true)
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CANCEL_THE_SALE);
			player.sendPacket(ExResponseCommissionDeletePacket.FAILED);
			return;
		}

		if (deleteItemFromDB(commissionId))
		{
			player.getInventory().addItem("Commission Cancellation", commissionItem.getItemInstance(), player, null);
			player.sendPacket(SystemMessageId.THE_SALE_IS_CANCELLED);
			player.sendPacket(ExResponseCommissionDeletePacket.SUCCEED);
		}
		else
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CANCEL_THE_SALE);
			player.sendPacket(ExResponseCommissionDeletePacket.FAILED);
		}
	}

	/**
	 * Buys the item for the given player.
	 * @param player the player
	 * @param commissionId the commission id
	 */
	public void buyItem(Player player, long commissionId)
	{
		CommissionItem? commissionItem = getCommissionItem(commissionId);
		if (commissionItem == null)
		{
			player.sendPacket(SystemMessageId.ITEM_PURCHASE_HAS_FAILED);
			player.sendPacket(ExResponseCommissionBuyItemPacket.FAILED);
			return;
		}

		Item itemInstance = commissionItem.getItemInstance();
		if (itemInstance.getOwnerId() == player.ObjectId)
		{
			player.sendPacket(SystemMessageId.ITEM_PURCHASE_HAS_FAILED);
			player.sendPacket(ExResponseCommissionBuyItemPacket.FAILED);
			return;
		}

		if (!player.isInventoryUnder80(false) || player.getWeightPenalty() >= 3)
		{
			player.sendPacket(SystemMessageId.TO_BUY_CANCEL_YOU_NEED_TO_FREE_20_OF_WEIGHT_AND_10_OF_SLOTS_IN_YOUR_INVENTORY);
			player.sendPacket(ExResponseCommissionBuyItemPacket.FAILED);
			return;
		}

		long totalPrice = itemInstance.getCount() * commissionItem.getPricePerUnit();
		if (!player.getInventory().reduceAdena("Commission Registration Fee", totalPrice, player, null))
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			player.sendPacket(ExResponseCommissionBuyItemPacket.FAILED);
			return;
		}

		if (_commissionItems.remove(commissionId) == null || commissionItem.getSaleEndTask()?.cancel(false) != true)
		{
			player.getInventory().addAdena("Commission error refund", totalPrice, player, null);
			player.sendPacket(SystemMessageId.ITEM_PURCHASE_HAS_FAILED);
			player.sendPacket(ExResponseCommissionBuyItemPacket.FAILED);
			return;
		}

		if (deleteItemFromDB(commissionId))
		{
			float discountFee = (float) commissionItem.getDiscountInPercentage() / 100;

			long saleFee = (long) Math.Max(MIN_REGISTRATION_AND_SALE_FEE, totalPrice * SALE_FEE_PER_DAY * Math.Min(commissionItem.getDurationInDays(), 7));
			long addDiscount = (long) (saleFee * discountFee);
			Message mail = new Message(itemInstance.getOwnerId(), itemInstance, MailType.COMMISSION_ITEM_SOLD);
			Mail attachement = mail.createAttachments();
			attachement.addItem("Commission Item Sold", Inventory.ADENA_ID, totalPrice - saleFee + addDiscount, player, null);
			MailManager.getInstance().sendMessage(mail);

			player.sendPacket(new ExResponseCommissionBuyItemPacket(commissionItem));
			player.getInventory().addItem("Commission Buy Item", commissionItem.getItemInstance(), player, null);
		}
		else
		{
			player.getInventory().addAdena("Commission error refund", totalPrice, player, null);
			player.sendPacket(ExResponseCommissionBuyItemPacket.FAILED);
		}
	}

	/**
	 * Deletes a commission item from database.
	 * @param commissionId the commission item
	 * @return {@code true} if the item was deleted successfully, {@code false} otherwise
	 */
	private bool deleteItemFromDB(long commissionId)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int deleted = ctx.CommissionItems.Where(c => c.Id == commissionId).ExecuteDelete();
			if (deleted > 0)
			{
				return true;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed deleting commission item. Commission ID: " + commissionId, e);
		}
		return false;
	}

	/**
	 * Expires the sale of a commission item and sends the item back to the player.
	 * @param commissionItem the comission item
	 */
	private void expireSale(CommissionItem commissionItem)
	{
		if (_commissionItems.remove(commissionItem.getCommissionId()) != null && deleteItemFromDB(commissionItem.getCommissionId()))
		{
			Message mail = new Message(commissionItem.getItemInstance().getOwnerId(), commissionItem.getItemInstance(), MailType.COMMISSION_ITEM_RETURNED);
			MailManager.getInstance().sendMessage(mail);
		}
	}

	/**
	 * Gets the commission item.
	 * @param commissionId the commission id to get
	 * @return the commission item if it exists, {@code null} otherwise
	 */
	public CommissionItem? getCommissionItem(long commissionId)
	{
		return _commissionItems.get(commissionId);
	}

	/**
	 * @param objectId
	 * @return {@code true} if player with the objectId has commission items, {@code false} otherwise
	 */
	public bool hasCommissionItems(int objectId)
	{
		foreach (CommissionItem item in _commissionItems.Values)
		{
			if (item.getItemInstance().ObjectId == objectId)
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * @param player the player
	 * @param itemId the item id
	 * @return {@code true} if the player has commissioned a specific item id, {@code false} otherwise.
	 */
	public bool hasCommissionedItemId(Player player, int itemId)
	{
		foreach (CommissionItem item in _commissionItems.Values)
		{
			if (item.getItemInstance().getOwnerId() == player.ObjectId && item.getItemInstance().getTemplate().Id == itemId)
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * Checks if the player is allowed to interact with commission manager.
	 * @param player the player
	 * @return {@code true} if the player is allowed to interact, {@code false} otherwise
	 */
	public static bool isPlayerAllowedToInteract(Player player)
	{
		Npc? npc = player.getLastFolkNPC();
		if (npc is CommissionManager)
		{
			return npc.Distance3D(player) <= INTERACTION_DISTANCE;
		}
		return false;
	}

	/**
	 * Gets the single instance.
	 * @return the single instance
	 */
	public static ItemCommissionManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ItemCommissionManager INSTANCE = new ItemCommissionManager();
	}
}