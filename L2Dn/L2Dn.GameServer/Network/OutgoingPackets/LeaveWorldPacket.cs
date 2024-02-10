using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct LeaveWorldPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        // 0x7E in C4
        writer.WriteByte(0x84); // packet code
    }
}
