using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExClosePartyRoomPacket: IOutgoingPacket
{
    public static readonly ExClosePartyRoomPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CLOSE_PARTY_ROOM);
    }
}