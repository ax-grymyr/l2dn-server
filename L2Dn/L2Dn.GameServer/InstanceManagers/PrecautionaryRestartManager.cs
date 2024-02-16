using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class PrecautionaryRestartManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PrecautionaryRestartManager));

	private const string SYSTEM_CPU_LOAD_VAR = "SystemCpuLoad";
	private const string PROCESS_CPU_LOAD_VAR = "ProcessCpuLoad";

	private static bool _restarting = false;

	protected PrecautionaryRestartManager()
	{
		ThreadPool.scheduleAtFixedRate(() =>
		{
			if (_restarting)
			{
				return;
			}

			if (Config.PRECAUTIONARY_RESTART_CPU &&
			    (getCpuLoad(SYSTEM_CPU_LOAD_VAR) > Config.PRECAUTIONARY_RESTART_PERCENTAGE))
			{
				if (serverBizzy())
				{
					return;
				}

				LOGGER.Info("PrecautionaryRestartManager: CPU usage over " + Config.PRECAUTIONARY_RESTART_PERCENTAGE +
				            "%.");
				LOGGER.Info("PrecautionaryRestartManager: Server is using " + getCpuLoad(PROCESS_CPU_LOAD_VAR) + "%.");
				Broadcast.toAllOnlinePlayers("Server will restart in 10 minutes.", false);
				Shutdown.getInstance().startShutdown(null, 600, true);
			}

			if (Config.PRECAUTIONARY_RESTART_MEMORY && (getProcessRamLoad() > Config.PRECAUTIONARY_RESTART_PERCENTAGE))
			{
				if (serverBizzy())
				{
					return;
				}

				LOGGER.Info("PrecautionaryRestartManager: Memory usage over " +
				            Config.PRECAUTIONARY_RESTART_PERCENTAGE + "%.");
				Broadcast.toAllOnlinePlayers("Server will restart in 10 minutes.", false);
				Shutdown.getInstance().startShutdown(null, 600, true);
			}
		}, Config.PRECAUTIONARY_RESTART_DELAY, Config.PRECAUTIONARY_RESTART_DELAY);
	}

	private static double getCpuLoad(String var)
	{
		try
		{
			MBeanServer mbs = ManagementFactory.getPlatformMBeanServer();
			ObjectName name = ObjectName.getInstance("java.lang:type=OperatingSystem");
			AttributeList list = mbs.getAttributes(name, new String[]
			{
				var
			});

			if (list.isEmpty())
			{
				return 0;
			}

			Attribute att = (Attribute)list.get(0);
			Double value = (Double)att.getValue();
			if (value == -1)
			{
				return 0;
			}

			return (value * 1000) / 10d;
		}
		catch (Exception e)
		{
		}

		return 0;
	}

	private static double getProcessRamLoad()
	{
		Runtime runTime = Runtime.getRuntime();
		long totalMemory = runTime.maxMemory();
		long usedMemory = totalMemory - ((totalMemory - runTime.totalMemory()) + runTime.freeMemory());
		return (usedMemory * 100) / totalMemory;
	}

	private bool serverBizzy()
	{
		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			if ((castle != null) && castle.getSiege().isInProgress())
			{
				return true;
			}
		}

		foreach (Fort fort in FortManager.getInstance().getForts())
		{
			if ((fort != null) && fort.getSiege().isInProgress())
			{
				return true;
			}
		}

		foreach (Player player in World.getInstance().getPlayers())
		{
			if ((player == null) || player.isInOfflineMode())
			{
				continue;
			}

			if (player.isInOlympiadMode())
			{
				return true;
			}

			if (player.isOnEvent())
			{
				return true;
			}

			if (player.isInInstance())
			{
				return true;
			}

			WorldObject target = player.getTarget();
			if ((target is RaidBoss) || (target is GrandBoss))
			{
				return true;
			}
		}

		return false;
	}

	public void restartEnabled()
	{
		_restarting = true;
	}

	public void restartAborted()
	{
		_restarting = false;
	}

	public static PrecautionaryRestartManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly PrecautionaryRestartManager INSTANCE = new PrecautionaryRestartManager();
	}
}