using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Gigi, Mobius
 */
public class ServerRestartManager
{
	static readonly Logger LOGGER = LogManager.GetLogger(nameof(ServerRestartManager));

	private DateTime? nextRestartTime;

	protected ServerRestartManager()
	{
		try
		{
			DateTime currentTime = DateTime.UtcNow;
			DateTime? lastRestart = null;
			TimeSpan lastDelay = TimeSpan.Zero;

			foreach (TimeOnly scheduledTime in Config.SERVER_RESTART_SCHEDULE)
			{
				DateTime restartTime = new DateTime(DateOnly.FromDateTime(currentTime), scheduledTime);
				if (restartTime < currentTime)
					restartTime = restartTime.AddDays(1);

				if (!Config.SERVER_RESTART_DAYS.IsDefaultOrEmpty)
				{
					while (!Config.SERVER_RESTART_DAYS.Contains(restartTime.DayOfWeek))
						restartTime = restartTime.AddDays(1);
				}

				TimeSpan delay = restartTime - currentTime;
				if (lastDelay == TimeSpan.Zero)
				{
					lastDelay = delay;
					lastRestart = restartTime;
				}

				if (delay < lastDelay)
				{
					lastDelay = delay;
					lastRestart = restartTime;
				}
			}

			if (lastRestart != null)
			{
				nextRestartTime = lastRestart;

				ThreadPool.schedule(new ServerRestartTask(), lastDelay - TimeSpan.FromSeconds(Config.SERVER_RESTART_SCHEDULE_COUNTDOWN));
				LOGGER.Info("Scheduled server restart at " + lastRestart + ".");
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("The scheduled server restart config is not set properly, please correct it!");
            LOGGER.Error(e);
		}
	}

	public DateTime? getNextRestartTime()
	{
		return nextRestartTime;
	}

	class ServerRestartTask: Runnable
	{
		public void run()
		{
			Shutdown.getInstance().startShutdown(null, Config.SERVER_RESTART_SCHEDULE_COUNTDOWN, true);
		}
	}

	public static ServerRestartManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ServerRestartManager INSTANCE = new ServerRestartManager();
	}
}