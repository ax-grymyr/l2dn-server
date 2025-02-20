using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExPvpRankingListPacket: IOutgoingPacket
{
	private readonly Player _player;
	private readonly int _season;
	private readonly RankingCategory _tabId;
	private readonly int _type;
	private readonly Race _race;
	private readonly CharacterClass _class;
	private readonly Map<int, StatSet> _playerList;
	private readonly Map<int, StatSet> _snapshotList;
	
	public ExPvpRankingListPacket(Player player, int season, RankingCategory tabId, int type, Race race, CharacterClass baseclass)
	{
		_player = player;
		_season = season;
		_tabId = tabId;
		_type = type;
		_race = race;
		_class = baseclass;
		_playerList = RankManager.getInstance().getPvpRankList();
		_snapshotList = RankManager.getInstance().getSnapshotPvpRankList();
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_PVP_RANKING_LIST);
		
		writer.WriteByte((byte)_season);
		writer.WriteByte((byte)_tabId);
		writer.WriteByte((byte)_type);
		writer.WriteInt32((int)_race);
		if (_playerList.Count != 0 && _type != 255 && _race != (Race)255)
		{
			writeFilteredRankingData(writer, _tabId, _tabId.getScopeByGroup(_type), _race, _class);
		}
		else
		{
			writer.WriteInt32(0);
		}
	}
	
	private void writeFilteredRankingData(PacketBitWriter writer, RankingCategory category, RankingScope scope, Race race, CharacterClass baseclass)
	{
		switch (category)
		{
			case RankingCategory.SERVER:
			{
				writeScopeData(writer, scope, _playerList.OrderBy(r => r.Key), _snapshotList.OrderBy(r => r.Key));
				break;
			}
			case RankingCategory.RACE:
			{
				writeScopeData(writer, scope,
					_playerList.Where(it => (Race)it.Value.getInt("race") == race).OrderBy(r => r.Key),
					_snapshotList.Where(it => (Race)it.Value.getInt("race") == race).OrderBy(r => r.Key));
				break;
			}
			case RankingCategory.CLAN:
			{
				var clan = _player.getClan();
				writeScopeData(writer, scope,
					clan == null
						? Enumerable.Empty<System.Collections.Generic.KeyValuePair<int, StatSet>>()
						: _playerList.Where(it => it.Value.getString("clanName") == clan.getName()).OrderBy(r => r.Key),
					clan == null
						? Enumerable.Empty<System.Collections.Generic.KeyValuePair<int, StatSet>>()
						: _snapshotList.Where(it => it.Value.getString("clanName") == clan.getName()).OrderBy(r => r.Key));
				break;
			}
			case RankingCategory.FRIEND:
			{
				var friendList = _player.getFriendList();
				writeScopeData(writer, scope,
					_playerList.Where(it => friendList.Contains(it.Value.getInt("charId"))).OrderBy(r => r.Key),
					_snapshotList.Where(it => friendList.Contains(it.Value.getInt("charId"))).OrderBy(r => r.Key));
				break;
			}
			case RankingCategory.CLASS:
			{
				writeScopeData(writer, scope,
					_playerList.Where(it => (CharacterClass)it.Value.getInt("classId") == baseclass)
						.OrderBy(r => r.Key),
					_snapshotList.Where(it => (CharacterClass)it.Value.getInt("classId") == baseclass)
						.OrderBy(r => r.Key));
				break;
			}
		}
	}
	
	private void writeScopeData(PacketBitWriter writer, RankingScope scope, IEnumerable<System.Collections.Generic.KeyValuePair<int, StatSet>> list, 
		IEnumerable<System.Collections.Generic.KeyValuePair<int, StatSet>> snapshot)
	{
		List<System.Collections.Generic.KeyValuePair<int, StatSet>> limited;
		switch (scope)
		{
			case RankingScope.TOP_100:
			{
				limited = list.Take(100).ToList();
				break;
			}
			case RankingScope.ALL:
			{
				limited = list.ToList();
				break;
			}
			case RankingScope.TOP_150:
			{
				limited = list.Take(150).ToList();
				break;
			}
			case RankingScope.SELF:
			{
				int playerId = _player.ObjectId;
				var playerData = list.FirstOrDefault(it => it.Value.getInt("charId", 0) == playerId);
				int indexOf = list.TakeWhile(it => it.Value.getInt("charId", 0) != playerId).Count();
				limited = (playerData.Key == 0
					? Enumerable.Empty<System.Collections.Generic.KeyValuePair<int, StatSet>>()
					: list.Skip(Math.Max(0, indexOf - 10)).Take(20)).ToList();
				
				break;
			}
			default:
			{
				limited = new List<System.Collections.Generic.KeyValuePair<int, StatSet>>();
				break;
			}
		}
		
		writer.WriteInt32(limited.Count);
		int rank = 1;
		foreach (var data in limited)
		{
			int curRank = rank++;
			StatSet player = data.Value;
			writer.WriteSizedString(player.getString("name"));
			writer.WriteSizedString(player.getString("clanName"));
			writer.WriteInt32(player.getInt("level"));
			writer.WriteInt32(player.getInt("race"));
			writer.WriteInt32(player.getInt("classId"));
			writer.WriteInt64(player.getInt("points")); // server rank
			if (snapshot.Any())
			{
				foreach (var ssData in snapshot)
				{
					StatSet snapshotData = ssData.Value;
					if (player.getInt("charId") == snapshotData.getInt("charId"))
					{
						writer.WriteInt32(scope == RankingScope.SELF ? ssData.Key : curRank); // server rank snapshot
						writer.WriteInt32(snapshotData.getInt("raceRank", 0)); // race rank snapshot
						writer.WriteInt32(player.getInt("kills"));
						writer.WriteInt32(player.getInt("deaths"));
					}
				}
			}
			else
			{
				writer.WriteInt32(scope == RankingScope.SELF ? data.Key : curRank);
				writer.WriteInt32(0);
				writer.WriteInt32(0);
				writer.WriteInt32(0);
			}
		}
	}
}