using L2Dn.GameServer.Geo.GeoDataImpl;

namespace L2Dn.GameServer.Geo.PathFindings.CellNodes;

/**
 * @author -Nemesiss-, HorridoJoho
 */
public class NodeLoc: AbstractNodeLoc
{
	private int _x;
	private int _y;
	private bool _goNorth;
	private bool _goEast;
	private bool _goSouth;
	private bool _goWest;
	private int _geoHeight;
	
	public NodeLoc(int x, int y, int z)
	{
		set(x, y, z);
	}
	
	public void set(int x, int y, int z)
	{
		_x = x;
		_y = y;
		_goNorth = GeoEngine.getInstance().checkNearestNswe(x, y, z, Cell.NSWE_NORTH);
		_goEast = GeoEngine.getInstance().checkNearestNswe(x, y, z, Cell.NSWE_EAST);
		_goSouth = GeoEngine.getInstance().checkNearestNswe(x, y, z, Cell.NSWE_SOUTH);
		_goWest = GeoEngine.getInstance().checkNearestNswe(x, y, z, Cell.NSWE_WEST);
		_geoHeight = GeoEngine.getInstance().getNearestZ(x, y, z);
	}
	
	public bool canGoNorth()
	{
		return _goNorth;
	}
	
	public bool canGoEast()
	{
		return _goEast;
	}
	
	public bool canGoSouth()
	{
		return _goSouth;
	}
	
	public bool canGoWest()
	{
		return _goWest;
	}
	
	public bool canGoNone()
	{
		return !canGoNorth() && !canGoEast() && !canGoSouth() && !canGoWest();
	}
	
	public bool canGoAll()
	{
		return canGoNorth() && canGoEast() && canGoSouth() && canGoWest();
	}
	
	public override int getX()
	{
		return GeoEngine.getWorldX(_x);
	}
	
	public override int getY()
	{
		return GeoEngine.getWorldY(_y);
	}
	
	public override int getZ()
	{
		return _geoHeight;
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
		
		int nswe = 0;
		if (canGoNorth())
		{
			nswe |= Cell.NSWE_NORTH;
		}
		if (canGoEast())
		{
			nswe |= Cell.NSWE_EAST;
		}
		if (canGoSouth())
		{
			nswe |= Cell.NSWE_SOUTH;
		}
		if (canGoWest())
		{
			nswe |= Cell.NSWE_WEST;
		}
		
		result = (prime * result) + (((_geoHeight & 0xFFFF) << 1) | nswe);
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
		if (!(obj is NodeLoc))
		{
			return false;
		}
		NodeLoc other = (NodeLoc) obj;
		return (_x == other._x) && (_y == other._y) && (!_goNorth == !other._goNorth) && (!_goEast == !other._goEast) && (!_goSouth == !other._goSouth) && (!_goWest == !other._goWest) && (_geoHeight == other._geoHeight);
	}
}