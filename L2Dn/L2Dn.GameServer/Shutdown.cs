using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer;

/**
 * This class provides the functions for shutting down and restarting the server.<br>
 * It closes all open client connections and saves all data.
 * @version $Revision: 1.2.4.5 $ $Date: 2005/03/27 15:29:09 $
 */
public class Shutdown
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Shutdown));
	
	private const int SIGTERM = 0;
	private const int GM_SHUTDOWN = 1;
	private const int GM_RESTART = 2;
	private const int ABORT = 3;
	private static readonly String[] MODE_TEXT =
	{
		"SIGTERM",
		"shutting down",
		"restarting",
		"aborting"
	};
	
	private static Shutdown _counterInstance;
	private static bool _countdownFinished;
	
	private int _secondsShut;
	private int _shutdownMode;
	
	/**
	 * This function starts a shutdown count down from Telnet (Copied from Function startShutdown())
	 * @param seconds seconds until shutdown
	 */
	private void sendServerQuit(int seconds)
	{
		SystemMessagePacket sysm = new SystemMessagePacket(SystemMessageId.THE_SERVER_WILL_BE_SHUT_DOWN_IN_S1_SEC_PLEASE_FIND_A_SAFE_PLACE_TO_LOG_OUT);
		sysm.Params.addInt(seconds);
		Broadcast.toAllOnlinePlayers(sysm);
	}
	
	/**
	 * Default constructor is only used internal to create the shutdown-hook instance
	 */
	protected Shutdown()
	{
		_secondsShut = -1;
		_shutdownMode = SIGTERM;
	}
	
	/**
	 * This creates a countdown instance of Shutdown.
	 * @param seconds how many seconds until shutdown
	 * @param restart true is the server shall restart after shutdown
	 */
	public Shutdown(int seconds, bool restart)
	{
		_secondsShut = Math.Max(0, seconds);
		_shutdownMode = restart ? GM_RESTART : GM_SHUTDOWN;
	}

	public void start()
	{
		System.Threading.Tasks.Task.Run(() => run());
	}
	
	/**
	 * This function is called, when a new thread starts if this thread is the thread of getInstance, then this is the shutdown hook and we save all data and disconnect all clients.<br>
	 * After this thread ends, the server will completely exit if this is not the thread of getInstance, then this is a countdown thread.<br>
	 * We start the countdown, and when we finished it, and it was not aborted, we tell the shutdown-hook why we call exit, and then call exit when the exit status of the server is 1, startServer.sh / startServer.bat will restart the server.
	 */
	public void run()
	{
		if (this == getInstance())
		{
			return;
		}
		
		if (_countdownFinished)
		{
			return;
		}
		
		// Send warnings and then call exit to start shutdown sequence.
		countdown();
		
		// Last point where logging is operational.
		LOGGER.Warn("GM shutdown countdown is over. " + MODE_TEXT[_shutdownMode] + " NOW!");
		
		switch (_shutdownMode)
		{
			case GM_SHUTDOWN:
			{
				getInstance().setMode(GM_SHUTDOWN);
				startShutdownActions();
				Environment.Exit(0);
				break;
			}
			case GM_RESTART:
			{
				getInstance().setMode(GM_RESTART);
				startShutdownActions();
				Environment.Exit(2);
				break;
			}
			case ABORT:
			{
				LoginServerThread.getInstance().setServerStatus(ServerStatus.STATUS_AUTO);
				break;
			}
		}
	}
	
	/**
	 * This functions starts a shutdown countdown.
	 * @param player GM who issued the shutdown command
	 * @param seconds seconds until shutdown
	 * @param restart true if the server will restart after shutdown
	 */
	public void startShutdown(Player player, int seconds, bool restart)
	{
		_shutdownMode = restart ? GM_RESTART : GM_SHUTDOWN;
		
		if (player != null)
		{
			LOGGER.Warn("GM: " + player.getName() + "(" + player.getObjectId() + ") issued shutdown command. " + MODE_TEXT[_shutdownMode] + " in " + seconds + " seconds!");
		}
		else
		{
			LOGGER.Warn("Server scheduled restart issued shutdown command. " + (restart ? "Restart" : "Shutdown") + " in " + seconds + " seconds!");
		}
		
		if (_shutdownMode > 0)
		{
			switch (seconds)
			{
				case 540:
				case 480:
				case 420:
				case 360:
				case 300:
				case 240:
				case 180:
				case 120:
				case 60:
				case 30:
				case 10:
				case 5:
				case 4:
				case 3:
				case 2:
				case 1:
				{
					break;
				}
				default:
				{
					sendServerQuit(seconds);
					break;
				}
			}
		}
		
		if (_counterInstance != null)
		{
			_counterInstance.abort();
		}
		
		if (Config.PRECAUTIONARY_RESTART_ENABLED)
		{
			PrecautionaryRestartManager.getInstance().restartEnabled();
		}
		
		// the main instance should only run for shutdown hook, so we start a new instance
		_counterInstance = new Shutdown(seconds, restart);
		_counterInstance.start();
	}
	
	/**
	 * This function aborts a running countdown.
	 * @param player GM who issued the abort command
	 */
	public void abort(Player player)
	{
		if (_countdownFinished)
		{
			LOGGER.Warn("GM: " + (player != null ? player.getName() + "(" + player.getObjectId() + ") " : "") + "shutdown ABORT failed because countdown has finished.");
			return;
		}
		
		LOGGER.Warn("GM: " + (player != null ? player.getName() + "(" + player.getObjectId() + ") " : "") + "issued shutdown ABORT. " + MODE_TEXT[_shutdownMode] + " has been stopped!");
		if (_counterInstance != null)
		{
			_counterInstance.abort();
			
			if (Config.PRECAUTIONARY_RESTART_ENABLED)
			{
				PrecautionaryRestartManager.getInstance().restartAborted();
			}
			
			Broadcast.toAllOnlinePlayers("Server aborts " + MODE_TEXT[_shutdownMode] + " and continues normal operation!", false);
		}
	}
	
	/**
	 * Set the shutdown mode.
	 * @param mode what mode shall be set
	 */
	private void setMode(int mode)
	{
		_shutdownMode = mode;
	}
	
	/**
	 * Set shutdown mode to ABORT.
	 */
	private void abort()
	{
		_shutdownMode = ABORT;
	}
	
	/**
	 * This counts the countdown and reports it to all players countdown is aborted if mode changes to ABORT.
	 */
	private void countdown()
	{
		try
		{
			while (_secondsShut > 0)
			{
				// Rehabilitate previous server status if shutdown is aborted.
				if (_shutdownMode == ABORT)
				{
					if (LoginServerThread.getInstance().getServerStatus() == ServerStatus.STATUS_DOWN)
					{
						LoginServerThread.getInstance().setServerStatus((Config.SERVER_GMONLY) ? ServerStatus.STATUS_GM_ONLY : ServerStatus.STATUS_AUTO);
					}
					break;
				}
				
				switch (_secondsShut)
				{
					case 540:
					case 480:
					case 420:
					case 360:
					case 300:
					case 240:
					case 180:
					case 120:
					case 60:
					case 30:
					case 10:
					case 5:
					case 4:
					case 3:
					case 2:
					case 1:
					{
						sendServerQuit(_secondsShut);
						break;
					}
				}
				
				// Prevent players from logging in.
				if ((_secondsShut <= 60) && (LoginServerThread.getInstance().getServerStatus() != ServerStatus.STATUS_DOWN))
				{
					LoginServerThread.getInstance().setServerStatus(ServerStatus.STATUS_DOWN);
				}
				
				_secondsShut--;
				
				Thread.Sleep(1000);
			}
		}
		catch (Exception e)
		{
			// this will never happen
		}
	}
	
	/**
	 * Actions performed when shutdown countdown completes.
	 */
	private void startShutdownActions()
	{
		if (_countdownFinished)
		{
			return;
		}
		_countdownFinished = true;
		
		TimeCounter tc = new TimeCounter();
		TimeCounter tc1 = new TimeCounter();
		
		try
		{
			if ((Config.OFFLINE_TRADE_ENABLE || Config.OFFLINE_CRAFT_ENABLE) && Config.RESTORE_OFFLINERS && !Config.STORE_OFFLINE_TRADE_IN_REALTIME)
			{
				OfflineTraderTable.getInstance().storeOffliners();
				LOGGER.Info("Offline Traders Table: Offline shops stored(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
			}
		}
		catch (Exception t)
		{
			LOGGER.Error("Error saving offline shops: " + t);
		}
		
		try
		{
			disconnectAllCharacters();
			LOGGER.Info("All players disconnected and saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		}
		catch (Exception t)
		{
			LOGGER.Error("Error disconnecting characters: " + t);
		}
		
		// ensure all services are stopped
		try
		{
			GameTimeTaskManager.getInstance().interrupt();
			LOGGER.Info("Game Time Task Manager: Thread interruped(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		}
		catch (Exception t)
		{
			LOGGER.Error("Error stopping services: " + t);
		}
		
		// stop all thread pools
		try
		{
			ThreadPool.shutdown();
			LOGGER.Info("Thread Pool Manager: Manager has been shut down(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		}
		catch (Exception t)
		{
			LOGGER.Error("Error stopping thread pool: " + t);
		}
		
		try
		{
			LoginServerThread.getInstance().interrupt();
			LOGGER.Info("Login Server Thread: Thread interruped(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		}
		catch (Exception t)
		{
			LOGGER.Error("Error stopping LoginServerThread: " + t);
		}
		
		// last byebye, save all data and quit this server
		saveData();
		tc.restartCounter();
		
		// commit data, last chance
		try
		{
			DatabaseFactory.close();
			LOGGER.Info("Database Factory: Database connection has been shut down(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		}
		catch (Exception t)
		{
			LOGGER.Error("Error stopping database factory: " + t);
		}
		
		// Backup database.
		if (Config.BACKUP_DATABASE)
		{
			DatabaseBackup.performBackup();
		}
		
		LOGGER.Info("The server has been successfully shut down in " + (tc1.getEstimatedTime() / 1000) + "seconds.");
	}
	
	/**
	 * This sends a last byebye, disconnects all players and saves data.
	 */
	private void saveData()
	{
		switch (_shutdownMode)
		{
			case SIGTERM:
			{
				LOGGER.Info("SIGTERM received. Shutting down NOW!");
				break;
			}
			case GM_SHUTDOWN:
			{
				LOGGER.Info("GM shutdown received. Shutting down NOW!");
				break;
			}
			case GM_RESTART:
			{
				LOGGER.Info("GM restart received. Restarting NOW!");
				break;
			}
		}
		
		TimeCounter tc = new TimeCounter();
		
		// Save all raidboss and GrandBoss status ^_^
		DBSpawnManager.getInstance().cleanUp();
		LOGGER.Info("RaidBossSpawnManager: All raidboss info saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		GrandBossManager.getInstance().cleanUp();
		LOGGER.Info("GrandBossManager: All Grand Boss info saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		ItemAuctionManager.getInstance().shutdown();
		LOGGER.Info("Item Auction Manager: All tasks stopped(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		Olympiad.getInstance().saveOlympiadStatus();
		LOGGER.Info("Olympiad System: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		
		Hero.getInstance().shutdown();
		LOGGER.Info("Hero System: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		ClanTable.getInstance().shutdown();
		LOGGER.Info("Clan System: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		
		RevengeHistoryManager.getInstance().storeMe();
		LOGGER.Info("Revenge History Manager: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		
		// Save Cursed Weapons data before closing.
		CursedWeaponsManager.getInstance().saveData();
		LOGGER.Info("Cursed Weapons Manager: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		
		// Save all manor data
		if (!Config.ALT_MANOR_SAVE_ALL_ACTIONS)
		{
			CastleManorManager.getInstance().storeMe();
			LOGGER.Info("Castle Manor Manager: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		}
		
		// Save all global (non-player specific) Quest data that needs to persist after reboot
		QuestManager.getInstance().save();
		LOGGER.Info("Quest Manager: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		
		// Save all global variables data
		GlobalVariablesManager.getInstance().storeMe();
		LOGGER.Info("Global Variables Manager: Variables saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		
		// Schemes save.
		SchemeBufferTable.getInstance().saveSchemes();
		LOGGER.Info("SchemeBufferTable data has been saved.");
		
		// Save World Exchange.
		if (Config.ENABLE_WORLD_EXCHANGE)
		{
			WorldExchangeManager.getInstance().storeMe();
			LOGGER.Info("World Exchange Manager: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		}
		
		// Save items on ground before closing
		if (Config.SAVE_DROPPED_ITEM)
		{
			ItemsOnGroundManager.getInstance().saveInDb();
			LOGGER.Info("Items On Ground Manager: Data saved(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
			ItemsOnGroundManager.getInstance().cleanUp();
			LOGGER.Info("Items On Ground Manager: Cleaned up(" + tc.getEstimatedTimeAndRestartCounter() + "ms).");
		}
		
		// Save bot reports to database
		if (Config.BOTREPORT_ENABLE)
		{
			BotReportTable.getInstance().saveReportedCharData();
			LOGGER.Info("Bot Report Table: Successfully saved reports to database!");
		}
		
		try
		{
			Thread.Sleep(5000);
		}
		catch (Exception e)
		{
			// this will never happen
		}
	}
	
	/**
	 * This disconnects all clients from the server.
	 */
	private void disconnectAllCharacters()
	{
		foreach (Player player in World.getInstance().getPlayers())
		{
			Disconnection.of(player).defaultSequence(ServerClose.STATIC_PACKET);
		}
	}
	
	/**
	 * A simple class used to track down the estimated time of method executions.<br>
	 * Once this class is created, it saves the start time, and when you want to get the estimated time, use the getEstimatedTime() method.
	 */
	private class TimeCounter
	{
		private DateTime _startTime;
		
		public TimeCounter()
		{
			restartCounter();
		}
		
		public void restartCounter()
		{
			_startTime = DateTime.UtcNow;
		}
		
		public TimeSpan getEstimatedTimeAndRestartCounter()
		{
			TimeSpan toReturn = DateTime.UtcNow - _startTime;
			restartCounter();
			return toReturn;
		}
		
		public TimeSpan getEstimatedTime()
		{
			return DateTime.UtcNow - _startTime;
		}
	}
	
	/**
	 * Get the shutdown-hook instance the shutdown-hook instance is created by the first call of this function, but it has to be registered externally.
	 * @return instance of Shutdown, to be used as shutdown hook
	 */
	public static Shutdown getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly Shutdown INSTANCE = new Shutdown();
	}
}