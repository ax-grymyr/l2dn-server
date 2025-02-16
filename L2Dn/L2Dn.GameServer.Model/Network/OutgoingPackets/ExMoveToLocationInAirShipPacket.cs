using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMoveToLocationInAirShipPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _airShipId;
    private readonly Location _destination;

    /**
     * @param player
     */
    public ExMoveToLocationInAirShipPacket(Player player)
    {
        _objectId = player.ObjectId;
        _airShipId = player.getAirShip().ObjectId;
        _destination = new Location(player.getInVehiclePosition(), player.getHeading());
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MOVE_TO_LOCATION_IN_AIR_SHIP);

        writer.WriteInt32(_objectId);
        writer.WriteInt32(_airShipId);
        writer.WriteLocation(_destination);
    }
}