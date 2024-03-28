using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct BrEventRankerListPacket: IIncomingPacket<GameSession>
{
    private int _eventId;
    private int _day;
    private int _ranking;

    public void ReadContent(PacketBitReader reader)
    {
        _eventId = reader.ReadInt32();
        _day = reader.ReadInt32(); // 0 - current, 1 - previous
        _ranking = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO count, bestScore, myScore
        int count = 0;
        int bestScore = 0;
        int myScore = 0;
        connection.Send(new ExBrLoadEventTopRankersPacket(_eventId, _day, count, bestScore, myScore));
        
        return ValueTask.CompletedTask;
    }
}