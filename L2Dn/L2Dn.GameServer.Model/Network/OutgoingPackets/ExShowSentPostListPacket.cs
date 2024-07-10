using L2Dn.Extensions;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowSentPostListPacket: IOutgoingPacket
{
    private readonly List<Message> _outbox;
	
    public ExShowSentPostListPacket(int objectId)
    {
        _outbox = MailManager.getInstance().getOutbox(objectId);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_SENT_POST_LIST);
        
        writer.WriteInt32(DateTime.UtcNow.getEpochSecond());
        if ((_outbox != null) && !_outbox.isEmpty())
        {
            writer.WriteInt32(_outbox.Count);
            foreach (Message msg in _outbox)
            {
                writer.WriteInt32(msg.getId());
                writer.WriteString(msg.getSubject());
                writer.WriteString(msg.getReceiverName());
                writer.WriteInt32(msg.isLocked());
                writer.WriteInt32(msg.getExpirationSeconds());
                writer.WriteInt32(msg.isUnread());
                writer.WriteInt32(1);
                writer.WriteInt32(msg.hasAttachments());
                writer.WriteInt32(0);
            }
        }
        else
        {
            writer.WriteInt32(0);
        }
    }
}