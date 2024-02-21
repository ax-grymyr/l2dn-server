using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TradeDonePacket: IOutgoingPacket
{
    private readonly int _num;
	
    public TradeDonePacket(int num)
    {
        _num = num;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TRADE_DONE);
        
        writer.WriteInt32(_num);
    }
}