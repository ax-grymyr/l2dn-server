using System.Net;
using System.Xml.Linq;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Data;

/**
 * @author BiggBoss
 */
public class BotReportTable
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(BotReportTable));

	public const int ATTACK_ACTION_BLOCK_ID = -1;
	public const int TRADE_ACTION_BLOCK_ID = -2;
	public const int PARTY_ACTION_BLOCK_ID = -3;
	public const int ACTION_BLOCK_ID = -4;
	public const int CHAT_BLOCK_ID = -5;

	private Map<int, DateTime> _ipRegistry = [];
	private Map<int, ReporterCharData> _charRegistry = [];
	private Map<int, ReportedCharData> _reports = [];
	private Map<int, PunishHolder> _punishments = [];

	protected BotReportTable()
	{
		if (Config.BOTREPORT_ENABLE)
		{
			try
			{
				// TODO: separate XML and SQL
				using FileStream stream = new FileStream("./Config/BotReportPunishments.xml", FileMode.Open,
					FileAccess.Read, FileShare.Read);

				XDocument document = XDocument.Load(stream);
				foreach (XElement element in document.Elements("list").Elements("punishment"))
				{
					try
					{
						int reportCount = element.GetAttributeValueAsInt32("neededReportCount");
						int skillId = element.GetAttributeValueAsInt32("skillId");
						int skillLevel = element.GetAttributeValueAsInt32("skillLevel");
						SystemMessageId? messageId =
							(SystemMessageId?)element.GetAttributeValueAsInt32OrNull("sysMessageId");

						addPunishment(reportCount, skillId, skillLevel, messageId);
					}
					catch (Exception e)
					{
						LOGGER.Warn("Problem with ./Config/BotReportPunishments.xml: " + e);
					}
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Could not load punishments from ./Config/BotReportPunishments.xml", e);
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
			DateTime lastResetTime = default;
			try
			{
				TimeOnly resetPoint = Config.BOTREPORT_RESETPOINT_HOUR;
				DateTime currentTime = DateTime.Now;
				DateTime calendar = new(currentTime.Year, currentTime.Month, currentTime.Day, resetPoint.Hour, resetPoint.Minute, 0);
				if (currentTime < calendar)
				{
					calendar = calendar.AddDays(-1);
				}

				lastResetTime = calendar;
			}
			catch (Exception exception)
			{
                LOGGER.Trace(exception);
			}

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var reports = ctx.BotReports;
			foreach (var report in reports)
			{
				int botId = report.BotId;
				int reporter = report.ReporterId;
				DateTime date = report.ReportTime;
				if (_reports.TryGetValue(botId, out ReportedCharData? reportedData))
				{
					reportedData.addReporter(reporter, date);
				}
				else
				{
					ReportedCharData rcd = new ReportedCharData();
					rcd.addReporter(reporter, date);
					_reports.put(report.BotId, rcd);
				}

				if (date > lastResetTime)
				{
					ReporterCharData? rcd = _charRegistry.get(reporter);
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

			LOGGER.Info(GetType().Name + ": Loaded " + _reports.Count + " bot reports");
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.BotReports.ExecuteDelete();
			foreach (var entrySet in _reports)
			{
				foreach (int reporterId in entrySet.Value.Reporters.Keys)
				{
					ctx.BotReports.Add(new()
					{
						BotId = entrySet.Key,
						ReporterId = reporterId,
						ReportTime = entrySet.Value.Reporters.get(reporterId)
					});
				}
			}

			ctx.SaveChanges();
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
		WorldObject? target = reporter.getTarget();
		if (target == null)
		{
			return false;
		}

		Creature bot = (Creature) target;
		if ((!bot.isPlayer() && !bot.isFakePlayer()) || (bot.isFakePlayer() && !((Npc) bot).getTemplate().isFakePlayerTalkable()) || target.ObjectId == reporter.ObjectId)
		{
			return false;
		}

		if (bot.isInsideZone(ZoneId.PEACE) || bot.isInsideZone(ZoneId.PVP))
		{
			reporter.sendPacket(SystemMessageId.YOU_CANNOT_REPORT_A_CHARACTER_WHO_IS_IN_A_PEACE_ZONE_OR_A_BATTLEGROUND);
			return false;
		}

        Player? botPlayer = bot.getActingPlayer();
		if (bot.isPlayer() && botPlayer != null && botPlayer.isInOlympiadMode())
		{
			reporter.sendPacket(SystemMessageId.THIS_CHARACTER_CANNOT_MAKE_A_REPORT_YOU_CANNOT_MAKE_A_REPORT_WHILE_LOCATED_INSIDE_A_PEACE_ZONE_OR_A_BATTLEGROUND_WHILE_YOU_ARE_AN_OPPOSING_CLAN_MEMBER_DURING_A_CLAN_WAR_OR_WHILE_PARTICIPATING_IN_THE_OLYMPIAD);
			return false;
		}

        Clan? botClan = bot.getClan();
        Clan? reporterClan = reporter.getClan();
		if (botClan != null && reporterClan != null && botClan.isAtWarWith(reporterClan))
		{
			reporter.sendPacket(SystemMessageId.YOU_CANNOT_REPORT_WHEN_A_CLAN_WAR_HAS_BEEN_DECLARED);
			return false;
		}

		if (bot.isPlayer() && botPlayer != null && botPlayer.getExp() == botPlayer.getStat().getStartingExp())
		{
			reporter.sendPacket(SystemMessageId.YOU_CANNOT_REPORT_A_CHARACTER_WHO_HAS_NOT_ACQUIRED_ANY_XP_AFTER_CONNECTING);
			return false;
		}

		ReportedCharData? rcd = _reports.get(bot.ObjectId);
		ReporterCharData? rcdRep = _charRegistry.get(reporter.ObjectId);
		int reporterId = reporter.ObjectId;

		SystemMessagePacket sm;
		lock (this)
		{
			if (_reports.ContainsKey(reporterId))
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

				if (!Config.BOTREPORT_ALLOW_REPORTS_FROM_SAME_CLAN_MEMBERS && reporterClan != null && rcd.reportedBySameClan(reporterClan))
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

				TimeSpan reuse = DateTime.UtcNow - rcdRep.getLastReporTime();
				if (reuse < TimeSpan.FromMilliseconds(Config.BOTREPORT_REPORT_DELAY))
				{
					sm = new SystemMessagePacket(SystemMessageId.YOU_CAN_MAKE_ANOTHER_REPORT_IN_S1_MIN_YOU_HAVE_S2_POINT_S_LEFT);
					sm.Params.addInt((int)reuse.TotalMinutes);
					sm.Params.addInt(rcdRep.getPointsLeft());
					reporter.sendPacket(sm);
					return false;
				}
			}

			DateTime curTime = DateTime.UtcNow;
			if (rcd == null)
			{
				rcd = new ReportedCharData();
				_reports.put(bot.ObjectId, rcd);
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

		sm = new SystemMessagePacket(SystemMessageId.C1_WAS_REPORTED_AS_A_BOT);
		sm.Params.addString(bot.getName());
		reporter.sendPacket(sm);

		sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_USED_A_REPORT_POINT_ON_C1_YOU_HAVE_S2_POINTS_REMAINING_ON_THIS_ACCOUNT);
		sm.Params.addString(bot.getName());
		sm.Params.addInt(rcdRep.getPointsLeft());
		reporter.sendPacket(sm);

		if (bot.isPlayer() && botPlayer != null)
		{
			handleReport(botPlayer, rcd);
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
			if (key < 0 && Math.Abs(key) <= rcd.getReportCount())
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
	private void punishBot(Player bot, PunishHolder? ph)
	{
		if (ph != null)
		{
			ph.Punish.applyEffects(bot, bot);
			if (ph.MessageId >= 0)
			{
				SystemMessageId? id = ph.MessageId;
				if (id != null)
				{
					bot.sendPacket(id.Value);
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
	void addPunishment(int neededReports, int skillId, int skillLevel, SystemMessageId? sysMsg)
	{
		Skill? sk = SkillData.getInstance().getSkill(skillId, skillLevel);
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
			foreach (ReporterCharData rcd in _charRegistry.Values)
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
			TimeOnly resetPoint = Config.BOTREPORT_RESETPOINT_HOUR;
			DateTime currentTime = DateTime.Now;
			DateTime calendar = new(currentTime.Year, currentTime.Month, currentTime.Day, resetPoint.Hour, resetPoint.Minute, 0);
			if (calendar < currentTime)
			{
				calendar = calendar.AddDays(1);
			}

			ThreadPool.schedule(new ResetPointTask(this), calendar - currentTime);
		}
		catch (Exception e)
		{
			ThreadPool.schedule(new ResetPointTask(this), TimeSpan.FromDays(1));
			LOGGER.Warn(GetType().Name + ": Could not properly schedule bot report points reset task. Scheduled in 24 hours.", e);
		}
	}

	public static BotReportTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	/**
	 * Returns a integer representative number from a connection
	 * @param player (The Player owner of the connection)
	 * @return int (hashed ip)
	 */
	private static int hashIp(Player player)
	{
		IPAddress? address = player.getClient()?.Connection?.GetRemoteAddress();
		if (address is not null)
			return IPAddressUtil.ConvertIP4AddressToInt(address);

		return 0;
	}

	/**
	 * Checks and return if the abstrat barrier specified by an integer (map key) has accomplished the waiting time
	 * @param map (a Map to study (Int = barrier, long = fully qualified unix time)
	 * @param objectId (an existent map key)
	 * @return true if the time has passed.
	 */
	private static bool timeHasPassed(Map<int, DateTime> map, int objectId)
	{
		if (map.TryGetValue(objectId, out DateTime value))
		{
			return DateTime.UtcNow - value > TimeSpan.FromMilliseconds(Config.BOTREPORT_REPORT_DELAY);
		}

		return true;
	}

	/**
	 * Represents the info about a reporter
	 */
	private class ReporterCharData
	{
		private DateTime _lastReport;
		private int _reportPoints;

		public ReporterCharData()
		{
			_reportPoints = 7;
		}

		public void registerReport(DateTime time)
		{
			_reportPoints -= 1;
			_lastReport = time;
		}

		public DateTime getLastReporTime()
		{
			return _lastReport;
		}

		public int getPointsLeft()
		{
			return _reportPoints;
		}

		public void setPoints(int points)
		{
			_reportPoints = points;
		}
	}

	/**
	 * Represents the info about a reported character
	 */
	private class ReportedCharData
	{
		private readonly Map<int, DateTime> _reporters;

		public ReportedCharData()
		{
			_reporters = new();
		}

		public Map<int, DateTime> Reporters => _reporters;

		public int getReportCount()
		{
			return _reporters.Count;
		}

		public bool alredyReportedBy(int objectId)
		{
			return _reporters.ContainsKey(objectId);
		}

		public void addReporter(int objectId, DateTime reportTime)
		{
			_reporters.put(objectId, reportTime);
		}

		public bool reportedBySameClan(Clan clan)
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

	private class PunishHolder(Skill sk, SystemMessageId? sysMsg)
	{
		public Skill Punish => sk;
		public SystemMessageId? MessageId => sysMsg;
	}

	private class ResetPointTask(BotReportTable table): Runnable
	{
		public void run()
		{
			table.resetPointsAndSchedule();
		}
	}

	private static class SingletonHolder
	{
		public static readonly BotReportTable INSTANCE = new BotReportTable();
	}
}