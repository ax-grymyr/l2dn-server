using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SetPrivateStoreListSellPacket: IIncomingPacket<GameSession>
{
    private bool _packageSale;
    private ItemData[]? _items;

    public void ReadContent(PacketBitReader reader)
    {
        _packageSale = reader.ReadInt32() == 1;
        int count = reader.ReadInt32();
        if (count < 1 || count > Config.Character.MAX_ITEM_IN_PACKET)
            return;

        _items = new ItemData[count];
        for (int i = 0; i < count; i++)
        {
            int itemId = reader.ReadInt32();
            long cnt = reader.ReadInt64();
            long price = reader.ReadInt64();
            if (itemId < 1 || cnt < 1 || price < 0)
            {
                _items = null;
                return;
            }

            // Unknown.
            reader.ReadString();

            _items[i] = new ItemData(itemId, cnt, price);
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		if (_items == null)
		{
			player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT);
			player.setPrivateStoreType(PrivateStoreType.NONE);
			player.broadcastUserInfo();
			return ValueTask.CompletedTask;
		}

		if (!player.getAccessLevel().AllowTransaction)
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return ValueTask.CompletedTask;
		}

		if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(player) || player.isInDuel())
		{
			player.sendPacket(SystemMessageId.WHILE_YOU_ARE_ENGAGED_IN_COMBAT_YOU_CANNOT_OPERATE_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
			player.sendPacket(new PrivateStoreManageListSellPacket(1, player, _packageSale));
			player.sendPacket(new PrivateStoreManageListSellPacket(2, player, _packageSale));
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (player.isInsideZone(ZoneId.NO_STORE))
		{
			player.sendPacket(new PrivateStoreManageListSellPacket(1, player, _packageSale));
			player.sendPacket(new PrivateStoreManageListSellPacket(2, player, _packageSale));
			player.sendPacket(SystemMessageId.YOU_CANNOT_OPEN_A_PRIVATE_STORE_HERE);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Check maximum number of allowed slots for pvt shops
		if (_items.Length > player.getPrivateSellStoreLimit())
		{
			player.sendPacket(new PrivateStoreManageListSellPacket(1, player, _packageSale));
			player.sendPacket(new PrivateStoreManageListSellPacket(2, player, _packageSale));
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED);
			return ValueTask.CompletedTask;
		}

		TradeList tradeList = player.getSellList();
		tradeList.clear();
		tradeList.setPackaged(_packageSale);

		long totalCost = player.getAdena();
		foreach (ItemData i in _items)
		{
			if (!i.addToTradeList(tradeList))
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to set price more than " + Inventory.MAX_ADENA + " adena in Private Store - Sell.",
					Config.General.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			totalCost += i.getPrice();
			if (totalCost > Inventory.MAX_ADENA)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to set total price more than " + Inventory.MAX_ADENA + " adena in Private Store - Sell.",
					Config.General.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}
		}

		player.sitDown();
		if (_packageSale)
		{
			player.setPrivateStoreType(PrivateStoreType.PACKAGE_SELL);
		}
		else
		{
			player.setPrivateStoreType(PrivateStoreType.SELL);
		}

		player.broadcastUserInfo();

		if (_packageSale)
		{
			player.broadcastPacket(new ExPrivateStoreSetWholeMsgPacket(player));
		}
		else
		{
			player.broadcastPacket(new PrivateStoreMsgSellPacket(player));
		}

		return ValueTask.CompletedTask;
	}

	private readonly record struct ItemData(int ObjectId, long Count, long Price)
	{
		public bool addToTradeList(TradeList list)
		{
			if (Inventory.MAX_ADENA / Count < Price)
			{
				return false;
			}

			list.addItem(ObjectId, Count, Price);
			return true;
		}

		public long getPrice()
		{
			return Count * Price;
		}
	}
}