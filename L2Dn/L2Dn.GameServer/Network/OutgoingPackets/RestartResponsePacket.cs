using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct RestartResponsePacket(bool response): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x71); // packet code (0x5F in C4)
        
        writer.WriteInt32(response ? 1 : 0);
        //writer.WriteString("ok merong~ khaha"); // Message like L2OFF (for C4 ???)
    }
}
