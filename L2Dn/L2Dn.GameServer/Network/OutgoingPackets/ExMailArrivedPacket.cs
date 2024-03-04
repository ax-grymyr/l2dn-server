using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMailArrivedPacket: IOutgoingPacket
{
    public static readonly ExMailArrivedPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MAIL_ARRIVED);
    }
}