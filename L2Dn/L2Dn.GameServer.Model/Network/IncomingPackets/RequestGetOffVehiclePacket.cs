using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestGetOffVehiclePacket: IIncomingPacket<GameSession>
{
    private int _boatId;
    private Location3D _location;

    public void ReadContent(PacketBitReader reader)
    {
        _boatId = reader.ReadInt32();
        _location = reader.ReadLocation3D();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Boat? boat = player.getBoat();
        if (!player.isInBoat() || boat == null || boat.ObjectId != _boatId || boat.isMoving() ||
            !player.IsInsideRadius3D(_location, 1000))
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.broadcastPacket(new StopMoveInVehiclePacket(player, _boatId));
        player.setVehicle(null);
        player.sendPacket(ActionFailedPacket.STATIC_PACKET);
        player.broadcastPacket(new GetOffVehiclePacket(player.ObjectId, _boatId, _location));
        player.setXYZ(_location);
        player.setInsideZone(ZoneId.PEACE, false);
        player.revalidateZone(true);
        return ValueTask.CompletedTask;
    }
}