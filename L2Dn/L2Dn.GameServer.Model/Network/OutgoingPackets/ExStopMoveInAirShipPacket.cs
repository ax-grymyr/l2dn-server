using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExStopMoveInAirShipPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _shipObjId;
    private readonly int _h;
    private readonly Location _location;

    public ExStopMoveInAirShipPacket(Player player, int shipObjId)
    {
        _player = player;
        _shipObjId = shipObjId;
        _location = new Location(player.getInVehiclePosition(), player.getHeading());
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STOP_MOVE_IN_AIR_SHIP);

        writer.WriteInt32(_player.ObjectId);
        writer.WriteInt32(_shipObjId);
        writer.WriteLocation(_location);
    }
}