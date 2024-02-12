using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct VehicleDeparturePacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _moveSpeed;
    private readonly int _rotationSpeed;
	
    public VehicleDeparturePacket(Vehicle boat)
    {
        _objId = boat.getObjectId();
        _x = boat.getXdestination();
        _y = boat.getYdestination();
        _z = boat.getZdestination();
        _moveSpeed = (int) boat.getMoveSpeed();
        _rotationSpeed = (int) boat.getStat().getRotationSpeed();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.VEHICLE_DEPARTURE);
        
        writer.WriteInt32(_objId);
        writer.WriteInt32(_moveSpeed);
        writer.WriteInt32(_rotationSpeed);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
    }
}