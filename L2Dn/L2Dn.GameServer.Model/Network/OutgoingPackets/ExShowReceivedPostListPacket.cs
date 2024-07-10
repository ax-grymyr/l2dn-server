using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowReceivedPostListPacket: IOutgoingPacket
{
    private const int MESSAGE_FEE = 100;
    private const int MESSAGE_FEE_PER_SLOT = 1000;
	
    private readonly List<Message> _inbox;
	
    public ExShowReceivedPostListPacket(int objectId)
    {
        _inbox = MailManager.getInstance().getInbox(objectId);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_RECEIVED_POST_LIST);
        
        writer.WriteInt32(DateTime.UtcNow.getEpochSecond());
        if (_inbox != null && _inbox.Count != 0)
        {
            writer.WriteInt32(_inbox.Count);
            foreach (Message msg in _inbox)
            {
                writer.WriteInt32((int)msg.getMailType());
                if (msg.getMailType() == MailType.COMMISSION_ITEM_SOLD)
                {
                    writer.WriteInt32((int)SystemMessageId.THE_ITEM_YOU_REGISTERED_HAS_BEEN_SOLD);
                }
                else if (msg.getMailType() == MailType.COMMISSION_ITEM_RETURNED)
                {
                    writer.WriteInt32((int)SystemMessageId.THE_REGISTRATION_PERIOD_FOR_THE_ITEM_YOU_REGISTERED_HAS_EXPIRED);
                }
                
                writer.WriteInt32(msg.getId());
                writer.WriteString(msg.getSubject());
                writer.WriteString(msg.getSenderName());
                writer.WriteInt32(msg.isLocked());
                writer.WriteInt32(msg.getExpirationSeconds());
                writer.WriteInt32(msg.isUnread());
                writer.WriteInt32(!(msg.getMailType() == MailType.COMMISSION_ITEM_SOLD || msg.getMailType() == MailType.COMMISSION_ITEM_RETURNED));
                writer.WriteInt32(msg.hasAttachments());
                writer.WriteInt32(msg.isReturned());
                writer.WriteInt32(0); // SysString in some case it seems
            }
        }
        else
        {
            writer.WriteInt32(0);
        }
        
        writer.WriteInt32(MESSAGE_FEE);
        writer.WriteInt32(MESSAGE_FEE_PER_SLOT);
    }
}