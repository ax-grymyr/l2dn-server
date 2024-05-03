using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones;

/**
 * Abstract base class for any zone form
 * @author durgus
 */
public abstract class ZoneForm
{
    protected const int STEP = 10;
	
    public abstract bool isInsideZone(int x, int y, int z);
	
    public abstract bool intersectsRectangle(int x1, int x2, int y1, int y2);
	
    public abstract double getDistanceToZone(int x, int y);
	
    public abstract int getLowZ(); // Support for the ability to extract the z coordinates of zones.
	
    public abstract int getHighZ(); // New fishing patch makes use of that to get the Z for the hook
	
    // landing coordinates.
	
    protected bool lineSegmentsIntersect(int ax1, int ay1, int ax2, int ay2, int bx1, int by1, int bx2, int by2)
    {
        return Line2D.linesIntersect(ax1, ay1, ax2, ay2, bx1, by1, bx2, by2);
    }
	
    public abstract void visualizeZone(int z);
	
    protected void dropDebugItem(int itemId, int num, int x, int y, int z)
    {
        Item item = new Item(IdManager.getInstance().getNextId(), itemId);
        item.setCount(num);
        item.spawnMe(new Location3D(x, y, z + 5));
        ZoneManager.getInstance().getDebugItems().add(item);
    }
	
    public abstract Location3D getRandomPoint();
}