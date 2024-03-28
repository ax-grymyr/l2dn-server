using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct DicePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _itemId;
    private readonly int _number;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
	
    /**
     * @param charObjId
     * @param itemId
     * @param number
     * @param x
     * @param y
     * @param z
     */
    public DicePacket(int charObjId, int itemId, int number, int x, int y, int z)
    {
        _objectId = charObjId;
        _itemId = itemId;
        _number = number;
        _x = x;
        _y = y;
        _z = z;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.DICE);
        
        writer.WriteInt32(_objectId); // object id of player
        writer.WriteInt32(_itemId); // item id of dice (spade) 4625,4626,4627,4628
        writer.WriteInt32(_number); // number rolled
        writer.WriteInt32(_x); // x
        writer.WriteInt32(_y); // y
        writer.WriteInt32(_z); // z
    }
}