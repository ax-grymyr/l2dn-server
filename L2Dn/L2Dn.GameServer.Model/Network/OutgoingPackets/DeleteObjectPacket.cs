using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct DeleteObjectPacket(int objectId): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x08); // packet code
        writer.WriteInt32(objectId);
        writer.WriteByte(0); // c2
    }
}