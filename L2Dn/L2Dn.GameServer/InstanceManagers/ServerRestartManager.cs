using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Gigi, Mobius
 */
public class ServerRestartManager
{
	static readonly Logger LOGGER = LogManager.GetLogger(nameof(ServerRestartManager));
	
	private String nextRestartTime = "unknown";
	
	protected ServerRestartManager()
	{
		try
		{
			Calendar currentTime = Calendar.getInstance();
			Calendar restartTime = Calendar.getInstance();
			Calendar lastRestart = null;
			long delay = 0;
			long lastDelay = 0;
			
			foreach (String scheduledTime in Config.SERVER_RESTART_SCHEDULE)
			{
				String[] splitTime = scheduledTime.Trim().Split(":");
				restartTime.set(Calendar.HOUR_OF_DAY, int.Parse(splitTime[0]));
				restartTime.set(Calendar.MINUTE, int.Parse(splitTime[1]));
				restartTime.set(Calendar.SECOND, 00);
				
				if (restartTime.getTimeInMillis() < currentTime.getTimeInMillis())
				{
					restartTime.add(Calendar.DAY_OF_WEEK, 1);
				}
				
				if (!Config.SERVER_RESTART_DAYS.isEmpty())
				{
					while (!Config.SERVER_RESTART_DAYS.Contains(restartTime.get(Calendar.DAY_OF_WEEK)))
					{
						restartTime.add(Calendar.DAY_OF_WEEK, 1);
					}
				}
				
				delay = restartTime.getTimeInMillis() - currentTime.getTimeInMillis();
				if (lastDelay == 0)
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
				if (Config.SERVER_RESTART_DAYS.isEmpty() || (Config.SERVER_RESTART_DAYS.Length == 7))
				{
					nextRestartTime = new SimpleDateFormat("HH:mm").format(lastRestart.getTime());
				}
				else
				{
					nextRestartTime = new SimpleDateFormat("MMMM d'" + getDayNumberSuffix(lastRestart.get(Calendar.DAY_OF_MONTH)) + "' HH:mm", Locale.UK).format(lastRestart.getTime());
				}
				ThreadPool.schedule(new ServerRestartTask(), lastDelay - (Config.SERVER_RESTART_SCHEDULE_COUNTDOWN * 1000));
				LOGGER.Info("Scheduled server restart at " + lastRestart.getTime() + ".");
			}
		}
		catch (Exception e)
		{
			LOGGER.Info("The scheduled server restart config is not set properly, please correct it!");
		}
	}
	
	private String getDayNumberSuffix(int day)
	{
		switch (day)
		{
			case 1:
			case 21:
			case 31:
			{
				return "st";
			}
			case 2:
			case 22:
			{
				return "nd";
			}
			case 3:
			case 23:
			{
				return "rd";
			}
			default:
			{
				return "th";
			}
		}
	}
	
	public String getNextRestartTime()
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