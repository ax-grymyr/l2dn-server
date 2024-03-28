using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantOneFailPacket: IOutgoingPacket
{
    public static readonly ExEnchantOneFailPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_ONE_FAIL);
    }
}