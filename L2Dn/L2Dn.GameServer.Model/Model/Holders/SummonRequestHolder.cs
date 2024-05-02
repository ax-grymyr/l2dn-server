using L2Dn.GameServer.Model.Actor;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Holders;

public class SummonRequestHolder
{
	private readonly Player _summoner;
	private readonly Location _location;

	public SummonRequestHolder(Player summoner)
	{
		_summoner = summoner;
		_location = summoner.getLocation();
	}

	public Player getSummoner()
	{
		return _summoner;
	}

	public Location Location => _location;
}