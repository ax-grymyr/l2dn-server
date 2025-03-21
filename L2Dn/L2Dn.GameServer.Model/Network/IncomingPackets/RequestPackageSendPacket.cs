using L2Dn.GameServer.Dto;
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
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPackageSendPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 12; // length of the one item

    private ItemHolder[]? _items;
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();

        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.Character.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
        {
            return;
        }

        _items = new ItemHolder[count];
        for (int i = 0; i < count; i++)
        {
            int objId = reader.ReadInt32();
            long cnt = reader.ReadInt64();
            if (objId < 1 || cnt < 0)
            {
                _items = null;
                return;
            }

            _items[i] = new ItemHolder(objId, cnt);
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (_items == null || player == null || !player.getAccountChars().ContainsKey(_objectId))
			return ValueTask.CompletedTask;

		// TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You depositing items too fast.");
		// 	return;
		// }

		if (player.hasItemRequest())
		{
			Util.handleIllegalPlayerAction(player, player + " tried to use enchant exploit!", Config.General.DEFAULT_PUNISH);
			return ValueTask.CompletedTask;
		}

		// get current tradelist if any
		if (player.getActiveTradeList() != null)
			return ValueTask.CompletedTask;

		// Alt game - Karma punishment
		if (!Config.Character.ALT_GAME_KARMA_PLAYER_CAN_USE_WAREHOUSE && player.getReputation() < 0)
			return ValueTask.CompletedTask;

		// Freight price from config per item slot.
		int fee = _items.Length * Config.Character.ALT_FREIGHT_PRICE;
		long currentAdena = player.getAdena();
		long slots = 0;

		ItemContainer warehouse = new PlayerFreight(_objectId);
		foreach (ItemHolder i in _items)
		{
			// Check validity of requested item
			Item? item = player.checkItemManipulation(i.Id, i.getCount(), "freight");
			if (item == null)
			{
				PacketLogger.Instance.Warn("Error depositing a warehouse object for char " + player.getName() +
				                           " (validity check)");

				warehouse.deleteMe();
				return ValueTask.CompletedTask;
			}

			if (!item.isFreightable())
			{
				warehouse.deleteMe();
				return ValueTask.CompletedTask;
			}

			// Calculate needed adena and slots
			if (item.Id == Inventory.AdenaId)
			{
				currentAdena -= i.getCount();
			}
			else if (!item.isStackable())
			{
				slots += i.getCount();
			}
			else if (warehouse.getItemByItemId(item.Id) == null)
			{
				slots++;
			}
		}

		// Item Max Limit Check
		if (!warehouse.validateCapacity(slots))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED);
			warehouse.deleteMe();
			return ValueTask.CompletedTask;
		}

		// Check if enough adena and charge the fee
		if (currentAdena < fee || !player.reduceAdena(warehouse.getName(), fee, player, false))
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			warehouse.deleteMe();
			return ValueTask.CompletedTask;
		}

		// Proceed to the transfer
		List<ItemInfo> items = new List<ItemInfo>();
		foreach (ItemHolder i in _items)
		{
			// Check validity of requested item
			Item? oldItem = player.checkItemManipulation(i.Id, i.getCount(), "deposit");
			if (oldItem == null)
			{
				PacketLogger.Instance.Warn("Error depositing a warehouse object for char " + player.getName() +
				                           " (olditem == null)");

				warehouse.deleteMe();
				return ValueTask.CompletedTask;
			}

			Item? newItem = player.getInventory().transferItem("Trade", i.Id, i.getCount(), warehouse, player, null);
			if (newItem == null)
			{
				PacketLogger.Instance.Warn("Error depositing a warehouse object for char " + player.getName() +
				                           " (newitem == null)");

				continue;
			}

			if (oldItem.getCount() > 0 && oldItem != newItem)
			{
				items.Add(new ItemInfo(oldItem, ItemChangeType.MODIFIED));
			}
			else
			{
				items.Add(new ItemInfo(oldItem, ItemChangeType.REMOVED));
			}

			// Remove item objects from the world.
			World.getInstance().removeObject(oldItem);
			World.getInstance().removeObject(newItem);
		}

		warehouse.deleteMe();

		// Send updated item list to the player
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(items);
		player.sendInventoryUpdate(playerIU);

		return ValueTask.CompletedTask;
    }
}