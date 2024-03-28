using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExMoveToLocationInShuttlePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _airShipId;
    private readonly int _targetX;
    private readonly int _targetY;
    private readonly int _targetZ;
    private readonly int _fromX;
    private readonly int _fromY;
    private readonly int _fromZ;
	
    public ExMoveToLocationInShuttlePacket(Player player, int fromX, int fromY, int fromZ)
    {
        _objectId = player.getObjectId();
        _airShipId = player.getShuttle().getObjectId();
        _targetX = player.getInVehiclePosition().getX();
        _targetY = player.getInVehiclePosition().getY();
        _targetZ = player.getInVehiclePosition().getZ();
        _fromX = fromX;
        _fromY = fromY;
        _fromZ = fromZ;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MOVE_TO_LOCATION_IN_SUTTLE);
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_airShipId);
        writer.WriteInt32(_targetX);
        writer.WriteInt32(_targetY);
        writer.WriteInt32(_targetZ);
        writer.WriteInt32(_fromX);
        writer.WriteInt32(_fromY);
        writer.WriteInt32(_fromZ);
    }
}