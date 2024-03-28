using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExUnReadMailCountPacket: IOutgoingPacket
{
    private readonly int _count;
	
    public ExUnReadMailCountPacket(int count)
    {
        _count = count;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_UN_READ_MAIL_COUNT);
        writer.WriteInt32(_count);
    }
}