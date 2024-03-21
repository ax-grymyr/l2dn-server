using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Shuttle;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Shuttles;

public struct CannotMoveAnymoreInShuttlePacket: IIncomingPacket<GameSession>
{
    private int _x;
    private int _y;
    private int _z;
    private int _heading;
    private int _boatId;

    public void ReadContent(PacketBitReader reader)
    {
        _boatId = reader.ReadInt32();
        _x = reader.ReadInt32();
        _y = reader.ReadInt32();
        _z = reader.ReadInt32();
        _heading = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.isInShuttle() && (player.getShuttle().getObjectId() == _boatId))
        {
            player.setInVehiclePosition(new Location(_x, _y, _z));
            player.setHeading(_heading);
            player.broadcastPacket(new ExStopMoveInShuttlePacket(player, _boatId));
        }
        
        return ValueTask.CompletedTask;
    }
}