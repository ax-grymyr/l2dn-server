using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.PrimeShop;
using L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.PrimeShop;

public struct RequestBrPresentBuyProductPacket: IIncomingPacket<GameSession>
{
    private static int HERO_COINS = 23805;

    private int _brId;
    private int _count;
    private string _charName;
    private string _mailTitle;
    private string _mailBody;

    public void ReadContent(PacketBitReader reader)
    {
        _brId = reader.ReadInt32();
        _count = reader.ReadInt32();
        _charName = reader.ReadString();
        _mailTitle = reader.ReadString();
        _mailBody = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		int receiverId = CharInfoTable.getInstance().getIdByName(_charName);
		if (receiverId <= 0)
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER));
			return ValueTask.CompletedTask;
		}

		if (player.hasItemRequest() || player.hasRequest<PrimeShopRequest>())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER_STATE));
			return ValueTask.CompletedTask;
		}

		player.addRequest(new PrimeShopRequest(player));

		PrimeShopGroup? item = PrimeShopData.getInstance().getItem(_brId);

		if (item != null && item.isVipGift())
		{
			player.sendMessage("You cannot gift a Vip Gift!");
			return ValueTask.CompletedTask;
		}

		if (ValidatePlayer(item, _count, player) && item != null)
		{
			int price = item.getPrice() * _count;
			if (price < 1)
			{
				player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.LACK_OF_POINT));
				player.removeRequest<PrimeShopRequest>();
				return ValueTask.CompletedTask;
			}

			int paymentId = validatePaymentId(item, price);
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
				if (Config.VIP_SYSTEM_PRIME_AFFECT)
				{
					player.updateVipPoints(price);
				}
			}

			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.SUCCESS));
			player.sendPacket(new ExBRGamePointPacket(player));

			Message mail = new Message(receiverId, _mailTitle, _mailBody, MailType.PRIME_SHOP_GIFT);
			Mail attachement = mail.createAttachments();

			foreach (PrimeShopItem subItem in item.getItems())
			{
				attachement.addItem("Prime Shop Gift", subItem.getId(), subItem.getCount() * _count, player, this);
			}

			MailManager.getInstance().sendMessage(mail);
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
			    Config.DEFAULT_PUNISH);

		    return false;
	    }

	    if (count < 1 || count > 99)
	    {
		    Util.handleIllegalPlayerAction(player,
			    player + " tried to buy invalid itemcount [" + count + "] from Prime", Config.DEFAULT_PUNISH);

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

	    int weight = item.getWeight() * count;
	    long slots = item.getCount() * count;
	    if (player.getInventory().validateWeight(weight))
	    {
		    if (!player.getInventory().validateCapacity(slots))
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

    private static int validatePaymentId(PrimeShopGroup item, long amount)
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