using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Sdw
 */
public class ClanHallAuctionManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanHallAuctionManager));
	
	private static readonly Map<int, ClanHallAuction> AUCTIONS = new();
	private static ScheduledFuture<?> _endTask;
	
	protected ClanHallAuctionManager()
	{
		long currentTime = System.currentTimeMillis();
		
		// Schedule of the start, next Wednesday at 19:00.
		Calendar start = Calendar.getInstance();
		start.set(Calendar.DAY_OF_WEEK, Calendar.WEDNESDAY);
		start.set(Calendar.HOUR_OF_DAY, 19);
		start.set(Calendar.MINUTE, 0);
		start.set(Calendar.SECOND, 0);
		if (start.getTimeInMillis() < currentTime)
		{
			start.add(Calendar.DAY_OF_YEAR, 1);
			while (start.get(Calendar.DAY_OF_WEEK) != Calendar.WEDNESDAY)
			{
				start.add(Calendar.DAY_OF_YEAR, 1);
			}
		}
		long startDelay = Math.max(0, start.getTimeInMillis() - currentTime);
		ThreadPool.scheduleAtFixedRate(this::onStart, startDelay, 604800000); // 604800000 = 1 week
		if (startDelay > 0)
		{
			onStart();
		}
		
		// Schedule of the end, next Wednesday at 11:00.
		Calendar end = Calendar.getInstance();
		end.set(Calendar.DAY_OF_WEEK, Calendar.WEDNESDAY);
		end.set(Calendar.HOUR_OF_DAY, 11);
		end.set(Calendar.MINUTE, 0);
		end.set(Calendar.SECOND, 0);
		if (end.getTimeInMillis() < currentTime)
		{
			end.add(Calendar.DAY_OF_YEAR, 1);
			while (end.get(Calendar.DAY_OF_WEEK) != Calendar.WEDNESDAY)
			{
				end.add(Calendar.DAY_OF_YEAR, 1);
			}
		}
		long endDelay = Math.max(0, end.getTimeInMillis() - currentTime);
		_endTask = ThreadPool.scheduleAtFixedRate(this::onEnd, endDelay, 604800000); // 604800000 = 1 week
	}
	
	private void onStart()
	{
		LOGGER.Info(GetType().Name +": Clan Hall Auction has started!");
		AUCTIONS.clear();
		ClanHallData.getInstance().getFreeAuctionableHall().forEach(c => AUCTIONS.put(c.getResidenceId(), new ClanHallAuction(c.getResidenceId())));
	}
	
	private void onEnd()
	{
		AUCTIONS.values().forEach(ClanHallAuction::finalizeAuctions);
		AUCTIONS.clear();
		LOGGER.Info(GetType().Name +": Clan Hall Auction has ended!");
	}
	
	public ClanHallAuction getClanHallAuctionById(int clanHallId)
	{
		return AUCTIONS.get(clanHallId);
	}
	
	public ClanHallAuction getClanHallAuctionByClan(Clan clan)
	{
		for (ClanHallAuction auction : AUCTIONS.values())
		{
			if (auction.getBids().containsKey(clan.getId()))
			{
				return auction;
			}
		}
		return null;
	}
	
	public bool checkForClanBid(int clanHallId, Clan clan)
	{
		foreach (var auction in AUCTIONS)
		{
			if ((auction.Key != clanHallId) && auction.Value.getBids().containsKey(clan.getId()))
			{
				return true;
			}
		}
		return false;
	}
	
	public long getRemainingTime()
	{
		return _endTask.getDelay(TimeUnit.MILLISECONDS);
	}
	
	public static ClanHallAuctionManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ClanHallAuctionManager INSTANCE = new ClanHallAuctionManager();
	}
}