using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExGetOffAirShipPacket: IOutgoingPacket
{
    private readonly int _playerId;
    private readonly int _airShipId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;

    public ExGetOffAirShipPacket(Creature creature, Creature ship, int x, int y, int z)
    {
        _playerId = creature.getObjectId();
        _airShipId = ship.getObjectId();
        _x = x;
        _y = y;
        _z = z;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_OFF_AIR_SHIP);
        writer.WriteInt32(_playerId);
        writer.WriteInt32(_airShipId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
    }
}