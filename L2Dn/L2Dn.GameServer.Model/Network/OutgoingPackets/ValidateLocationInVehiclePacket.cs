using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ValidateLocationInVehiclePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatObjId;
    private readonly Location3D _location;
    private readonly int _heading;

    public ValidateLocationInVehiclePacket(Player player)
    {
        _objectId = player.getObjectId();
        _boatObjId = player.getBoat().getObjectId();
        _location = player.getInVehiclePosition();
        _heading = player.getHeading();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.VALIDATE_LOCATION_IN_VEHICLE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatObjId);
        writer.WriteLocation3D(_location);
        writer.WriteInt32(_heading);
    }
}