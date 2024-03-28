using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

/**
 * @author NosBit
 */
public readonly struct ExShowCommissionPacket: IOutgoingPacket
{
    public static readonly ExShowCommissionPacket STATIC_PACKET = default;

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_COMMISSION);
        writer.WriteInt32(1);
    }
}