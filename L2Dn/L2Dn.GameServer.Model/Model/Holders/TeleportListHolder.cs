using System.Collections.Immutable;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author NviX, Index
 */
public class TeleportListHolder
{
	private readonly int _locId;
	private readonly ImmutableArray<Location3D> _locations;
	private readonly int _price;
	private readonly bool _special;

	public TeleportListHolder(int locId, Location3D location, int price, bool special)
	{
		_locId = locId;
		_locations = [location];
		_price = price;
		_special = special;
	}

	public TeleportListHolder(int locId, ImmutableArray<Location3D> locations, int price, bool special)
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

	public ImmutableArray<Location3D> getLocations()
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

	public Location3D getLocation()
	{
		return _locations[Rnd.get(_locations.Length)];
	}
}