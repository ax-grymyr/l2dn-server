using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ensoul;

public readonly struct ExShowEnsoulExtractionWindowPacket: IOutgoingPacket
{
    public static readonly ExShowEnsoulExtractionWindowPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENSOUL_EXTRACTION_SHOW);
    }
}