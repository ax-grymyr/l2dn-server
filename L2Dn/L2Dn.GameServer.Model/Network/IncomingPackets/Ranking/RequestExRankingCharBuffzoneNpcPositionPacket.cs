using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Ranking;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Ranking;

public struct RequestExRankingCharBuffzoneNpcPositionPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        int ranker = RankManager.getInstance().getPlayerGlobalRank(player);
        if (ranker == 1)
        {
            player.sendPacket(new ExRankingBuffZoneNpcInfoPacket());
        }
 
        player.sendPacket(new ExRankingBuffZoneNpcPositionPacket());

        return ValueTask.CompletedTask;
    }
}