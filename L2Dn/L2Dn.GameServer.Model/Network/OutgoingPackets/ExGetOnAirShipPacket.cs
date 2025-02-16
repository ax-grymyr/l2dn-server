using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExGetOnAirShipPacket: IOutgoingPacket
{
    private readonly int _playerId;
    private readonly int _airShipId;
    private readonly Location3D _location;

    public ExGetOnAirShipPacket(Player player, Creature ship)
    {
        _playerId = player.ObjectId;
        _airShipId = ship.ObjectId;
        _location = player.getInVehiclePosition();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_ON_AIR_SHIP);
        writer.WriteInt32(_playerId);
        writer.WriteInt32(_airShipId);
        writer.WriteLocation3D(_location);
    }
}