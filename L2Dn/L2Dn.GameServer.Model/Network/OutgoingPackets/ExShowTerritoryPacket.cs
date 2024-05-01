using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowTerritoryPacket(int minZ, int maxZ, List<Location2D> vertices): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_TERRITORY);

        writer.WriteInt32(vertices.Count);
        writer.WriteInt32(minZ);
        writer.WriteInt32(maxZ);
        foreach (Location2D loc in vertices)
        {
            writer.WriteInt32(loc.X);
            writer.WriteInt32(loc.Y);
        }
    }
}