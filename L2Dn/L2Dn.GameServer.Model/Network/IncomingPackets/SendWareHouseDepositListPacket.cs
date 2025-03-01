using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SendWareHouseDepositListPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 12;
    private List<ItemHolder>? _items;

    public void ReadContent(PacketBitReader reader)
    {
        int size = reader.ReadInt32();
        if (size <= 0 || size > Config.MAX_ITEM_IN_PACKET || size * BATCH_LENGTH != reader.Length)
            return;

        _items = new(size);
        for (int i = 0; i < size; i++)
        {
            int objId = reader.ReadInt32();
            long count = reader.ReadInt64();
            if (objId < 1 || count < 0)
            {
                _items = null;
                return;
            }

            _items.Add(new ItemHolder(objId, count));
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		if (_items == null)
			return ValueTask.CompletedTask;

		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		// TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are depositing items too fast.");
		// 	return ValueTask.CompletedTask;
		// }

		ItemContainer? warehouse = player.getActiveWarehouse();
		if (warehouse == null)
			return ValueTask.CompletedTask;

		Npc? manager = player.getLastFolkNPC();
		if ((manager == null || !manager.isWarehouse() || !manager.canInteract(player)) && !player.isGM())
			return ValueTask.CompletedTask;

		bool isPrivate = warehouse is PlayerWarehouse;
		if (!isPrivate && !player.getAccessLevel().allowTransaction())
		{
			player.sendMessage("Transactions are disabled for your Access Level.");
			return ValueTask.CompletedTask;
		}

		if (player.hasItemRequest())
		{
			Util.handleIllegalPlayerAction(player, player + " tried to use enchant Exploit!", Config.DEFAULT_PUNISH);
			return ValueTask.CompletedTask;
		}

		// Alt game - Karma punishment
		if (!Config.ALT_GAME_KARMA_PLAYER_CAN_USE_WAREHOUSE && player.getReputation() < 0)
			return ValueTask.CompletedTask;

		// Freight price from config or normal price per item slot (30)
		long fee = _items.Count * 30;
		long currentAdena = player.getAdena();
		long slots = 0;
		foreach (ItemHolder itemHolder in _items)
		{
			Item? item = player.checkItemManipulation(itemHolder.getId(), itemHolder.getCount(), "deposit");
			if (item == null)
			{
				PacketLogger.Instance.Warn("Error depositing a warehouse object for char " + player.getName() +
				                           " (validity check)");

				return ValueTask.CompletedTask;
			}

			// Calculate needed adena and slots
			if (item.getId() == Inventory.ADENA_ID)
			{
				currentAdena -= itemHolder.getCount();
			}
			if (!item.isStackable())
			{
				slots += itemHolder.getCount();
			}
			else if (warehouse.getItemByItemId(item.getId()) == null)
			{
				slots++;
			}
		}

		// Item Max Limit Check
		if (!warehouse.validateCapacity(slots))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED);
			return ValueTask.CompletedTask;
		}

		// Check if enough adena and charge the fee
		if (currentAdena < fee || !player.reduceAdena(warehouse.getName(), fee, manager, false))
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			return ValueTask.CompletedTask;
		}

		// get current tradelist if any
		if (player.getActiveTradeList() != null)
			return ValueTask.CompletedTask;

		// Proceed to the transfer
		List<ItemInfo> itemInfos = new List<ItemInfo>();
		foreach (ItemHolder itemHolder in _items)
		{
			// Check validity of requested item
			Item? oldItem = player.checkItemManipulation(itemHolder.getId(), itemHolder.getCount(), "deposit");
			if (oldItem == null)
			{
				PacketLogger.Instance.Warn("Error depositing a warehouse object for char " + player.getName() +
				                           " (olditem == null)");

				return ValueTask.CompletedTask;
			}

			if (!oldItem.isDepositable(isPrivate) || !oldItem.isAvailable(player, true, isPrivate))
			{
				continue;
			}

			Item? newItem = player.getInventory().transferItem(warehouse.getName(), itemHolder.getId(), itemHolder.getCount(), warehouse, player, manager);
			if (newItem == null)
			{
				PacketLogger.Instance.Warn("Error depositing a warehouse object for char " + player.getName() +
				                           " (newitem == null)");

				continue;
			}

			if (oldItem.getCount() > 0 && oldItem != newItem)
			{
				itemInfos.Add(new ItemInfo(oldItem, ItemChangeType.MODIFIED));
			}
			else
			{
				itemInfos.Add(new ItemInfo(oldItem, ItemChangeType.REMOVED));
			}
		}

		// Send updated item list to the player
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(itemInfos);
		player.sendInventoryUpdate(playerIU);
		return ValueTask.CompletedTask;
    }
}