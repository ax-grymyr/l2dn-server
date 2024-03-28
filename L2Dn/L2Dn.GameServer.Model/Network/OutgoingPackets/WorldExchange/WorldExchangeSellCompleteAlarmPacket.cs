using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;

public readonly struct WorldExchangeSellCompleteAlarmPacket: IOutgoingPacket
{
    private readonly int _itemId;
    private readonly long _amount;
	
    public WorldExchangeSellCompleteAlarmPacket(int itemId, long amount)
    {
        _itemId = itemId;
        _amount = amount;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_EXCHANGE_SELL_COMPLETE_ALARM);
        writer.WriteInt32(_itemId);
        writer.WriteInt64(_amount);
    }
}