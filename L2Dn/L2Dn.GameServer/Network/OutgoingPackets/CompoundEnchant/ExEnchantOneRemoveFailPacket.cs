using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantOneRemoveFailPacket: IOutgoingPacket
{
    public static readonly ExEnchantOneRemoveFailPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_ONE_REMOVE_FAIL);
    }
}