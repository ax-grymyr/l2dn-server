using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExStopMoveInShuttlePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatId;
    private readonly Location _location;

    public ExStopMoveInShuttlePacket(Player player, int boatId)
    {
        _objectId = player.ObjectId;
        _boatId = boatId;
        _location = new Location(player.getInVehiclePosition(), player.getHeading());
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STOP_MOVE_IN_SHUTTLE);
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatId);
        writer.WriteLocation(_location);
    }
}