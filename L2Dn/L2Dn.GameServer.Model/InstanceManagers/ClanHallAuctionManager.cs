using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Sdw
 */
public class ClanHallAuctionManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanHallAuctionManager));

	private static readonly Map<int, ClanHallAuction> AUCTIONS = new();
	private static ScheduledFuture? _endTask;

	protected ClanHallAuctionManager()
	{
		DateTime currentTime = DateTime.Now;

		// Schedule of the start, next Wednesday at 19:00.
		DateTime start = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 19, 0, 0);
		while (start.DayOfWeek != DayOfWeek.Wednesday)
			start = start.AddDays(1);

		if (start < currentTime)
		{
			start = start.AddDays(1);
			while (start.DayOfWeek != DayOfWeek.Wednesday)
				start = start.AddDays(1);
		}

		TimeSpan startDelay = start - currentTime;
		if (startDelay < TimeSpan.Zero)
			startDelay = TimeSpan.Zero;

		ThreadPool.scheduleAtFixedRate(onStart, startDelay, TimeSpan.FromDays(7));
		if (startDelay > TimeSpan.Zero)
			onStart();

		// Schedule of the end, next Wednesday at 11:00.
		DateTime end = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 11, 0, 0);
		while (end.DayOfWeek != DayOfWeek.Wednesday)
			end = end.AddDays(1);

		if (end < currentTime)
		{
			end = end.AddDays(1);
			while (end.DayOfWeek != DayOfWeek.Wednesday)
				end = end.AddDays(1);
		}

		TimeSpan endDelay = end - currentTime;
		if (endDelay < TimeSpan.Zero)
			endDelay = TimeSpan.Zero;

		_endTask = ThreadPool.scheduleAtFixedRate(onEnd, endDelay, TimeSpan.FromDays(7));
	}

	private void onStart()
	{
		LOGGER.Info(GetType().Name +": Clan Hall Auction has started!");
		AUCTIONS.Clear();
		ClanHallData.getInstance().getFreeAuctionableHall().ForEach(c => AUCTIONS.put(c.getResidenceId(), new ClanHallAuction(c.getResidenceId())));
	}

	private void onEnd()
	{
		AUCTIONS.Values.ForEach(x => x.finalizeAuctions());
		AUCTIONS.Clear();
		LOGGER.Info(GetType().Name +": Clan Hall Auction has ended!");
	}

	public ClanHallAuction? getClanHallAuctionById(int clanHallId)
	{
		return AUCTIONS.get(clanHallId);
	}

	public ClanHallAuction? getClanHallAuctionByClan(Clan clan)
	{
		foreach (ClanHallAuction auction in AUCTIONS.Values)
		{
			if (auction.getBids().ContainsKey(clan.getId()))
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
			if (auction.Key != clanHallId && auction.Value.getBids().ContainsKey(clan.getId()))
			{
				return true;
			}
		}
		return false;
	}

	public TimeSpan getRemainingTime()
	{
		return _endTask?.getDelay() ?? TimeSpan.Zero;
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