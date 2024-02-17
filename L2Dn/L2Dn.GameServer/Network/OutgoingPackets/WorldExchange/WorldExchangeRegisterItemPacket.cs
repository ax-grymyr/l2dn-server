using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;

public readonly struct WorldExchangeRegisterItemPacket: IOutgoingPacket
{
    public static readonly WorldExchangeRegisterItemPacket FAIL = new(-1, -1L, 0);
	
    private readonly int _itemObjectId;
    private readonly long _itemAmount;
    private readonly byte _type;
	
    public WorldExchangeRegisterItemPacket(int itemObjectId, long itemAmount, byte type)
    {
        _itemObjectId = itemObjectId;
        _itemAmount = itemAmount;
        _type = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_EXCHANGE_REGI_ITEM);
        writer.WriteInt32(_itemObjectId);
        writer.WriteInt64(_itemAmount);
        writer.WriteByte(_type);
    }
}