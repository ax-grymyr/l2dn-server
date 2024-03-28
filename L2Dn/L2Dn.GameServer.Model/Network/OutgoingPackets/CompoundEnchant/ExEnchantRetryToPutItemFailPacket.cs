using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantRetryToPutItemFailPacket: IOutgoingPacket
{
    public static readonly ExEnchantRetryToPutItemFailPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_RETRY_TO_PUT_ITEM_FAIL);
    }
}