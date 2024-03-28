using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Shuttles;

public struct RequestShuttleGetOnPacket: IIncomingPacket<GameSession>
{
    private int _x;
    private int _y;
    private int _z;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadInt32(); // charId
        _x = reader.ReadInt32();
        _y = reader.ReadInt32();
        _z = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: better way?
        foreach (Shuttle shuttle in World.getInstance().getVisibleObjects<Shuttle>(player))
        {
            if (shuttle.calculateDistance3D(player) < 1000)
            {
                shuttle.addPassenger(player);
                player.getInVehiclePosition().setXYZ(_x, _y, _z);
                break;
            }

            PacketLogger.Instance.Info(GetType().Name + ": range between char and shuttle: " +
                                       shuttle.calculateDistance3D(player));
        }
        
        return ValueTask.CompletedTask;
    }
}