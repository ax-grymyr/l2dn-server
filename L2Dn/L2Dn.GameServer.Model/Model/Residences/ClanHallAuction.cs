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
	
	private readonly int _clanHallId;
	private const string DELETE_CLANHALL_BIDDERS = "DELETE FROM clanhall_auctions_bidders WHERE clanHallId=?";
	private const string INSERT_CLANHALL_BIDDER = "REPLACE INTO clanhall_auctions_bidders (clanHallId, clanId, bid, bidTime) VALUES (?,?,?,?)";
	private const string DELETE_CLANHALL_BIDDER = "DELETE FROM clanhall_auctions_bidders WHERE clanId=?";
	
	public ClanHallAuction(int clanHallId)
	{
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
				Clan clan = ClanTable.getInstance().getClan(record.ClanId);
				addBid(clan, record.Bid, record.BidTime);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed loading clan hall auctions bidder for clan hall " + _clanHallId + ": " + e);
		}
	}
	
	private Map<int, Bidder> _bidders;
	
	public Map<int, Bidder> getBids()
	{
		return _bidders == null ? new() : _bidders;
	}
	
	public void addBid(Clan clan, long bid)
	{
		addBid(clan, bid, DateTime.UtcNow);
	}
	
	public void addBid(Clan clan, long bid, DateTime bidTime)
	{
		if (_bidders == null)
		{
			lock (this)
			{
				if (_bidders == null)
				{
					_bidders = new();
				}
			}
		}

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
		ClanHall clanHall = ClanHallData.getInstance().getClanHallById(_clanHallId);
		if (getBids().Count != 0)
			return getBids().values().Select(b => b.getBid()).Max();
		return clanHall.getMinBid();
	}

	public long getClanBid(Clan clan)
	{
		return getBids().get(clan.getId()).getBid();
	}
	
	public Bidder? getHighestBidder()
	{
		return getBids().values().MaxBy(x => x.getBid());
	}
	
	public int getBidCount()
	{
		return getBids().values().Count;
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
			ClanHall clanHall = ClanHallData.getInstance().getClanHallById(_clanHallId);
			clanHall.setOwner(highestBidder.getClan());
			getBids().clear();
			
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