using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;

internal readonly struct UpdateStatusPacket(bool online, ushort playerCount): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.UpdateStatus);
        
        writer.WriteByte(online);
        writer.WriteUInt16(playerCount);
    }
}