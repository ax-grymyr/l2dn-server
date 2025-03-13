using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Packets;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExRankingCharRankersPacket: IOutgoingPacket
{
	private readonly Player _player;
	private readonly int _group;
	private readonly int _scope;
	private readonly int _race;
	private readonly int _class;
	private readonly Map<int, StatSet> _playerList;
	private readonly Map<int, StatSet> _snapshotList;

	public ExRankingCharRankersPacket(Player player, int group, int scope, int race, int baseclass)
	{
		_player = player;
		_group = group;
		_scope = scope;
		_race = race;
		_class = baseclass;
		_playerList = RankManager.getInstance().getRankList();
		_snapshotList = RankManager.getInstance().getSnapshotList();
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_RANKING_CHAR_RANKERS);

		writer.WriteByte((byte)_group);
		writer.WriteByte((byte)_scope);
		writer.WriteInt32(_race);
		writer.WriteInt32((int)_player.getClassId());
		if (_playerList.Count != 0)
		{
			switch (_group)
			{
				case 0: // all
				{
					if (_scope == 0) // all
					{
						int count = _playerList.Count > 150 ? 150 : _playerList.Count;
						writer.WriteInt32(count);
						foreach (int id in _playerList.Keys)
						{
							StatSet player = _playerList[id];
							writer.WriteSizedString(player.getString("name"));
							writer.WriteSizedString(player.getString("clanName"));
							writer.WriteInt32(Config.SERVER_ID);
							writer.WriteInt32(player.getInt("level"));
							writer.WriteInt32(player.getInt("classId"));
							writer.WriteInt32(player.getInt("race"));
							writer.WriteInt32(id); // server rank
							if (_snapshotList.Count != 0)
							{
								foreach (int id2 in _snapshotList.Keys)
								{
									StatSet snapshot = _snapshotList[id2];
									if (player.getInt("charId") == snapshot.getInt("charId"))
									{
										writer.WriteInt32(id2); // server rank snapshot
										writer.WriteInt32(snapshot.getInt("raceRank", 0)); // race rank snapshot
										writer.WriteInt32(snapshot.getInt("classRank", 0)); // nClassRank_Snapshot
									}
								}
							}
							else
							{
								writer.WriteInt32(id);
								writer.WriteInt32(0);
								writer.WriteInt32(0);
							}
						}
					}
					else
					{
						bool found = false;
						foreach (int id in _playerList.Keys)
						{
							StatSet player = _playerList[id];
							if (player.getInt("charId") == _player.ObjectId)
							{
								found = true;
								int first = id > 10 ? id - 9 : 1;
								int last = _playerList.Count >= id + 10 ? id + 10 : id + (_playerList.Count - id);
								if (first == 1)
								{
									writer.WriteInt32(last - (first - 1));
								}
								else
								{
									writer.WriteInt32(last - first);
								}
								for (int id2 = first; id2 <= last; id2++)
								{
									StatSet plr = _playerList[id2];
									writer.WriteSizedString(plr.getString("name"));
									writer.WriteSizedString(plr.getString("clanName"));
									writer.WriteInt32(Config.SERVER_ID);
									writer.WriteInt32(plr.getInt("level"));
									writer.WriteInt32(plr.getInt("classId"));
									writer.WriteInt32(plr.getInt("race"));
									writer.WriteInt32(id2); // server rank
									if (_snapshotList.Count != 0)
									{
										foreach (int id3 in _snapshotList.Keys)
										{
											StatSet snapshot = _snapshotList[id3];
											if (player.getInt("charId") == snapshot.getInt("charId"))
											{
												writer.WriteInt32(id3); // server rank snapshot
												writer.WriteInt32(snapshot.getInt("raceRank", 0));
												writer.WriteInt32(snapshot.getInt("classRank", 0)); // nClassRank_Snapshot
											}
										}
									}
								}
							}
						}
						if (!found)
						{
							writer.WriteInt32(0);
						}
					}
					break;
				}
				case 1: // race
				{
					if (_scope == 0) // all
					{
						int count = 0;
						for (int j = 1; j <= _playerList.Count; j++)
						{
							StatSet player = _playerList[j];
							if (_race == player.getInt("race"))
							{
								count++;
							}
						}
						writer.WriteInt32(count > 100 ? 100 : count);
						int i = 1;
						foreach (int id in _playerList.Keys)
						{
							StatSet player = _playerList[id];
							if (_race == player.getInt("race"))
							{
								writer.WriteSizedString(player.getString("name"));
								writer.WriteSizedString(player.getString("clanName"));
								writer.WriteInt32(Config.SERVER_ID);
								writer.WriteInt32(player.getInt("level"));
								writer.WriteInt32(player.getInt("classId"));
								writer.WriteInt32(player.getInt("race"));
								writer.WriteInt32(i); // server rank
								if (_snapshotList.Count != 0)
								{
									Map<int, StatSet> snapshotRaceList = new();
									int j = 1;
									foreach (int id2 in _snapshotList.Keys)
									{
										StatSet snapshot = _snapshotList[id2];
										if (_race == snapshot.getInt("race"))
										{
											snapshotRaceList.put(j, _snapshotList[id2]);
											j++;
										}
									}
									foreach (int id2 in snapshotRaceList.Keys)
									{
										StatSet snapshot = snapshotRaceList[id2];
										if (player.getInt("charId") == snapshot.getInt("charId"))
										{
											writer.WriteInt32(id2); // server rank snapshot
											writer.WriteInt32(snapshot.getInt("raceRank", 0)); // race rank snapshot
											writer.WriteInt32(snapshot.getInt("classRank", 0)); // nClassRank_Snapshot
										}
									}
								}
								else
								{
									writer.WriteInt32(i);
									writer.WriteInt32(i);
									writer.WriteInt32(i); // nClassRank_Snapshot
								}
								i++;
							}
						}
					}
					else
					{
						bool found = false;
						Map<int, StatSet> raceList = new();
						int i = 1;
						foreach (int id in _playerList.Keys)
						{
							StatSet set = _playerList[id];
							if (_player.getRace() == (Race)set.getInt("race"))
							{
								raceList.put(i, _playerList[id]);
								i++;
							}
						}
						foreach (int id in raceList.Keys)
						{
							StatSet player = raceList[id];
							if (player.getInt("charId") == _player.ObjectId)
							{
								found = true;
								int first = id > 10 ? id - 9 : 1;
								int last = raceList.Count >= id + 10 ? id + 10 : id + (raceList.Count - id);
								if (first == 1)
								{
									writer.WriteInt32(last - (first - 1));
								}
								else
								{
									writer.WriteInt32(last - first);
								}
								for (int id2 = first; id2 <= last; id2++)
								{
									StatSet plr = raceList[id2];
									writer.WriteSizedString(plr.getString("name"));
									writer.WriteSizedString(plr.getString("clanName"));
									writer.WriteInt32(Config.SERVER_ID);
									writer.WriteInt32(plr.getInt("level"));
									writer.WriteInt32(plr.getInt("classId"));
									writer.WriteInt32(plr.getInt("race"));
									writer.WriteInt32(id2); // server rank
									writer.WriteInt32(id2);
									writer.WriteInt32(id2);
									writer.WriteInt32(id2); // nClassRank_Snapshot
								}
							}
						}
						if (!found)
						{
							writer.WriteInt32(0);
						}
					}
					break;
				}
				case 2: // clan
				{
					if (_player.getClan() != null)
					{
						Map<int, StatSet> clanList = new();
						int i = 1;
						foreach (int id in _playerList.Keys)
						{
							StatSet set = _playerList[id];
                            Clan? clan = _player.getClan();
							if (clan?.getName() == set.getString("clanName"))
							{
								clanList.put(i, _playerList[id]);
								i++;
							}
						}
						writer.WriteInt32(clanList.Count);
						foreach (int id in clanList.Keys)
						{
							StatSet player = clanList[id];
							writer.WriteSizedString(player.getString("name"));
							writer.WriteSizedString(player.getString("clanName"));
							writer.WriteInt32(Config.SERVER_ID);
							writer.WriteInt32(player.getInt("level"));
							writer.WriteInt32(player.getInt("classId"));
							writer.WriteInt32(player.getInt("race"));
							writer.WriteInt32(id); // clan rank
							if (_snapshotList.Count != 0)
							{
								foreach (int id2 in _snapshotList.Keys)
								{
									StatSet snapshot = _snapshotList[id2];
									if (player.getInt("charId") == snapshot.getInt("charId"))
									{
										writer.WriteInt32(id2); // server rank snapshot
										writer.WriteInt32(snapshot.getInt("raceRank", 0)); // race rank snapshot
										writer.WriteInt32(snapshot.getInt("classRank", 0)); // nClassRank_Snapshot
									}
								}
							}
							else
							{
								writer.WriteInt32(id);
								writer.WriteInt32(0);
								writer.WriteInt32(0);
							}
						}
					}
					else
					{
						writer.WriteInt32(0);
					}
					break;
				}
				case 3: // friend
				{
					if (!_player.getFriendList().isEmpty())
					{
						Set<int> friendList = new();
						int count = 1;
						foreach (int id in _player.getFriendList())
						{
							foreach (int id2 in _playerList.Keys)
							{
								StatSet temp = _playerList[id2];
								if (temp.getInt("charId") == id)
								{
									friendList.add(temp.getInt("charId"));
									count++;
								}
							}
						}
						friendList.add(_player.ObjectId);
						writer.WriteInt32(count);
						foreach (int id in _playerList.Keys)
						{
							StatSet player = _playerList[id];
							if (friendList.Contains(player.getInt("charId")))
							{
								writer.WriteSizedString(player.getString("name"));
								writer.WriteSizedString(player.getString("clanName"));
								writer.WriteInt32(Config.SERVER_ID);
								writer.WriteInt32(player.getInt("level"));
								writer.WriteInt32(player.getInt("classId"));
								writer.WriteInt32(player.getInt("race"));
								writer.WriteInt32(id); // friend rank
								if (_snapshotList.Count != 0)
								{
									foreach (int id2 in _snapshotList.Keys)
									{
										StatSet snapshot = _snapshotList[id2];
										if (player.getInt("charId") == snapshot.getInt("charId"))
										{
											writer.WriteInt32(id2); // server rank snapshot
											writer.WriteInt32(snapshot.getInt("raceRank", 0)); // race rank snapshot
											writer.WriteInt32(snapshot.getInt("classRank", 0)); // nClassRank_Snapshot
										}
									}
								}
								else
								{
									writer.WriteInt32(id);
									writer.WriteInt32(0);
									writer.WriteInt32(0);
								}
							}
						}
					}
					else
					{
						writer.WriteInt32(1);
						writer.WriteSizedString(_player.getName());
						writer.WriteSizedString(_player.getClan()?.getName() ?? string.Empty);
						writer.WriteInt32(Config.SERVER_ID);
						writer.WriteInt32(_player.getStat().getBaseLevel());
						writer.WriteInt32((int)_player.getBaseClass());
						writer.WriteInt32((int)_player.getRace());
						writer.WriteInt32(1); // clan rank
						if (_snapshotList.Count != 0)
						{
							foreach (int id in _snapshotList.Keys)
							{
								StatSet snapshot = _snapshotList[id];
								if (_player.ObjectId == snapshot.getInt("charId"))
								{
									writer.WriteInt32(id); // server rank snapshot
									writer.WriteInt32(snapshot.getInt("raceRank", 0)); // race rank snapshot
									writer.WriteInt32(snapshot.getInt("classRank", 0)); // nClassRank_Snapshot
								}
							}
						}
						else
						{
							writer.WriteInt32(0);
							writer.WriteInt32(0);
							writer.WriteInt32(0);
						}
					}
					break;
				}
				case 4: // class
				{
					if (_scope == 0) // all
					{
						int count = 0;
						for (int j = 1; j <= _playerList.Count; j++)
						{
							StatSet player = _playerList[j];
							if (_class == player.getInt("classId"))
							{
								count++;
							}
						}
						writer.WriteInt32(count > 100 ? 100 : count);
						int i = 1;
						foreach (int id in _playerList.Keys)
						{
							StatSet player = _playerList[id];
							if (_class == player.getInt("classId"))
							{
								writer.WriteSizedString(player.getString("name"));
								writer.WriteSizedString(player.getString("clanName"));
								writer.WriteInt32(Config.SERVER_ID);
								writer.WriteInt32(player.getInt("level"));
								writer.WriteInt32(player.getInt("classId"));
								writer.WriteInt32(player.getInt("race"));
								writer.WriteInt32(i); // server rank
								if (_snapshotList.Count > 0)
								{
									Map<int, StatSet> snapshotClassList = new();
									int j = 1;
									foreach (int id2 in _snapshotList.Keys)
									{
										StatSet snapshot = _snapshotList[id2];
										if (_class == snapshot.getInt("classId"))
										{
											snapshotClassList.put(j, _snapshotList[id2]);
											j++;
										}
									}
									foreach (int id2 in snapshotClassList.Keys)
									{
										StatSet snapshot = snapshotClassList[id2];
										if (player.getInt("charId") == snapshot.getInt("charId"))
										{
											writer.WriteInt32(id2); // server rank snapshot
											writer.WriteInt32(snapshot.getInt("raceRank", 0)); // race rank snapshot
											writer.WriteInt32(snapshot.getInt("classRank", 0)); // nClassRank_Snapshot
										}
									}
								}
								else
								{
									writer.WriteInt32(i);
									writer.WriteInt32(i);
									writer.WriteInt32(i); // nClassRank_Snapshot?
								}
								i++;
							}
						}
					}
					else
					{
						bool found = false;

						Map<int, StatSet> classList = new();
						int i = 1;
						foreach (int id in _playerList.Keys)
						{
							StatSet set = _playerList[id];
							if (_player.getBaseClass() == (CharacterClass)set.getInt("classId"))
							{
								classList.put(i, _playerList[id]);
								i++;
							}
						}

						foreach (int id in classList.Keys)
						{
							StatSet player = classList[id];
							if (player.getInt("charId") == _player.ObjectId)
							{
								found = true;
								int first = id > 10 ? id - 9 : 1;
								int last = classList.Count >= id + 10 ? id + 10 : id + (classList.Count - id);
								if (first == 1)
								{
									writer.WriteInt32(last - (first - 1));
								}
								else
								{
									writer.WriteInt32(last - first);
								}
								for (int id2 = first; id2 <= last; id2++)
								{
									StatSet plr = classList[id2];
									writer.WriteSizedString(plr.getString("name"));
									writer.WriteSizedString(plr.getString("clanName"));
									writer.WriteInt32(Config.SERVER_ID);
									writer.WriteInt32(plr.getInt("level"));
									writer.WriteInt32(plr.getInt("classId"));
									writer.WriteInt32(plr.getInt("race"));
									writer.WriteInt32(id2); // server rank
									writer.WriteInt32(id2);
									writer.WriteInt32(id2);
									writer.WriteInt32(id2); // nClassRank_Snapshot?
								}
							}
						}
						if (!found)
						{
							writer.WriteInt32(0);
						}
					}
					break;
				}
			}
		}
		else
		{
			writer.WriteInt32(0);
		}
	}
}