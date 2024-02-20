using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartySmallWindowDeleteAllPacket: IOutgoingPacket
{
    public static readonly PartySmallWindowDeleteAllPacket STATIC_PACKET = default;

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_SMALL_WINDOW_DELETE_ALL);
    }
}