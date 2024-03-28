using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExGetOnAirShipPacket: IOutgoingPacket
{
    private readonly int _playerId;
    private readonly int _airShipId;
    private readonly Location _pos;
	
    public ExGetOnAirShipPacket(Player player, Creature ship)
    {
        _playerId = player.getObjectId();
        _airShipId = ship.getObjectId();
        _pos = player.getInVehiclePosition();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_ON_AIR_SHIP);
        writer.WriteInt32(_playerId);
        writer.WriteInt32(_airShipId);
        writer.WriteInt32(_pos.getX());
        writer.WriteInt32(_pos.getY());
        writer.WriteInt32(_pos.getZ());
    }
}