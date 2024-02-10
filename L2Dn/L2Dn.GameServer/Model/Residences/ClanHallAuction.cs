using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author Sdw
 */
public class ClanHallAuction
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanHallAuction));
	
	private readonly int _clanHallId;
	private const String LOAD_CLANHALL_BIDDERS = "SELECT * FROM clanhall_auctions_bidders WHERE clanHallId=?";
	private const String DELETE_CLANHALL_BIDDERS = "DELETE FROM clanhall_auctions_bidders WHERE clanHallId=?";
	private const String INSERT_CLANHALL_BIDDER = "REPLACE INTO clanhall_auctions_bidders (clanHallId, clanId, bid, bidTime) VALUES (?,?,?,?)";
	private const String DELETE_CLANHALL_BIDDER = "DELETE FROM clanhall_auctions_bidders WHERE clanId=?";
	
	public ClanHallAuction(int clanHallId)
	{
		_clanHallId = clanHallId;
		loadBidder();
	}
	
	private void loadBidder()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(LOAD_CLANHALL_BIDDERS);
			ps.setInt(1, _clanHallId);
			{
				ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					Clan clan = ClanTable.getInstance().getClan(rs.getInt("clanId"));
					addBid(clan, rs.getLong("bid"), rs.getLong("bidTime"));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed loading clan hall auctions bidder for clan hall " + _clanHallId + ": " + e);
		}
	}
	
	private Map<int, Bidder> _bidders;
	
	public Map<int, Bidder> getBids()
	{
		return _bidders == null ? new() : _bidders;
	}
	
	public void addBid(Clan clan, long bid)
	{
		addBid(clan, bid, System.currentTimeMillis());
	}
	
	public void addBid(Clan clan, long bid, long bidTime)
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(INSERT_CLANHALL_BIDDER);
			ps.setInt(1, _clanHallId);
			ps.setInt(2, clan.getId());
			ps.setLong(3, bid);
			ps.setLong(4, bidTime);
			ps.execute();
			_bidders.put(clan.getId(), new Bidder(clan, bid, bidTime));
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed insert clan hall auctions bidder " + clan.getName() + " for clan hall " + _clanHallId + ": " + e);
		}
	}
	
	public void removeBid(Clan clan)
	{
		getBids().remove(clan.getId());
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(DELETE_CLANHALL_BIDDER);
			ps.setInt(1, clan.getId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed clearing bidder " + clan.getName() + " for clan hall " + _clanHallId + ": " + e);
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
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement ps = con.prepareStatement(DELETE_CLANHALL_BIDDERS);
				ps.setInt(1, _clanHallId);
				ps.execute();
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