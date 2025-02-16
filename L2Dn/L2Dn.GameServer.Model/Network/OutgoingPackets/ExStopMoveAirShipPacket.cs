using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExStopMoveAirShipPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _heading;
	
    public ExStopMoveAirShipPacket(Creature creature)
    {
        _objectId = creature.ObjectId;
        _x = creature.getX();
        _y = creature.getY();
        _z = creature.getZ();
        _heading = creature.getHeading();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STOP_MOVE_AIR_SHIP);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(_heading);
    }
}