using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExRemoveEnchantSupportItemResultPacket: IOutgoingPacket
{
    public static readonly ExRemoveEnchantSupportItemResultPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_REMOVE_ENCHANT_SUPPORT_ITEM_RESULT);
    }
}