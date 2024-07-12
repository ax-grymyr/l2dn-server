using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers.Games;

public class MonsterRace
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(MonsterRace));
	
	public enum RaceState
	{
		ACCEPTING_BETS,
		WAITING,
		STARTING_RACE,
		RACE_END
	}
	
	protected static readonly PlaySoundPacket SOUND_1 = new PlaySoundPacket(1, "S_Race", 0, 0, 0, 0, 0);
	protected static readonly PlaySoundPacket SOUND_2 = new PlaySoundPacket("ItemSound2.race_start");

	protected static readonly int[][] CODES = [[-1, 0], [0, 15322], [13765, -1]];
	
	protected readonly int[] _npcTemplates; // List holding npc templates, shuffled on a new race.
	protected readonly List<HistoryInfo> _history = new(); // List holding old race records.
	protected readonly Map<int, long> _betsPerLane = new(); // Map holding all bets for each lane ; values setted to 0 after every race.
	protected readonly List<double> _odds = new(); // List holding sorted odds per lane ; cleared at new odds calculation.
	
	protected int _raceNumber = 1;
	protected int _finalCountdown = 0;
	protected RaceState _state = RaceState.RACE_END;
	
	protected MonsterRaceInfoPacket _packet;
	
	private readonly Npc[] _monsters = new Npc[8];
	private int[][] _speeds;
	private readonly int[] _first = new int[2];
	private readonly int[] _second = new int[2];
	
	protected MonsterRace()
	{
		if (!Config.ALLOW_RACE)
		{
			return;
		}

		_speeds = new int[8][];
		for (int i = 0; i < _speeds.Length; i++)
			_speeds[i] = new int[20];
		
		// Feed _history with previous race results.
		loadHistory();
		
		// Feed _betsPerLane with stored information on bets.
		loadBets();
		
		// Feed _npcTemplates, we will only have to shuffle it when needed.
		const int startNpcId = 31003;
		const int endNpcId = 31027;
		_npcTemplates = new int[endNpcId - startNpcId];
		for (int i = startNpcId; i < endNpcId; i++)
			_npcTemplates[i - startNpcId] = i;
		
		ThreadPool.scheduleAtFixedRate(new Announcement(this), 0, 1000);
	}
	
	public class HistoryInfo
	{
		private readonly int _raceId;
		private int _first;
		private int _second;
		private double _oddRate;
		
		public HistoryInfo(int raceId, int first, int second, double oddRate)
		{
			_raceId = raceId;
			_first = first;
			_second = second;
			_oddRate = oddRate;
		}
		
		public int getRaceId()
		{
			return _raceId;
		}
		
		public int getFirst()
		{
			return _first;
		}
		
		public int getSecond()
		{
			return _second;
		}
		
		public double getOddRate()
		{
			return _oddRate;
		}
		
		public void setFirst(int first)
		{
			_first = first;
		}
		
		public void setSecond(int second)
		{
			_second = second;
		}
		
		public void setOddRate(double oddRate)
		{
			_oddRate = oddRate;
		}
	}
	
	private class Announcement: Runnable
	{
		private readonly MonsterRace _race;

		public Announcement(MonsterRace race)
		{
			_race = race;
		}
		
		public void run()
		{
			if (_race._finalCountdown > 1200)
			{
				_race._finalCountdown = 0;
			}
			
			switch (_race._finalCountdown)
			{
				case 0:
				{
					_race.newRace();
					_race.newSpeeds();
					
					_race._state = RaceState.ACCEPTING_BETS;
					_race._packet = new MonsterRaceInfoPacket(CODES[0][0], CODES[0][1], _race.getMonsters(), _race.getSpeeds());
					
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.TICKETS_ARE_NOW_AVAILABLE_FOR_MONSTER_RACE_S1);
					msg.Params.addInt(_race._raceNumber);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(_race._packet, msg);
					break;
				}
				case 30: // 30 sec
				case 60: // 1 min
				case 90: // 1 min 30 sec
				case 120: // 2 min
				case 150: // 2 min 30
				case 180: // 3 min
				case 210: // 3 min 30
				case 240: // 4 min
				case 270: // 4 min 30 sec
				case 330: // 5 min 30 sec
				case 360: // 6 min
				case 390: // 6 min 30 sec
				case 420: // 7 min
				case 450: // 7 min 30
				case 480: // 8 min
				case 510: // 8 min 30
				case 540: // 9 min
				case 570: // 9 min 30 sec
				case 630: // 10 min 30 sec
				case 660: // 11 min
				case 690: // 11 min 30 sec
				case 720: // 12 min
				case 750: // 12 min 30
				case 780: // 13 min
				case 810: // 13 min 30
				case 870: // 14 min 30 sec
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.TICKETS_ARE_NOW_AVAILABLE_FOR_MONSTER_RACE_S1);
					msg.Params.addInt(_race._raceNumber);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg);
					break;
				}
				case 300: // 5 min
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.NOW_SELLING_TICKETS_FOR_MONSTER_RACE_S1);
					msg.Params.addInt(_race._raceNumber);
					SystemMessagePacket msg2 = new SystemMessagePacket(SystemMessageId.TICKET_SALES_FOR_MONSTER_RACE_S1_ARE_CLOSED);
					msg2.Params.addInt(10);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg, msg2);
					break;
				}
				case 600: // 10 min
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.NOW_SELLING_TICKETS_FOR_MONSTER_RACE_S1);
					msg.Params.addInt(_race._raceNumber);
					SystemMessagePacket msg2 = new SystemMessagePacket(SystemMessageId.TICKET_SALES_FOR_MONSTER_RACE_S1_ARE_CLOSED);
					msg2.Params.addInt(5);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg, msg2);
					break;
				}
				case 840: // 14 min
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.NOW_SELLING_TICKETS_FOR_MONSTER_RACE_S1);
					msg.Params.addInt(_race._raceNumber);
					SystemMessagePacket msg2 = new SystemMessagePacket(SystemMessageId.TICKET_SALES_FOR_MONSTER_RACE_S1_ARE_CLOSED);
					msg2.Params.addInt(1);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg, msg2);
					break;
				}
				case 900: // 15 min
				{
					_race._state = RaceState.WAITING;
					
					_race.calculateOdds();
					
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.TICKETS_ARE_NOW_AVAILABLE_FOR_MONSTER_RACE_S1);
					msg.Params.addInt(_race._raceNumber);
					SystemMessagePacket msg2 = new SystemMessagePacket(SystemMessageId.TICKETS_SALES_ARE_CLOSED_FOR_MONSTER_RACE_S1_YOU_CAN_SEE_THE_AMOUNT_OF_WIN);
					msg2.Params.addInt(_race._raceNumber);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg, msg2);
					break;
				}
				case 960: // 16 min
				case 1020: // 17 min
				{
					int minutes = (_race._finalCountdown == 960) ? 2 : 1;
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.MONSTER_RACE_S2_WILL_BEGIN_IN_S1_MIN);
					msg.Params.addInt(minutes);
					msg.Params.addInt(_race._raceNumber);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg);
					break;
				}
				case 1050: // 17 min 30 sec
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.MONSTER_RACE_S1_WILL_BEGIN_IN_30_SEC);
					msg.Params.addInt(_race._raceNumber);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg);
					break;
				}
				case 1070: // 17 min 50 sec
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.MONSTER_RACE_S1_IS_ABOUT_TO_BEGIN_COUNTDOWN_IN_5_SEC);
					msg.Params.addInt(_race._raceNumber);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg);
					break;
				}
				case 1075: // 17 min 55 sec
				case 1076: // 17 min 56 sec
				case 1077: // 17 min 57 sec
				case 1078: // 17 min 58 sec
				case 1079: // 17 min 59 sec
				{
					int seconds = 1080 - _race._finalCountdown;
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.THE_RACE_BEGINS_IN_S1_SEC);
					msg.Params.addInt(seconds);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg);
					break;
				}
				case 1080: // 18 min
				{
					_race._state = RaceState.STARTING_RACE;
					_race._packet = new MonsterRaceInfoPacket(CODES[1][0], CODES[1][1], _race.getMonsters(), _race.getSpeeds());
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(new SystemMessagePacket(SystemMessageId.THEY_RE_OFF), SOUND_1, SOUND_2, _race._packet);
					break;
				}
				case 1085: // 18 min 5 sec
				{
					_race._packet = new MonsterRaceInfoPacket(CODES[2][0], CODES[2][1], _race.getMonsters(), _race.getSpeeds());
					
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(_race._packet);
					break;
				}
				case 1115: // 18 min 35 sec
				{
					_race._state = RaceState.RACE_END;
					
					// Populate history info with data, stores it in database.
					HistoryInfo info = _race._history[^1];
					info.setFirst(_race.getFirstPlace());
					info.setSecond(_race.getSecondPlace());
					info.setOddRate(_race._odds[_race.getFirstPlace() - 1]);
					
					_race.saveHistory(info);
					_race.clearBets();
					
					SystemMessagePacket msg = new(SystemMessageId.FIRST_PRIZE_GOES_TO_THE_PLAYER_IN_LANE_S1_SECOND_PRIZE_GOES_TO_THE_PLAYER_IN_LANE_S2);
					msg.Params.addInt(_race.getFirstPlace());
					msg.Params.addInt(_race.getSecondPlace());
					SystemMessagePacket msg2 = new(SystemMessageId.MONSTER_RACE_S1_IS_FINISHED);
					msg2.Params.addInt(_race._raceNumber);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(msg, msg2);
					_race._raceNumber++;
					break;
				}
				case 1140: // 19 min
				{
					Npc[] monsters = _race.getMonsters();
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>().SendPackets(new DeleteObjectPacket(monsters[0].getObjectId()),
						new DeleteObjectPacket(monsters[1].getObjectId()), new DeleteObjectPacket(monsters[2].getObjectId()),
						new DeleteObjectPacket(monsters[3].getObjectId()), new DeleteObjectPacket(monsters[4].getObjectId()),
						new DeleteObjectPacket(monsters[5].getObjectId()), new DeleteObjectPacket(monsters[6].getObjectId()),
						new DeleteObjectPacket(monsters[7].getObjectId()));
					break;
				}
			}
			
			_race._finalCountdown += 1;
		}
	}
	
	public void newRace()
	{
		// Edit _history.
		_history.Add(new HistoryInfo(_raceNumber, 0, 0, 0));
		
		// Randomize _npcTemplates.
		Random.Shared.Shuffle(_npcTemplates);
		
		// Setup 8 new creatures ; pickup the first 8 from _npcTemplates.
		for (int i = 0; i < 8; i++)
		{
			try
			{
				NpcTemplate template = NpcData.getInstance().getTemplate(_npcTemplates[i]);
				_monsters[i] = template.CreateInstance();
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
			}
		}
	}
	
	public void newSpeeds()
	{
		int total = 0;
		_first[1] = 0;
		_second[1] = 0;
		
		for (int i = 0; i < 8; i++)
		{
			total = 0;
			for (int j = 0; j < 20; j++)
			{
				if (j == 19)
				{
					_speeds[i][j] = 100;
				}
				else
				{
					_speeds[i][j] = Rnd.get(60) + 65;
				}
				total += _speeds[i][j];
			}
			
			if (total >= _first[1])
			{
				_second[0] = _first[0];
				_second[1] = _first[1];
				_first[0] = 8 - i;
				_first[1] = total;
			}
			else if (total >= _second[1])
			{
				_second[0] = 8 - i;
				_second[1] = total;
			}
		}
	}
	
	/**
	 * Load past races informations, feeding _history arrayList.<br>
	 * Also sets _raceNumber, based on latest HistoryInfo loaded.
	 */
	protected void loadHistory()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var records = ctx.DerbyHistory;
			foreach (MonsterDerbyHistory record in records)
			{
				_history.Add(new HistoryInfo(record.RaceId, record.First, record.Second, record.OddRate));
				_raceNumber++;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("MonsterRace: Can't load history: " + e);
		}
		
		LOGGER.Info("MonsterRace: loaded " + _history.Count + " records, currently on race #" + _raceNumber);
	}
	
	/**
	 * Save an history record into database.
	 * @param history The infos to store.
	 */
	protected void saveHistory(HistoryInfo history)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.DerbyHistory.Add(new MonsterDerbyHistory()
			{
				RaceId = history.getRaceId(),
				First = history.getFirst(),
				Second = history.getSecond(),
				OddRate = history.getOddRate(),
			});

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("MonsterRace: Can't save history: " + e);
		}
	}
	
	/**
	 * Load current bets per lane ; initialize the map keys.
	 */
	protected void loadBets()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.DerbyBets.ForEach(bet => setBetOnLane(bet.LaneId, bet.Bet, false));
		}
		catch (Exception e)
		{
			LOGGER.Warn("MonsterRace: Can't load bets: " + e);
		}
	}
	
	/**
	 * Save the current lane bet into database.
	 * @param lane : The lane to affect.
	 * @param sum : The sum to set.
	 */
	protected void saveBet(int lane, long sum)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			MonsterDerbyBet? bet = ctx.DerbyBets.SingleOrDefault(b => b.LaneId == lane);
			if (bet is null)
			{
				bet = new MonsterDerbyBet();
				ctx.DerbyBets.Add(bet);
				bet.LaneId = (byte)lane;
			}

			bet.Bet = sum;
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Warn("MonsterRace: Can't save bet: " + e);
		}
	}
	
	/**
	 * Clear all lanes bets, either on database or Map.
	 */
	protected void clearBets()
	{
		foreach (int key in _betsPerLane.Keys)
		{
			_betsPerLane.put(key, 0L);
		}
		
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.DerbyBets.ExecuteUpdate(s => s.SetProperty(bet => bet.Bet, 0));
		}
		catch (Exception e)
		{
			LOGGER.Warn("MonsterRace: Can't clear bets: " + e);
		}
	}
	
	/**
	 * Setup lane bet, based on previous value (if any).
	 * @param lane : The lane to edit.
	 * @param amount : The amount to add.
	 * @param saveOnDb : Should it be saved on db or not.
	 */
	public void setBetOnLane(int lane, long amount, bool saveOnDb)
	{
		long sum = (_betsPerLane.TryGetValue(lane, out long bet)) ? bet + amount : amount;
		
		_betsPerLane.put(lane, sum);
		
		if (saveOnDb)
		{
			saveBet(lane, sum);
		}
	}
	
	/**
	 * Calculate odds for every lane, based on others lanes.
	 */
	protected void calculateOdds()
	{
		// Clear previous List holding old odds.
		_odds.Clear();
		
		// Sort bets lanes per lane.
		Map<int, long> sortedLanes = new();
		
		// Pass a first loop in order to calculate total sum of all lanes.
		long sumOfAllLanes = 0;
		foreach (long amount in sortedLanes.Values)
		{
			sumOfAllLanes += amount;
		}
		
		// As we get the sum, we can now calculate the odd rate of each lane.
		foreach (long amount  in sortedLanes.Values)
		{
			_odds.Add((amount == 0) ? 0D : Math.Max(1.25, (sumOfAllLanes * 0.7) / amount));
		}
	}
	
	public Npc[] getMonsters()
	{
		return _monsters;
	}
	
	public int[][] getSpeeds()
	{
		return _speeds;
	}
	
	public int getFirstPlace()
	{
		return _first[0];
	}
	
	public int getSecondPlace()
	{
		return _second[0];
	}
	
	public MonsterRaceInfoPacket getRacePacket()
	{
		return _packet;
	}
	
	public RaceState getCurrentRaceState()
	{
		return _state;
	}
	
	public int getRaceNumber()
	{
		return _raceNumber;
	}
	
	public List<HistoryInfo> getHistory()
	{
		return _history;
	}
	
	public List<double> getOdds()
	{
		return _odds;
	}
	
	public static MonsterRace getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MonsterRace INSTANCE = new MonsterRace();
	}
}