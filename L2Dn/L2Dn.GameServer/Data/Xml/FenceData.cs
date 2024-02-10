using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author HoridoJoho / FBIagent
 */
public class FenceData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FenceData));
	
	private const int MAX_Z_DIFF = 100;
	
	private readonly Map<int, Fence> _fences = new();
	
	protected FenceData()
	{
		load();
	}
	
	public void load()
	{
		if (!_fences.isEmpty())
		{
			// Remove old fences when reloading
			_fences.values().forEach(x => removeFence(x));
		}
		
		parseDatapackFile("data/FenceData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _fences.size() + " fences.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "fence", this::spawnFence));
	}
	
	public int getLoadedElementsCount()
	{
		return _fences.size();
	}
	
	private void spawnFence(Node fenceNode)
	{
		StatSet set = new StatSet(parseAttributes(fenceNode));
		spawnFence(set.getInt("x"), set.getInt("y"), set.getInt("z"), set.getString("name"), set.getInt("width"), set.getInt("length"), set.getInt("height"), 0, set.getEnum("state", FenceState.class, FenceState.CLOSED));
	}
	
	public Fence spawnFence(int x, int y, int z, int width, int length, int height, int instanceId, FenceState state)
	{
		return spawnFence(x, y, z, null, width, length, height, instanceId, state);
	}
	
	public Fence spawnFence(int x, int y, int z, String name, int width, int length, int height, int instanceId, FenceState state)
	{
		Fence fence = new Fence(x, y, name, width, length, height, state);
		if (instanceId > 0)
		{
			fence.setInstanceById(instanceId);
		}
		fence.spawnMe(x, y, z);
		addFence(fence);
		
		return fence;
	}
	
	private void addFence(Fence fence)
	{
		_fences.put(fence.getObjectId(), fence);
	}
	
	public void removeFence(Fence fence)
	{
		_fences.remove(fence.getObjectId());
	}
	
	public Map<int, Fence> getFences()
	{
		return _fences;
	}
	
	public Fence getFence(int objectId)
	{
		return _fences.get(objectId);
	}
	
	public bool checkIfFenceBetween(int x, int y, int z, int tx, int ty, int tz, Instance instance)
	{
		WorldRegion region = World.getInstance().getRegion(x, y);
		List<Fence> fences = region != null ? region.getFences() : null;
		if ((fences == null) || fences.isEmpty())
		{
			return false;
		}
		
		foreach (Fence fence in fences)
		{
			// Check if fence is geodata enabled.
			if (!fence.getState().isGeodataEnabled())
			{
				continue;
			}
			
			// Check if fence is within the instance we search for.
			int instanceId = (instance == null) ? 0 : instance.getId();
			if (fence.getInstanceId() != instanceId)
			{
				continue;
			}
			
			int xMin = fence.getXMin();
			int xMax = fence.getXMax();
			int yMin = fence.getYMin();
			int yMax = fence.getYMax();
			if ((x < xMin) && (tx < xMin))
			{
				continue;
			}
			if ((x > xMax) && (tx > xMax))
			{
				continue;
			}
			if ((y < yMin) && (ty < yMin))
			{
				continue;
			}
			if ((y > yMax) && (ty > yMax))
			{
				continue;
			}
			if ((x > xMin) && (tx > xMin) && (x < xMax) && (tx < xMax) && (y > yMin) && (ty > yMin) && (y < yMax) && (ty < yMax))
			{
				continue;
			}
			if ((crossLinePart(xMin, yMin, xMax, yMin, x, y, tx, ty, xMin, yMin, xMax, yMax) || crossLinePart(xMax, yMin, xMax, yMax, x, y, tx, ty, xMin, yMin, xMax, yMax) || crossLinePart(xMax, yMax, xMin, yMax, x, y, tx, ty, xMin, yMin, xMax, yMax) || crossLinePart(xMin, yMax, xMin, yMin, x, y, tx, ty, xMin, yMin, xMax, yMax)) && (z > (fence.getZ() - MAX_Z_DIFF)) && (z < (fence.getZ() + MAX_Z_DIFF)))
			{
				return true;
			}
		}
		return false;
	}
	
	private bool crossLinePart(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double xMin, double yMin, double xMax, double yMax)
	{
		double[] result = intersection(x1, y1, x2, y2, x3, y3, x4, y4);
		if (result == null)
		{
			return false;
		}
		
		double xCross = result[0];
		double yCross = result[1];
		if ((xCross <= xMax) && (xCross >= xMin))
		{
			return true;
		}
		if ((yCross <= yMax) && (yCross >= yMin))
		{
			return true;
		}
		
		return false;
	}
	
	private double[] intersection(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
	{
		double d = ((x1 - x2) * (y3 - y4)) - ((y1 - y2) * (x3 - x4));
		if (d == 0)
		{
			return null;
		}
		
		double xi = (((x3 - x4) * ((x1 * y2) - (y1 * x2))) - ((x1 - x2) * ((x3 * y4) - (y3 * x4)))) / d;
		double yi = (((y3 - y4) * ((x1 * y2) - (y1 * x2))) - ((y1 - y2) * ((x3 * y4) - (y3 * x4)))) / d;
		return new double[]
		{
			xi,
			yi
		};
	}
	
	public static FenceData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly FenceData INSTANCE = new();
	}
}