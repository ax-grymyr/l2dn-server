using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author NviX, Index
 */
public class TeleportListHolder
{
	private readonly int _locId;
	private readonly List<Location> _locations;
	private readonly int _price;
	private readonly bool _special;

	public TeleportListHolder(int locId, int x, int y, int z, int price, bool special)
	{
		_locId = locId;
		_locations = new(1);
		_locations.Add(new Location(x, y, z));
		_price = price;
		_special = special;
	}

	public TeleportListHolder(int locId, List<Location> locations, int price, bool special)
	{
		_locId = locId;
		_locations = locations;
		_price = price;
		_special = special;
	}

	public int getLocId()
	{
		return _locId;
	}

	public List<Location> getLocations()
	{
		return _locations;
	}

	public int getPrice()
	{
		return _price;
	}

	public bool isSpecial()
	{
		return _special;
	}

	public Location getLocation()
	{
		return _locations[Rnd.get(_locations.Count)];
	}
}