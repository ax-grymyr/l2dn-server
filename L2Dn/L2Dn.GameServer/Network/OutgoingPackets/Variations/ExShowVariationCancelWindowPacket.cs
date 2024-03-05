using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Variations;

public readonly struct ExShowVariationCancelWindowPacket: IOutgoingPacket
{
    public static readonly ExShowVariationCancelWindowPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_VARIATION_CANCEL_WINDOW);
    }
}