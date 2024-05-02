using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct TeleportToLocationPacket(int objectId, LocationHeading location): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TELEPORT_TO_LOCATION);
        
        writer.WriteInt32(objectId);
        writer.WriteLocation3D(location.Location);
        writer.WriteInt32(0); // Fade 0, Instant 1.
        writer.WriteInt32(location.Heading);
        writer.WriteInt32(0); // Unknown.
    }
}