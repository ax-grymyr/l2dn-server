using L2Dn.GameServer.Model;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TradeOwnAddPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly TradeItem _item;
	
    public TradeOwnAddPacket(int sendType, TradeItem item)
    {
        _sendType = sendType;
        _item = item;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TRADE_OWN_ADD);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(1);
        }
        
        writer.WriteInt32(1);
        InventoryPacketHelper.WriteItem(writer, _item);
    }
}