using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SendWareHouseWithDrawListPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 12; // length of the one item

    private ItemHolder[]? _items;

    public void ReadContent(PacketBitReader reader)
    {
        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
            return;

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
		if (_items == null)
			return ValueTask.CompletedTask;

		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		// TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are withdrawing items too fast.");
		// 	return ValueTask.CompletedTask;
		// }

		ItemContainer? warehouse = player.getActiveWarehouse();
		if (warehouse == null)
			return ValueTask.CompletedTask;

		if (!(warehouse is PlayerWarehouse) && !player.getAccessLevel().allowTransaction())
		{
			player.sendMessage("Transactions are disabled for your Access Level.");
			return ValueTask.CompletedTask;
		}

		// Alt game - Karma punishment
		if (!Config.ALT_GAME_KARMA_PLAYER_CAN_USE_WAREHOUSE && player.getReputation() < 0)
			return ValueTask.CompletedTask;

		if (Config.ALT_MEMBERS_CAN_WITHDRAW_FROM_CLANWH)
		{
			if (warehouse is ClanWarehouse && !player.hasClanPrivilege(ClanPrivilege.CL_VIEW_WAREHOUSE))
			return ValueTask.CompletedTask;
		}
		else if (warehouse is ClanWarehouse && !player.isClanLeader())
		{
			player.sendPacket(SystemMessageId.ITEMS_LEFT_IN_THE_CLAN_WAREHOUSE_CAN_ONLY_BE_RETRIEVED_BY_THE_CLAN_LEADER_CONTINUE);
			return ValueTask.CompletedTask;
		}

		long weight = 0;
		long slots = 0;
		foreach (ItemHolder i in _items)
		{
			// Calculate needed slots
			Item? item = warehouse.getItemByObjectId(i.getId());
			if (item == null || item.getCount() < i.getCount())
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to withdraw non-existent item from warehouse.", Config.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			weight += i.getCount() * item.getTemplate().getWeight();
			if (!item.isStackable())
			{
				slots += i.getCount();
			}
			else if (player.getInventory().getItemByItemId(item.getId()) == null)
			{
				slots++;
			}
		}

		// Item Max Limit Check
		if (!player.getInventory().validateCapacity(slots))
		{
			player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			return ValueTask.CompletedTask;
		}

		// Weight limit Check
		if (!player.getInventory().validateWeight(weight))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
			return ValueTask.CompletedTask;
		}

		// Proceed to the transfer
		foreach (ItemHolder i in _items)
		{
			Item? oldItem = warehouse.getItemByObjectId(i.getId());
			if (oldItem == null || oldItem.getCount() < i.getCount())
			{
				PacketLogger.Instance.Warn("Error withdrawing a warehouse object for char " + player.getName() +
				                           " (olditem == null)");

				return ValueTask.CompletedTask;
			}

			Item? newItem = warehouse.transferItem(warehouse.getName(), i.getId(), i.getCount(), player.getInventory(),
				player, player.getLastFolkNPC());

			if (newItem == null)
			{
				PacketLogger.Instance.Warn("Error withdrawing a warehouse object for char " + player.getName() +
				                           " (newitem == null)");

				return ValueTask.CompletedTask;
			}
		}

		// Send updated item list to the player
		player.sendItemList();
		return ValueTask.CompletedTask;
    }
}