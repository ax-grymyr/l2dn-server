using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestCancelPostAttachmentPacket: IIncomingPacket<GameSession>
{
    private int _msgId;

    public void ReadContent(PacketBitReader reader)
    {
        _msgId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    if (!Config.ALLOW_MAIL || !Config.ALLOW_ATTACHMENTS)
		    return ValueTask.CompletedTask;

	    Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// 	return ValueTask.CompletedTask;

		Message? msg = MailManager.getInstance().getMessage(_msgId);
		if (msg == null)
			return ValueTask.CompletedTask;

		if (msg.getSenderId() != player.ObjectId)
		{
			Util.handleIllegalPlayerAction(player, player + " tried to cancel not own post!", Config.DEFAULT_PUNISH);
			return ValueTask.CompletedTask;
		}

		if (!player.isInsideZone(ZoneId.PEACE))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CANCEL_IN_A_NON_PEACE_ZONE_LOCATION);
			return ValueTask.CompletedTask;
		}

		if (player.getActiveTradeList() != null)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CANCEL_DURING_AN_EXCHANGE);
			return ValueTask.CompletedTask;
		}

		if (player.hasItemRequest())
		{
			player.sendPacket(SystemMessageId.UNAVAILABLE_WHILE_THE_ENCHANTING_IS_IN_PROCESS);
			return ValueTask.CompletedTask;
		}

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CANCEL_BECAUSE_THE_PRIVATE_STORE_OR_WORKSHOP_IS_IN_PROGRESS);
			return ValueTask.CompletedTask;
		}

		if (!msg.hasAttachments())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CANCEL_SENT_MAIL_SINCE_THE_RECIPIENT_RECEIVED_IT);
			return ValueTask.CompletedTask;
		}

		ItemContainer? attachments = msg.getAttachments();
		if (attachments == null || attachments.getSize() == 0)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CANCEL_SENT_MAIL_SINCE_THE_RECIPIENT_RECEIVED_IT);
			return ValueTask.CompletedTask;
		}

		long weight = 0;
		long slots = 0;
		foreach (Item item in attachments.getItems())
		{
			if (item == null)
			{
				continue;
			}

			if (item.getOwnerId() != player.ObjectId)
			{
				Util.handleIllegalPlayerAction(player, player + " tried to get not own item from cancelled attachment!", Config.DEFAULT_PUNISH);
				return ValueTask.CompletedTask;
			}

			if (item.getItemLocation() != ItemLocation.MAIL)
			{
				Util.handleIllegalPlayerAction(player, player + " tried to get items not from mail !", Config.DEFAULT_PUNISH);
				return ValueTask.CompletedTask;
			}

			if (item.getLocationSlot() != msg.getId())
			{
				Util.handleIllegalPlayerAction(player, player + " tried to get items from different attachment!", Config.DEFAULT_PUNISH);
				return ValueTask.CompletedTask;
			}

			weight += item.getCount() * item.getTemplate().getWeight();
			if (!item.isStackable())
			{
				slots += item.getCount();
			}
			else if (player.getInventory().getItemByItemId(item.getId()) == null)
			{
				slots++;
			}
		}

		if (!player.getInventory().validateCapacity(slots))
		{
			player.sendPacket(SystemMessageId.YOU_COULD_NOT_CANCEL_RECEIPT_BECAUSE_YOUR_INVENTORY_IS_FULL);
			return ValueTask.CompletedTask;
		}

		if (!player.getInventory().validateWeight(weight))
		{
			player.sendPacket(SystemMessageId.YOU_COULD_NOT_CANCEL_RECEIPT_BECAUSE_YOUR_INVENTORY_IS_FULL);
			return ValueTask.CompletedTask;
		}

		// Proceed to the transfer
		List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		foreach (Item item in attachments.getItems())
		{
			if (item == null)
			{
				continue;
			}

			long count = item.getCount();
			Item? newItem = attachments.transferItem(attachments.getName(), item.ObjectId, count,
				player.getInventory(), player, null);

			if (newItem == null)
				return ValueTask.CompletedTask;

			if (newItem.isStackable() && newItem.getCount() > count)
			{
				itemsToUpdate.Add(new ItemInfo(newItem, ItemChangeType.MODIFIED));
			}
			else
			{
				itemsToUpdate.Add(new ItemInfo(newItem, ItemChangeType.ADDED));
			}

			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
			sm.Params.addItemName(item.getId());
			sm.Params.addLong(count);
			player.sendPacket(sm);
		}

		msg.removeAttachments();

		// Send updated item list to the player
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(itemsToUpdate);
		player.sendInventoryUpdate(playerIU);

		// Send full list to avoid duplicates.
		player.sendItemList();

		Player? receiver = World.getInstance().getPlayer(msg.getReceiverId());
		if (receiver != null)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_CANCELLED_SENDING_A_MAIL);
			sm.Params.addString(player.getName());
			receiver.sendPacket(sm);
			receiver.sendPacket(new ExChangePostStatePacket(true, _msgId, Message.DELETED));
		}

		MailManager.getInstance().deleteMessageInDb(_msgId);

		player.sendPacket(new ExChangePostStatePacket(false, _msgId, Message.DELETED));
		player.sendPacket(SystemMessageId.YOU_VE_CANCELLED_SENDING_A_MAIL);

        return ValueTask.CompletedTask;
    }
}