using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExStopMoveInAirShipPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _shipObjId;
    private readonly int _h;
    private readonly Location _loc;
	
    public ExStopMoveInAirShipPacket(Player player, int shipObjId)
    {
        _player = player;
        _shipObjId = shipObjId;
        _h = player.getHeading();
        _loc = player.getInVehiclePosition();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STOP_MOVE_IN_AIR_SHIP);
        
        writer.WriteInt32(_player.getObjectId());
        writer.WriteInt32(_shipObjId);
        writer.WriteInt32(_loc.getX());
        writer.WriteInt32(_loc.getY());
        writer.WriteInt32(_loc.getZ());
        writer.WriteInt32(_h);
    }
}