using L2Dn.GameServer.Model;
using L2Dn.Geometry;

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

	public override Location3D Location => new(World.WORLD_X_MIN + _x * 128 + 48, World.WORLD_Y_MIN + _y * 128 + 48, _z);
	public override int Z => _z;

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
		return HashCode.Combine(_x, _y, _z);
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
		return _x == other._x && _y == other._y && _z == other._z;
	}
}