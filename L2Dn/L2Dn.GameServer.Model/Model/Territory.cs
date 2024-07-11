using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

public class Territory
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Territory));
	
	public class Point
	{
		public int _x;
		public int _y;
		public int _zmin;
		public int _zmax;
		public int _proc;
		
		public Point(int x, int y, int zmin, int zmax, int proc)
		{
			_x = x;
			_y = y;
			_zmin = zmin;
			_zmax = zmax;
			_proc = proc;
		}
	}
	
	private readonly List<Point> _points = new();
	private readonly int _terr;
	private int _xMin;
	private int _xMax;
	private int _yMin;
	private int _yMax;
	private int _zMin;
	private int _zMax;
	private int _procMax;
	
	public Territory(int terr)
	{
		_terr = terr;
		_xMin = 999999;
		_xMax = -999999;
		_yMin = 999999;
		_yMax = -999999;
		_zMin = 999999;
		_zMax = -999999;
		_procMax = 0;
	}
	
	public void add(int x, int y, int zmin, int zmax, int proc)
	{
		_points.Add(new Point(x, y, zmin, zmax, proc));
		if (x < _xMin)
		{
			_xMin = x;
		}
		if (y < _yMin)
		{
			_yMin = y;
		}
		if (x > _xMax)
		{
			_xMax = x;
		}
		if (y > _yMax)
		{
			_yMax = y;
		}
		if (zmin < _zMin)
		{
			_zMin = zmin;
		}
		if (zmax > _zMax)
		{
			_zMax = zmax;
		}
		_procMax += proc;
	}
	
	public bool isIntersect(int x, int y, Point p1, Point p2)
	{
		double dy1 = p1._y - y;
		double dy2 = p2._y - y;
		if (Math.Abs(Math.Sign(dy1) - Math.Sign(dy2)) <= 1e-6)
		{
			return false;
		}
		
		double dx1 = p1._x - x;
		double dx2 = p2._x - x;
		if ((dx1 >= 0) && (dx2 >= 0))
		{
			return true;
		}
		
		if ((dx1 < 0) && (dx2 < 0))
		{
			return false;
		}
		
		double dx0 = (dy1 * (p1._x - p2._x)) / (p1._y - p2._y);
		return dx0 <= dx1;
	}
	
	public bool isInside(int x, int y)
	{
		int intersectCount = 0;
		for (int i = 0; i < _points.Count; i++)
		{
			Point p1 = _points[i > 0 ? i - 1 : _points.Count - 1];
			Point p2 = _points[i];
			if (isIntersect(x, y, p1, p2))
			{
				intersectCount++;
			}
		}
		return (intersectCount % 2) == 1;
	}
	
	public Location3D? getRandomPoint()
	{
		if (_procMax > 0)
		{
			int pos = 0;
			int rnd = Rnd.get(_procMax);
			foreach (Point p1 in _points)
			{
				pos += p1._proc;
				if (rnd <= pos)
				{
					return new Location3D(p1._x, p1._y, Rnd.get(p1._zmin, p1._zmax));
				}
			}
		}
		for (int i = 0; i < 100; i++)
		{
			int x = Rnd.get(_xMin, _xMax);
			int y = Rnd.get(_yMin, _yMax);
			if (isInside(x, y))
			{
				double curdistance = 0;
				int zmin = _zMin;
				foreach (Point p1 in _points)
				{
					double distance = double.Hypot(p1._x - x, p1._y - y);
					if ((curdistance == 0) || (distance < curdistance))
					{
						curdistance = distance;
						zmin = p1._zmin;
					}
				}
				return new Location3D(x, y, Rnd.get(zmin, _zMax));
			}
		}

		LOGGER.Warn("Can't make point for territory " + _terr);
		return null;
	}
	
	public int getProcMax()
	{
		return _procMax;
	}
}