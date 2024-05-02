using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Shuttles;

public struct RequestShuttleGetOnPacket: IIncomingPacket<GameSession>
{
    private Location3D _location;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadInt32(); // charId
        _location = reader.ReadLocation3D();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: better way?
        foreach (Shuttle shuttle in World.getInstance().getVisibleObjects<Shuttle>(player))
        {
            if (shuttle.calculateDistance3D(player.getLocation().ToLocation3D()) < 1000)
            {
                shuttle.addPassenger(player);
                player.setInVehiclePosition(_location);
                break;
            }

            PacketLogger.Instance.Info(GetType().Name + ": range between char and shuttle: " +
                                       shuttle.calculateDistance3D(player.getLocation().ToLocation3D()));
        }
        
        return ValueTask.CompletedTask;
    }
}