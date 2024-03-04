using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMoveToLocationInAirShipPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _airShipId;
    private readonly Location _destination;
    private readonly int _heading;
	
    /**
     * @param player
     */
    public ExMoveToLocationInAirShipPacket(Player player)
    {
        _objectId = player.getObjectId();
        _airShipId = player.getAirShip().getObjectId();
        _destination = player.getInVehiclePosition();
        _heading = player.getHeading();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MOVE_TO_LOCATION_IN_AIR_SHIP);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_airShipId);
        writer.WriteInt32(_destination.getX());
        writer.WriteInt32(_destination.getY());
        writer.WriteInt32(_destination.getZ());
        writer.WriteInt32(_heading);
    }
}