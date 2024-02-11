using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class PremiumManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PremiumManager));
	
	private const string LOAD_SQL = "SELECT account_name,enddate FROM account_premium WHERE account_name = ?";
	private const string UPDATE_SQL = "REPLACE INTO account_premium (account_name,enddate) VALUE (?,?)";
	private const string DELETE_SQL = "DELETE FROM account_premium WHERE account_name = ?";
	
	class PremiumExpireTask: Runnable
	{
		Player _player;
		
		PremiumExpireTask(Player player)
		{
			_player = player;
		}
		
		public void run()
		{
			_player.setPremiumStatus(false);
		}
	}
	
	// Data Cache
	private readonly Map<String, long> _premiumData = new();
	
	// expireTasks
	private readonly Map<String, ScheduledFuture<?>> _expiretasks = new();
	
	// Listeners
	private readonly ListenersContainer _listenerContainer = Containers.Players();
	
	private readonly Action<OnPlayerLogin> _playerLoginEvent = @event =>
	{
		Player player = @event.getPlayer();
		String accountName = player.getAccountName();
		loadPremiumData(accountName);
		long now = System.currentTimeMillis();
		long premiumExpiration = getPremiumExpiration(accountName);
		player.setPremiumStatus(premiumExpiration > now);
		if (player.hasPremiumStatus())
		{
			startExpireTask(player, premiumExpiration - now);
		}
		else if (premiumExpiration > 0)
		{
			removePremiumStatus(accountName, false);
		}
	};
	
	private readonly Action<OnPlayerLogout> _playerLogoutEvent = @event =>
	{
		stopExpireTask(@event.getPlayer());
	};
	
	protected PremiumManager()
	{
		_listenerContainer.addListener(new ConsumerEventListener(_listenerContainer, EventType.ON_PLAYER_LOGIN, _playerLoginEvent, this));
		_listenerContainer.addListener(new ConsumerEventListener(_listenerContainer, EventType.ON_PLAYER_LOGOUT, _playerLogoutEvent, this));
	}
	
	/**
	 * @param player
	 * @param delay
	 */
	private void startExpireTask(Player player, long delay)
	{
		_expiretasks.put(player.getAccountName().ToLower(), ThreadPool.schedule(new PremiumExpireTask(player), delay));
	}
	
	/**
	 * @param player
	 */
	private void stopExpireTask(Player player)
	{
		ScheduledFuture<?> task = _expiretasks.remove(player.getAccountName().ToLower());
		if (task != null)
		{
			task.cancel(false);
			task = null;
		}
	}
	
	private void loadPremiumData(String accountName)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement stmt = con.prepareStatement(LOAD_SQL);
			stmt.setString(1, accountName.toLowerCase());
			try (ResultSet rset = stmt.executeQuery())
			{
				while (rset.next())
				{
					_premiumData.put(rset.getString(1).toLowerCase(), rset.getLong(2));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Problem with PremiumManager: " + e);
		}
	}
	
	public long getPremiumExpiration(String accountName)
	{
		return _premiumData.getOrDefault(accountName.ToLower(), 0L);
	}
	
	public void addPremiumTime(String accountName, int timeValue, TimeUnit timeUnit)
	{
		long addTime = timeUnit.toMillis(timeValue);
		long now = System.currentTimeMillis();
		// new premium task at least from now
		long oldPremiumExpiration = Math.Max(now, getPremiumExpiration(accountName));
		long newPremiumExpiration = oldPremiumExpiration + addTime;
		
		// UPDATE DATABASE
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement stmt = con.prepareStatement(UPDATE_SQL);
			stmt.setString(1, accountName.toLowerCase());
			stmt.setLong(2, newPremiumExpiration);
			stmt.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Problem with PremiumManager: " + e);
		}
		
		// UPDATE CACHE
		_premiumData.put(accountName.ToLower(), newPremiumExpiration);
		
		// UPDATE PlAYER PREMIUMSTATUS
		foreach (Player player in World.getInstance().getPlayers())
		{
			if (accountName.equalsIgnoreCase(player.getAccountName()))
			{
				stopExpireTask(player);
				startExpireTask(player, newPremiumExpiration - now);
				if (!player.hasPremiumStatus())
				{
					player.setPremiumStatus(true);
				}
				break;
			}
		}
	}
	
	public void removePremiumStatus(String accountName, bool checkOnline)
	{
		if (checkOnline)
		{
			foreach (Player player in World.getInstance().getPlayers())
			{
				if (accountName.equalsIgnoreCase(player.getAccountName()) && player.hasPremiumStatus())
				{
					player.setPremiumStatus(false);
					stopExpireTask(player);
					break;
				}
			}
		}
		
		// UPDATE CACHE
		_premiumData.remove(accountName.ToLower());
		
		// UPDATE DATABASE
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement stmt = con.prepareStatement(DELETE_SQL);
			stmt.setString(1, accountName.ToLower());
			stmt.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Problem with PremiumManager: " + e);
		}
	}
	
	public static PremiumManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PremiumManager INSTANCE = new PremiumManager();
	}
}