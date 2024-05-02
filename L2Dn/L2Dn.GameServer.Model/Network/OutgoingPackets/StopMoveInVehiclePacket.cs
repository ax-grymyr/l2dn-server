using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct StopMoveInVehiclePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatId;
    private readonly Location3D _location;
    private readonly int _heading;

    public StopMoveInVehiclePacket(Player player, int boatId)
    {
        _objectId = player.getObjectId();
        _boatId = boatId;
        _location = player.getInVehiclePosition();
        _heading = player.getHeading();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.STOP_MOVE_IN_VEHICLE);

        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatId);
        writer.WriteLocation3D(_location);
        writer.WriteInt32(_heading);
    }
}