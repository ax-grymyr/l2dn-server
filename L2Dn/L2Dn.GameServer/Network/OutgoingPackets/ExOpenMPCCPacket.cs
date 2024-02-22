using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExOpenMPCCPacket: IOutgoingPacket
{
    public static readonly ExOpenMPCCPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_OPEN_MPCC);
    }
}