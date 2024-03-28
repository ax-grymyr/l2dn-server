using L2Dn.GameServer.InstanceManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;

public readonly struct WorldExchangeAveragePricePacket: IOutgoingPacket
{
    private readonly int _itemId;
    private readonly long _averagePrice;

    public WorldExchangeAveragePricePacket(int itemId)
    {
        _itemId = itemId;
        _averagePrice = WorldExchangeManager.getInstance().getAveragePriceOfItem(itemId);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_EXCHANGE_AVERAGE_PRICE);
        writer.WriteInt32(_itemId);
        writer.WriteInt64(_averagePrice);
    }
}