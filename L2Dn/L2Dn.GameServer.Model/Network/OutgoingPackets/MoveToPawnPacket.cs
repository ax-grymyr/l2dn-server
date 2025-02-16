using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct MoveToPawnPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _targetId;
    private readonly int _distance;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _tx;
    private readonly int _ty;
    private readonly int _tz;

    public MoveToPawnPacket(Creature creature, WorldObject target, int distance)
    {
        _objectId = creature.ObjectId;
        _targetId = target.ObjectId;
        _distance = distance;
        _x = creature.getX();
        _y = creature.getY();
        _z = creature.getZ();
        _tx = target.getX();
        _ty = target.getY();
        _tz = target.getZ();
    }
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MOVE_TO_PAWN);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_targetId);
        writer.WriteInt32(_distance);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(_tx);
        writer.WriteInt32(_ty);
        writer.WriteInt32(_tz);
    }
}