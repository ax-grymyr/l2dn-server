using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantOneOkPacket: IOutgoingPacket
{
    public static readonly ExEnchantOneOkPacket STATIC_PACKET = default;

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_ONE_OK);
    }
}