using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSendPostPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 12; // length of the one item

    private const int MAX_RECV_LENGTH = 16;
    private const int MAX_SUBJ_LENGTH = 128;
    private const int MAX_TEXT_LENGTH = 512;
    private const int MAX_ATTACHMENTS = 8;
    private const int INBOX_SIZE = 240;
    private const int OUTBOX_SIZE = 240;

    private const int MESSAGE_FEE = 100;
    private const int MESSAGE_FEE_PER_SLOT = 1000; // 100 adena message fee + 1000 per each item slot

    private string _receiver;
    private bool _isCod;
    private string _subject;
    private string _text;
    private AttachmentItem[]? _items;
    private long _reqAdena;

    public void ReadContent(PacketBitReader reader)
    {
        _receiver = reader.ReadString();
        _isCod = reader.ReadInt32() != 0;
        _subject = reader.ReadString();
        _text = reader.ReadString();

        int attachCount = reader.ReadInt32();
        if (attachCount < 0 || attachCount > Config.Character.MAX_ITEM_IN_PACKET ||
            attachCount * BATCH_LENGTH + 8 != reader.Length)
        {
	        return;
        }

        if (attachCount > 0)
        {
            _items = new AttachmentItem[attachCount];
            for (int i = 0; i < attachCount; i++)
            {
                int objectId = reader.ReadInt32();
                long count = reader.ReadInt64();
                if (objectId < 1 || count < 0)
                {
                    _items = null;
                    return;
                }

                _items[i] = new AttachmentItem(objectId, count);
            }
        }

        _reqAdena = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    if (!Config.General.ALLOW_MAIL)
		    return ValueTask.CompletedTask;

	    Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (!Config.General.ALLOW_ATTACHMENTS)
		{
			_items = null;
			_isCod = false;
			_reqAdena = 0;
		}

		if (!player.getAccessLevel().AllowTransaction)
		{
			player.sendMessage("Transactions are disabled for your Access Level.");
			return ValueTask.CompletedTask;
		}

		if (player.isInCombat() && _items != null)
		{
			player.sendPacket(SystemMessageId.NOT_AVAILABLE_IN_COMBAT);
			return ValueTask.CompletedTask;
		}

		if (player.isDead() && _items != null)
		{
			player.sendPacket(SystemMessageId.YOU_ARE_DEAD_AND_CANNOT_PERFORM_THIS_ACTION);
			return ValueTask.CompletedTask;
		}

		if (player.getActiveTradeList() != null)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_FORWARD_DURING_AN_EXCHANGE);
			return ValueTask.CompletedTask;
		}

		if (player.hasItemRequest())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_SEND_MAIL_WHILE_ENCHANTING_AN_ITEM_BESTOWING_AN_ATTRIBUTE_OR_COMBINING_JEWELS);
			return ValueTask.CompletedTask;
		}

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_FORWARD_BECAUSE_THE_PRIVATE_STORE_OR_WORKSHOP_IS_IN_PROGRESS);
			return ValueTask.CompletedTask;
		}

		if (_receiver.Length > MAX_RECV_LENGTH)
		{
			player.sendPacket(SystemMessageId.THE_ALLOWED_LENGTH_FOR_RECIPIENT_EXCEEDED);
			return ValueTask.CompletedTask;
		}

		if (_subject.Length > MAX_SUBJ_LENGTH)
		{
			player.sendPacket(SystemMessageId.THE_ALLOWED_LENGTH_FOR_A_TITLE_EXCEEDED);
			return ValueTask.CompletedTask;
		}

		if (_text.Length > MAX_TEXT_LENGTH)
		{
			// not found message for this
			player.sendPacket(SystemMessageId.THE_ALLOWED_LENGTH_FOR_A_TITLE_EXCEEDED);
			return ValueTask.CompletedTask;
		}

		if (_items != null && _items.Length > MAX_ATTACHMENTS)
		{
			player.sendPacket(SystemMessageId.ITEM_SELECTION_IS_POSSIBLE_UP_TO_8);
			return ValueTask.CompletedTask;
		}

		if (_reqAdena < 0 || _reqAdena > Inventory.MAX_ADENA)
			return ValueTask.CompletedTask;

		if (_isCod)
		{
			if (_reqAdena == 0)
			{
				player.sendPacket(SystemMessageId.WHEN_NOT_ENTERING_THE_AMOUNT_FOR_THE_PAYMENT_REQUEST_YOU_CANNOT_SEND_ANY_MAIL);
				return ValueTask.CompletedTask;
			}

			if (_items == null || _items.Length == 0)
			{
				player.sendPacket(SystemMessageId.IT_S_A_PAYMENT_REQUEST_TRANSACTION_PLEASE_ATTACH_THE_ITEM);
				return ValueTask.CompletedTask;
			}
		}

		if (FakePlayerData.getInstance().isTalkable(_receiver))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_BLOCKED_YOU_YOU_CANNOT_SEND_MAIL_TO_THIS_CHARACTER);
			sm.Params.addString(FakePlayerData.getInstance().getProperName(_receiver) ?? string.Empty);
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}

		int receiverId = CharInfoTable.getInstance().getIdByName(_receiver);
		if (receiverId <= 0)
		{
			player.sendPacket(SystemMessageId.WHEN_THE_RECIPIENT_DOESN_T_EXIST_OR_THE_CHARACTER_HAS_BEEN_DELETED_SENDING_MAIL_IS_NOT_POSSIBLE);
			return ValueTask.CompletedTask;
		}

		if (receiverId == player.ObjectId)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_SEND_A_MAIL_TO_YOURSELF);
			return ValueTask.CompletedTask;
		}

		int level = CharInfoTable.getInstance().getAccessLevelById(receiverId);
		AccessLevel accessLevel = AccessLevelData.Instance.GetAccessLevel(level);
		if (accessLevel.IsGM && !player.getAccessLevel().IsGM)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_MESSAGE_TO_C1_DID_NOT_REACH_ITS_RECIPIENT_YOU_CANNOT_SEND_MAIL_TO_THE_GM_STAFF);
			sm.Params.addString(_receiver);
			player.sendPacket(sm);
            return ValueTask.CompletedTask;
		}

		if (player.isJailed() && ((Config.General.JAIL_DISABLE_TRANSACTION && _items != null) || Config.General.JAIL_DISABLE_CHAT))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_FORWARD_IN_A_NON_PEACE_ZONE_LOCATION);
			return ValueTask.CompletedTask;
		}

		if (BlockList.isInBlockList(receiverId, player.ObjectId))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_BLOCKED_YOU_YOU_CANNOT_SEND_MAIL_TO_THIS_CHARACTER);
			sm.Params.addString(_receiver);
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}

		if (MailManager.getInstance().getOutboxSize(player.ObjectId) >= OUTBOX_SIZE)
		{
			player.sendPacket(SystemMessageId.THE_MAIL_LIMIT_240_HAS_BEEN_EXCEEDED_AND_THIS_CANNOT_BE_FORWARDED);
			return ValueTask.CompletedTask;
		}

		if (MailManager.getInstance().getInboxSize(receiverId) >= INBOX_SIZE)
		{
			player.sendPacket(SystemMessageId.THE_MAIL_LIMIT_240_HAS_BEEN_EXCEEDED_AND_THIS_CANNOT_BE_FORWARDED);
			return ValueTask.CompletedTask;
		}

		// TODO: flood protection
		// if (!client.getFloodProtectors().canSendMail())
		// {
		// 	player.sendPacket(SystemMessageId.THE_PREVIOUS_MAIL_WAS_FORWARDED_LESS_THAN_10_SEC_AGO_AND_THIS_CANNOT_BE_FORWARDED);
		// 	return ValueTask.CompletedTask;
		// }

		Message msg = new Message(player.ObjectId, receiverId, _isCod, _subject, _text, _reqAdena);
		if (removeItems(player, msg))
		{
			MailManager.getInstance().sendMessage(msg);
			player.sendPacket(ExNoticePostSentPacket.valueOf(true));
			player.sendPacket(SystemMessageId.MAIL_SUCCESSFULLY_SENT);
		}

		return ValueTask.CompletedTask;
    }

	private bool removeItems(Player player, Message msg)
	{
		long currentAdena = player.getAdena();
		long fee = MESSAGE_FEE;
		if (_items != null)
		{
			foreach (AttachmentItem i in _items)
			{
				// Check validity of requested item
				Item? item = player.checkItemManipulation(i.ObjectId, i.Count, "attach");
				if (item == null || !item.isTradeable() || item.isEquipped())
				{
					player.sendPacket(SystemMessageId.THE_ITEM_THAT_YOU_RE_TRYING_TO_SEND_CANNOT_BE_FORWARDED_BECAUSE_IT_ISN_T_PROPER);
					return false;
				}

				fee += MESSAGE_FEE_PER_SLOT;
				if (item.getId() == Inventory.ADENA_ID)
				{
					currentAdena -= i.Count;
				}
			}
		}

		// Check if enough adena and charge the fee
		if (currentAdena < fee || !player.reduceAdena("MailFee", fee, null, false))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_FORWARD_BECAUSE_YOU_DON_T_HAVE_ENOUGH_ADENA);
			return false;
		}

		if (_items == null)
		{
			return true;
		}

		Mail attachments = msg.createAttachments();

		// message already has attachments ? oO
		if (attachments == null)
		{
			return false;
		}

		// Proceed to the transfer
		List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		foreach (AttachmentItem i in _items)
		{
			// Check validity of requested item
			Item? oldItem = player.checkItemManipulation(i.ObjectId, i.Count, "attach");
			if (oldItem == null || !oldItem.isTradeable() || oldItem.isEquipped())
			{
				PacketLogger.Instance.Warn("Error adding attachment for char " + player.getName() +
				                           " (olditem == null)");

				return false;
			}

			Item? newItem = player.getInventory().transferItem("SendMail", i.ObjectId, i.Count, attachments,
				player, msg.getReceiverName() + "[" + msg.getReceiverId() + "]");

			if (newItem == null)
			{
				PacketLogger.Instance.Warn("Error adding attachment for char " + player.getName() +
				                           " (newitem == null)");

				continue;
			}

			newItem.setItemLocation(newItem.getItemLocation(), msg.getId());

			if (oldItem.getCount() > 0 && oldItem != newItem)
			{
				itemsToUpdate.Add(new ItemInfo(oldItem, ItemChangeType.MODIFIED));
			}
			else
			{
				itemsToUpdate.Add(new ItemInfo(oldItem, ItemChangeType.REMOVED));
			}
		}

		// Send updated item list to the player
		InventoryUpdatePacket playerIU = new InventoryUpdatePacket(itemsToUpdate);
		player.sendInventoryUpdate(playerIU);

		return true;
	}

	private record struct AttachmentItem(int ObjectId, long Count);
}