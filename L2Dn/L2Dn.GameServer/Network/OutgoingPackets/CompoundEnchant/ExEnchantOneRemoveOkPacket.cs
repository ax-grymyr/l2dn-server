using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantOneRemoveOkPacket: IOutgoingPacket
{
    public static readonly ExEnchantOneRemoveOkPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_ONE_REMOVE_OK);
    }
}