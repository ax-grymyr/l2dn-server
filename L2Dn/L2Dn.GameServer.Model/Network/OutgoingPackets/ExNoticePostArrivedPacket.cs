using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExNoticePostArrivedPacket: IOutgoingPacket
{
    private readonly bool _showAnim;
	
    public ExNoticePostArrivedPacket(bool showAnimation)
    {
        _showAnim = showAnimation;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NOTICE_POST_ARRIVED);
        writer.WriteInt32(_showAnim);
    }
}