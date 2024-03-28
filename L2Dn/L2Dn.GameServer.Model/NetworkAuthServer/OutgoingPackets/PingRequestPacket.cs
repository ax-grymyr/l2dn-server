using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;

internal readonly struct PingRequestPacket(int value): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PingRequest);
        
        writer.WriteInt32(value);
    }
}