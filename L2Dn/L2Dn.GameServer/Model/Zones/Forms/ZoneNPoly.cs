using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Forms;

/**
 * A not so primitive npoly zone
 * @author durgus
 */
public class ZoneNPoly: ZoneForm
{
	private readonly Polygon _p;
	private readonly int _z1;
	private readonly int _z2;
	
	/**
	 * @param x
	 * @param y
	 * @param z1
	 * @param z2
	 */
	public ZoneNPoly(int[] x, int[] y, int z1, int z2)
	{
		_p = new Polygon(x, y, x.Length);
		
		_z1 = Math.Min(z1, z2);
		_z2 = Math.Max(z1, z2);
	}
	
	public override bool isInsideZone(int x, int y, int z)
	{
		return _p.contains(x, y) && (z >= _z1) && (z <= _z2);
	}
	
	public override bool intersectsRectangle(int ax1, int ax2, int ay1, int ay2)
	{
		return _p.intersects(Math.Min(ax1, ax2), Math.Min(ay1, ay2), Math.Abs(ax2 - ax1), Math.Abs(ay2 - ay1));
	}
	
	public override double getDistanceToZone(int x, int y)
	{
		int[] xPoints = _p.xpoints;
		int[] yPoints = _p.ypoints;
		double test;
		double shortestDist = Math.Pow(xPoints[0] - x, 2) + Math.Pow(yPoints[0] - y, 2);
		
		for (int i = 1; i < _p.npoints; i++)
		{
			test = Math.Pow(xPoints[i] - x, 2) + Math.Pow(yPoints[i] - y, 2);
			if (test < shortestDist)
			{
				shortestDist = test;
			}
		}
		
		return Math.Sqrt(shortestDist);
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
		for (int i = 0; i < _p.npoints; i++)
		{
			int nextIndex = (i + 1) == _p.xpoints.Length ? 0 : i + 1;
			int vx = _p.xpoints[nextIndex] - _p.xpoints[i];
			int vy = _p.ypoints[nextIndex] - _p.ypoints[i];
			float length = (float) Math.Sqrt((vx * vx) + (vy * vy)) / STEP;
			for (int o = 1; o <= length; o++)
			{
				dropDebugItem(Inventory.ADENA_ID, 1, (int) (_p.xpoints[i] + ((o / length) * vx)), (int) (_p.ypoints[i] + ((o / length) * vy)), z);
			}
		}
	}
	
	public override Location getRandomPoint()
	{
		int minX = _p.getBounds().x;
		int maxX = _p.getBounds().x + _p.getBounds().width;
		int minY = _p.getBounds().y;
		int maxY = _p.getBounds().y + _p.getBounds().height;
		
		int x = Rnd.get(minX, maxX);
		int y = Rnd.get(minY, maxY);
		
		int antiBlocker = 0;
		while (!_p.contains(x, y) && (antiBlocker++ < 1000))
		{
			x = Rnd.get(minX, maxX);
			y = Rnd.get(minY, maxY);
		}
		
		return new Location(x, y, GeoEngine.getInstance().getHeight(x, y, (_z1 + _z2) / 2));
	}
	
	public int[] getX()
	{
		return _p.xpoints;
	}
	
	public int[] getY()
	{
		return _p.ypoints;
	}
}