using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct LeaveWorldPacket: IOutgoingPacket
{
    public static LeaveWorldPacket STATIC_PACKET => default;
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.LEAVE_WORLD);
    }
}