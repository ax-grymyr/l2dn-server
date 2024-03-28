using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExRankingCharHistoryPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ICollection<RankingHistoryDataHolder> _history;
	
    public ExRankingCharHistoryPacket(Player player)
    {
        _player = player;
        _history = _player.getRankingHistoryData();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RANKING_CHAR_HISTORY);
        
        writer.WriteInt32(_history.Count);
        foreach (RankingHistoryDataHolder rankingData in _history)
        {
            writer.WriteInt32(rankingData.getDay().DayNumber); // TODO: probably wrong date number
            writer.WriteInt32(rankingData.getRank());
            writer.WriteInt64(rankingData.getExp());
        }
    }
}