using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct MagicSkillCancelledPacket(int objectId): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x49); // packet code

        writer.WriteInt32(objectId);
    }
}
