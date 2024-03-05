using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExPutEnchantTargetItemResultPacket: IOutgoingPacket
{
    private readonly int _result;
	
    public ExPutEnchantTargetItemResultPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_ENCHANT_TARGET_ITEM_RESULT);
        
        writer.WriteInt32(_result);
    }
}