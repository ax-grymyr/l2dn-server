using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SendTradeRequestPacket: IOutgoingPacket
{
    private readonly int _senderId;
	
    public SendTradeRequestPacket(int senderId)
    {
        _senderId = senderId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TRADE_REQUEST);
        
        writer.WriteInt32(_senderId);
    }
}