using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExPutEnchantSupportItemResultPacket: IOutgoingPacket
{
    private readonly int _result;
	
    public ExPutEnchantSupportItemResultPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_ENCHANT_SUPPORT_ITEM_RESULT);
        
        writer.WriteInt32(_result);
    }
}