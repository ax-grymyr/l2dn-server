using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct ActionFailedPacket(int castingType): IOutgoingPacket
{
    public static readonly ActionFailedPacket STATIC_PACKET = new ActionFailedPacket(0);
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x1F); // packet code (0x25 in C4)

        writer.WriteInt32(castingType); // MagicSkillUse castingType
    }
}
