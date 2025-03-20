using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestBuyItemPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 12;
    private const int CUSTOM_CB_SELL_LIST = 423;

    private int _listId;
    private List<ItemHolder>? _items;

    public void ReadContent(PacketBitReader reader)
    {
        _listId = reader.ReadInt32();
        int size = reader.ReadInt32();
        if (size <= 0 || size > Config.Character.MAX_ITEM_IN_PACKET || size * BATCH_LENGTH != reader.Length)
        {
            return;
        }

        _items = new(size);
        for (int i = 0; i < size; i++)
        {
            int itemId = reader.ReadInt32();
            long count = reader.ReadInt64();
            if (itemId < 1 || count < 1)
            {
                _items = null;
                return;
            }

            _items.Add(new ItemHolder(itemId, count));
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		// TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are buying too fast.");
		// 	return;
		// }

		if (_items == null)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Alt game - Karma punishment
		if (!Config.Character.ALT_GAME_KARMA_PLAYER_CAN_SHOP && player.getReputation() < 0)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		WorldObject? target = player.getTarget();
		Merchant? merchant = null;
		if (!player.isGM() && _listId != CUSTOM_CB_SELL_LIST)
		{
			if (!(target is Merchant) || !player.IsInsideRadius3D(target, Npc.INTERACTION_DISTANCE) ||
			    player.getInstanceWorld() != target.getInstanceWorld())
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			merchant = (Merchant)target; // FIXME: Doesn't work for GMs.
		}

		if (merchant == null && !player.isGM() && _listId != CUSTOM_CB_SELL_LIST)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		ProductList? buyList = BuyListData.getInstance().getBuyList(_listId);
		if (buyList == null)
		{
			Util.handleIllegalPlayerAction(player,
				"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
				" sent a false BuyList list_id " + _listId, Config.General.DEFAULT_PUNISH);

			return ValueTask.CompletedTask;
		}

		double castleTaxRate = 0;
		if (merchant != null)
		{
			if (!buyList.isNpcAllowed(merchant.Id))
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			castleTaxRate = merchant.getCastleTaxRate(TaxType.BUY);
		}

		long subTotal = 0;

		// Check for buylist validity and calculates summary values
		long slots = 0;
		long weight = 0;
		foreach (ItemHolder i in _items)
		{
			Product? product = buyList.getProductByItemId(i.Id);
			if (product == null)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" sent a false BuyList list_id " + _listId + " and item_id " + i.Id, Config.General.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			if (!product.getItem().isStackable() && i.getCount() > 1)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to purchase invalid quantity of items at the same time.", Config.General.DEFAULT_PUNISH);

				player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED);
				return ValueTask.CompletedTask;
			}

			long price = product.getPrice();
			if (price < 0)
			{
				PacketLogger.Instance.Warn("ERROR, no price found .. wrong buylist ??");
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			if (price == 0 && !player.isGM() && Config.General.ONLY_GM_ITEMS_FREE)
			{
				player.sendMessage("Ohh Cheat dont work? You have a problem now!");
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried buy item for 0 adena.", Config.General.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			// trying to buy more then available
			if (product.hasLimitedStock() && i.getCount() > product.getCount())
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			if (Inventory.MAX_ADENA / i.getCount() < price)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to purchase over " + Inventory.MAX_ADENA + " adena worth of goods.", Config.General.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			// first calculate price per item with tax, then multiply by count
			price = (long) (price * (1 + castleTaxRate + product.getBaseTaxRate()));
			subTotal += i.getCount() * price;
			if (subTotal > Inventory.MAX_ADENA)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to purchase over " + Inventory.MAX_ADENA + " adena worth of goods.", Config.General.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			weight += i.getCount() * product.getItem().getWeight();
			if (player.getInventory().getItemByItemId(product.getItemId()) == null)
			{
				slots++;
			}
		}

		if (!player.isGM() && (weight > int.MaxValue || weight < 0 || !player.getInventory().validateWeight((int) weight)))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (!player.isGM() && (slots > int.MaxValue || slots < 0 || !player.getInventory().validateCapacity((int) slots)))
		{
			player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Charge buyer and add tax to castle treasury if not owned by npc clan
		if (subTotal < 0 || !player.reduceAdena("Buy", subTotal, player.getLastFolkNPC(), false))
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Proceed the purchase
		foreach (ItemHolder i in _items)
		{
			Product? product = buyList.getProductByItemId(i.Id);
			if (product == null)
			{
				Util.handleIllegalPlayerAction(player, "Warning!! Character " + player.getName() + " of account " + player.getAccountName() + " sent a false BuyList list_id " + _listId + " and item_id " + i.Id, Config.General.DEFAULT_PUNISH);
				continue;
			}

			if (product.hasLimitedStock())
			{
				if (product.decreaseCount(i.getCount()))
				{
					player.getInventory().addItem("Buy", i.Id, i.getCount(), player, merchant);
				}
			}
			else
			{
				player.getInventory().addItem("Buy", i.Id, i.getCount(), player, merchant);
			}
		}

		// add to castle treasury
		if (merchant != null)
		{
			merchant.handleTaxPayment((long)(subTotal * castleTaxRate));
		}

		player.sendPacket(new ExUserInfoInventoryWeightPacket(player));
		player.sendPacket(new ExBuySellListPacket(player, true));
		player.sendPacket(SystemMessageId.EXCHANGE_IS_SUCCESSFUL);
		return ValueTask.CompletedTask;
    }
}