using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ServerClosePacket: IOutgoingPacket
{
    public static readonly ServerClosePacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SEVER_CLOSE);
    }
}