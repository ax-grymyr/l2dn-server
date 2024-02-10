using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer.OutgoingPacket;

internal readonly struct PingResponsePacket(int value): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PingResponse);
        writer.WriteInt32(value);
    }
}