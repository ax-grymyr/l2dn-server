using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantTwoRemoveFailPacket: IOutgoingPacket
{
    public static readonly ExEnchantTwoRemoveFailPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_TWO_REMOVE_FAIL);
    }
}