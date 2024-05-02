using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExValidateLocationInShuttlePacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _shipId;
    private readonly LocationHeading _location;

    public ExValidateLocationInShuttlePacket(Player player)
    {
        _player = player;
        _shipId = _player.getShuttle().getObjectId();
        _location = new LocationHeading(player.getInVehiclePosition(), player.getHeading());
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VALIDATE_LOCATION_IN_SHUTTLE);
        writer.WriteInt32(_player.getObjectId());
        writer.WriteInt32(_shipId);
        writer.WriteLocationWithHeading(_location);
    }
}