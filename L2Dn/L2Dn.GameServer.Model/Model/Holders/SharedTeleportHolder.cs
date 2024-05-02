using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author NasSeKa
 */
public class SharedTeleportHolder
{
	private readonly int _id;
	private readonly string _name;
	private int _count;
	private readonly Location3D _location;

	public SharedTeleportHolder(int id, string name, int count, Location3D location)
	{
		_id = id;
		_name = name;
		_count = count;
		_location = location;
	}

	public int getId()
	{
		return _id;
	}

	public string getName()
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

	public Location3D getLocation()
	{
		return _location;
	}
}