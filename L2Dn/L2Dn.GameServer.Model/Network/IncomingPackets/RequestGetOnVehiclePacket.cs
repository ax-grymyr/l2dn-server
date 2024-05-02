using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestGetOnVehiclePacket: IIncomingPacket<GameSession>
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

        Boat boat;
        if (player.isInBoat())
        {
            boat = player.getBoat();
            if (boat.getObjectId() != _boatId)
            {
                player.sendPacket(ActionFailedPacket.STATIC_PACKET);
                return ValueTask.CompletedTask;
            }
        }
        else
        {
            boat = BoatManager.getInstance().getBoat(_boatId);
            if ((boat == null) || boat.isMoving() || !player.isInsideRadius3D(boat.getLocation().Location3D, 1000))
            {
                player.sendPacket(ActionFailedPacket.STATIC_PACKET);
                return ValueTask.CompletedTask;
            }
        }

        player.setInVehiclePosition(_location);
        player.setVehicle(boat);
        player.broadcastPacket(new GetOnVehiclePacket(player.getObjectId(), boat.getObjectId(), _location));
        player.setXYZ(boat.getX(), boat.getY(), boat.getZ());
        player.setInsideZone(ZoneId.PEACE, true);
        player.revalidateZone(true);
        return ValueTask.CompletedTask;
    }
}