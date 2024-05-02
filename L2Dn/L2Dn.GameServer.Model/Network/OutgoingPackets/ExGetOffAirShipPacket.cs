using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExGetOffAirShipPacket: IOutgoingPacket
{
    private readonly int _playerId;
    private readonly int _airShipId;
    private readonly Location3D _location;

    public ExGetOffAirShipPacket(Creature creature, Creature ship, Location3D location)
    {
        _playerId = creature.getObjectId();
        _airShipId = ship.getObjectId();
        _location = location;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_OFF_AIR_SHIP);
        writer.WriteInt32(_playerId);
        writer.WriteInt32(_airShipId);
        writer.WriteLocation3D(_location);
    }
}