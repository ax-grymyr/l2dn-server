using System.Globalization;
using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author godson
 */
public class Olympiad: ListenersContainer
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Olympiad));
	protected static readonly Logger LOGGER_OLYMPIAD = LogManager.GetLogger("olympiad");
	
	private static readonly Map<int, StatSet> NOBLES = new();
	private static readonly Map<int, int> NOBLES_RANK = new();
	
	public const String OLYMPIAD_HTML_PATH = "data/html/olympiad/";
	private const String OLYMPIAD_LOAD_DATA = "SELECT current_cycle, period, olympiad_end, validation_end, next_weekly_change FROM olympiad_data WHERE id = 0";
	private const String OLYMPIAD_SAVE_DATA = "INSERT INTO olympiad_data (id, current_cycle, period, olympiad_end, validation_end, next_weekly_change) VALUES (0,?,?,?,?,?) ON DUPLICATE KEY UPDATE current_cycle=?, period=?, olympiad_end=?, validation_end=?, next_weekly_change=?";
	private const String OLYMPIAD_LOAD_NOBLES = "SELECT olympiad_nobles.charId, olympiad_nobles.class_id, characters.char_name, olympiad_nobles.olympiad_points, olympiad_nobles.competitions_done, olympiad_nobles.competitions_won, olympiad_nobles.competitions_lost, olympiad_nobles.competitions_drawn, olympiad_nobles.competitions_done_week FROM olympiad_nobles, characters WHERE characters.charId = olympiad_nobles.charId";
	private const String OLYMPIAD_SAVE_NOBLES = "INSERT INTO olympiad_nobles (`charId`,`class_id`,`olympiad_points`,`competitions_done`,`competitions_won`,`competitions_lost`,`competitions_drawn`, `competitions_done_week`) VALUES (?,?,?,?,?,?,?,?)";
	private const String OLYMPIAD_UPDATE_NOBLES = "UPDATE olympiad_nobles SET olympiad_points = ?, competitions_done = ?, competitions_won = ?, competitions_lost = ?, competitions_drawn = ?, competitions_done_week = ? WHERE charId = ?";
	private const String OLYMPIAD_GET_HEROS = "SELECT olympiad_nobles.charId, characters.char_name FROM olympiad_nobles, characters WHERE characters.charId = olympiad_nobles.charId AND olympiad_nobles.class_id in (?, ?) AND olympiad_nobles.competitions_done >= " + Config.ALT_OLY_MIN_MATCHES + " AND olympiad_nobles.competitions_won > 0 ORDER BY olympiad_nobles.olympiad_points DESC, olympiad_nobles.competitions_done DESC, olympiad_nobles.competitions_won DESC";
	private const String OLYMPIAD_GET_LEGEND = "SELECT olympiad_nobles.charId FROM olympiad_nobles WHERE olympiad_nobles.competitions_done >=" + Config.ALT_OLY_MIN_MATCHES + " ORDER BY olympiad_nobles.olympiad_points DESC LIMIT 1";
	private const String GET_ALL_CLASSIFIED_NOBLESS = "SELECT charId from olympiad_nobles_eom WHERE competitions_done >= " + Config.ALT_OLY_MIN_MATCHES + " ORDER BY olympiad_points DESC, competitions_done DESC, competitions_won DESC";
	private const String GET_EACH_CLASS_LEADER = "SELECT characters.char_name from olympiad_nobles_eom, characters WHERE characters.charId = olympiad_nobles_eom.charId AND olympiad_nobles_eom.class_id = ? AND olympiad_nobles_eom.competitions_done >= " + Config.ALT_OLY_MIN_MATCHES + " ORDER BY olympiad_nobles_eom.olympiad_points DESC, olympiad_nobles_eom.competitions_done DESC, olympiad_nobles_eom.competitions_won DESC LIMIT 10";
	private const String GET_EACH_CLASS_LEADER_CURRENT = "SELECT characters.char_name from olympiad_nobles, characters WHERE characters.charId = olympiad_nobles.charId AND olympiad_nobles.class_id = ? AND olympiad_nobles.competitions_done >= " + Config.ALT_OLY_MIN_MATCHES + " ORDER BY olympiad_nobles.olympiad_points DESC, olympiad_nobles.competitions_done DESC, olympiad_nobles.competitions_won DESC LIMIT 10";
	private const String GET_EACH_CLASS_LEADER_SOULHOUND = "SELECT characters.char_name from olympiad_nobles_eom, characters WHERE characters.charId = olympiad_nobles_eom.charId AND (olympiad_nobles_eom.class_id = ? OR olympiad_nobles_eom.class_id = 133) AND olympiad_nobles_eom.competitions_done >= " + Config.ALT_OLY_MIN_MATCHES + " ORDER BY olympiad_nobles_eom.olympiad_points DESC, olympiad_nobles_eom.competitions_done DESC, olympiad_nobles_eom.competitions_won DESC LIMIT 10";
	private const String GET_EACH_CLASS_LEADER_CURRENT_SOULHOUND = "SELECT characters.char_name from olympiad_nobles, characters WHERE characters.charId = olympiad_nobles.charId AND (olympiad_nobles.class_id = ? OR olympiad_nobles.class_id = 133) AND olympiad_nobles.competitions_done >= " + Config.ALT_OLY_MIN_MATCHES + " ORDER BY olympiad_nobles.olympiad_points DESC, olympiad_nobles.competitions_done DESC, olympiad_nobles.competitions_won DESC LIMIT 10";
	
	private const String REMOVE_UNCLAIMED_POINTS = "DELETE FROM character_variables WHERE charId=? AND var=?";
	private const String INSERT_UNCLAIMED_POINTS = "INSERT INTO character_variables (charId, var, val) VALUES (?, ?, ?)";
	public const String UNCLAIMED_OLYMPIAD_POINTS_VAR = "UNCLAIMED_OLYMPIAD_POINTS";
	
	private const String OLYMPIAD_DELETE_ALL = "TRUNCATE olympiad_nobles";
	private const String OLYMPIAD_MONTH_CLEAR = "TRUNCATE olympiad_nobles_eom";
	private const String OLYMPIAD_MONTH_CREATE = "INSERT INTO olympiad_nobles_eom SELECT charId, class_id, olympiad_points, competitions_done, competitions_won, competitions_lost, competitions_drawn FROM olympiad_nobles";
	
	private static readonly Set<int> HERO_IDS = CategoryData.getInstance().getCategoryByType(CategoryType.FOURTH_CLASS_GROUP);
	
	private static readonly int COMP_START = Config.ALT_OLY_START_TIME; // 6PM
	private static readonly int COMP_MIN = Config.ALT_OLY_MIN; // 00 mins
	private static readonly long COMP_PERIOD = Config.ALT_OLY_CPERIOD; // 6 hours
	protected static readonly long WEEKLY_PERIOD = Config.ALT_OLY_WPERIOD; // 1 week
	protected static readonly long VALIDATION_PERIOD = Config.ALT_OLY_VPERIOD; // 24 hours
	
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
	
	protected long _olympiadEnd;
	protected long _validationEnd;
	
	/**
	 * The current period of the olympiad.<br>
	 * <b>0 -</b> Competition period<br>
	 * <b>1 -</b> Validation Period
	 */
	protected int _period;
	protected long _nextWeeklyChange;
	protected int _currentCycle;
	private long _compEnd;
	private Calendar _compStart;
	protected static bool _inCompPeriod;
	protected static bool _compStarted = false;
	protected ScheduledFuture<?> _scheduledCompStart;
	protected ScheduledFuture<?> _scheduledCompEnd;
	protected ScheduledFuture<?> _scheduledOlympiadEnd;
	protected ScheduledFuture<?> _scheduledWeeklyTask;
	protected ScheduledFuture<?> _scheduledValdationTask;
	protected ScheduledFuture<?> _gameManager = null;
	protected ScheduledFuture<?> _gameAnnouncer = null;
	
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
	
	private void load()
	{
		NOBLES.clear();
		
		bool loaded = false;
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(OLYMPIAD_LOAD_DATA);
			ResultSet rset = statement.executeQuery();
			while (rset.next())
			{
				_currentCycle = rset.getInt("current_cycle");
				_period = rset.getInt("period");
				_olympiadEnd = rset.getLong("olympiad_end");
				_validationEnd = rset.getLong("validation_end");
				_nextWeeklyChange = rset.getLong("next_weekly_change");
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
			
			Properties olympiadProperties = new Properties();
			try
			{
				InputStream @is = new FileInputStream(Config.OLYMPIAD_CONFIG_FILE);
				olympiadProperties.load(@is);
			}
			catch (Exception e)
			{
				LOGGER.Error("Olympiad System: Error loading olympiad properties: " + e);
				return;
			}
			
			_currentCycle = int.Parse(olympiadProperties.getProperty("CurrentCycle", "1"));
			_period = int.Parse(olympiadProperties.getProperty("Period", "0"));
			_olympiadEnd = long.Parse(olympiadProperties.getProperty("OlympiadEnd", "0"));
			_validationEnd = long.Parse(olympiadProperties.getProperty("ValidationEnd", "0"));
			_nextWeeklyChange = long.Parse(olympiadProperties.getProperty("NextWeeklyChange", "0"));
		}
		
		long currentTime = System.currentTimeMillis();
		switch (_period)
		{
			case 0:
			{
				if ((_olympiadEnd == 0) || (_olympiadEnd < currentTime))
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
					_scheduledValdationTask = ThreadPool.schedule(new ValidationEndTask(), getMillisToValidationEnd());
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(OLYMPIAD_LOAD_NOBLES);
			ResultSet rset = statement.executeQuery();
			StatSet statData;
			while (rset.next())
			{
				statData = new StatSet();
				statData.set(CLASS_ID, rset.getInt(CLASS_ID));
				statData.set(CHAR_NAME, rset.getString(CHAR_NAME));
				statData.set(POINTS, rset.getInt(POINTS));
				statData.set(COMP_DONE, rset.getInt(COMP_DONE));
				statData.set(COMP_WON, rset.getInt(COMP_WON));
				statData.set(COMP_LOST, rset.getInt(COMP_LOST));
				statData.set(COMP_DRAWN, rset.getInt(COMP_DRAWN));
				statData.set(COMP_DONE_WEEK, rset.getInt(COMP_DONE_WEEK));
				statData.set("to_save", false);
				
				addNobleStats(rset.getInt(CHAR_ID), statData);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Error loading noblesse data from database: ", e);
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
			
			long milliToEnd;
			if (_period == 0)
			{
				milliToEnd = getMillisToOlympiadEnd();
			}
			else
			{
				milliToEnd = getMillisToValidationEnd();
			}
			
			double numSecs = (milliToEnd / 1000) % 60;
			double countDown = ((milliToEnd / 1000.0) - numSecs) / 60;
			int numMins = (int) Math.Floor(countDown % 60);
			countDown = (countDown - numMins) / 60;
			int numHours = (int) Math.Floor(countDown % 24);
			int numDays = (int) Math.Floor((countDown - numHours) / 24);
			
			LOGGER.Info("Olympiad System: " + numDays + " days, " + numHours + " hours and " + numMins + " mins until period ends.");
			
			if (_period == 0)
			{
				milliToEnd = getMillisToWeekChange();
				double numSecs2 = (milliToEnd / 1000) % 60;
				double countDown2 = ((milliToEnd / 1000.) - numSecs2) / 60;
				int numMins2 = (int) Math.Floor(countDown % 60);
				countDown2 = (countDown2 - numMins) / 60;
				int numHours2 = (int) Math.Floor(countDown2 % 24);
				int numDays2 = (int) Math.Floor((countDown2 - numHours) / 24);
				
				LOGGER.Info("Olympiad System: Next weekly change is in " + numDays2 + " days, " + numHours2 + " hours and " + numMins2 + " mins.");
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(GET_ALL_CLASSIFIED_NOBLESS);
			ResultSet rset = statement.executeQuery();
			int place = 1;
			while (rset.next())
			{
				tmpPlace.put(rset.getInt(CHAR_ID), place++);
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
						Connection con = DatabaseFactory.getConnection();
						PreparedStatement statement = con.prepareStatement(REMOVE_UNCLAIMED_POINTS);
						statement.setInt(1, noblesId);
						statement.setString(2, UNCLAIMED_OLYMPIAD_POINTS_VAR);
						statement.execute();
					}
					catch (Exception e)
					{
						LOGGER.Warn("Olympiad System: Couldn't remove unclaimed olympiad points from DB!");
					}
					// Add new value.
					try 
					{
						Connection con = DatabaseFactory.getConnection();
						PreparedStatement statement = con.prepareStatement(INSERT_UNCLAIMED_POINTS);
						statement.setInt(1, noblesId);
						statement.setString(2, UNCLAIMED_OLYMPIAD_POINTS_VAR);
						statement.setString(3, points.ToString(CultureInfo.InvariantCulture));
						statement.execute();
					}
					catch (Exception e)
					{
						LOGGER.Warn("Olympiad System: Couldn't store unclaimed olympiad points to DB!");
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
		
		_compStart = Calendar.getInstance();
		int currentDay = _compStart.get(Calendar.DAY_OF_WEEK);
		bool dayFound = false;
		int dayCounter = 0;
		for (int i = currentDay; i < 8; i++)
		{
			if (Config.ALT_OLY_COMPETITION_DAYS.contains(i))
			{
				dayFound = true;
				break;
			}
			dayCounter++;
		}
		if (!dayFound)
		{
			for (int i = 1; i < 8; i++)
			{
				if (Config.ALT_OLY_COMPETITION_DAYS.contains(i))
				{
					break;
				}
				dayCounter++;
			}
		}
		if (dayCounter > 0)
		{
			_compStart.add(Calendar.DAY_OF_MONTH, dayCounter);
		}
		_compStart.set(Calendar.HOUR_OF_DAY, COMP_START);
		_compStart.set(Calendar.MINUTE, COMP_MIN);
		_compEnd = _compStart.getTimeInMillis() + COMP_PERIOD;
		
		if (_scheduledOlympiadEnd != null)
		{
			_scheduledOlympiadEnd.cancel(true);
		}
		
		_scheduledOlympiadEnd = ThreadPool.schedule(new OlympiadEndTask(), getMillisToOlympiadEnd());
		
		updateCompStatus();
	}
	
	protected class OlympiadEndTask: Runnable
	{
		public void run()
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.ROUND_S1_OF_THE_OLYMPIAD_HAS_NOW_ENDED);
			sm.addInt(_currentCycle);
			
			Broadcast.toAllOnlinePlayers(sm);
			
			if (_scheduledWeeklyTask != null)
			{
				_scheduledWeeklyTask.cancel(true);
			}
			
			saveNobleData();
			
			_period = 1;
			List<StatSet> heroesToBe = sortHerosToBe();
			Hero.getInstance().resetData();
			Hero.getInstance().computeNewHeroes(heroesToBe);
			
			saveOlympiadStatus();
			updateMonthlyData();
			
			Calendar validationEnd = Calendar.getInstance();
			_validationEnd = validationEnd.getTimeInMillis() + VALIDATION_PERIOD;
			
			loadNoblesRank();
			_scheduledValdationTask = ThreadPool.schedule(new ValidationEndTask(), getMillisToValidationEnd());
		}
	}
	
	protected class ValidationEndTask: Runnable
	{
		public void run()
		{
			Broadcast.toAllOnlinePlayers("Olympiad Validation Period has ended");
			_period = 0;
			_currentCycle++;
			deleteNobles();
			setNewOlympiadEnd();
			init();
		}
	}
	
	protected static int getNobleCount()
	{
		return NOBLES.size();
	}
	
	public static StatSet getNobleStats(int playerId)
	{
		return NOBLES.get(playerId);
	}
	
	public static void removeNobleStats(int playerId)
	{
		NOBLES.remove(playerId);
		NOBLES_RANK.remove(playerId);
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps1 = con.prepareStatement("DELETE FROM olympiad_nobles WHERE charId=?");
			PreparedStatement ps2 = con.prepareStatement("DELETE FROM olympiad_nobles_eom WHERE charId=?");
			ps1.setInt(1, playerId);
			ps2.setInt(1, playerId);
			ps1.execute();
			ps2.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Error removing noblesse data from database: " + e);
		}
	}
	
	private void updateCompStatus()
	{
		// _compStarted = false;
		
		lock (this)
		{
			long milliToStart = getMillisToCompBegin();
			
			double numSecs = (milliToStart / 1000) % 60;
			double countDown = ((milliToStart / 1000.0) - numSecs) / 60;
			int numMins = (int) Math.Floor(countDown % 60);
			countDown = (countDown - numMins) / 60;
			int numHours = (int) Math.Floor(countDown % 24);
			int numDays = (int) Math.Floor((countDown - numHours) / 24);
			
			LOGGER.Info("Olympiad System: Competition Period Starts in " + numDays + " days, " + numHours + " hours and " + numMins + " mins.");
			
			LOGGER.Info("Olympiad System: Event starts/started: " + _compStart.getTime());
		}
		
		_scheduledCompStart = ThreadPool.schedule(() =>
		{
			if (isOlympiadEnd())
			{
				return;
			}
			
			_inCompPeriod = true;
			
			Broadcast.toAllOnlinePlayers(new SystemMessage(SystemMessageId.THE_OLYMPIAD_HAS_BEGAN));
			LOGGER.Info("Olympiad System: Olympiad Games have started.");
			LOGGER_OLYMPIAD.Info("Result,Player1,Player2,Player1 HP,Player2 HP,Player1 Damage,Player2 Damage,Points,Classed");
			
			_gameManager = ThreadPool.scheduleAtFixedRate(OlympiadGameManager.getInstance(), 30000, 30000);
			if (Config.ALT_OLY_ANNOUNCE_GAMES)
			{
				_gameAnnouncer = ThreadPool.scheduleAtFixedRate(new OlympiadAnnouncer(), 30000, 500);
			}
			
			long regEnd = getMillisToCompEnd() - 600000;
			if (regEnd > 0)
			{
				ThreadPool.schedule(() => Broadcast.toAllOnlinePlayers(new SystemMessage(SystemMessageId.THE_OLYMPIAD_REGISTRATION_PERIOD_HAS_ENDED)), regEnd);
			}
			
			_scheduledCompEnd = ThreadPool.schedule(() =>
			{
				if (isOlympiadEnd())
				{
					return;
				}
				_inCompPeriod = false;
				Broadcast.toAllOnlinePlayers(new SystemMessage(SystemMessageId.THE_OLYMPIAD_IS_OVER));
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
	
	private long getMillisToOlympiadEnd()
	{
		// if (_olympiadEnd > System.currentTimeMillis())
		return _olympiadEnd - System.currentTimeMillis();
		// return 10;
	}
	
	public void manualSelectHeroes()
	{
		if (_scheduledOlympiadEnd != null)
		{
			_scheduledOlympiadEnd.cancel(true);
		}
		
		_scheduledOlympiadEnd = ThreadPool.schedule(new OlympiadEndTask(), 0);
	}
	
	protected long getMillisToValidationEnd()
	{
		long currentTime = System.currentTimeMillis();
		if (_validationEnd > currentTime)
		{
			return _validationEnd - currentTime;
		}
		return 10;
	}
	
	public bool isOlympiadEnd()
	{
		return _period != 0;
	}
	
	protected void setNewOlympiadEnd()
	{
		SystemMessage sm = new SystemMessage(SystemMessageId.ROUND_S1_OF_THE_OLYMPIAD_GAMES_HAS_STARTED);
		sm.addInt(_currentCycle);
		Broadcast.toAllOnlinePlayers(sm);
		
		Calendar currentTime = Calendar.getInstance();
		currentTime.set(Calendar.AM_PM, Calendar.AM);
		currentTime.set(Calendar.HOUR_OF_DAY, 12);
		currentTime.set(Calendar.MINUTE, 0);
		currentTime.set(Calendar.SECOND, 0);
		
		Calendar nextChange = Calendar.getInstance();
		
		switch (Config.ALT_OLY_PERIOD)
		{
			case "DAY":
			{
				currentTime.add(Calendar.DAY_OF_MONTH, Config.ALT_OLY_PERIOD_MULTIPLIER);
				currentTime.add(Calendar.DAY_OF_MONTH, -1); // last day is for validation
				
				if (Config.ALT_OLY_PERIOD_MULTIPLIER >= 14)
				{
					_nextWeeklyChange = nextChange.getTimeInMillis() + WEEKLY_PERIOD;
				}
				else if (Config.ALT_OLY_PERIOD_MULTIPLIER >= 7)
				{
					_nextWeeklyChange = nextChange.getTimeInMillis() + (WEEKLY_PERIOD / 2);
				}
				else
				{
					LOGGER.warning("Invalid config value for Config.ALT_OLY_PERIOD_MULTIPLIER, must be >= 7");
				}
				break;
			}
			case "WEEK":
			{
				currentTime.add(Calendar.WEEK_OF_MONTH, Config.ALT_OLY_PERIOD_MULTIPLIER);
				currentTime.add(Calendar.DAY_OF_MONTH, -1); // last day is for validation
				
				if (Config.ALT_OLY_PERIOD_MULTIPLIER > 1)
				{
					_nextWeeklyChange = nextChange.getTimeInMillis() + WEEKLY_PERIOD;
				}
				else
				{
					_nextWeeklyChange = nextChange.getTimeInMillis() + (WEEKLY_PERIOD / 2);
				}
				break;
			}
			case "MONTH":
			{
				currentTime.add(Calendar.MONTH, Config.ALT_OLY_PERIOD_MULTIPLIER);
				currentTime.add(Calendar.DAY_OF_MONTH, -1); // last day is for validation
				
				_nextWeeklyChange = nextChange.getTimeInMillis() + WEEKLY_PERIOD;
				break;
			}
		}
		_olympiadEnd = currentTime.getTimeInMillis();
		
		scheduleWeeklyChange();
	}
	
	public bool inCompPeriod()
	{
		return _inCompPeriod;
	}
	
	private long getMillisToCompBegin()
	{
		long currentTime = System.currentTimeMillis();
		if ((_compStart.getTimeInMillis() < currentTime) && (_compEnd > currentTime))
		{
			return 10;
		}
		
		if (_compStart.getTimeInMillis() > currentTime)
		{
			return _compStart.getTimeInMillis() - currentTime;
		}
		
		return setNewCompBegin();
	}
	
	private long setNewCompBegin()
	{
		_compStart = Calendar.getInstance();
		
		int currentDay = _compStart.get(Calendar.DAY_OF_WEEK);
		_compStart.set(Calendar.HOUR_OF_DAY, COMP_START);
		_compStart.set(Calendar.MINUTE, COMP_MIN);
		
		// Today's competitions ended, start checking from next day.
		if (currentDay == _compStart.get(Calendar.DAY_OF_WEEK))
		{
			if (currentDay == Calendar.SATURDAY)
			{
				currentDay = Calendar.SUNDAY;
			}
			else
			{
				currentDay++;
			}
		}
		
		bool dayFound = false;
		int dayCounter = 0;
		for (int i = currentDay; i < 8; i++)
		{
			if (Config.ALT_OLY_COMPETITION_DAYS.contains(i))
			{
				dayFound = true;
				break;
			}
			dayCounter++;
		}
		if (!dayFound)
		{
			for (int i = 1; i < 8; i++)
			{
				if (Config.ALT_OLY_COMPETITION_DAYS.contains(i))
				{
					break;
				}
				dayCounter++;
			}
		}
		if (dayCounter > 0)
		{
			_compStart.add(Calendar.DAY_OF_MONTH, dayCounter);
		}
		_compStart.add(Calendar.HOUR_OF_DAY, 24);
		_compEnd = _compStart.getTimeInMillis() + COMP_PERIOD;
		
		LOGGER.Info("Olympiad System: New Schedule @ " + _compStart.getTime());
		
		return _compStart.getTimeInMillis() - System.currentTimeMillis();
	}
	
	protected long getMillisToCompEnd()
	{
		// if (_compEnd > System.currentTimeMillis())
		return _compEnd - System.currentTimeMillis();
		// return 10;
	}
	
	private long getMillisToWeekChange()
	{
		long currentTime = System.currentTimeMillis();
		if (_nextWeeklyChange > currentTime)
		{
			return _nextWeeklyChange - currentTime;
		}
		return 10;
	}
	
	private void scheduleWeeklyChange()
	{
		_scheduledWeeklyTask = ThreadPool.scheduleAtFixedRate(() ->
		{
			addWeeklyPoints();
			LOGGER.Info("Olympiad System: Added weekly points to nobles");
			resetWeeklyMatches();
			LOGGER.Info("Olympiad System: Reset weekly matches to nobles");
			
			_nextWeeklyChange = System.currentTimeMillis() + WEEKLY_PERIOD;
		}, getMillisToWeekChange(), WEEKLY_PERIOD);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected void addWeeklyPoints()
	{
		if (_period == 1)
		{
			return;
		}
		
		int currentPoints;
		foreach (StatSet nobleInfo in NOBLES.values())
		{
			currentPoints = nobleInfo.getInt(POINTS);
			currentPoints += WEEKLY_POINTS;
			nobleInfo.set(POINTS, currentPoints);
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
		
		foreach (StatSet nobleInfo in NOBLES.values())
		{
			nobleInfo.set(COMP_DONE_WEEK, 0);
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
			Connection con = DatabaseFactory.getConnection();
			foreach (var entry in NOBLES)
			{
				StatSet nobleInfo = entry.Value;
				
				if (nobleInfo == null)
				{
					continue;
				}
				
				int charId = entry.Key;
				int classId = nobleInfo.getInt(CLASS_ID);
				int points = nobleInfo.getInt(POINTS);
				int compDone = nobleInfo.getInt(COMP_DONE);
				int compWon = nobleInfo.getInt(COMP_WON);
				int compLost = nobleInfo.getInt(COMP_LOST);
				int compDrawn = nobleInfo.getInt(COMP_DRAWN);
				int compDoneWeek = nobleInfo.getInt(COMP_DONE_WEEK);
				bool toSave = nobleInfo.getBoolean("to_save");
				
				{
					PreparedStatement statement =
						con.prepareStatement(toSave ? OLYMPIAD_SAVE_NOBLES : OLYMPIAD_UPDATE_NOBLES);
					if (toSave)
					{
						statement.setInt(1, charId);
						statement.setInt(2, classId);
						statement.setInt(3, points);
						statement.setInt(4, compDone);
						statement.setInt(5, compWon);
						statement.setInt(6, compLost);
						statement.setInt(7, compDrawn);
						statement.setInt(8, compDoneWeek);
						
						nobleInfo.set("to_save", false);
					}
					else
					{
						statement.setInt(1, points);
						statement.setInt(2, compDone);
						statement.setInt(3, compWon);
						statement.setInt(4, compLost);
						statement.setInt(5, compDrawn);
						statement.setInt(6, compDoneWeek);
						statement.setInt(7, charId);
					}
					statement.execute();
				}
			}
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(OLYMPIAD_SAVE_DATA);
			statement.setInt(1, _currentCycle);
			statement.setInt(2, _period);
			statement.setLong(3, _olympiadEnd);
			statement.setLong(4, _validationEnd);
			statement.setLong(5, _nextWeeklyChange);
			statement.setInt(6, _currentCycle);
			statement.setInt(7, _period);
			statement.setLong(8, _olympiadEnd);
			statement.setLong(9, _validationEnd);
			statement.setLong(10, _nextWeeklyChange);
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Olympiad System: Failed to save olympiad data to database: " + e);
		}
		//@formatter:off
		/*
		Properties OlympiadProperties = new Properties();
		try (FileOutputStream fos = new FileOutputStream(new File("./" + OLYMPIAD_DATA_FILE)))
		{
			OlympiadProperties.setProperty("CurrentCycle", String.valueOf(_currentCycle));
			OlympiadProperties.setProperty("Period", String.valueOf(_period));
			OlympiadProperties.setProperty("OlympiadEnd", String.valueOf(_olympiadEnd));
			OlympiadProperties.setProperty("ValdationEnd", String.valueOf(_validationEnd));
			OlympiadProperties.setProperty("NextWeeklyChange", String.valueOf(_nextWeeklyChange));
			OlympiadProperties.store(fos, "Olympiad Properties");
		}
		catch (Exception e)
		{
			LOGGER.warning("Olympiad System: Unable to save olympiad properties to file: ", e);
		}
		*/
		//@formatter:on
	}
	
	protected void updateMonthlyData()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps1 = con.prepareStatement(OLYMPIAD_MONTH_CLEAR);
			PreparedStatement ps2 = con.prepareStatement(OLYMPIAD_MONTH_CREATE);
			ps1.execute();
			ps2.execute();
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
		StatSet nobleInfo;
		foreach (var entry in NOBLES)
		{
			nobleInfo = entry.Value;
			if (nobleInfo == null)
			{
				continue;
			}
			
			int charId = entry.Key;
			int classId = nobleInfo.getInt(CLASS_ID);
			String charName = nobleInfo.getString(CHAR_NAME);
			int points = nobleInfo.getInt(POINTS);
			int compDone = nobleInfo.getInt(COMP_DONE);
			
			LOGGER_OLYMPIAD.Info(charName + "," + charId + "," + classId + "," + compDone + "," + points);
		}
		
		List<StatSet> heroesToBe = new();
		
		int legendId = 0;
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(OLYMPIAD_GET_LEGEND);
			{
				ResultSet rset = statement.executeQuery();
				if (rset.next())
				{
					legendId = rset.getInt("charId");
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Couldnt load legend from DB" + e);
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(OLYMPIAD_GET_HEROS);
			StatSet hero;
			foreach (int element in HERO_IDS.Keys)
			{
				// Classic can have 2nd and 3rd class competitors, but only 1 hero
				ClassId parent = ClassListData.getInstance().getClass(element).getParentClassId();
				statement.setInt(1, element);
				statement.setInt(2, parent.getId());

				{
					ResultSet rset = statement.executeQuery();
					if (rset.next())
					{
						hero = new StatSet();
						int charId = rset.getInt(CHAR_ID);
						hero.set(CLASS_ID, element); // save the 3rd class title
						hero.set(CHAR_ID, charId);
						hero.set(CHAR_NAME, rset.getString(CHAR_NAME));
						hero.set("LEGEND", charId == legendId ? 1 : 0);
						
						LOGGER_OLYMPIAD.Info("Hero " + hero.getString(CHAR_NAME) + "," + charId + "," + hero.getInt(CLASS_ID));
						heroesToBe.add(hero);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Couldnt load heros from DB");
		}
		
		return heroesToBe;
	}
	
	public List<String> getClassLeaderBoard(int classId)
	{
		List<String> names = new();
		String query = Config.ALT_OLY_SHOW_MONTHLY_WINNERS ? ((classId == 132) ? GET_EACH_CLASS_LEADER_SOULHOUND : GET_EACH_CLASS_LEADER) : ((classId == 132) ? GET_EACH_CLASS_LEADER_CURRENT_SOULHOUND : GET_EACH_CLASS_LEADER_CURRENT);
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(query);
			ps.setInt(1, classId);

			{
				ResultSet rset = ps.executeQuery();
				while (rset.next())
				{
					names.add(rset.getString(CHAR_NAME));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Couldn't load olympiad leaders from DB!");
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
		
		StatSet noble = NOBLES.get(objectId);
		if ((noble == null) || (noble.getInt(POINTS) == 0))
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
			}
		}
		
		// Win/no win matches point bonus
		points += getCompetitionWon(objectId) > 0 ? 10 : 0;
		
		// This is a one time calculation.
		noble.set(POINTS, 0);
		
		return points;
	}
	
	public int getNoblePoints(Player player)
	{
		if (!NOBLES.containsKey(player.getObjectId()))
		{
			StatSet statDat = new StatSet();
			statDat.set(CLASS_ID, player.getBaseClass());
			statDat.set(CHAR_NAME, player.getName());
			statDat.set(POINTS, DEFAULT_POINTS);
			statDat.set(COMP_DONE, 0);
			statDat.set(COMP_WON, 0);
			statDat.set(COMP_LOST, 0);
			statDat.set(COMP_DRAWN, 0);
			statDat.set(COMP_DONE_WEEK, 0);
			statDat.set("to_save", true);
			addNobleStats(player.getObjectId(), statDat);
		}
		return NOBLES.get(player.getObjectId()).getInt(POINTS);
	}
	
	public int getLastNobleOlympiadPoints(int objId)
	{
		int result = 0;
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps =
				con.prepareStatement("SELECT olympiad_points FROM olympiad_nobles_eom WHERE charId = ?");
			ps.setInt(1, objId);

			{
				ResultSet rs = ps.executeQuery();
				if (rs.first())
				{
					result = rs.getInt(1);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Olympiad System: Could not load last olympiad points: " + e);
		}
		return result;
	}
	
	public int getCompetitionDone(int objId)
	{
		if (!NOBLES.containsKey(objId))
		{
			return 0;
		}
		return NOBLES.get(objId).getInt(COMP_DONE);
	}
	
	public int getCompetitionWon(int objId)
	{
		if (!NOBLES.containsKey(objId))
		{
			return 0;
		}
		return NOBLES.get(objId).getInt(COMP_WON);
	}
	
	public int getCompetitionLost(int objId)
	{
		if (!NOBLES.containsKey(objId))
		{
			return 0;
		}
		return NOBLES.get(objId).getInt(COMP_LOST);
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
		return NOBLES.get(objId).getInt(COMP_DONE_WEEK);
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(OLYMPIAD_DELETE_ALL);
			statement.execute();
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
	public static StatSet addNobleStats(int charId, StatSet data)
	{
		return NOBLES.put(charId, data);
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