using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExCloseCommissionPacket: IOutgoingPacket
{
    public static readonly ExCloseCommissionPacket STATIC_PACKET = default;
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CLOSE_COMMISSION);
    }
}