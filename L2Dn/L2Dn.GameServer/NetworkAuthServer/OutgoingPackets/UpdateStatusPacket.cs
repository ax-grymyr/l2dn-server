using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;

internal readonly struct UpdateStatusPacket(ushort playerCount): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.UpdateStatus);
        
        writer.WriteUInt16(playerCount);
    }
}