namespace L2Dn.GameServer.Model.Holders;

/**
 * @author NasSeKa
 */
public class SharedTeleportHolder
{
	private readonly int _id;
	private readonly String _name;
	private int _count;
	private readonly Location _location;

	public SharedTeleportHolder(int id, String name, int count, int x, int y, int z)
	{
		_id = id;
		_name = name;
		_count = count;
		_location = new Location(x, y, z);
	}

	public int getId()
	{
		return _id;
	}

	public String getName()
	{
		return _name;
	}

	public int getCount()
	{
		return Math.Max(0, _count);
	}

	public void decrementCount()
	{
		_count -= 1;
	}

	public Location getLocation()
	{
		return _location;
	}
}