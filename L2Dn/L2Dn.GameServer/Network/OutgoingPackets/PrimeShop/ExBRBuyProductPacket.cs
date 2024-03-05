using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;

public readonly struct ExBRBuyProductPacket: IOutgoingPacket
{
    private readonly ExBrProductReplyType _reply;
	
    public ExBRBuyProductPacket(ExBrProductReplyType type)
    {
        _reply = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_BUY_PRODUCT);
        
        writer.WriteInt32((int)_reply);
    }
}