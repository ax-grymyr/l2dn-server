using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct ShowMiniMapPacket(int mapId, int sevenSignsPeriod): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0xA3); // packet code (0x9D in C4)
        
        writer.WriteInt32(mapId);
        writer.WriteInt32(sevenSignsPeriod);
    }
}
