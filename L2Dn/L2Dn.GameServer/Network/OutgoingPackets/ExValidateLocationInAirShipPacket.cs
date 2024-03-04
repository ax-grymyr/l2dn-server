using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExValidateLocationInAirShipPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _shipId;
    private readonly int _heading;
    private readonly Location _loc;
	
    public ExValidateLocationInAirShipPacket(Player player)
    {
        _player = player;
        _shipId = _player.getAirShip().getObjectId();
        _loc = player.getInVehiclePosition();
        _heading = player.getHeading();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VALIDATE_LOCATION_IN_AIR_SHIP);
        
        writer.WriteInt32(_player.getObjectId());
        writer.WriteInt32(_shipId);
        writer.WriteInt32(_loc.getX());
        writer.WriteInt32(_loc.getY());
        writer.WriteInt32(_loc.getZ());
        writer.WriteInt32(_heading);
    }
}