using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExMoveToLocationInShuttlePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _airShipId;
    private readonly Location3D _targetLocation;
    private readonly Location3D _fromLocation;

    public ExMoveToLocationInShuttlePacket(Player player, Location3D from)
    {
        _objectId = player.getObjectId();
        _airShipId = player.getShuttle().getObjectId();
        _targetLocation = player.getInVehiclePosition();
        _fromLocation = from;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MOVE_TO_LOCATION_IN_SUTTLE);
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_airShipId);
        writer.WriteLocation3D(_targetLocation);
        writer.WriteLocation3D(_fromLocation);
    }
}