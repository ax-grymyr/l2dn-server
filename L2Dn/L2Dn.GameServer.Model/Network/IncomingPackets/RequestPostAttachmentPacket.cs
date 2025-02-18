using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPostAttachmentPacket: IIncomingPacket<GameSession>
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

		if (!player.getAccessLevel().allowTransaction())
		{
			player.sendMessage("Transactions are disabled for your Access Level");
			return ValueTask.CompletedTask;
		}

		if (player.isInCombat())
		{
			player.sendPacket(SystemMessageId.NOT_AVAILABLE_IN_COMBAT);
			return ValueTask.CompletedTask;
		}

		if (player.isDead())
		{
			player.sendPacket(SystemMessageId.YOU_ARE_DEAD_AND_CANNOT_PERFORM_THIS_ACTION);
			return ValueTask.CompletedTask;
		}

		if (player.getActiveTradeList() != null)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_RECEIVE_DURING_AN_EXCHANGE);
			return ValueTask.CompletedTask;
		}

		if (player.hasItemRequest())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_RECEIVE_MAIL_WHILE_ENCHANTING_AN_ITEM_BESTOWING_AN_ATTRIBUTE_OR_COMBINING_JEWELS);
			return ValueTask.CompletedTask;
		}

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_RECEIVE_BECAUSE_THE_PRIVATE_STORE_OR_WORKSHOP_IS_IN_PROGRESS);
			return ValueTask.CompletedTask;
		}

		Message msg = MailManager.getInstance().getMessage(_msgId);
		if (msg == null)
			return ValueTask.CompletedTask;

		if (msg.getReceiverId() != player.ObjectId)
		{
			Util.handleIllegalPlayerAction(player, player + " tried to get not own attachment!", Config.DEFAULT_PUNISH);
			return ValueTask.CompletedTask;
		}

		if (!msg.hasAttachments())
			return ValueTask.CompletedTask;

		ItemContainer attachments = msg.getAttachments();
		if (attachments == null)
			return ValueTask.CompletedTask;

		long weight = 0;
		long slots = 0;
		foreach (Item item in attachments.getItems())
		{
			if (item == null)
			{
				continue;
			}

			// Calculate needed slots
			if (item.getOwnerId() != msg.getSenderId())
			{
				Util.handleIllegalPlayerAction(player,
					player + " tried to get wrong item (ownerId != senderId) from attachment!", Config.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			if (item.getItemLocation() != ItemLocation.MAIL)
			{
				Util.handleIllegalPlayerAction(player,
					player + " tried to get wrong item (Location != MAIL) from attachment!", Config.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			if (item.getLocationSlot() != msg.getId())
			{
				Util.handleIllegalPlayerAction(player, player + " tried to get items from different attachment!",
					Config.DEFAULT_PUNISH);

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

		// Item Max Limit Check
		if (!player.getInventory().validateCapacity(slots))
		{
			player.sendPacket(SystemMessageId.YOU_COULD_NOT_RECEIVE_BECAUSE_YOUR_INVENTORY_IS_FULL);
			return ValueTask.CompletedTask;
		}

		// Weight limit Check
		if (!player.getInventory().validateWeight(weight))
		{
			player.sendPacket(SystemMessageId.YOU_COULD_NOT_RECEIVE_BECAUSE_YOUR_INVENTORY_IS_FULL);
			return ValueTask.CompletedTask;
		}

		long adena = msg.getReqAdena();
		if (adena > 0 && !player.reduceAdena("PayMail", adena, null, true))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_RECEIVE_BECAUSE_YOU_DON_T_HAVE_ENOUGH_ADENA);
			return ValueTask.CompletedTask;
		}

		// Proceed to the transfer
		SystemMessagePacket sm;
		List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		foreach (Item item in attachments.getItems())
		{
			if (item == null)
			{
				continue;
			}

			if (item.getOwnerId() != msg.getSenderId())
			{
				Util.handleIllegalPlayerAction(player, player + " tried to get items with owner != sender !",
					Config.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			long count = item.getCount();
			Item? newItem = attachments.transferItem(attachments.getName(), item.ObjectId, item.getCount(), player.getInventory(), player, null);
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

			sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
			sm.Params.addItemName(item.getId());
			sm.Params.addLong(count);
			player.sendPacket(sm);
		}

		// Send updated item list to the player
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(itemsToUpdate);
		player.sendInventoryUpdate(playerIU);

		// Send full list to avoid duplicates.
		player.sendItemList();

		msg.removeAttachments();

		Player? sender = World.getInstance().getPlayer(msg.getSenderId());
		if (adena > 0)
		{
			if (sender != null)
			{
				sender.addAdena("PayMail", adena, player, false);
				sm = new SystemMessagePacket(SystemMessageId.S2_COMPLETED_THE_PAYMENT_AND_YOU_RECEIVE_S1_ADENA);
				sm.Params.addLong(adena);
				sm.Params.addString(player.getName());
				sender.sendPacket(sm);
			}
			else
			{
				Item paidAdena = ItemData.getInstance().createItem("PayMail", Inventory.ADENA_ID, adena, player, null);
				paidAdena.setOwnerId(msg.getSenderId());
				paidAdena.setItemLocation(ItemLocation.INVENTORY);
				paidAdena.updateDatabase(true);
				World.getInstance().removeObject(paidAdena);
			}
		}
		else if (sender != null)
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_ACQUIRED_THE_ATTACHED_ITEM_TO_YOUR_MAIL);
			sm.Params.addString(player.getName());
			sender.sendPacket(sm);
		}

		player.sendPacket(new ExChangePostStatePacket(true, _msgId, Message.READED));
		player.sendPacket(SystemMessageId.MAIL_SUCCESSFULLY_RECEIVED);

        return ValueTask.CompletedTask;
    }
}