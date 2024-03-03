using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantSuccessPacket: IOutgoingPacket
{
    private readonly int _itemId;
	
    public ExEnchantSuccessPacket(int itemId)
    {
        _itemId = itemId;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_SUCESS);
    
        writer.WriteInt32(_itemId);
    }
}