using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExPutEnchantScrollItemResultPacket: IOutgoingPacket
{
    private readonly int _result;
	
    public ExPutEnchantScrollItemResultPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_ENCHANT_SCROLL_ITEM_RESULT);
        
        writer.WriteInt32(_result);
    }
}