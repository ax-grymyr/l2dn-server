using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TradeOtherDonePacket: IOutgoingPacket
{
    public static readonly TradeOtherDonePacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TRADE_PRESS_OTHER_OK);
    }
}