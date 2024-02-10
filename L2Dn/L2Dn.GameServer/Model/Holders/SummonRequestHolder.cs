using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class SummonRequestHolder
{
	private readonly Player _summoner;
	private readonly Location _location;

	public SummonRequestHolder(Player summoner)
	{
		_summoner = summoner;
		_location = summoner == null
			? null
			: new Location(summoner.getX(), summoner.getY(), summoner.getZ(), summoner.getHeading());
	}

	public Player getSummoner()
	{
		return _summoner;
	}

	public Location getLocation()
	{
		return _location;
	}
}