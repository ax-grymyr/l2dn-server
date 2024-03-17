using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Ranking;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Ranking;

public struct RequestRankingCharRankersPacket: IIncomingPacket<GameSession>
{
    private int _group;
    private int _scope;
    private int _ordinal;
    private int _baseclass;

    public void ReadContent(PacketBitReader reader)
    {
        _group = reader.ReadByte(); // Tab Id
        _scope = reader.ReadByte(); // All or personal
        _ordinal = reader.ReadInt32();
        _baseclass = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        connection.Send(new ExRankingCharRankersPacket(player, _group, _scope, _ordinal, _baseclass));
        return ValueTask.CompletedTask;
    }
}