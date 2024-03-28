using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ValidateLocationInVehiclePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatObjId;
    private readonly int _heading;
    private readonly Location _pos;
	
    public ValidateLocationInVehiclePacket(Player player)
    {
        _objectId = player.getObjectId();
        _boatObjId = player.getBoat().getObjectId();
        _heading = player.getHeading();
        _pos = player.getInVehiclePosition();
    }
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.VALIDATE_LOCATION_IN_VEHICLE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatObjId);
        writer.WriteInt32(_pos.getX());
        writer.WriteInt32(_pos.getY());
        writer.WriteInt32(_pos.getZ());
        writer.WriteInt32(_heading);
    }
}