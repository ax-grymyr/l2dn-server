using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

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
	
	protected static readonly PlaySound SOUND_1 = new PlaySound(1, "S_Race", 0, 0, 0, 0, 0);
	protected static readonly PlaySound SOUND_2 = new PlaySound("ItemSound2.race_start");

	protected static readonly int[][] CODES = [[-1, 0], [0, 15322], [13765, -1]];
	
	protected readonly List<int> _npcTemplates = new(); // List holding npc templates, shuffled on a new race.
	protected readonly List<HistoryInfo> _history = new(); // List holding old race records.
	protected readonly Map<int, long> _betsPerLane = new(); // Map holding all bets for each lane ; values setted to 0 after every race.
	protected readonly List<Double> _odds = new(); // List holding sorted odds per lane ; cleared at new odds calculation.
	
	protected int _raceNumber = 1;
	protected int _finalCountdown = 0;
	protected RaceState _state = RaceState.RACE_END;
	
	protected MonRaceInfo _packet;
	
	private readonly Npc[] _monsters = new Npc[8];
	private int[][] _speeds = new int[8][20];
	private readonly int[] _first = new int[2];
	private readonly int[] _second = new int[2];
	
	protected MonsterRace()
	{
		if (!Config.ALLOW_RACE)
		{
			return;
		}
		
		// Feed _history with previous race results.
		loadHistory();
		
		// Feed _betsPerLane with stored informations on bets.
		loadBets();
		
		// Feed _npcTemplates, we will only have to shuffle it when needed.
		for (int i = 31003; i < 31027; i++)
		{
			_npcTemplates.add(i);
		}
		
		ThreadPool.scheduleAtFixedRate(new Announcement(), 0, 1000);
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
		public Announcement()
		{
		}
		
		public void run()
		{
			if (_finalCountdown > 1200)
			{
				_finalCountdown = 0;
			}
			
			switch (_finalCountdown)
			{
				case 0:
				{
					newRace();
					newSpeeds();
					
					_state = RaceState.ACCEPTING_BETS;
					_packet = new MonRaceInfo(CODES[0][0], CODES[0][1], getMonsters(), getSpeeds());
					
					SystemMessage msg = new SystemMessage(SystemMessageId.TICKETS_ARE_NOW_AVAILABLE_FOR_MONSTER_RACE_S1);
					msg.addInt(_raceNumber);
					Broadcast.toAllPlayersInZoneType<DerbyTrackZone>(_packet, msg);
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
					SystemMessage msg = new SystemMessage(SystemMessageId.TICKETS_ARE_NOW_AVAILABLE_FOR_MONSTER_RACE_S1);
					msg.addInt(_raceNumber);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg);
					break;
				}
				case 300: // 5 min
				{
					SystemMessage msg = new SystemMessage(SystemMessageId.NOW_SELLING_TICKETS_FOR_MONSTER_RACE_S1);
					msg.addInt(_raceNumber);
					SystemMessage msg2 = new SystemMessage(SystemMessageId.TICKET_SALES_FOR_MONSTER_RACE_S1_ARE_CLOSED);
					msg2.addInt(10);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg, msg2);
					break;
				}
				case 600: // 10 min
				{
					SystemMessage msg = new SystemMessage(SystemMessageId.NOW_SELLING_TICKETS_FOR_MONSTER_RACE_S1);
					msg.addInt(_raceNumber);
					SystemMessage msg2 = new SystemMessage(SystemMessageId.TICKET_SALES_FOR_MONSTER_RACE_S1_ARE_CLOSED);
					msg2.addInt(5);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg, msg2);
					break;
				}
				case 840: // 14 min
				{
					SystemMessage msg = new SystemMessage(SystemMessageId.NOW_SELLING_TICKETS_FOR_MONSTER_RACE_S1);
					msg.addInt(_raceNumber);
					SystemMessage msg2 = new SystemMessage(SystemMessageId.TICKET_SALES_FOR_MONSTER_RACE_S1_ARE_CLOSED);
					msg2.addInt(1);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg, msg2);
					break;
				}
				case 900: // 15 min
				{
					_state = RaceState.WAITING;
					
					calculateOdds();
					
					SystemMessage msg = new SystemMessage(SystemMessageId.TICKETS_ARE_NOW_AVAILABLE_FOR_MONSTER_RACE_S1);
					msg.addInt(_raceNumber);
					SystemMessage msg2 = new SystemMessage(SystemMessageId.TICKETS_SALES_ARE_CLOSED_FOR_MONSTER_RACE_S1_YOU_CAN_SEE_THE_AMOUNT_OF_WIN);
					msg2.addInt(_raceNumber);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg, msg2);
					break;
				}
				case 960: // 16 min
				case 1020: // 17 min
				{
					int minutes = (_finalCountdown == 960) ? 2 : 1;
					SystemMessage msg = new SystemMessage(SystemMessageId.MONSTER_RACE_S2_WILL_BEGIN_IN_S1_MIN);
					msg.addInt(minutes);
					msg.addInt(_raceNumber);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg);
					break;
				}
				case 1050: // 17 min 30 sec
				{
					SystemMessage msg = new SystemMessage(SystemMessageId.MONSTER_RACE_S1_WILL_BEGIN_IN_30_SEC);
					msg.addInt(_raceNumber);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg);
					break;
				}
				case 1070: // 17 min 50 sec
				{
					SystemMessage msg = new SystemMessage(SystemMessageId.MONSTER_RACE_S1_IS_ABOUT_TO_BEGIN_COUNTDOWN_IN_5_SEC);
					msg.addInt(_raceNumber);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg);
					break;
				}
				case 1075: // 17 min 55 sec
				case 1076: // 17 min 56 sec
				case 1077: // 17 min 57 sec
				case 1078: // 17 min 58 sec
				case 1079: // 17 min 59 sec
				{
					int seconds = 1080 - _finalCountdown;
					SystemMessage msg = new SystemMessage(SystemMessageId.THE_RACE_BEGINS_IN_S1_SEC);
					msg.addInt(seconds);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg);
					break;
				}
				case 1080: // 18 min
				{
					_state = RaceState.STARTING_RACE;
					_packet = new MonRaceInfo(CODES[1][0], CODES[1][1], getMonsters(), getSpeeds());
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, new SystemMessage(SystemMessageId.THEY_RE_OFF), SOUND_1, SOUND_2, _packet);
					break;
				}
				case 1085: // 18 min 5 sec
				{
					_packet = new MonRaceInfo(CODES[2][0], CODES[2][1], getMonsters(), getSpeeds());
					
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, _packet);
					break;
				}
				case 1115: // 18 min 35 sec
				{
					_state = RaceState.RACE_END;
					
					// Populate history info with data, stores it in database.
					HistoryInfo info = _history.get(_history.size() - 1);
					info.setFirst(getFirstPlace());
					info.setSecond(getSecondPlace());
					info.setOddRate(_odds.get(getFirstPlace() - 1));
					
					saveHistory(info);
					clearBets();
					
					SystemMessage msg = new SystemMessage(SystemMessageId.FIRST_PRIZE_GOES_TO_THE_PLAYER_IN_LANE_S1_SECOND_PRIZE_GOES_TO_THE_PLAYER_IN_LANE_S2);
					msg.addInt(getFirstPlace());
					msg.addInt(getSecondPlace());
					SystemMessage msg2 = new SystemMessage(SystemMessageId.MONSTER_RACE_S1_IS_FINISHED);
					msg2.addInt(_raceNumber);
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, msg, msg2);
					_raceNumber++;
					break;
				}
				case 1140: // 19 min
				{
					Broadcast.toAllPlayersInZoneType(DerbyTrackZone.class, new DeleteObject(getMonsters()[0]), new DeleteObject(getMonsters()[1]), new DeleteObject(getMonsters()[2]), new DeleteObject(getMonsters()[3]), new DeleteObject(getMonsters()[4]), new DeleteObject(getMonsters()[5]), new DeleteObject(getMonsters()[6]), new DeleteObject(getMonsters()[7]));
					break;
				}
			}
			_finalCountdown += 1;
		}
	}
	
	public void newRace()
	{
		// Edit _history.
		_history.add(new HistoryInfo(_raceNumber, 0, 0, 0));
		
		// Randomize _npcTemplates.
		Collections.shuffle(_npcTemplates);
		
		// Setup 8 new creatures ; pickup the first 8 from _npcTemplates.
		for (int i = 0; i < 8; i++)
		{
			try
			{
				NpcTemplate template = NpcData.getInstance().getTemplate(_npcTemplates.get(i));
				_monsters[i] = (Npc) Class.forName("org.l2jmobius.gameserver.model.actor.instance." + template.getType()).getConstructors()[0].newInstance(template);
			}
			catch (Exception e)
			{
				LOGGER.Warn("", e);
			}
		}
	}
	
	public void newSpeeds()
	{
		_speeds = new int[8][20];
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
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement("SELECT * FROM mdt_history");
			ResultSet rset = statement.executeQuery();
			
			while (rset.next())
			{
				_history.add(new HistoryInfo(rset.getInt("race_id"), rset.getInt("first"), rset.getInt("second"), rset.getDouble("odd_rate")));
				_raceNumber++;
			}
			rset.close();
			statement.close();
		}
		catch (Exception e)
		{
			LOGGER.Warn("MonsterRace: Can't load history: " + e);
		}
		LOGGER.Info("MonsterRace: loaded " + _history.size() + " records, currently on race #" + _raceNumber);
	}
	
	/**
	 * Save an history record into database.
	 * @param history The infos to store.
	 */
	protected void saveHistory(HistoryInfo history)
	{
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement("REPLACE INTO mdt_history (race_id, first, second, odd_rate) VALUES (?,?,?,?)");
			statement.setInt(1, history.getRaceId());
			statement.setInt(2, history.getFirst());
			statement.setInt(3, history.getSecond());
			statement.setDouble(4, history.getOddRate());
			statement.execute();
			statement.close();
		}
		catch (Exception e)
		{
			LOGGER.Warn("MonsterRace: Can't save history: " + e);
		}
	}
	
	/**
	 * Load current bets per lane ; initialize the map keys.
	 */
	protected void loadBets()
	{
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement("SELECT * FROM mdt_bets");
			ResultSet rset = statement.executeQuery();
			
			while (rset.next())
			{
				setBetOnLane(rset.getInt("lane_id"), rset.getLong("bet"), false);
			}
			
			rset.close();
			statement.close();
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
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement("REPLACE INTO mdt_bets (lane_id, bet) VALUES (?,?)");
			statement.setInt(1, lane);
			statement.setLong(2, sum);
			statement.execute();
			statement.close();
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
		foreach (int key in _betsPerLane.keySet())
		{
			_betsPerLane.put(key, 0L);
		}
		
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement("UPDATE mdt_bets SET bet = 0");
			statement.execute();
			statement.close();
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
		long sum = (_betsPerLane.containsKey(lane)) ? _betsPerLane.get(lane) + amount : amount;
		
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
		foreach (long amount in sortedLanes.values())
		{
			sumOfAllLanes += amount;
		}
		
		// As we get the sum, we can now calculate the odd rate of each lane.
		foreach (long amount  in sortedLanes.values())
		{
			_odds.add((amount == 0) ? 0D : Math.Max(1.25, (sumOfAllLanes * 0.7) / amount));
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
	
	public MonRaceInfo getRacePacket()
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
	
	public List<Double> getOdds()
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