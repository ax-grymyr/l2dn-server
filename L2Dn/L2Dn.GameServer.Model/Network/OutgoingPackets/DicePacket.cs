using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct DicePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _itemId;
    private readonly int _number;
    private readonly Location3D _location;

    /**
     * @param charObjId
     * @param itemId
     * @param number
     * @param x
     * @param y
     * @param z
     */
    public DicePacket(int charObjId, int itemId, int number, Location3D location)
    {
        _objectId = charObjId;
        _itemId = itemId;
        _number = number;
        _location = location;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.DICE);

        writer.WriteInt32(_objectId); // object id of player
        writer.WriteInt32(_itemId); // item id of dice (spade) 4625,4626,4627,4628
        writer.WriteInt32(_number); // number rolled
        writer.WriteLocation3D(_location);
    }
}