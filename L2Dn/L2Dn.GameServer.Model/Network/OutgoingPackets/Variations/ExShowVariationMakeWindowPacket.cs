using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Variations;

public readonly struct ExShowVariationMakeWindowPacket: IOutgoingPacket
{
    public static readonly ExShowVariationMakeWindowPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_VARIATION_MAKE_WINDOW);
    }
}