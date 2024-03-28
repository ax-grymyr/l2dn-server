using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct MoveToLocationPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _xDst;
    private readonly int _yDst;
    private readonly int _zDst;

    public MoveToLocationPacket(Creature creature)
    {
        _objectId = creature.getObjectId();
        _x = creature.getX();
        _y = creature.getY();
        _z = creature.getZ();
        _xDst = creature.getXdestination();
        _yDst = creature.getYdestination();
        _zDst = creature.getZdestination();
    }
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MOVE_TO_LOCATION);

        writer.WriteInt32(_objectId);
        writer.WriteInt32(_xDst);
        writer.WriteInt32(_yDst);
        writer.WriteInt32(_zDst);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
    }
}