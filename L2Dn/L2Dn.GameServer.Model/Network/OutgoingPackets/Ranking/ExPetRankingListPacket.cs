using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExPetRankingListPacket: IOutgoingPacket
{
	private readonly Player _player;
	private readonly int _season;
	private readonly RankingCategory _tabId;
	private readonly int _type;
	private readonly int _petItemObjectId;
	private readonly Map<int, StatSet> _playerList;
	private readonly Map<int, StatSet> _snapshotList;
	
	public ExPetRankingListPacket(Player player, int season, RankingCategory tabId, int type, int petItemObjectId)
	{
		_player = player;
		_season = season;
		_tabId = tabId;
		_type = type;
		_petItemObjectId = petItemObjectId;
		_playerList = RankManager.getInstance().getPetRankList();
		_snapshotList = RankManager.getInstance().getSnapshotPetRankList();
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_PET_RANKING_LIST);
		
		writer.WriteByte((byte)_season);
		writer.WriteByte((byte)_tabId);
		writer.WriteInt16((short)_type);
		writer.WriteInt32(_petItemObjectId);
		if (_playerList.Count != 0)
		{
			writeFilteredRankingData(writer, _tabId, _tabId.getScopeByGroup(_season));
		}
		else
		{
			writer.WriteInt32(0);
		}
	}
	
	private void writeFilteredRankingData(PacketBitWriter writer, RankingCategory category, RankingScope scope)
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
				int type = _type;
				writeScopeData(writer, scope, _playerList.Where(it => it.Value.getInt("petType") == type).OrderBy(r => r.Key),
					_snapshotList.Where(it => it.Value.getInt("petType") == type).OrderBy(r => r.Key));
				break;
			}
			case RankingCategory.CLAN:
			{
				Clan clan = _player.getClan();
				writeScopeData(writer, scope,
					clan == null
						? Enumerable.Empty<System.Collections.Generic.KeyValuePair<int, StatSet>>()
						: _playerList
							.Where(it => it.Value.getString("clanName").equals(clan.getName())).OrderBy(r => r.Key),
					clan == null
						? Enumerable.Empty<System.Collections.Generic.KeyValuePair<int, StatSet>>()
						: _snapshotList.Where(it => it.Value.getString("clanName").equals(clan.getName())).OrderBy(r => r.Key));
				break;
			}
			case RankingCategory.FRIEND:
			{
				Set<int> friendList = _player.getFriendList();
				writeScopeData(writer, scope, _playerList.Where(it => friendList.Contains(it.Value.getInt("charId"))).OrderBy(r => r.Key),
					_snapshotList.Where(it => friendList.Contains(it.Value.getInt("charId"))).OrderBy(r => r.Key));
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
				limited = new List<KeyValuePair<int, StatSet>>();
				break;
			}
		}
		
		writer.WriteInt32(limited.Count);
		int rank = 1;
		foreach (var data in limited)
		{
			int curRank = rank++;
			StatSet pet = data.Value;
			writer.WriteSizedString(pet.getString("name"));
			writer.WriteSizedString(pet.getString("owner_name"));
			writer.WriteSizedString(pet.getString("clanName"));
			writer.WriteInt32(1000000 + pet.getInt("npcId"));
			writer.WriteInt16((short)pet.getInt("petType"));
			writer.WriteInt16((short)pet.getInt("level"));
			writer.WriteInt16((short)pet.getInt("owner_race"));
			writer.WriteInt16((short)pet.getInt("owner_level"));
			writer.WriteInt32(scope == RankingScope.SELF ? data.Key : curRank); // server rank
			if (snapshot.Any())
			{
				foreach (var ssData in snapshot)
				{
					StatSet snapshotData = ssData.Value;
					if (pet.getInt("controlledItemObjId") == snapshotData.getInt("controlledItemObjId"))
					{
						writer.WriteInt32(scope == RankingScope.SELF ? ssData.Key : curRank); // server rank snapshot
					}
				}
			}
			else
			{
				writer.WriteInt32(scope == RankingScope.SELF ? data.Key : curRank); // server rank
			}
		}
	}
}