using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Blessing;

public readonly struct ExBlessOptionEnchantPacket: IOutgoingPacket
{
    private readonly int _result;
	
    public ExBlessOptionEnchantPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLESS_OPTION_ENCHANT);
        
        writer.WriteInt32(_result);
    }
}