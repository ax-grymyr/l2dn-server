using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Zones.Forms;

/**
 * A primitive circular zone
 * @author durgus
 */
public class ZoneCylinder: ZoneForm
{
	private readonly int _x;
	private readonly int _y;
	private readonly int _z1;
	private readonly int _z2;
	private readonly int _rad;
	private readonly int _radS;
	
	public ZoneCylinder(int x, int y, int z1, int z2, int rad)
	{
		_x = x;
		_y = y;
		_z1 = z1;
		_z2 = z2;
		_rad = rad;
		_radS = rad * rad;
	}
	
	public override bool isInsideZone(int x, int y, int z)
	{
		return Math.Pow(_x - x, 2) + Math.Pow(_y - y, 2) <= _radS && z >= _z1 && z <= _z2;
	}
	
	public override bool intersectsRectangle(int ax1, int ax2, int ay1, int ay2)
	{
		// Circles point inside the rectangle?
		if (_x > ax1 && _x < ax2 && _y > ay1 && _y < ay2)
		{
			return true;
		}
		
		// Any point of the rectangle intersecting the Circle?
		if (Math.Pow(ax1 - _x, 2) + Math.Pow(ay1 - _y, 2) < _radS)
		{
			return true;
		}
		if (Math.Pow(ax1 - _x, 2) + Math.Pow(ay2 - _y, 2) < _radS)
		{
			return true;
		}
		if (Math.Pow(ax2 - _x, 2) + Math.Pow(ay1 - _y, 2) < _radS)
		{
			return true;
		}
		if (Math.Pow(ax2 - _x, 2) + Math.Pow(ay2 - _y, 2) < _radS)
		{
			return true;
		}
		
		// Collision on any side of the rectangle?
		if (_x > ax1 && _x < ax2)
		{
			if (Math.Abs(_y - ay2) < _rad)
			{
				return true;
			}
			if (Math.Abs(_y - ay1) < _rad)
			{
				return true;
			}
		}
		if (_y > ay1 && _y < ay2)
		{
			if (Math.Abs(_x - ax2) < _rad)
			{
				return true;
			}
			if (Math.Abs(_x - ax1) < _rad)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public override double getDistanceToZone(int x, int y)
	{
		return MathUtil.hypot(_x - x, _y - y) - _rad;
	}
	
	// getLowZ() / getHighZ() - These two functions were added to cope with the demand of the new fishing algorithms, wich are now able to correctly place the hook in the water, thanks to getHighZ(). getLowZ() was added, considering potential future modifications.
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
		int count = (int) (2 * Math.PI * _rad / STEP);
		double angle = 2 * Math.PI / count;
		for (int i = 0; i < count; i++)
		{
			dropDebugItem(Inventory.ADENA_ID, 1, _x + (int) (Math.Cos(angle * i) * _rad), _y + (int) (Math.Sin(angle * i) * _rad), z);
		}
	}

	public override Location3D getRandomPoint()
	{
		int x = 0;
		int y = 0;
		int x2 = _x - _rad;
		int y2 = _y - _rad;
		int x3 = _x + _rad;
		int y3 = _y + _rad;
		while (Math.Pow(_x - x, 2) + Math.Pow(_y - y, 2) > _radS)
		{
			x = Rnd.get(x2, x3);
			y = Rnd.get(y2, y3);
		}

		return new Location3D(x, y, GeoEngine.getInstance().getHeight(x, y, (_z1 + _z2) / 2));
	}
}