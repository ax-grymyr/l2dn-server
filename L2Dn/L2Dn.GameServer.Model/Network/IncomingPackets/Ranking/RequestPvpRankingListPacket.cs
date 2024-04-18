using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Ranking;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Ranking;

public struct RequestPvpRankingListPacket: IIncomingPacket<GameSession>
{
    private int _season;
    private RankingCategory _tabId;
    private int _type;
    private Race _race;

    public void ReadContent(PacketBitReader reader)
    {
        _season = reader.ReadByte(); // CurrentSeason
        _tabId = (RankingCategory)reader.ReadByte(); // RankingGroup
        _type = reader.ReadByte(); // RankingScope
        _race = (Race)reader.ReadInt32(); // Race
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPvpRankingListPacket(player, _season, _tabId, _type, _race, player.getBaseClass()));

        return ValueTask.CompletedTask;
    }
}