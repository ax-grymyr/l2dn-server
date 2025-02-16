using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMoveToLocationAirShipPacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly int _tx;
    private readonly int _ty;
    private readonly int _tz;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
	
    public ExMoveToLocationAirShipPacket(Creature creature)
    {
        _objId = creature.ObjectId;
        _tx = creature.getXdestination();
        _ty = creature.getYdestination();
        _tz = creature.getZdestination();
        _x = creature.getX();
        _y = creature.getY();
        _z = creature.getZ();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MOVE_TO_LOCATION_AIR_SHIP);
        
        writer.WriteInt32(_objId);
        writer.WriteInt32(_tx);
        writer.WriteInt32(_ty);
        writer.WriteInt32(_tz);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
    }
}