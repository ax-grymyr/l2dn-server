using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;

public readonly struct WorldExchangeBuyItemPacket: IOutgoingPacket
{
    public static readonly WorldExchangeBuyItemPacket FAIL = new(-1, -1L, (byte) 0);
	
    private readonly int _itemObjectId;
    private readonly long _itemAmount;
    private readonly byte _type;
	
    public WorldExchangeBuyItemPacket(int itemObjectId, long itemAmount, byte type)
    {
        _itemObjectId = itemObjectId;
        _itemAmount = itemAmount;
        _type = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_EXCHANGE_BUY_ITEM);
        writer.WriteInt32(_itemObjectId);
        writer.WriteInt64(_itemAmount);
        writer.WriteByte(_type);
    }
}