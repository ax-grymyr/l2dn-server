using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct EarthquakePacket(Location3D location, int intensity, int duration): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EARTHQUAKE);

        writer.WriteLocation3D(location);
        writer.WriteInt32(intensity);
        writer.WriteInt32(duration);
        writer.WriteInt32(0); // Unknown
    }
}