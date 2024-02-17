using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;

public readonly struct WorldExchangeTotalListPacket: IOutgoingPacket
{
    private readonly List<int> _itemIds;
	
    public WorldExchangeTotalListPacket(List<int> itemIds)
    {
        _itemIds = itemIds;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_EXCHANGE_TOTAL_LIST);
        writer.WriteInt32(_itemIds.Count);
        foreach (int id in _itemIds)
        {
            writer.WriteInt32(id); // ItemClassID
            writer.WriteInt64(0); // MinPricePerPiece
            writer.WriteInt64(0); // Price
            writer.WriteInt64(1); // Amount
        }
    }
}