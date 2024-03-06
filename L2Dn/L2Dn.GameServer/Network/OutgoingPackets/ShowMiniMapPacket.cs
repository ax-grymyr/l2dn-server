using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShowMiniMapPacket(int mapId): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOW_MINIMAP);
        
        writer.WriteInt32(mapId);
        writer.WriteInt32(0); // Seven Signs state
    }
}
