using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Blessing;

public readonly struct ExBlessOptionPutItemPacket: IOutgoingPacket
{
    private readonly int _result;
	
    public ExBlessOptionPutItemPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLESS_OPTION_PUT_ITEM);
        
        writer.WriteByte((byte)_result);
    }
}