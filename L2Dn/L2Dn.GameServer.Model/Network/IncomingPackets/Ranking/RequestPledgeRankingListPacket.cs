using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Ranking;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Ranking;

public struct RequestPledgeRankingListPacket: IIncomingPacket<GameSession>
{
    private int _category;

    public void ReadContent(PacketBitReader reader)
    {
        _category = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPledgeRankingListPacket(player, _category));

        return ValueTask.CompletedTask;
    }
}