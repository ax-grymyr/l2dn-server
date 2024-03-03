using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Blessing;

public readonly struct ExBlessOptionCancelPacket: IOutgoingPacket
{
    private readonly int _result;
	
    public ExBlessOptionCancelPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLESS_OPTION_CANCEL);
        
        writer.WriteByte((byte)_result);
    }
}