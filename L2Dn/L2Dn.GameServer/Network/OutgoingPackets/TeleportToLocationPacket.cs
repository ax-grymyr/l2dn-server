using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct TeleportToLocationPacket(int objectId, Location location): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x22); // packet code

        writer.WriteInt32(objectId);
        writer.WriteInt32(location.X);
        writer.WriteInt32(location.Y);
        writer.WriteInt32(location.Z);
        writer.WriteInt32(0); // Fade 0, Instant 1.
        writer.WriteInt32(0); // heading
        writer.WriteInt32(0); // Unknown.
    }
}
