using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCloseMPCCPacket: IOutgoingPacket
{
    public static readonly ExCloseMPCCPacket STATIC_PACKET = default;

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CLOSE_MPCC);
    }
}