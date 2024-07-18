using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExPledgeRankingListPacket: IOutgoingPacket
{
	private readonly Player _player;
	private readonly int _category;
	private readonly Map<int, StatSet> _rankingClanList;
	private readonly Map<int, StatSet> _snapshotClanList;
	
	public ExPledgeRankingListPacket(Player player, int category)
	{
		_player = player;
		_category = category;
		_rankingClanList = RankManager.getInstance().getClanRankList();
		_snapshotClanList = RankManager.getInstance().getSnapshotClanRankList();
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_RANKING_LIST);
		
		writer.WriteByte((byte)_category);
		if (_rankingClanList.Count != 0)
		{
			writeScopeData(writer, _category == 0, _rankingClanList, _snapshotClanList);
		}
		else
		{
			writer.WriteInt32(0);
		}
	}
	
	private void writeScopeData(PacketBitWriter writer, bool isTop150, Map<int, StatSet> list, Map<int, StatSet> snapshot)
	{
		int? clanId = _player.getClanId();
		var playerData = list.FirstOrDefault(it => it.Value.getInt("clan_id", 0) == clanId);
		int indexOf = list.OrderBy(r => r.Key).TakeWhile(it => it.Value.getInt("clan_id", 0) == clanId).Count();
		List<KeyValuePair<int, StatSet>> limited = isTop150 ? list.OrderBy(r => r.Key).Take(150).ToList() :
			playerData.Value == null ? new List<KeyValuePair<int, StatSet>>() :
			list.Skip(Math.Max(0, indexOf - 10)).Take(20).ToList();
		
		writer.WriteInt32(limited.Count);
		int rank = 1;
		foreach (var data in limited)
		{
			int curRank = rank++;
			StatSet player = data.Value;
			writer.WriteInt32(!isTop150 ? data.Key : curRank);
			foreach (var ssData in snapshot.OrderBy(r => r.Key))
			{
				StatSet snapshotData = ssData.Value;
				if (player.getInt("clan_id") == snapshotData.getInt("clan_id"))
				{
					writer.WriteInt32(!isTop150 ? ssData.Key : curRank); // server rank snapshot
				}
			}
			
			writer.WriteSizedString(player.getString("clan_name"));
			writer.WriteInt32(player.getInt("clan_level"));
			writer.WriteSizedString(player.getString("char_name"));
			writer.WriteInt32(player.getInt("level"));
			writer.WriteInt32(ClanTable.getInstance().getClan(player.getInt("clan_id")) != null ? ClanTable.getInstance().getClan(player.getInt("clan_id")).getMembersCount() : 0);
			writer.WriteInt32((int)Math.Min(int.MaxValue, player.getLong("exp")));
		}
	}
}