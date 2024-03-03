using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;

public readonly struct ExEnchantFailPacket: IOutgoingPacket
{
    public static readonly ExEnchantFailPacket STATIC_PACKET = new(0, 0);
	
    private readonly int _itemOne;
    private readonly int _itemTwo;
	
    public ExEnchantFailPacket(int itemOne, int itemTwo)
    {
        _itemOne = itemOne;
        _itemTwo = itemTwo;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_FAIL);
        
        writer.WriteInt32(_itemOne);
        writer.WriteInt32(_itemTwo);
    }
}