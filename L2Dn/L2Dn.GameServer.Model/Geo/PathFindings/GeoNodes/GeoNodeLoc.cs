using L2Dn.GameServer.Model;

namespace L2Dn.GameServer.Geo.PathFindings.GeoNodes;

/**
 * @author -Nemesiss-
 */
public class GeoNodeLoc: AbstractNodeLoc
{
	private readonly short _x;
	private readonly short _y;
	private readonly short _z;
	
	public GeoNodeLoc(short x, short y, short z)
	{
		_x = x;
		_y = y;
		_z = z;
	}
	
	public override int getX()
	{
		return World.WORLD_X_MIN + (_x * 128) + 48;
	}
	
	public override int getY()
	{
		return World.WORLD_Y_MIN + (_y * 128) + 48;
	}
	
	public override int getZ()
	{
		return _z;
	}
	
	public override void setZ(short z)
	{
	}
	
	public override int getNodeX()
	{
		return _x;
	}
	
	public override int getNodeY()
	{
		return _y;
	}
	
	public override int GetHashCode()
	{
		int prime = 31;
		int result = 1;
		result = (prime * result) + _x;
		result = (prime * result) + _y;
		result = (prime * result) + _z;
		return result;
	}
	
	public override bool Equals(Object? obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		if (!(obj is GeoNodeLoc))
		{
			return false;
		}
		GeoNodeLoc other = (GeoNodeLoc) obj;
		return (_x == other._x) && (_y == other._y) && (_z == other._z);
	}
}