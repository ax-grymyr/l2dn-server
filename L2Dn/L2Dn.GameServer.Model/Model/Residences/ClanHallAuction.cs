using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author Sdw
 */
public class ClanHallAuction
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanHallAuction));

    private readonly Map<int, Bidder> _bidders = [];
	private readonly int _clanHallId;
    private readonly ClanHall _clanHall;

    public ClanHallAuction(int clanHallId)
    {
        // TODO: pass clan hall data instead of id
        _clanHall = ClanHallData.getInstance().getClanHallById(_clanHallId) ??
            throw new ArgumentException($"Clan hall id={clanHallId} not found");

		_clanHallId = clanHallId;
		loadBidder();
	}

	private void loadBidder()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = ctx.ClanHallBidders.Where(r => r.ClanHallId == _clanHallId);
			foreach (var record in query)
			{
				Clan? clan = ClanTable.getInstance().getClan(record.ClanId);
                if (clan != null)
				    addBid(clan, record.Bid, record.BidTime);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed loading clan hall auctions bidder for clan hall " + _clanHallId + ": " + e);
		}
	}

	public Map<int, Bidder> getBids()
	{
		return _bidders;
	}

	public void addBid(Clan clan, long bid)
	{
		addBid(clan, bid, DateTime.UtcNow);
	}

	public void addBid(Clan clan, long bid, DateTime bidTime)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ClanHallBidders.Add(new DbClanHallBidder()
			{
				ClanHallId = _clanHallId,
				ClanId = clan.getId(),
				Bid = bid,
				BidTime = bidTime
			});

			ctx.SaveChanges();
			_bidders.put(clan.getId(), new Bidder(clan, bid, bidTime));
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed insert clan hall auctions bidder " + clan.getName() + " for clan hall " + _clanHallId +
			            ": " + e);
		}
	}

	public void removeBid(Clan clan)
	{
		getBids().remove(clan.getId());
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int clanId = clan.getId();
			ctx.ClanHallBidders.Where(c => c.ClanId == clanId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed clearing bidder " + clan.getName() + " for clan hall " + _clanHallId + ": " + e);
		}
	}

	public long getHighestBid()
	{
		if (getBids().Count != 0)
			return getBids().Values.Select(b => b.getBid()).Max();

		return _clanHall.getMinBid();
	}

	public long getClanBid(Clan clan)
	{
		return getBids().get(clan.getId())?.getBid() ?? 0;
	}

	public Bidder? getHighestBidder()
	{
		return getBids().Values.MaxBy(x => x.getBid());
	}

	public int getBidCount()
	{
		return getBids().Values.Count;
	}

	public void returnAdenas(Bidder bidder)
	{
		bidder.getClan().getWarehouse().addItem("Clan Hall Auction Outbid", Inventory.ADENA_ID, bidder.getBid(), null, null);
	}

	public void finalizeAuctions()
	{
		Bidder? potentialHighestBidder = getHighestBidder();
		if (potentialHighestBidder != null)
		{
			Bidder highestBidder = potentialHighestBidder;
			_clanHall.setOwner(highestBidder.getClan());
			getBids().Clear();

			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.ClanHallBidders.Where(r => r.ClanHallId == _clanHallId).ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error("Failed clearing bidder for clan hall " + _clanHallId + ": " + e);
			}
		}
	}

	public int getClanHallId()
	{
		return _clanHallId;
	}
}