using L2Dn.GameServer.Model.Interfaces;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

/**
 * A datatype used to retain a 3D (x/y/z/heading) point. It got the capability to be set and cleaned.
 */
public sealed class Location: ILocational, ILocationHeading
{
	private int _x;
	private int _y;
	private int _z;
	private int _heading;
	
	public Location(int x, int y, int z, int heading = 0)
	{
		_x = x;
		_y = y;
		_z = z;
		_heading = heading;
	}

	/**
	 * Get the x coordinate.
	 * @return the x coordinate
	 */
	public int getX()
	{
		return _x;
	}
	
	/**
	 * Get the y coordinate.
	 * @return the y coordinate
	 */
	public int getY()
	{
		return _y;
	}
	
	/**
	 * Get the z coordinate.
	 * @return the z coordinate
	 */
	public int getZ()
	{
		return _z;
	}

	/**
	 * Set the x, y, z coordinates.
	 * @param loc The location.
	 */
	public void setXYZ(Location3D loc)
	{
		_x = loc.X;
		_y = loc.Y;
		_z = loc.Z;
	}

	/**
	 * Get the heading.
	 * @return the heading
	 */
	public int getHeading()
	{
		return _heading;
	}
	
	/**
	 * Set the heading.
	 * @param heading the heading
	 */
	public void setHeading(int heading)
	{
		_heading = heading;
	}

	public void setLocation(Location3D loc, int heading)
	{
		setXYZ(loc);
		_heading = heading;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_x, _y, _z);
	}

	public override bool Equals(object? obj)
	{
		if (obj is Location loc)
		{
			return getX() == loc.X && getY() == loc.getY() && getZ() == loc.getZ() && getHeading() == loc.getHeading();
		}

		return false;
	}

	public override string ToString()
	{
		return "[" + GetType().Name + "] X: " + _x + " Y: " + _y + " Z: " + _z + " Heading: " + _heading;
	}

	public Location3D ToLocation3D()
	{
		return new Location3D(_x, _y, _z);
	}

	public LocationHeading ToLocationHeading()
	{
		return new LocationHeading(_x, _y, _z, _heading);
	}

	public Location2D ToLocation2D()
	{
		return new Location2D(_x, _y);
	}

	public int X => _x;
	public int Y => _y;
	public int Z => _z;
	public int Heading => _heading;
}