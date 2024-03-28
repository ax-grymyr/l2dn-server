using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GameGuardQueryPacket: IOutgoingPacket
{
    public static readonly GameGuardQueryPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GAME_GUARD_QUERY);
        
        writer.WriteUInt32(0x27533DD9);
        writer.WriteUInt32(0x2E72A51D);
        writer.WriteUInt32(0x2017038B);
        writer.WriteUInt32(0xC35B1EA3);
    }
}