using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Data;

/**
 * @author BiggBoss
 */
public class BotReportTable
{
	// Zoey76: TODO: Split XML parsing from SQL operations, use IGameXmlReader instead of SAXParser.
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(BotReportTable));
	
	private const int COLUMN_BOT_ID = 1;
	private const int COLUMN_REPORTER_ID = 2;
	private const int COLUMN_REPORT_TIME = 3;
	
	public const int ATTACK_ACTION_BLOCK_ID = -1;
	public const int TRADE_ACTION_BLOCK_ID = -2;
	public const int PARTY_ACTION_BLOCK_ID = -3;
	public const int ACTION_BLOCK_ID = -4;
	public const int CHAT_BLOCK_ID = -5;
	
	private const string SQL_LOAD_REPORTED_CHAR_DATA = "SELECT * FROM bot_reported_char_data";
	private const string SQL_INSERT_REPORTED_CHAR_DATA = "INSERT INTO bot_reported_char_data VALUES (?,?,?)";
	private const string SQL_CLEAR_REPORTED_CHAR_DATA = "DELETE FROM bot_reported_char_data";
	
	private Map<int, long> _ipRegistry;
	private Map<int, ReporterCharData> _charRegistry;
	private Map<int, ReportedCharData> _reports;
	private Map<int, PunishHolder> _punishments;
	
	protected BotReportTable()
	{
		if (Config.BOTREPORT_ENABLE)
		{
			_ipRegistry = new();
			_charRegistry = new();
			_reports = new();
			_punishments = new();
			
			try
			{
				File punishments = new File("./config/BotReportPunishments.xml");
				if (!punishments.exists())
				{
					throw new FileNotFoundException(punishments.getName());
				}
				
				SAXParser parser = SAXParserFactory.newInstance().newSAXParser();
				parser.parse(punishments, new PunishmentsLoader());
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Could not load punishments from /config/BotReportPunishments.xml", e);
			}
			
			loadReportedCharData();
			scheduleResetPointTask();
		}
	}
	
	/**
	 * Loads all reports of each reported bot into this cache class.<br>
	 * Warning: Heavy method, used only on server start up
	 */
	private void loadReportedCharData()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			Statement st = con.createStatement();
			ResultSet rset = st.executeQuery(SQL_LOAD_REPORTED_CHAR_DATA);
			long lastResetTime = 0;
			try
			{
				int hour = int.Parse(Config.BOTREPORT_RESETPOINT_HOUR[0]);
				int minute = int.Parse(Config.BOTREPORT_RESETPOINT_HOUR[1]);
				long currentTime = System.currentTimeMillis();
				Calendar calendar = Calendar.getInstance();
				calendar.set(Calendar.HOUR_OF_DAY, hour);
				calendar.set(Calendar.MINUTE, minute);
				if (currentTime < calendar.getTimeInMillis())
				{
					calendar.set(Calendar.DAY_OF_YEAR, calendar.get(Calendar.DAY_OF_YEAR) - 1);
				}
				lastResetTime = calendar.getTimeInMillis();
			}
			catch (Exception e)
			{
				// Ignore.
			}
			
			while (rset.next())
			{
				int botId = rset.getInt(COLUMN_BOT_ID);
				int reporter = rset.getInt(COLUMN_REPORTER_ID);
				long date = rset.getLong(COLUMN_REPORT_TIME);
				if (_reports.containsKey(botId))
				{
					_reports.get(botId).addReporter(reporter, date);
				}
				else
				{
					ReportedCharData rcd = new ReportedCharData();
					rcd.addReporter(reporter, date);
					_reports.put(rset.getInt(COLUMN_BOT_ID), rcd);
				}
				
				if (date > lastResetTime)
				{
					ReporterCharData rcd = _charRegistry.get(reporter);
					if (rcd != null)
					{
						rcd.setPoints(rcd.getPointsLeft() - 1);
					}
					else
					{
						rcd = new ReporterCharData();
						rcd.setPoints(6);
						_charRegistry.put(reporter, rcd);
					}
				}
			}
			
			LOGGER.Info(GetType().Name + ": Loaded " + _reports.size() + " bot reports");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not load reported char data!", e);
		}
	}
	
	/**
	 * Save all reports for each reported bot down to database.<br>
	 * Warning: Heavy method, used only at server shutdown
	 */
	public void saveReportedCharData()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			Statement st = con.createStatement();
			PreparedStatement ps = con.prepareStatement(SQL_INSERT_REPORTED_CHAR_DATA);
			st.execute(SQL_CLEAR_REPORTED_CHAR_DATA);
			
			foreach (var entrySet in _reports)
			{
				foreach (int reporterId in entrySet.Value._reporters.Keys)
				{
					ps.setInt(1, entrySet.Key);
					ps.setInt(2, reporterId);
					ps.setLong(3, entrySet.Value._reporters.get(reporterId));
					ps.execute();
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Could not update reported char data in database!", e);
		}
	}
	
	/**
	 * Attempts to perform a bot report. R/W to ip and char id registry is synchronized. Triggers bot punish management
	 * @param reporter (Player who issued the report)
	 * @return True, if the report was registered, False otherwise
	 */
	public bool reportBot(Player reporter)
	{
		WorldObject target = reporter.getTarget();
		if (target == null)
		{
			return false;
		}
		
		Creature bot = ((Creature) target);
		if ((!bot.isPlayer() && !bot.isFakePlayer()) || (bot.isFakePlayer() && !((Npc) bot).getTemplate().isFakePlayerTalkable()) || (target.getObjectId() == reporter.getObjectId()))
		{
			return false;
		}
		
		if (bot.isInsideZone(ZoneId.PEACE) || bot.isInsideZone(ZoneId.PVP))
		{
			reporter.sendPacket(SystemMessageId.YOU_CANNOT_REPORT_A_CHARACTER_WHO_IS_IN_A_PEACE_ZONE_OR_A_BATTLEGROUND);
			return false;
		}
		
		if (bot.isPlayer() && bot.getActingPlayer().isInOlympiadMode())
		{
			reporter.sendPacket(SystemMessageId.THIS_CHARACTER_CANNOT_MAKE_A_REPORT_YOU_CANNOT_MAKE_A_REPORT_WHILE_LOCATED_INSIDE_A_PEACE_ZONE_OR_A_BATTLEGROUND_WHILE_YOU_ARE_AN_OPPOSING_CLAN_MEMBER_DURING_A_CLAN_WAR_OR_WHILE_PARTICIPATING_IN_THE_OLYMPIAD);
			return false;
		}
		
		if ((bot.getClan() != null) && (reporter.getClan() != null) && bot.getClan().isAtWarWith(reporter.getClan()))
		{
			reporter.sendPacket(SystemMessageId.YOU_CANNOT_REPORT_WHEN_A_CLAN_WAR_HAS_BEEN_DECLARED);
			return false;
		}
		
		if (bot.isPlayer() && (bot.getActingPlayer().getExp() == bot.getActingPlayer().getStat().getStartingExp()))
		{
			reporter.sendPacket(SystemMessageId.YOU_CANNOT_REPORT_A_CHARACTER_WHO_HAS_NOT_ACQUIRED_ANY_XP_AFTER_CONNECTING);
			return false;
		}
		
		ReportedCharData rcd = _reports.get(bot.getObjectId());
		ReporterCharData rcdRep = _charRegistry.get(reporter.getObjectId());
		int reporterId = reporter.getObjectId();
		
		lock (this)
		{
			if (_reports.containsKey(reporterId))
			{
				reporter.sendPacket(SystemMessageId.YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_AND_CANNOT_REPORT_OTHER_USERS);
				return false;
			}
			
			int ip = hashIp(reporter);
			if (!timeHasPassed(_ipRegistry, ip))
			{
				reporter.sendPacket(SystemMessageId.THIS_CHARACTER_CANNOT_MAKE_A_REPORT_THE_TARGET_HAS_ALREADY_BEEN_REPORTED_BY_EITHER_YOUR_CLAN_OR_HAS_ALREADY_BEEN_REPORTED_FROM_YOUR_CURRENT_IP);
				return false;
			}
			
			if (rcd != null)
			{
				if (rcd.alredyReportedBy(reporterId))
				{
					reporter.sendPacket(SystemMessageId.YOU_CANNOT_REPORT_THIS_PERSON_AGAIN_AT_THIS_TIME);
					return false;
				}
				
				if (!Config.BOTREPORT_ALLOW_REPORTS_FROM_SAME_CLAN_MEMBERS && rcd.reportedBySameClan(reporter.getClan()))
				{
					reporter.sendPacket(SystemMessageId.THIS_CHARACTER_CANNOT_MAKE_A_REPORT_THE_TARGET_HAS_ALREADY_BEEN_REPORTED_BY_EITHER_YOUR_CLAN_OR_HAS_ALREADY_BEEN_REPORTED_FROM_YOUR_CURRENT_IP);
					return false;
				}
			}
			
			if (rcdRep != null)
			{
				if (rcdRep.getPointsLeft() == 0)
				{
					reporter.sendPacket(SystemMessageId.YOU_HAVE_USED_ALL_AVAILABLE_POINTS_POINTS_ARE_RESET_EVERYDAY_AT_NOON);
					return false;
				}
				
				long reuse = (System.currentTimeMillis() - rcdRep.getLastReporTime());
				if (reuse < Config.BOTREPORT_REPORT_DELAY)
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.YOU_CAN_MAKE_ANOTHER_REPORT_IN_S1_MIN_YOU_HAVE_S2_POINT_S_LEFT);
					sm.addInt((int) (reuse / 60000));
					sm.addInt(rcdRep.getPointsLeft());
					reporter.sendPacket(sm);
					return false;
				}
			}
			
			long curTime = System.currentTimeMillis();
			if (rcd == null)
			{
				rcd = new ReportedCharData();
				_reports.put(bot.getObjectId(), rcd);
			}
			rcd.addReporter(reporterId, curTime);
			if (rcdRep == null)
			{
				rcdRep = new ReporterCharData();
			}
			rcdRep.registerReport(curTime);
			
			_ipRegistry.put(ip, curTime);
			_charRegistry.put(reporterId, rcdRep);
		}
		
		SystemMessage sm = new SystemMessage(SystemMessageId.C1_WAS_REPORTED_AS_A_BOT);
		sm.addString(bot.getName());
		reporter.sendPacket(sm);
		
		sm = new SystemMessage(SystemMessageId.YOU_HAVE_USED_A_REPORT_POINT_ON_C1_YOU_HAVE_S2_POINTS_REMAINING_ON_THIS_ACCOUNT);
		sm.addString(bot.getName());
		sm.addInt(rcdRep.getPointsLeft());
		reporter.sendPacket(sm);
		
		if (bot.isPlayer())
		{
			handleReport(bot.getActingPlayer(), rcd);
		}
		
		return true;
	}
	
	/**
	 * Find the punishs to apply to the given bot and triggers the punish method.
	 * @param bot (Player to be punished)
	 * @param rcd (RepotedCharData linked to this bot)
	 */
	private void handleReport(Player bot, ReportedCharData rcd)
	{
		// Report count punishment
		punishBot(bot, _punishments.get(rcd.getReportCount()));
		
		// Range punishments
		foreach (var entry in _punishments)
		{
			int key = entry.Key;
			if ((key < 0) && (Math.Abs(key) <= rcd.getReportCount()))
			{
				punishBot(bot, entry.Value);
			}
		}
	}
	
	/**
	 * Applies the given punish to the bot if the action is secure
	 * @param bot (Player to punish)
	 * @param ph (PunishHolder containing the debuff and a possible system message to send)
	 */
	private void punishBot(Player bot, PunishHolder ph)
	{
		if (ph != null)
		{
			ph._punish.applyEffects(bot, bot);
			if (ph._systemMessageId > -1)
			{
				SystemMessageId id = SystemMessageId.getSystemMessageId(ph._systemMessageId);
				if (id != null)
				{
					bot.sendPacket(id);
				}
			}
		}
	}
	
	/**
	 * Adds a debuff punishment into the punishments record. If skill does not exist, will log it and return
	 * @param neededReports (report count to trigger this debuff)
	 * @param skillId
	 * @param skillLevel
	 * @param sysMsg (id of a system message to send when applying the punish)
	 */
	void addPunishment(int neededReports, int skillId, int skillLevel, int sysMsg)
	{
		Skill sk = SkillData.getInstance().getSkill(skillId, skillLevel);
		if (sk != null)
		{
			_punishments.put(neededReports, new PunishHolder(sk, sysMsg));
		}
		else
		{
			LOGGER.Warn(GetType().Name + ": Could not add punishment for " + neededReports + " report(s): Skill " + skillId + "-" + skillLevel + " does not exist!");
		}
	}
	
	void resetPointsAndSchedule()
	{
		lock (_charRegistry)
		{
			foreach (ReporterCharData rcd in _charRegistry.values())
			{
				rcd.setPoints(7);
			}
		}
		
		scheduleResetPointTask();
	}
	
	private void scheduleResetPointTask()
	{
		try
		{
			int hour = int.Parse(Config.BOTREPORT_RESETPOINT_HOUR[0]);
			int minute = int.Parse(Config.BOTREPORT_RESETPOINT_HOUR[1]);
			long currentTime = System.currentTimeMillis();
			Calendar calendar = Calendar.getInstance();
			calendar.set(Calendar.HOUR_OF_DAY, hour);
			calendar.set(Calendar.MINUTE, minute);
			if (calendar.getTimeInMillis() < currentTime)
			{
				calendar.add(Calendar.DAY_OF_YEAR, 1);
			}
			ThreadPool.schedule(new ResetPointTask(), calendar.getTimeInMillis() - currentTime);
		}
		catch (Exception e)
		{
			ThreadPool.schedule(new ResetPointTask(), 24 * 3600 * 1000);
			LOGGER.Warn(GetType().Name + ": Could not properly schedule bot report points reset task. Scheduled in 24 hours.", e);
		}
	}
	
	public static BotReportTable getInstance()
	{
		return Data.SingletonHolder.INSTANCE;
	}
	
	/**
	 * Returns a integer representative number from a connection
	 * @param player (The Player owner of the connection)
	 * @return int (hashed ip)
	 */
	private static int hashIp(Player player)
	{
		String con = player.getClient().getIp();
		String[] rawByte = con.Split("\\.");
		int[] rawIp = new int[4];
		for (int i = 0; i < 4; i++)
		{
			rawIp[i] = int.Parse(rawByte[i]);
		}
		return rawIp[0] | (rawIp[1] << 8) | (rawIp[2] << 16) | (rawIp[3] << 24);
	}
	
	/**
	 * Checks and return if the abstrat barrier specified by an integer (map key) has accomplished the waiting time
	 * @param map (a Map to study (Int = barrier, long = fully qualified unix time)
	 * @param objectId (an existent map key)
	 * @return true if the time has passed.
	 */
	private static bool timeHasPassed(Map<int, long> map, int objectId)
	{
		if (map.containsKey(objectId))
		{
			return (System.currentTimeMillis() - map.get(objectId)) > Config.BOTREPORT_REPORT_DELAY;
		}
		return true;
	}
	
	/**
	 * Represents the info about a reporter
	 */
	private class ReporterCharData
	{
		private long _lastReport;
		private byte _reportPoints;
		
		ReporterCharData()
		{
			_reportPoints = 7;
			_lastReport = 0;
		}
		
		void registerReport(long time)
		{
			_reportPoints -= 1;
			_lastReport = time;
		}
		
		long getLastReporTime()
		{
			return _lastReport;
		}
		
		byte getPointsLeft()
		{
			return _reportPoints;
		}
		
		void setPoints(int points)
		{
			_reportPoints = (byte) points;
		}
	}
	
	/**
	 * Represents the info about a reported character
	 */
	private class ReportedCharData
	{
		Map<int, long> _reporters;
		
		ReportedCharData()
		{
			_reporters = new();
		}
		
		int getReportCount()
		{
			return _reporters.size();
		}
		
		bool alredyReportedBy(int objectId)
		{
			return _reporters.containsKey(objectId);
		}
		
		void addReporter(int objectId, long reportTime)
		{
			_reporters.put(objectId, reportTime);
		}
		
		bool reportedBySameClan(Clan clan)
		{
			if (clan == null)
			{
				return false;
			}
			
			foreach (int reporterId in _reporters.Keys)
			{
				if (clan.isMember(reporterId))
				{
					return true;
				}
			}
			
			return false;
		}
	}
	
	/**
	 * SAX loader to parse /config/BotReportPunishments.xml file
	 */
	private class PunishmentsLoader: DefaultHandler
	{
		PunishmentsLoader()
		{
		}
		
		public void startElement(String uri, String localName, String qName, Attributes attr)
		{
			if (qName.equals("punishment"))
			{
				int reportCount = -1;
				int skillId = -1;
				int skillLevel = 1;
				int sysMessage = -1;
				try
				{
					reportCount = int.Parse(attr.getValue("neededReportCount"));
					skillId = int.Parse(attr.getValue("skillId"));
					String level = attr.getValue("skillLevel");
					String systemMessageId = attr.getValue("sysMessageId");
					if (level != null)
					{
						skillLevel = int.Parse(level);
					}
					
					if (systemMessageId != null)
					{
						sysMessage = int.Parse(systemMessageId);
					}
				}
				catch (Exception e)
				{
					LOGGER.Warn("Problem with BotReportTable: " + e);
				}
				
				addPunishment(reportCount, skillId, skillLevel, sysMessage);
			}
		}
	}
	
	private class PunishHolder
	{
		Skill _punish;
		int _systemMessageId;
		
		public PunishHolder(Skill sk, int sysMsg)
		{
			_punish = sk;
			_systemMessageId = sysMsg;
		}
	}
	
	private class ResetPointTask: Runnable
	{
		public ResetPointTask()
		{
		}
		
		public void run()
		{
			resetPointsAndSchedule();
		}
	}
	
	private static class SingletonHolder
	{
		public static readonly BotReportTable INSTANCE = new BotReportTable();
	}
}