using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantTwoOkPacket: IOutgoingPacket
{
    public static readonly ExEnchantTwoOkPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_TWO_OK);
        
        writer.WriteInt32(0); // success percent (if 0 - takes from dat, if 1 - will be 0.01)
    }
}