using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Zones;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author Sdw
 */
public class Bidder
{
	private readonly Clan _clan;
	private readonly long _bid;
	private readonly long _time;

	public Bidder(Clan clan, long bid, long time)
	{
		_clan = clan;
		_bid = bid;
		_time = time == 0 ? Instant.now().toEpochMilli() : time;
	}

	public Clan getClan()
	{
		return _clan;
	}

	public String getClanName()
	{
		return _clan.getName();
	}

	public String getLeaderName()
	{
		return _clan.getLeaderName();
	}

	public long getBid()
	{
		return _bid;
	}

	public long getTime()
	{
		return _time;
	}

	public String getFormattedTime()
	{
		return DateTimeFormatter.ofPattern("dd/MM/yyyy HH:mm")
			.format(Instant.ofEpochMilli(_time).atZone(ZoneId.systemDefault()));
	}
}