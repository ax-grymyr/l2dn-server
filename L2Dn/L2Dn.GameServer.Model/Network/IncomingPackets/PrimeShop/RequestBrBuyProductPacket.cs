using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.PrimeShop;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.PrimeShop;

public struct RequestBrBuyProductPacket: IIncomingPacket<GameSession>
{
    private const int HERO_COINS = 23805;

    private int _brId;
    private int _count;

    public void ReadContent(PacketBitReader reader)
    {
        _brId = reader.ReadInt32();
        _count = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		if (player.hasItemRequest() || player.hasRequest<PrimeShopRequest>())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER_STATE));
			return ValueTask.CompletedTask;
		}

		player.addRequest(new PrimeShopRequest(player));

		PrimeShopGroup? item = PrimeShopData.getInstance().getItem(_brId);
		if (ValidatePlayer(item, _count, player) && item != null)
		{
			int price = item.getPrice() * _count;
			if (price < 1)
			{
				player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.LACK_OF_POINT));
				player.removeRequest<PrimeShopRequest>();
				return ValueTask.CompletedTask;
			}

			int paymentId = validatePaymentId(item);
			if (paymentId < 0)
			{
				player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.LACK_OF_POINT));
				player.removeRequest<PrimeShopRequest>();
				return ValueTask.CompletedTask;
			}

			if (paymentId > 0)
			{
				if (!player.destroyItemByItemId("PrimeShop-" + item.getBrId(), paymentId, price, player, true))
				{
					player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.LACK_OF_POINT));
					player.removeRequest<PrimeShopRequest>();
					return ValueTask.CompletedTask;
				}
			}
			else if (paymentId == 0)
			{
				if (player.getPrimePoints() < price)
				{
					player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.LACK_OF_POINT));
					player.removeRequest<PrimeShopRequest>();
					return ValueTask.CompletedTask;
				}

				player.setPrimePoints(player.getPrimePoints() - price);
				if (Config.VipSystem.VIP_SYSTEM_PRIME_AFFECT)
				{
					player.updateVipPoints(price);
				}
			}

			foreach (PrimeShopItem subItem in item.getItems())
			{
				player.addItem("PrimeShop", subItem.getId(), subItem.getCount() * _count, player, true);
			}

			if (item.isVipGift())
			{
				player.getAccountVariables().Set(AccountVariables.VIP_ITEM_BOUGHT, DateTime.UtcNow);
			}

			// Update account variables.
			if (item.getAccountDailyLimit() > 0)
			{
				player.getAccountVariables().Set(AccountVariables.PRIME_SHOP_PRODUCT_DAILY_COUNT + item.getBrId(),
					player.getAccountVariables()
						.Get(AccountVariables.PRIME_SHOP_PRODUCT_DAILY_COUNT + item.getBrId(), 0) +
					item.getCount() * _count);
			}
			else if (item.getAccountBuyLimit() > 0)
			{
				player.getAccountVariables().Set(AccountVariables.PRIME_SHOP_PRODUCT_COUNT + item.getBrId(),
					player.getAccountVariables().Get(AccountVariables.PRIME_SHOP_PRODUCT_COUNT + item.getBrId(), 0) +
					item.getCount() * _count);
			}

			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.SUCCESS));
			player.sendPacket(new ExBRGamePointPacket(player));
		}

		player.removeRequest<PrimeShopRequest>();

		return ValueTask.CompletedTask;
	}

	/**
	 * @param item
	 * @param count
	 * @param player
	 * @return
	 */
	private static bool ValidatePlayer(PrimeShopGroup? item, int count, Player player)
	{
		if (item == null)
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_PRODUCT));
			Util.handleIllegalPlayerAction(player, player + " tried to buy invalid brId from Prime",
				Config.General.DEFAULT_PUNISH);

			return false;
		}

		if (count < 1 || count > 99)
		{
			Util.handleIllegalPlayerAction(player,
				player + " tried to buy invalid itemcount [" + count + "] from Prime", Config.General.DEFAULT_PUNISH);

			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER_STATE));
			return false;
		}

		if (item.getMinLevel() > 0 && item.getMinLevel() > player.getLevel())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER));
			return false;
		}

		if (item.getMaxLevel() > 0 && item.getMaxLevel() < player.getLevel())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER));
			return false;
		}

		if (item.getMinBirthday() > 0 && item.getMinBirthday() > player.getBirthdays())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER_STATE));
			return false;
		}

		if (item.getMaxBirthday() > 0 && item.getMaxBirthday() < player.getBirthdays())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER_STATE));
			return false;
		}

		DateTime currentTime = DateTime.UtcNow;
		if (!item.getDaysOfWeek().Contains(currentTime.DayOfWeek))
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.NOT_DAY_OF_WEEK));
			return false;
		}

		if (item.getStartSale() > currentTime)
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.BEFORE_SALE_DATE));
			return false;
		}

		if (item.getEndSale() < currentTime)
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.AFTER_SALE_DATE));
			return false;
		}

		if (item.getAccountDailyLimit() > 0 &&
		    count + player.getAccountVariables()
			    .Get(AccountVariables.PRIME_SHOP_PRODUCT_DAILY_COUNT + item.getBrId(), 0) >
		    item.getAccountDailyLimit())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.SOLD_OUT));
			return false;
		}

		if (item.getAccountBuyLimit() > 0 &&
		    count + player.getAccountVariables().Get(AccountVariables.PRIME_SHOP_PRODUCT_COUNT + item.getBrId(), 0) >
		    item.getAccountBuyLimit())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.SOLD_OUT));
			return false;
		}

		if (item.getVipTier() > player.getVipTier() || (item.isVipGift() && !canReceiveGift(player, item)))
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.SOLD_OUT));
			return false;
		}

		int weight = item.getWeight() * count;
		long slots = item.getCount() * count;
		if (player.getInventory().validateWeight(weight))
		{
			if (item.getCount() == 1)
			{
				if (!player.getInventory().validateCapacity(slots))
				{
					player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVENTORY_OVERFLOW));
					return false;
				}
			}
			else if (!player.getInventory().validateCapacity(count))
			{
				player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVENTORY_OVERFLOW));
				return false;
			}
		}
		else
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVENTORY_OVERFLOW));
			return false;
		}

		return true;
	}

	/**
	 * Check if player can receive Gift from L2 Store
	 * @param player player in question
	 * @param item requested item.
	 * @return true if player can receive gift item.
	 */
	private static bool canReceiveGift(Player player, PrimeShopGroup item)
	{
		if (!Config.VipSystem.VIP_SYSTEM_ENABLED)
		{
			return false;
		}
		if (player.getVipTier() <= 0)
		{
			return false;
		}

		if (item.getVipTier() != player.getVipTier())
		{
			player.sendMessage("This item is not for your vip tier!");
			return false;
		}

		return player.getAccountVariables().Get(AccountVariables.VIP_ITEM_BOUGHT, 0L) <= 0;
	}

	private static int validatePaymentId(PrimeShopGroup item)
	{
		switch (item.getPaymentType())
		{
			case 0: // Prime points
			{
				return 0;
			}
			case 1: // Adenas
			{
				return Inventory.ADENA_ID;
			}
			case 2: // Hero coins
			{
				return HERO_COINS;
			}
		}
		return -1;
	}
}