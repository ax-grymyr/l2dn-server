using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewHenna;

public readonly struct NewHennaPotenEnchantResetPacket: IOutgoingPacket
{
    private readonly bool _success;
	
    public NewHennaPotenEnchantResetPacket(bool success)
    {
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEW_HENNA_POTEN_ENCHANT_RESET);
        
        writer.WriteByte(_success);
    }
}