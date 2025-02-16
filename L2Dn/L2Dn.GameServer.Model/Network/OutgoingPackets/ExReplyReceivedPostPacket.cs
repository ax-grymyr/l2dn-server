using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExReplyReceivedPostPacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ExReplyReceivedPostPacket));
    private readonly Message _msg;
    private readonly ICollection<Item> _items;
	
    public ExReplyReceivedPostPacket(Message msg)
    {
        _msg = msg;
        if (msg.hasAttachments())
        {
            ItemContainer attachments = msg.getAttachments();
            if ((attachments != null) && (attachments.getSize() > 0))
            {
                _items = attachments.getItems();
            }
            else
            {
                _logger.Warn("Message " + msg.getId() + " has attachments but itemcontainer is empty.");
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_REPLY_RECEIVED_POST);
        
        writer.WriteInt32((int)_msg.getMailType()); // GOD
        if (_msg.getMailType() == MailType.COMMISSION_ITEM_RETURNED)
        {
            writer.WriteInt32((int)SystemMessageId.THE_REGISTRATION_PERIOD_FOR_THE_ITEM_YOU_REGISTERED_HAS_EXPIRED);
            writer.WriteInt32((int)SystemMessageId.THE_AUCTION_HOUSE_REGISTRATION_PERIOD_HAS_EXPIRED_AND_THE_CORRESPONDING_ITEM_IS_BEING_FORWARDED);
        }
        else if (_msg.getMailType() == MailType.COMMISSION_ITEM_SOLD)
        {
            writer.WriteInt32(_msg.getItemId());
            writer.WriteInt32(_msg.getEnchantLvl());
            for (int i = 0; i < 6; i++)
            {
                writer.WriteInt32(_msg.getElementals()[i]);
            }
            writer.WriteInt32((int)SystemMessageId.THE_ITEM_YOU_REGISTERED_HAS_BEEN_SOLD);
            writer.WriteInt32((int)SystemMessageId.S1_SOLD);
        }
        writer.WriteInt32(_msg.getId());
        writer.WriteInt32(_msg.isLocked());
        writer.WriteInt32(0); // Unknown
        writer.WriteString(_msg.getSenderName());
        writer.WriteString(_msg.getSubject());
        writer.WriteString(_msg.getContent());
        if ((_items != null) && _items.Count != 0)
        {
            writer.WriteInt32(_items.Count);
            foreach (Item item in _items)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt32(item.ObjectId);
            }
        }
        else
        {
            writer.WriteInt32(0);
        }
        
        writer.WriteInt64(_msg.getReqAdena());
        writer.WriteInt32(_msg.hasAttachments());
        writer.WriteInt32(_msg.isReturned());
    }
}