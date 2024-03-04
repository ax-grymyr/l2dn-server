using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExReplySentPostPacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ExReplySentPostPacket));
    private readonly Message _msg;
    private readonly ICollection<Item> _items = null;
	
    public ExReplySentPostPacket(Message msg)
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
        writer.WritePacketCode(OutgoingPacketCodes.EX_REPLY_SENT_POST);
        
        writer.WriteInt32(0); // GOD
        writer.WriteInt32(_msg.getId());
        writer.WriteInt32(_msg.isLocked());
        writer.WriteString(_msg.getReceiverName());
        writer.WriteString(_msg.getSubject());
        writer.WriteString(_msg.getContent());
        if ((_items != null) && _items.Count != 0)
        {
            writer.WriteInt32(_items.Count);
            foreach (Item item in _items)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt32(item.getObjectId());
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