using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class PremiumManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PremiumManager));
	
	private class PremiumExpireTask(Player player): Runnable
	{
		public void run()
		{
			player.setPremiumStatus(false);
		}
	}
	
	// Data Cache
	private readonly Map<int, DateTime> _premiumData = new();
	
	// expireTasks
	private readonly Map<int, ScheduledFuture> _expiretasks = new();
	
	protected PremiumManager()
	{
		void PlayerLoginEvent(OnPlayerLogin @event)
		{
			Player player = @event.getPlayer();
			int accountId = player.getAccountId();
			loadPremiumData(accountId);
			DateTime now = DateTime.UtcNow;
			DateTime premiumExpiration = getPremiumExpiration(accountId);
			player.setPremiumStatus(premiumExpiration > now);
			if (player.hasPremiumStatus())
			{
				startExpireTask(player, premiumExpiration - now);
			}
			else
			{
				removePremiumStatus(accountId, false);
			}
		}

		void PlayerLogoutEvent(OnPlayerLogout @event)
		{
			stopExpireTask(@event.getPlayer());
		}

		GlobalEvents.Players.Subscribe<OnPlayerLogin>(this, PlayerLoginEvent);
		GlobalEvents.Players.Subscribe<OnPlayerLogout>(this, PlayerLogoutEvent);
	}
	
	/**
	 * @param player
	 * @param delay
	 */
	private void startExpireTask(Player player, TimeSpan delay)
	{
		_expiretasks.put(player.getAccountId(), ThreadPool.schedule(new PremiumExpireTask(player), delay));
	}
	
	/**
	 * @param player
	 */
	private void stopExpireTask(Player player)
	{
		ScheduledFuture task = _expiretasks.remove(player.getAccountId());
		if (task != null)
		{
			task.cancel(false);
			task = null;
		}
	}
	
	private void loadPremiumData(int accountId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			foreach (AccountPremium? record in ctx.AccountPremiums.Where(r => r.AccountId == accountId))
			{
				_premiumData.put(record.AccountId, record.EndTime);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Problem with PremiumManager: " + e);
		}
	}
	
	public DateTime getPremiumExpiration(int accountId)
	{
		return _premiumData.getOrDefault(accountId, DateTime.MinValue);
	}
	
	public void addPremiumTime(int accountId, TimeSpan value)
	{
		// new premium task at least from now
		DateTime oldPremiumExpiration = getPremiumExpiration(accountId);
		DateTime now = DateTime.UtcNow;
		if (oldPremiumExpiration < now)
			oldPremiumExpiration = now;
		
		DateTime newPremiumExpiration = oldPremiumExpiration + value;
		
		// UPDATE DATABASE
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.AccountPremiums.Where(r => r.AccountId == accountId)
				.ExecuteUpdate(s => s.SetProperty(r => r.EndTime, newPremiumExpiration));
		}
		catch (Exception e)
		{
			LOGGER.Warn("Problem with PremiumManager: " + e);
		}
		
		// UPDATE CACHE
		_premiumData.put(accountId, newPremiumExpiration);
		
		// UPDATE PLAYER PREMIUM STATUS
		foreach (Player player in World.getInstance().getPlayers())
		{
			if (player.getAccountId() == accountId)
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
	
	public void removePremiumStatus(int accountId, bool checkOnline)
	{
		if (checkOnline)
		{
			foreach (Player player in World.getInstance().getPlayers())
			{
				if (player.getAccountId() == accountId && player.hasPremiumStatus())
				{
					player.setPremiumStatus(false);
					stopExpireTask(player);
					break;
				}
			}
		}
		
		// UPDATE CACHE
		_premiumData.remove(accountId);
		
		// UPDATE DATABASE
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.AccountPremiums.Where(r => r.AccountId == accountId).ExecuteDelete();
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