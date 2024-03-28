using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExValidateLocationInShuttlePacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _shipId;
    private readonly int _heading;
    private readonly Location _loc;
	
    public ExValidateLocationInShuttlePacket(Player player)
    {
        _player = player;
        _shipId = _player.getShuttle().getObjectId();
        _loc = player.getInVehiclePosition();
        _heading = player.getHeading();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VALIDATE_LOCATION_IN_SHUTTLE);
        writer.WriteInt32(_player.getObjectId());
        writer.WriteInt32(_shipId);
        writer.WriteInt32(_loc.getX());
        writer.WriteInt32(_loc.getY());
        writer.WriteInt32(_loc.getZ());
        writer.WriteInt32(_heading);
    }
}