using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct ValidatePositionPacket(int objectId, Location location, int heading)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x79); // packet code (0x61 in C4)
        
        writer.WriteInt32(objectId);
        writer.WriteInt32(location.X);
        writer.WriteInt32(location.Y);
        writer.WriteInt32(location.Z);
        writer.WriteInt32(heading);
        writer.WriteByte(0xFF); // unknown
    }
}
