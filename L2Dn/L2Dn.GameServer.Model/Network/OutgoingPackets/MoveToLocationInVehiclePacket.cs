using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct MoveToLocationInVehiclePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatId;
    private readonly Location _destination;
    private readonly Location _origin;
	
    /**
     * @param player
     * @param destination
     * @param origin
     */
    public MoveToLocationInVehiclePacket(Player player, Location destination, Location origin)
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
        writer.WriteInt32(_destination.getX());
        writer.WriteInt32(_destination.getY());
        writer.WriteInt32(_destination.getZ());
        writer.WriteInt32(_origin.getX());
        writer.WriteInt32(_origin.getY());
        writer.WriteInt32(_origin.getZ());
    }
}