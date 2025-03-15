using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExOlympiadRankingInfoPacket: IOutgoingPacket
{
	private readonly Player _player;
	private readonly RankingOlympiadCategory _category;
	private readonly int _rankingType;
	private readonly int _unk;
	private readonly CharacterClass _classId;
	private readonly int _serverId;
	private readonly Map<int, StatSet> _playerList;
	private readonly Map<int, StatSet> _snapshotList;

	public ExOlympiadRankingInfoPacket(Player player, RankingOlympiadCategory category, int rankingType, int unk, CharacterClass classId, int serverId)
	{
		_player = player;
		_category = category;
		_rankingType = rankingType;
		_unk = unk;
		_classId = classId;
		_serverId = serverId;
		_playerList = RankManager.getInstance().getOlyRankList();
		_snapshotList = RankManager.getInstance().getSnapshotOlyList();
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_OLYMPIAD_RANKING_INFO);

		writer.WriteByte((byte)_category); // Tab id
		writer.WriteByte((byte)_rankingType); // ranking type
		writer.WriteByte((byte)_unk); // unk, shows 1 all time
		writer.WriteInt32((int)_classId); // class id (default 148) or caller class id for personal rank
		writer.WriteInt32(_serverId); // 0 - all servers, server id - for caller server
		writer.WriteInt32(933); // unk, 933 all time
		if (_playerList.Count != 0)
		{
			writeFilteredRankingData(writer, _category, _category.getScopeByGroup(_rankingType), _classId);
		}
	}

	private void writeFilteredRankingData(PacketBitWriter writer, RankingOlympiadCategory category,
		RankingOlympiadScope scope, CharacterClass classId)
	{
		switch (category)
		{
			case RankingOlympiadCategory.SERVER:
			{
				writeScopeData(writer, scope, _playerList.OrderBy(r => r.Key), _snapshotList.OrderBy(r => r.Key));
				break;
			}
			case RankingOlympiadCategory.CLASS:
			{
				writeScopeData(writer, scope,
					_playerList.Where(it => (CharacterClass)it.Value.getInt("classId") == classId).OrderBy(r => r.Key),
					_snapshotList.Where(it => (CharacterClass)it.Value.getInt("classId") == classId)
						.OrderBy(r => r.Key));
				break;
			}
		}
	}

	private void writeScopeData(PacketBitWriter writer, RankingOlympiadScope scope, IEnumerable<System.Collections.Generic.KeyValuePair<int, StatSet>> list, IEnumerable<System.Collections.Generic.KeyValuePair<int, StatSet>> snapshot)
	{
		List<KeyValuePair<int, StatSet>> limited;
		switch (scope)
		{
			case RankingOlympiadScope.TOP_100:
			{
				limited = list.Take(100).ToList();
				break;
			}
			case RankingOlympiadScope.ALL:
			{
				limited = list.ToList();
				break;
			}
			case RankingOlympiadScope.TOP_50:
			{
				limited = list.Take(50).ToList();
				break;
			}
			case RankingOlympiadScope.SELF:
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
				limited = new List<KeyValuePair<int, StatSet>>();
				break;
			}
		}

		writer.WriteInt32(limited.Count);
		int rank = 1;
		foreach (var data in limited)
		{
			int curRank = rank++;
			StatSet player = data.Value;
			writer.WriteSizedString(player.getString("name")); // name
			writer.WriteSizedString(player.getString("clanName")); // clan name
			writer.WriteInt32(scope == RankingOlympiadScope.SELF ? data.Key : curRank); // rank
			if (snapshot.Any())
			{
				int snapshotRank = 1;
				foreach (var ssData in snapshot)
				{
					StatSet snapshotData = ssData.Value;
					if (player.getInt("charId") == snapshotData.getInt("charId"))
					{
						writer.WriteInt32(scope == RankingOlympiadScope.SELF ? ssData.Key : snapshotRank++); // previous rank
					}
				}
			}
			else
			{
				writer.WriteInt32(scope == RankingOlympiadScope.SELF ? data.Key : curRank);
			}

			writer.WriteInt32(ServerConfig.Instance.GameServerParams.ServerId); // server id
			writer.WriteInt32(player.getInt("level")); // level
			writer.WriteInt32(player.getInt("classId")); // class id
			writer.WriteInt32(player.getInt("clanLevel")); // clan level
			writer.WriteInt32(player.getInt("competitions_won")); // win count
			writer.WriteInt32(player.getInt("competitions_lost")); // lose count
			writer.WriteInt32(player.getInt("olympiad_points")); // points
			writer.WriteInt32(player.getInt("legend_count")); // legend count
			writer.WriteInt32(player.getInt("count")); // hero count
		}
	}
}