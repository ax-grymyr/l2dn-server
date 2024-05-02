using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct MoveToLocationInVehiclePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatId;
    private readonly Location3D _destination;
    private readonly Location3D _origin;
	
    /**
     * @param player
     * @param destination
     * @param origin
     */
    public MoveToLocationInVehiclePacket(Player player, Location3D destination, Location3D origin)
    {
        _objectId = player.getObjectId();
        _boatId = player.getBoat().getObjectId();
        _destination = destination;
        _origin = origin;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MOVE_TO_LOCATION_IN_VEHICLE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatId);
        writer.WriteLocation3D(_destination);
        writer.WriteLocation3D(_origin);
    }
}