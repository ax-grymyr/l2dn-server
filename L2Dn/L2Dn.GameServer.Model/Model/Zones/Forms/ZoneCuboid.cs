using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Zones.Forms;

/**
 * A primitive rectangular zone
 * @author durgus
 */
public class ZoneCuboid: ZoneForm
{
	private readonly int _z1;
	private readonly int _z2;
	private readonly Rectangle _r;
	
	public ZoneCuboid(int x1, int x2, int y1, int y2, int z1, int z2)
	{
		int _x1 = Math.Min(x1, x2);
		int _x2 = Math.Max(x1, x2);
		int _y1 = Math.Min(y1, y2);
		int _y2 = Math.Max(y1, y2);
		
		_r = new Rectangle(_x1, _y1, _x2 - _x1, _y2 - _y1);
		
		_z1 = Math.Min(z1, z2);
		_z2 = Math.Max(z1, z2);
	}
	
	public override bool isInsideZone(int x, int y, int z)
	{
		return _r.contains(x, y) && (z >= _z1) && (z <= _z2);
	}
	
	public override bool intersectsRectangle(int ax1, int ax2, int ay1, int ay2)
	{
		return _r.intersects(Math.Min(ax1, ax2), Math.Min(ay1, ay2), Math.Abs(ax2 - ax1), Math.Abs(ay2 - ay1));
	}
	
	public override double getDistanceToZone(int x, int y)
	{
		int _x1 = _r.x;
		int _x2 = _r.x + _r.width;
		int _y1 = _r.y;
		int _y2 = _r.y + _r.height;
		double test = Math.Pow(_x1 - x, 2) + Math.Pow(_y2 - y, 2);
		double shortestDist = Math.Pow(_x1 - x, 2) + Math.Pow(_y1 - y, 2);
		
		if (test < shortestDist)
		{
			shortestDist = test;
		}
		
		test = Math.Pow(_x2 - x, 2) + Math.Pow(_y1 - y, 2);
		if (test < shortestDist)
		{
			shortestDist = test;
		}
		
		test = Math.Pow(_x2 - x, 2) + Math.Pow(_y2 - y, 2);
		if (test < shortestDist)
		{
			shortestDist = test;
		}
		
		return Math.Sqrt(shortestDist);
	}
	
	/*
	 * getLowZ() / getHighZ() - These two functions were added to cope with the demand of the new fishing algorithms, which are now able to correctly place the hook in the water, thanks to getHighZ(). getLowZ() was added, considering potential future modifications.
	 */
	public override int getLowZ()
	{
		return _z1;
	}
	
	public override int getHighZ()
	{
		return _z2;
	}
	
	public override void visualizeZone(int z)
	{
		int _x1 = _r.x;
		int _x2 = _r.x + _r.width;
		int _y1 = _r.y;
		int _y2 = _r.y + _r.height;
		
		// x1->x2
		for (int x = _x1; x < _x2; x += STEP)
		{
			dropDebugItem(Inventory.ADENA_ID, 1, x, _y1, z);
			dropDebugItem(Inventory.ADENA_ID, 1, x, _y2, z);
		}
		// y1->y2
		for (int y = _y1; y < _y2; y += STEP)
		{
			dropDebugItem(Inventory.ADENA_ID, 1, _x1, y, z);
			dropDebugItem(Inventory.ADENA_ID, 1, _x2, y, z);
		}
	}

	public override Location3D getRandomPoint()
	{
		int x = Rnd.get(_r.x, _r.x + _r.width);
		int y = Rnd.get(_r.y, _r.y + _r.height);

		return new Location3D(x, y, GeoEngine.getInstance().getHeight(new Location3D(x, y, (_z1 + _z2) / 2)));
	}
}