using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct MagicSkillLaunchedPacket(int objectId): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x54); // packet code

        writer.WriteInt32(0); // Casting bar type: 0 - default, 1 - default up, 2 - blue, 3 - green, 4 - red.
        writer.WriteInt32(objectId);
        writer.WriteInt32(60018); // teleport
        writer.WriteInt32(1); // skill level
        writer.WriteInt32(1); // targets count

        writer.WriteInt32(objectId); // targets object id
    }
}
