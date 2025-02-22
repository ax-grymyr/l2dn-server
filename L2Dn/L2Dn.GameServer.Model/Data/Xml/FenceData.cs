using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author HoridoJoho / FBIagent
 */
public class FenceData: DataReaderBase
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
		if (_fences.Count != 0)
		{
			// Remove old fences when reloading
			_fences.Values.ForEach(removeFence);
		}

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "FenceData.xml");
		document.Elements("list").Elements("fence").ForEach(spawnFence);

		LOGGER.Info(GetType().Name + ": Loaded " + _fences.Count + " fences.");
	}

	public int getLoadedElementsCount()
	{
		return _fences.Count;
	}

	private void spawnFence(XElement fenceNode)
	{
		int x = fenceNode.GetAttributeValueAsInt32("x");
		int y = fenceNode.GetAttributeValueAsInt32("y");
		int z = fenceNode.GetAttributeValueAsInt32("z");
		string name = fenceNode.GetAttributeValueAsString("name");
		int width = fenceNode.GetAttributeValueAsInt32("width");
		int length = fenceNode.GetAttributeValueAsInt32("length");
		int height = fenceNode.GetAttributeValueAsInt32("height");
		FenceState state = fenceNode.Attribute("state").GetEnum(FenceState.CLOSED);
		spawnFence(new Location3D(x, y, z), name, width, length, height, 0, state);
	}

	public Fence spawnFence(Location3D location, int width, int length, int height, int instanceId, FenceState state)
	{
		return spawnFence(location, string.Empty, width, length, height, instanceId, state);
	}

	public Fence spawnFence(Location3D location, string name, int width, int length, int height, int instanceId,
		FenceState state)
	{
		Fence fence = new Fence(location.X, location.Y, name, width, length, height, state);
		if (instanceId > 0)
		{
			fence.setInstanceById(instanceId);
		}

		fence.spawnMe(location);
		addFence(fence);

		return fence;
	}

	private void addFence(Fence fence)
	{
		_fences.put(fence.ObjectId, fence);
	}

	public void removeFence(Fence fence)
	{
		_fences.remove(fence.ObjectId);
	}

	public Map<int, Fence> getFences()
	{
		return _fences;
	}

	public Fence? getFence(int objectId)
	{
		return _fences.get(objectId);
	}

	public bool checkIfFenceBetween(Location3D location, Location3D targetLocation, Instance? instance = null)
	{
		WorldRegion? region = World.getInstance().getRegion(location.X, location.Y);
		IReadOnlyCollection<Fence> fences = region.Fences;
		if (fences.Count == 0)
			return false;

		foreach (Fence fence in fences)
		{
			// Check if fence is geodata enabled.
			FenceState state = fence.getState();
			if (!(state == FenceState.CLOSED_HIDDEN || state == FenceState.CLOSED))
			{
				continue;
			}

			// Check if fence is within the instance we search for.
			int instanceId = instance == null ? 0 : instance.getId();
			if (fence.getInstanceId() != instanceId)
			{
				continue;
			}

			int xMin = fence.getXMin();
			int xMax = fence.getXMax();
			int yMin = fence.getYMin();
			int yMax = fence.getYMax();
			if (location.X < xMin && targetLocation.X < xMin)
			{
				continue;
			}

			if (location.X > xMax && targetLocation.X > xMax)
			{
				continue;
			}

			if (location.Y < yMin && targetLocation.Y < yMin)
			{
				continue;
			}

			if (location.Y > yMax && targetLocation.Y > yMax)
			{
				continue;
			}

			if (location.X > xMin && targetLocation.X > xMin && location.X < xMax && targetLocation.X < xMax &&
			    location.Y > yMin && targetLocation.Y > yMin && location.Y < yMax && targetLocation.Y < yMax)
			{
				continue;
			}

			if ((crossLinePart(xMin, yMin, xMax, yMin, location.X, location.Y, targetLocation.X, targetLocation.Y, xMin,
					    yMin, xMax, yMax) ||
				    crossLinePart(xMax, yMin, xMax, yMax, location.X, location.Y, targetLocation.X, targetLocation.Y,
					    xMin, yMin, xMax, yMax) ||
				    crossLinePart(xMax, yMax, xMin, yMax, location.X, location.Y, targetLocation.X, targetLocation.Y,
					    xMin, yMin, xMax, yMax) ||
				    crossLinePart(xMin, yMax, xMin, yMin, location.X, location.Y, targetLocation.X, targetLocation.Y,
					    xMin, yMin, xMax, yMax)) &&
			    location.Z > fence.getZ() - MAX_Z_DIFF && location.Z < fence.getZ() + MAX_Z_DIFF)
			{
				return true;
			}
		}

		return false;
	}

	private static bool crossLinePart(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4,
		double xMin, double yMin, double xMax, double yMax)
	{
		double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
		if (d == 0)
			return false;

		double xCross = ((x3 - x4) * (x1 * y2 - y1 * x2) - (x1 - x2) * (x3 * y4 - y3 * x4)) / d;
		double yCross = ((y3 - y4) * (x1 * y2 - y1 * x2) - (y1 - y2) * (x3 * y4 - y3 * x4)) / d;
		if (xCross <= xMax && xCross >= xMin)
			return true;

		return yCross <= yMax && yCross >= yMin;
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