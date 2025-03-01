using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Shuttles;

public struct RequestShuttleGetOffPacket: IIncomingPacket<GameSession>
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

        Shuttle? shuttle = player.getShuttle();
        if (shuttle != null)
        {
            shuttle.removePassenger(player, _x, _y, _z);
        }

        return ValueTask.CompletedTask;
    }
}