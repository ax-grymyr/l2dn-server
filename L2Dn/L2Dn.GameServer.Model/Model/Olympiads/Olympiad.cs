using System.Collections.Frozen;
using System.Globalization;
using System.Runtime.CompilerServices;
using L2Dn.Configuration;
using L2Dn.Events;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author godson
 */
public class Olympiad
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Olympiad));
	protected static readonly Logger LOGGER_OLYMPIAD = LogManager.GetLogger("olympiad");
	
	private static readonly Map<int, NobleData> NOBLES = new();
	private static readonly Map<int, int> NOBLES_RANK = new();
	
	public const String OLYMPIAD_HTML_PATH = "html/olympiad/";
	public const String UNCLAIMED_OLYMPIAD_POINTS_VAR = "UNCLAIMED_OLYMPIAD_POINTS";
	
	private static readonly FrozenSet<int> HERO_IDS = CategoryData.getInstance().getCategoryByType(CategoryType.FOURTH_CLASS_GROUP);
	
	private static readonly int COMP_START_HOUR = Config.ALT_OLY_START_TIME; // 6PM
	private static readonly int COMP_START_MIN = Config.ALT_OLY_MIN; // 00 mins
	private static readonly TimeSpan COMP_PERIOD = TimeSpan.FromMilliseconds(Config.ALT_OLY_CPERIOD); // 6 hours
	protected static readonly TimeSpan WEEKLY_PERIOD = TimeSpan.FromMilliseconds(Config.ALT_OLY_WPERIOD); // 1 week
	protected static readonly TimeSpan VALIDATION_PERIOD = TimeSpan.FromMilliseconds(Config.ALT_OLY_VPERIOD); // 24 hours
	
	public static readonly int DEFAULT_POINTS = Config.ALT_OLY_START_POINTS;
	protected static readonly int WEEKLY_POINTS = Config.ALT_OLY_WEEKLY_POINTS;
	
	public const String CHAR_ID = "charId";
	public const String CLASS_ID = "class_id";
	public const String CHAR_NAME = "char_name";
	public const String POINTS = "olympiad_points";
	public const String COMP_DONE = "competitions_done";
	public const String COMP_WON = "competitions_won";
	public const String COMP_LOST = "competitions_lost";
	public const String COMP_DRAWN = "competitions_drawn";
	public const String COMP_DONE_WEEK = "competitions_done_week";
	
	protected DateTime _olympiadEnd;
	protected DateTime _validationEnd;
	
	/**
	 * The current period of the olympiad.<br>
	 * <b>0 -</b> Competition period<br>
	 * <b>1 -</b> Validation Period
	 */
	protected int _period;
	protected DateTime _nextWeeklyChange;
	protected int _currentCycle;
	private DateTime _compEnd;
	private DateTime _compStart;
	public static bool _inCompPeriod;
	protected static bool _compStarted = false;
	protected ScheduledFuture _scheduledCompStart;
	protected ScheduledFuture _scheduledCompEnd;
	protected ScheduledFuture _scheduledOlympiadEnd;
	protected ScheduledFuture _scheduledWeeklyTask;
	protected ScheduledFuture _scheduledValdationTask;
	protected ScheduledFuture _gameManager = null;
	protected ScheduledFuture _gameAnnouncer = null;

	private readonly EventContainer _eventContainer = new("Olympiad", GlobalEvents.Global);
	
	protected Olympiad()
	{
		if (Config.OLYMPIAD_ENABLED)
		{
			load();
			AntiFeedManager.getInstance().registerEvent(AntiFeedManager.OLYMPIAD_ID);
			
			if (_period == 0)
			{
				init();
			}
		}
		else
		{
			LOGGER.Info("Disabled.");
		}
	}

	public EventContainer Events => _eventContainer;
	
	private void load()
	{
		NOBLES.clear();
		
		bool loaded = false;
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var record = ctx.OlympiadData.SingleOrDefault(r => r.Id == 0);
			if (record is not null)
			{
				_currentCycle = record.CurrentCycle;
				_period = record.Period;
				_olympiadEnd = record.OlympiadEnd;
				_validationEnd = record.ValidationEnd;
				_nextWeeklyChange = record.NextWeeklyChange;
				loaded = true;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Error loading olympiad data from database: " + e);
		}
		
		if (!loaded)
		{
			// LOGGER.info("Olympiad System: Failed to load data from database, trying to load from file.");
			ConfigurationParser parser = new ConfigurationParser();
			try
			{
				parser.LoadConfig(Config.OLYMPIAD_CONFIG_FILE);
			}
			catch (Exception e)
			{
				LOGGER.Error("Olympiad System: Error loading olympiad properties: " + e);
				return;
			}
			
			_currentCycle = parser.getInt("CurrentCycle", 1);
			_period = parser.getInt("Period", 0);
			_olympiadEnd = DateTime.MinValue; // parser.getLong("OlympiadEnd", 0);
			_validationEnd = DateTime.MinValue; // parser.getLong("ValidationEnd", 0);
			_nextWeeklyChange = DateTime.MinValue; // parser.getLong("NextWeeklyChange", 0);
		}
		
		DateTime currentTime = DateTime.UtcNow;
		switch (_period)
		{
			case 0:
			{
				if (_olympiadEnd < currentTime)
				{
					setNewOlympiadEnd();
				}
				else
				{
					scheduleWeeklyChange();
				}
				break;
			}
			case 1:
			{
				if (_validationEnd > currentTime)
				{
					loadNoblesRank();
					_scheduledValdationTask = ThreadPool.schedule(new ValidationEndTask(this), getMillisToValidationEnd());
				}
				else
				{
					_currentCycle++;
					_period = 0;
					deleteNobles();
					setNewOlympiadEnd();
				}
				break;
			}
			default:
			{
				LOGGER.Warn("Olympiad System: Omg something went wrong in loading!! Period = " + _period);
				return;
			}
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = from n in ctx.OlympiadNobles
				from c in ctx.Characters
				where n.CharacterId == c.Id
				select new
				{
					n.CharacterId,
					n.Class,
					CharacterName = c.Name,
					n.OlympiadPoints,
					n.CompetitionsDone,
					n.CompetitionsWon,
					n.CompetitionsLost,
					n.CompetitionsDrawn,
					n.CompetitionsDoneWeek
				};
			
			foreach (var record in query)
			{
				NobleData nobleData = new NobleData();
				nobleData.Class = record.Class;
				nobleData.CharacterName = record.CharacterName;
				nobleData.OlympiadPoints = record.OlympiadPoints;
				nobleData.CompetitionsDone = record.CompetitionsDone;
				nobleData.CompetitionsWon = record.CompetitionsWon;
				nobleData.CompetitionsLost = record.CompetitionsLost;
				nobleData.CompetitionsDrawn = record.CompetitionsDrawn;
				nobleData.CompetitionsDoneWeek = record.CompetitionsDoneWeek;
				addNobleStats(record.CharacterId, nobleData);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Error loading noblesse data from database: ", e);
		}
		
		lock (this)
		{
			LOGGER.Info("Olympiad System: Loading....");
			if (_period == 0)
			{
				LOGGER.Info("Olympiad System: Currently in Olympiad Period");
			}
			else
			{
				LOGGER.Info("Olympiad System: Currently in Validation Period");
			}
			
			TimeSpan milliToEnd;
			if (_period == 0)
			{
				milliToEnd = getMillisToOlympiadEnd();
			}
			else
			{
				milliToEnd = getMillisToValidationEnd();
			}
			
			LOGGER.Info("Olympiad System: " + milliToEnd.Days + " days, " + milliToEnd.Hours + " hours and " + milliToEnd.Minutes + " mins until period ends.");
			
			if (_period == 0)
			{
				milliToEnd = getMillisToWeekChange();
				LOGGER.Info("Olympiad System: Next weekly change is in " + milliToEnd.Days + " days, " + milliToEnd.Hours + " hours and " + milliToEnd.Minutes + " mins.");
			}
		}
		
		LOGGER.Info("Olympiad System: Loaded " + NOBLES.size() + " Nobles");
	}
	
	public int getOlympiadRank(Player player)
	{
		return NOBLES_RANK.getOrDefault(player.getObjectId(), 0);
	}
	
	public void loadNoblesRank()
	{
		NOBLES_RANK.clear();
		Map<int, int> tmpPlace = new();
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = ctx.OlympiadNoblesEom.Where(r => r.CompetitionsDone >= Config.ALT_OLY_MIN_MATCHES)
				.OrderByDescending(r => r.OlympiadPoints).ThenByDescending(r => r.CompetitionsDone)
				.ThenByDescending(r => r.CompetitionsWon);
			
			int place = 1;
			foreach (var record in query)
			{
				tmpPlace.put(record.CharacterId, place++);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Error loading noblesse data from database for Ranking: ", e);
		}
		
		int rank1 = (int) Math.Round(tmpPlace.size() * 0.01);
		int rank2 = (int) Math.Round(tmpPlace.size() * 0.10);
		int rank3 = (int) Math.Round(tmpPlace.size() * 0.25);
		int rank4 = (int) Math.Round(tmpPlace.size() * 0.50);
		if (rank1 == 0)
		{
			rank1 = 1;
			rank2++;
			rank3++;
			rank4++;
		}
		foreach (var chr in tmpPlace)
		{
			if (chr.Value <= rank1)
			{
				NOBLES_RANK.put(chr.Key, 1);
			}
			else if (tmpPlace.get(chr.Key) <= rank2)
			{
				NOBLES_RANK.put(chr.Key, 2);
			}
			else if (tmpPlace.get(chr.Key) <= rank3)
			{
				NOBLES_RANK.put(chr.Key, 3);
			}
			else if (tmpPlace.get(chr.Key) <= rank4)
			{
				NOBLES_RANK.put(chr.Key, 4);
			}
			else
			{
				NOBLES_RANK.put(chr.Key, 5);
			}
		}
		
		// Store remaining hero reward points to player variables.
		foreach (int noblesId in NOBLES.Keys)
		{
			int points = getOlympiadTradePoint(noblesId);
			if (points > 0)
			{
				Player player = World.getInstance().getPlayer(noblesId);
				if (player != null)
				{
					player.getVariables().set(UNCLAIMED_OLYMPIAD_POINTS_VAR, points);
				}
				else
				{
					// Remove previous record.
					try 
					{
						using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
						ctx.CharacterVariables
							.Where(r => r.CharacterId == noblesId && r.Name == UNCLAIMED_OLYMPIAD_POINTS_VAR)
							.ExecuteDelete();
					}
					catch (Exception e)
					{
						LOGGER.Error("Olympiad System: Couldn't remove unclaimed olympiad points from DB! " + e);
					}

					// Add new value.
					try 
					{
						using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
						ctx.CharacterVariables.Add(new CharacterVariable()
						{
							CharacterId = noblesId,
							Name = UNCLAIMED_OLYMPIAD_POINTS_VAR,
							Value = points.ToString(CultureInfo.InvariantCulture)
						});

						ctx.SaveChanges();
					}
					catch (Exception e)
					{
						LOGGER.Error("Olympiad System: Couldn't store unclaimed olympiad points to DB! " + e);
					}
				}
			}
		}
	}
	
	protected void init()
	{
		if (_period == 1)
		{
			return;
		}
		
		DateTime compStart = DateTime.Now;

		if (Config.ALT_OLY_COMPETITION_DAYS.Count != 0)
		{
			DayOfWeek currentDay = compStart.DayOfWeek;
			int dayCounter = 0;
			while (!Config.ALT_OLY_COMPETITION_DAYS.Contains(currentDay))
			{
				dayCounter++;
				currentDay = currentDay == DayOfWeek.Saturday ? DayOfWeek.Sunday : currentDay + 1;
			}

			compStart = compStart.AddDays(dayCounter);
		}

		_compStart = new DateTime(compStart.Year, compStart.Month, compStart.Day, COMP_START_HOUR, COMP_START_MIN, 0);
		_compEnd = _compStart + COMP_PERIOD;
		
		if (_scheduledOlympiadEnd != null)
		{
			_scheduledOlympiadEnd.cancel(true);
		}
		
		_scheduledOlympiadEnd = ThreadPool.schedule(new OlympiadEndTask(this), getMillisToOlympiadEnd());
		
		updateCompStatus();
	}
	
	protected class OlympiadEndTask: Runnable
	{
		private readonly Olympiad _olympiad;

		public OlympiadEndTask(Olympiad olympiad)
		{
			_olympiad = olympiad;
		}
		
		public void run()
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.ROUND_S1_OF_THE_OLYMPIAD_HAS_NOW_ENDED);
			sm.Params.addInt(_olympiad._currentCycle);
			
			Broadcast.toAllOnlinePlayers(sm);
			
			if (_olympiad._scheduledWeeklyTask != null)
			{
				_olympiad._scheduledWeeklyTask.cancel(true);
			}
			
			_olympiad.saveNobleData();
			
			_olympiad._period = 1;
			List<StatSet> heroesToBe = _olympiad.sortHerosToBe();
			Hero.getInstance().resetData();
			Hero.getInstance().computeNewHeroes(heroesToBe);
			
			_olympiad.saveOlympiadStatus();
			_olympiad.updateMonthlyData();
			
			DateTime validationEnd = DateTime.UtcNow;
			_olympiad._validationEnd = validationEnd + VALIDATION_PERIOD;
			
			_olympiad.loadNoblesRank();
			_olympiad._scheduledValdationTask = ThreadPool.schedule(new ValidationEndTask(_olympiad), _olympiad.getMillisToValidationEnd());
		}
	}
	
	protected class ValidationEndTask: Runnable
	{
		private readonly Olympiad _olympiad;

		public ValidationEndTask(Olympiad olympiad)
		{
			_olympiad = olympiad;
		}
		
		public void run()
		{
			Broadcast.toAllOnlinePlayers("Olympiad Validation Period has ended");
			_olympiad._period = 0;
			_olympiad._currentCycle++;
			_olympiad.deleteNobles();
			_olympiad.setNewOlympiadEnd();
			_olympiad.init();
		}
	}
	
	protected static int getNobleCount()
	{
		return NOBLES.size();
	}
	
	public static NobleData getNobleStats(int playerId)
	{
		return NOBLES.get(playerId);
	}
	
	public static void removeNobleStats(int playerId)
	{
		NOBLES.remove(playerId);
		NOBLES_RANK.remove(playerId);
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.OlympiadNobles.Where(r => r.CharacterId == playerId).ExecuteDelete();
			ctx.OlympiadNoblesEom.Where(r => r.CharacterId == playerId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Error removing noblesse data from database: " + e);
		}
	}
	
	private void updateCompStatus()
	{
		TimeSpan milliToStart;
		lock (this)
		{
			milliToStart = getMillisToCompBegin();
		}

		LOGGER.Info("Olympiad System: Competition Period Starts in " + milliToStart.Days + " days, " +
		            milliToStart.Hours + " hours and " + milliToStart.Minutes + " mins.");
		
		LOGGER.Info("Olympiad System: Event starts/started: " + _compStart);
		
		_scheduledCompStart = ThreadPool.schedule(() =>
		{
			if (isOlympiadEnd())
			{
				return;
			}
			
			_inCompPeriod = true;
			
			Broadcast.toAllOnlinePlayers(new SystemMessagePacket(SystemMessageId.THE_OLYMPIAD_HAS_BEGAN));
			LOGGER.Info("Olympiad System: Olympiad Games have started.");
			LOGGER_OLYMPIAD.Info("Result,Player1,Player2,Player1 HP,Player2 HP,Player1 Damage,Player2 Damage,Points,Classed");
			
			_gameManager = ThreadPool.scheduleAtFixedRate(OlympiadGameManager.getInstance(), 30000, 30000);
			if (Config.ALT_OLY_ANNOUNCE_GAMES)
			{
				_gameAnnouncer = ThreadPool.scheduleAtFixedRate(new OlympiadAnnouncer(), 30000, 500);
			}
			
			TimeSpan regEnd = getMillisToCompEnd() - TimeSpan.FromMilliseconds(600000);
			if (regEnd > TimeSpan.Zero)
			{
				ThreadPool.schedule(
					() => Broadcast.toAllOnlinePlayers(
						new SystemMessagePacket(SystemMessageId.THE_OLYMPIAD_REGISTRATION_PERIOD_HAS_ENDED)), regEnd);
			}
			
			_scheduledCompEnd = ThreadPool.schedule(() =>
			{
				if (isOlympiadEnd())
				{
					return;
				}
				_inCompPeriod = false;
				Broadcast.toAllOnlinePlayers(new SystemMessagePacket(SystemMessageId.THE_OLYMPIAD_IS_OVER));
				LOGGER.Info("Olympiad System: Olympiad games have ended.");
				
				while (OlympiadGameManager.getInstance().isBattleStarted()) // cleared in game manager
				{
					try
					{
						// wait 1 minutes for end of pending games
						Thread.Sleep(60000);
					}
					catch (Exception e)
					{
						// Ignore.
					}
				}
				
				if (_gameManager != null)
				{
					_gameManager.cancel(false);
					_gameManager = null;
				}
				
				if (_gameAnnouncer != null)
				{
					_gameAnnouncer.cancel(false);
					_gameAnnouncer = null;
				}
				
				saveOlympiadStatus();
				
				init();
			}, getMillisToCompEnd());
		}, getMillisToCompBegin());
	}
	
	private TimeSpan getMillisToOlympiadEnd()
	{
		// if (_olympiadEnd > System.currentTimeMillis())
		return _olympiadEnd - DateTime.Now;
		// return 10;
	}
	
	public void manualSelectHeroes()
	{
		if (_scheduledOlympiadEnd != null)
		{
			_scheduledOlympiadEnd.cancel(true);
		}
		
		_scheduledOlympiadEnd = ThreadPool.schedule(new OlympiadEndTask(this), 0);
	}
	
	protected TimeSpan getMillisToValidationEnd()
	{
		DateTime currentTime = DateTime.UtcNow;
		if (_validationEnd > currentTime)
		{
			return _validationEnd - currentTime;
		}
		
		return TimeSpan.FromMilliseconds(10);
	}
	
	public bool isOlympiadEnd()
	{
		return _period != 0;
	}
	
	protected void setNewOlympiadEnd()
	{
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.ROUND_S1_OF_THE_OLYMPIAD_GAMES_HAS_STARTED);
		sm.Params.addInt(_currentCycle);
		Broadcast.toAllOnlinePlayers(sm);
		
		DateTime currentTime = DateTime.Today;
		DateTime nextChange = DateTime.Now;
	
		switch (Config.ALT_OLY_PERIOD)
		{
			case "DAY":
			{
				currentTime = currentTime.AddDays(Config.ALT_OLY_PERIOD_MULTIPLIER);
				if (Config.ALT_OLY_PERIOD_MULTIPLIER >= 14)
				{
					_nextWeeklyChange = nextChange + WEEKLY_PERIOD;
				}
				else if (Config.ALT_OLY_PERIOD_MULTIPLIER >= 7)
				{
					_nextWeeklyChange = nextChange + (WEEKLY_PERIOD / 2);
				}
				else
				{
					LOGGER.Warn("Invalid config value for Config.ALT_OLY_PERIOD_MULTIPLIER, must be >= 7");
				}
				break;
			}
			case "WEEK":
			{
				currentTime = currentTime.AddDays(7 * Config.ALT_OLY_PERIOD_MULTIPLIER);
				
				if (Config.ALT_OLY_PERIOD_MULTIPLIER > 1)
				{
					_nextWeeklyChange = nextChange + WEEKLY_PERIOD;
				}
				else
				{
					_nextWeeklyChange = nextChange + (WEEKLY_PERIOD / 2);
				}
				break;
			}
			case "MONTH":
			{
				currentTime = currentTime.AddMonths(Config.ALT_OLY_PERIOD_MULTIPLIER);
				_nextWeeklyChange = nextChange + WEEKLY_PERIOD;
				break;
			}
		}

		_olympiadEnd = currentTime;
		
		scheduleWeeklyChange();
	}
	
	public bool inCompPeriod()
	{
		return _inCompPeriod;
	}
	
	private TimeSpan getMillisToCompBegin()
	{
		DateTime currentTime = DateTime.Now;
		if (_compStart < currentTime && _compEnd > currentTime)
		{
			return TimeSpan.FromMilliseconds(10);
		}
		
		if (_compStart > currentTime)
		{
			return _compStart - currentTime;
		}
		
		return setNewCompBegin();
	}
	
	private TimeSpan setNewCompBegin()
	{
		DateTime compStart = DateTime.Now;
		compStart = new DateTime(compStart.Year, compStart.Month, compStart.Day, COMP_START_HOUR, COMP_START_MIN, 0);
		
		DayOfWeek currentDay = compStart.DayOfWeek;
		
		// Today's competitions ended, start checking from next day.
		if (currentDay == compStart.DayOfWeek)
		{
			if (currentDay == DayOfWeek.Saturday)
			{
				currentDay = DayOfWeek.Sunday;
			}
			else
			{
				currentDay++;
			}
		}
		
		int dayCounter = 0;
		if (Config.ALT_OLY_COMPETITION_DAYS.Count != 0)
		{
			while (!Config.ALT_OLY_COMPETITION_DAYS.Contains(currentDay))
			{
				currentDay = currentDay == DayOfWeek.Saturday ? DayOfWeek.Sunday : currentDay + 1;
				dayCounter++;
			}
		}

		_compStart = compStart.AddDays(dayCounter + 1);
		_compEnd = _compStart + COMP_PERIOD;
		
		LOGGER.Info("Olympiad System: New Schedule @ " + _compStart);
		
		return _compStart - DateTime.Now;
	}
	
	public TimeSpan getMillisToCompEnd()
	{
		// if (_compEnd > System.currentTimeMillis())
		return _compEnd - DateTime.Now;
		// return 10;
	}
	
	private TimeSpan getMillisToWeekChange()
	{
		DateTime currentTime = DateTime.Now;
		if (_nextWeeklyChange > currentTime)
		{
			return _nextWeeklyChange - currentTime;
		}

		return TimeSpan.FromMilliseconds(10);
	}
	
	private void scheduleWeeklyChange()
	{
		_scheduledWeeklyTask = ThreadPool.scheduleAtFixedRate(() =>
		{
			addWeeklyPoints();
			LOGGER.Info("Olympiad System: Added weekly points to nobles");
			resetWeeklyMatches();
			LOGGER.Info("Olympiad System: Reset weekly matches to nobles");
			
			_nextWeeklyChange = DateTime.UtcNow + WEEKLY_PERIOD;
		}, getMillisToWeekChange(), WEEKLY_PERIOD);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected void addWeeklyPoints()
	{
		if (_period == 1)
		{
			return;
		}
		
		foreach (NobleData nobleInfo in NOBLES.values())
		{
			nobleInfo.OlympiadPoints += WEEKLY_POINTS;
		}
	}
	
	/**
	 * Resets number of matches, classed matches, non classed matches, team matches done by noble characters in the week.
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected void resetWeeklyMatches()
	{
		if (_period == 1)
		{
			return;
		}
		
		foreach (NobleData nobleInfo in NOBLES.values())
		{
			nobleInfo.CompetitionsDoneWeek = 0;
		}
	}
	
	public int getCurrentCycle()
	{
		return _currentCycle;
	}
	
	public int getPeriod()
	{
		return _period;
	}
	
	public bool playerInStadia(Player player)
	{
		return ZoneManager.getInstance().getOlympiadStadium(player) != null;
	}
	
	/**
	 * Save noblesse data to database
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected void saveNobleData()
	{
		if (NOBLES.isEmpty())
		{
			return;
		}
		
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (var entry in NOBLES)
			{
				NobleData nobleInfo = entry.Value;
				
				if (nobleInfo == null)
				{
					continue;
				}

				var record = ctx.OlympiadNobles.SingleOrDefault(r => r.CharacterId == entry.Key);
				if (record is null)
				{
					record = new OlympiadNoble();
					record.CharacterId = entry.Key;
					ctx.OlympiadNobles.Add(record);
				}

				record.Class = nobleInfo.Class;
				record.OlympiadPoints = nobleInfo.OlympiadPoints;
				record.CompetitionsDone = nobleInfo.CompetitionsDone;
				record.CompetitionsWon = nobleInfo.CompetitionsWon;
				record.CompetitionsLost = nobleInfo.CompetitionsLost;
				record.CompetitionsDrawn = nobleInfo.CompetitionsDrawn;
				record.CompetitionsDoneWeek = nobleInfo.CompetitionsDoneWeek;
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Failed to save noblesse data to database: " + e);
		}
	}
	
	/**
	 * Save olympiad.properties file with current olympiad status and update noblesse table in database
	 */
	public void saveOlympiadStatus()
	{
		saveNobleData();
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			DbOlympiadData? record = ctx.OlympiadData.SingleOrDefault(r => r.Id == 0);
			if (record is null)
			{
				record = new DbOlympiadData()
				{
					Id = 0,
				};

				ctx.OlympiadData.Add(record);
			}
			
			record.CurrentCycle = (short)_currentCycle;
			record.Period = (short)_period;
			record.OlympiadEnd = _olympiadEnd;
			record.ValidationEnd = _validationEnd;
			record.NextWeeklyChange = _nextWeeklyChange;
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Failed to save olympiad data to database: " + e);
		}
	}
	
	protected void updateMonthlyData()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.OlympiadNoblesEom.ExecuteDelete();

			ctx.OlympiadNoblesEom.AddRange(ctx.OlympiadNobles.Select(r => new OlympiadNobleEom()
			{
				CharacterId = r.CharacterId,
				Class = r.Class,
				CompetitionsDone = r.CompetitionsDone,
				CompetitionsDrawn = r.CompetitionsDrawn,
                CompetitionsLost = r.CompetitionsLost,
                CompetitionsWon = r.CompetitionsWon,
                OlympiadPoints = r.OlympiadPoints
			}));

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Failed to update monthly noblese data: " + e);
		}
	}
	
	protected List<StatSet> sortHerosToBe()
	{
		if (_period != 1)
		{
			return new();
		}
		
		LOGGER_OLYMPIAD.Info("Noble,charid,classid,compDone,points");
		foreach (var entry in NOBLES)
		{
			NobleData nobleInfo = entry.Value;
			if (nobleInfo == null)
			{
				continue;
			}
			
			int charId = entry.Key;
			CharacterClass classId = nobleInfo.Class;
			string charName = nobleInfo.CharacterName;
			int points = nobleInfo.OlympiadPoints;
			int compDone = nobleInfo.CompetitionsDone;
			
			LOGGER_OLYMPIAD.Info(charName + "," + charId + "," + classId + "," + compDone + "," + points);
		}
		
		List<StatSet> heroesToBe = new();
		
		int legendId = 0;
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			int? result = ctx.OlympiadNobles.Where(r => r.CompetitionsDone >= Config.ALT_OLY_MIN_MATCHES)
				.OrderByDescending(r => r.OlympiadPoints).Select(r => (int?)r.CharacterId).FirstOrDefault();

			if (result != null)
			{
				legendId = result.Value;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Couldnt load legend from DB: " + e);
		}

		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (int element in HERO_IDS)
			{
				// Classic can have 2nd and 3rd class competitors, but only 1 hero
				CharacterClass heroClass = (CharacterClass)element; 
				CharacterClass? parent = ClassListData.getInstance().getClass(heroClass).getParentClassId();

				var query = (from n in ctx.OlympiadNobles
					from c in ctx.Characters
					where n.CharacterId == c.Id && n.CompetitionsDone >= Config.ALT_OLY_MIN_MATCHES &&
					      n.CompetitionsWon > 0 && (n.Class == heroClass || n.Class == parent)
					orderby n.OlympiadPoints descending, n.CompetitionsDone descending, n.CompetitionsWon descending
					select new
					{
						n.CharacterId,
						c.Name,
					}).FirstOrDefault();

				if (query != null)
				{
					StatSet hero = new StatSet();
					int charId = query.CharacterId;
					hero.set(CLASS_ID, heroClass); // save the 3rd class title
					hero.set(CHAR_ID, charId);
					hero.set(CHAR_NAME, query.Name);
					hero.set("LEGEND", charId == legendId ? 1 : 0);
						
					LOGGER_OLYMPIAD.Info("Hero " + query.Name + "," + charId + "," + heroClass);
					heroesToBe.Add(hero);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Couldnt load heros from DB: " + e);
		}
		
		return heroesToBe;
	}
	
	public List<String> getClassLeaderBoard(CharacterClass classId)
	{
		List<String> names = new();
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			IQueryable<string> query;
			if (Config.ALT_OLY_SHOW_MONTHLY_WINNERS)
				query = from n in ctx.OlympiadNoblesEom
					from c in ctx.Characters
					where n.CharacterId == c.Id &&
					      (n.Class == classId || (classId == (CharacterClass)132 && n.Class == (CharacterClass)133)) &&
					      n.CompetitionsDone >= Config.ALT_OLY_MIN_MATCHES
					orderby n.OlympiadPoints descending, n.CompetitionsDone descending, n.CompetitionsWon descending
					select c.Name;
			else
				query = from n in ctx.OlympiadNobles
					from c in ctx.Characters
					where n.CharacterId == c.Id &&
					      (n.Class == classId || (classId == (CharacterClass)132 && n.Class == (CharacterClass)133)) &&
					      n.CompetitionsDone >= Config.ALT_OLY_MIN_MATCHES
					orderby n.OlympiadPoints descending, n.CompetitionsDone descending, n.CompetitionsWon descending
					select c.Name;

			foreach (string name in query.Take(10))
			{
				names.Add(name);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Couldn't load olympiad leaders from DB!");
		}
		return names;
	}
	
	private int getOlympiadTradePoint(int objectId)
	{
		if ((_period != 1) || NOBLES_RANK.isEmpty())
		{
			return 0;
		}
		
		if (!NOBLES_RANK.containsKey(objectId))
		{
			return 0;
		}
		
		NobleData noble = NOBLES.get(objectId);
		if ((noble == null) || (noble.OlympiadPoints == 0))
		{
			return 0;
		}
		
		// Hero point bonus
		int points = Hero.getInstance().isHero(objectId) || Hero.getInstance().isUnclaimedHero(objectId) ? Config.ALT_OLY_HERO_POINTS : 0;
		// Rank point bonus
		switch (NOBLES_RANK.get(objectId))
		{
			case 1:
			{
				points += Config.ALT_OLY_RANK1_POINTS;
				break;
			}
			case 2:
			{
				points += Config.ALT_OLY_RANK2_POINTS;
				break;
			}
			case 3:
			{
				points += Config.ALT_OLY_RANK3_POINTS;
				break;
			}
			case 4:
			{
				points += Config.ALT_OLY_RANK4_POINTS;
				break;
			}
			default:
			{
				points += Config.ALT_OLY_RANK5_POINTS;
				break;
			}
		}
		
		// Win/no win matches point bonus
		points += getCompetitionWon(objectId) > 0 ? 10 : 0;
		
		// This is a one time calculation.
		noble.OlympiadPoints = 0;
		
		return points;
	}
	
	public int getNoblePoints(Player player)
	{
		if (!NOBLES.containsKey(player.getObjectId()))
		{
			NobleData nobleData = new NobleData()
			{
				Class = player.getBaseClass(),
				CharacterName = player.getName(),
				OlympiadPoints = DEFAULT_POINTS
			};

			addNobleStats(player.getObjectId(), nobleData);
		}
		
		return NOBLES.get(player.getObjectId()).OlympiadPoints;
	}
	
	public int getLastNobleOlympiadPoints(int objId)
	{
		int result = 0;
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var record = ctx.OlympiadNoblesEom.Where(r => r.CharacterId == objId).Select(r => (int?)r.OlympiadPoints)
				.SingleOrDefault();

			if (record != null)
			{
				result = record.Value;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Could not load last olympiad points: " + e);
		}

		return result;
	}
	
	public int getCompetitionDone(int objId)
	{
		if (!NOBLES.containsKey(objId))
		{
			return 0;
		}
		return NOBLES.get(objId).CompetitionsDone;
	}
	
	public int getCompetitionWon(int objId)
	{
		if (!NOBLES.containsKey(objId))
		{
			return 0;
		}
		return NOBLES.get(objId).CompetitionsWon;
	}
	
	public int getCompetitionLost(int objId)
	{
		if (!NOBLES.containsKey(objId))
		{
			return 0;
		}
		return NOBLES.get(objId).CompetitionsLost;
	}
	
	/**
	 * Gets how many matches a noble character did in the week
	 * @param objId id of a noble character
	 * @return number of weekly competitions done
	 */
	public int getCompetitionDoneWeek(int objId)
	{
		if (!NOBLES.containsKey(objId))
		{
			return 0;
		}
		return NOBLES.get(objId).CompetitionsDoneWeek;
	}
	
	/**
	 * Number of remaining matches a noble character can join in the week
	 * @param objId id of a noble character
	 * @return difference between maximum allowed weekly matches and currently done weekly matches.
	 */
	public int getRemainingWeeklyMatches(int objId)
	{
		return Math.Max(Config.ALT_OLY_MAX_WEEKLY_MATCHES - getCompetitionDoneWeek(objId), 0);
	}
	
	protected void deleteNobles()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.OlympiadNobles.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Couldn't delete nobles from DB!");
		}
		NOBLES.clear();
	}
	
	/**
	 * @param charId the noble object Id.
	 * @param data the stats set data to add.
	 * @return the old stats set if the noble is already present, null otherwise.
	 */
	public static void addNobleStats(int charId, NobleData data)
	{
		NOBLES.put(charId, data);
	}
	
	public static Olympiad getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly Olympiad INSTANCE = new Olympiad();
	}
}

public class NobleData
{
	public CharacterClass Class { get; set; }
	public string CharacterName { get; set; } = string.Empty;
	public int OlympiadPoints { get; set; }
	public short CompetitionsDone { get; set; }
	public short CompetitionsWon { get; set; }
	public short CompetitionsLost { get; set; }
	public short CompetitionsDrawn { get; set; }
	public short CompetitionsDoneWeek { get; set; }
}
