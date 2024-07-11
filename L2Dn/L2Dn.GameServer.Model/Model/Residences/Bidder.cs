using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author Sdw
 */
public class Bidder
{
	private readonly Clan _clan;
	private readonly long _bid;
	private readonly DateTime _time;

	public Bidder(Clan clan, long bid, DateTime time)
	{
		_clan = clan;
		_bid = bid;
		_time = time;
	}

	public Clan getClan()
	{
		return _clan;
	}

	public string getClanName()
	{
		return _clan.getName();
	}

	public string getLeaderName()
	{
		return _clan.getLeaderName();
	}

	public long getBid()
	{
		return _bid;
	}

	public DateTime getTime()
	{
		return _time;
	}
}