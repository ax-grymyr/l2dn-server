using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExNoticePostSentPacket: IOutgoingPacket
{
    private static readonly ExNoticePostSentPacket STATIC_PACKET_TRUE = new(true);
    private static readonly ExNoticePostSentPacket STATIC_PACKET_FALSE = new(false);
	
    public static ExNoticePostSentPacket valueOf(bool result)
    {
        return result ? STATIC_PACKET_TRUE : STATIC_PACKET_FALSE;
    }
	
    private readonly bool _showAnim;
	
    public ExNoticePostSentPacket(bool showAnimation)
    {
        _showAnim = showAnimation;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_REPLY_WRITE_POST);
        
        writer.WriteInt32(_showAnim);
    }
}