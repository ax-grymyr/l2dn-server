using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Geo.PathFindings.GeoNodes;

/**
 * @author -Nemesiss-
 */
public class GeoPathFinding: PathFinding
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(GeoPathFinding));

	private static readonly Map<short, byte[]> PATH_NODES = new();
	private static readonly Map<short, int[]> PATH_NODE_INDEX = new();

	public override bool pathNodesExist(short regionoffset)
	{
		return PATH_NODE_INDEX.ContainsKey(regionoffset);
	}

	public override List<AbstractNodeLoc>? findPath(Location3D location, Location3D targetLocation, Instance? instance, bool playable)
	{
		int gx = (location.X - World.WORLD_X_MIN) >> 4;
		int gy = (location.Y - World.WORLD_Y_MIN) >> 4;
		short gz = (short)location.Z;
		int gtx = (targetLocation.X - World.WORLD_X_MIN) >> 4;
		int gty = (targetLocation.Y - World.WORLD_Y_MIN) >> 4;
		short gtz = (short)targetLocation.Z;

		GeoNode? start = readNode(gx, gy, gz);
		GeoNode? end = readNode(gtx, gty, gtz);
		if (start == null || end == null || ReferenceEquals(start, end))
		{
			return null;
		}

		Location3D startLoc = start.getLoc().Location;
		if (Math.Abs(startLoc.Z - location.Z) > 55)
		{
			return null; // Not correct layer.
		}

		Location3D endLoc = end.getLoc().Location;
		if (Math.Abs(endLoc.Z - targetLocation.Z) > 55)
		{
			return null; // Not correct layer.
		}

		// TODO: Find closest path node we CAN access. Now only checks if we can not reach the closest.
		Location3D temp = GeoEngine.getInstance().getValidLocation(location, startLoc, instance);
		if (temp.X != startLoc.X || temp.Y != startLoc.Y)
		{
			return null; // Cannot reach closest...
		}

		// TODO: Find closest path node around target, now only checks if location can be reached.
		temp = GeoEngine.getInstance().getValidLocation(targetLocation, endLoc, instance);
		if (temp.X != endLoc.X || temp.Y != endLoc.Y)
		{
			return null; // Cannot reach closest...
		}

		return searchByClosest2(start, end);
	}

	public List<AbstractNodeLoc>? searchByClosest2(GeoNode start, GeoNode end)
	{
		// Always continues checking from the closest to target non-blocked
		// node from to_visit list. There's extra length in path if needed
		// to go backwards/sideways but when moving generally forwards, this is extra fast
		// and accurate. And can reach insane distances (try it with 800 nodes..).
		// Minimum required node count would be around 300-400.
		// Generally returns a bit (only a bit) more intelligent looking routes than
		// the basic version. Not a true distance image (which would increase CPU
		// load) level of intelligence though.

		// List of Visited Nodes.
		List<GeoNode> visited = new();

		// List of Nodes to Visit.
		List<GeoNode> toVisit = new();
		toVisit.Add(start);
		int targetX = end.getLoc().getNodeX();
		int targetY = end.getLoc().getNodeY();

		int dx, dy;
		bool added;
		int i = 0;
		while (i < 550)
		{
			GeoNode node;
			try
			{
				node = toVisit[0];
				toVisit.RemoveAt(0); // TODO: very inefficient
			}
			catch (Exception exception)
			{
				// No Path found
                LOGGER.Trace(exception);
				return null;
			}
			if (node.Equals(end))
			{
				return constructPath2(node);
			}

			i++;
			visited.Add(node);
			node.attachNeighbors(readNeighbors(node));
			GeoNode[] neighbors = node.getNeighbors();
			if (neighbors == null)
			{
				continue;
			}
			foreach (GeoNode n in neighbors)
			{
				if (visited.LastIndexOf(n) == -1 && !toVisit.Contains(n))
				{
					added = false;
					n.setParent(node);
					dx = targetX - n.getLoc().getNodeX();
					dy = targetY - n.getLoc().getNodeY();
					n.setCost(dx * dx + dy * dy);
					for (int index = 0; index < toVisit.Count; index++)
					{
						// Supposed to find it quite early.
						if (toVisit[index].getCost() > n.getCost())
						{
							toVisit.Insert(index, n);
							added = true;
							break;
						}
					}
					if (!added)
					{
						toVisit.Add(n);
					}
				}
			}
		}
		// No Path found.
		return null;
	}

	public List<AbstractNodeLoc> constructPath2(AbstractNode<GeoNodeLoc> node)
	{
		List<AbstractNodeLoc> path = new();
		int previousDirectionX = -1000;
		int previousDirectionY = -1000;
		int directionX;
		int directionY;

		AbstractNode<GeoNodeLoc> tempNode = node;
		while (tempNode.getParent() != null)
		{
			// Only add a new route point if moving direction changes.
			directionX = tempNode.getLoc().getNodeX() - tempNode.getParent().getLoc().getNodeX();
			directionY = tempNode.getLoc().getNodeY() - tempNode.getParent().getLoc().getNodeY();

			if (directionX != previousDirectionX || directionY != previousDirectionY)
			{
				previousDirectionX = directionX;
				previousDirectionY = directionY;
				path.Insert(0, tempNode.getLoc());
			}
			tempNode = tempNode.getParent();
		}
		return path;
	}

	private GeoNode[]? readNeighbors(GeoNode n)
    {
        GeoNodeLoc? loc = n.getLoc();
		if (loc == null)
		{
			return null;
		}

		int idx = n.getNeighborsIdx();

		int nodeX = loc.getNodeX();
		int nodeY = loc.getNodeY();

		short regoffset = getRegionOffset(getRegionX(nodeX), getRegionY(nodeY));
		byte[] pn = PATH_NODES[regoffset];

		List<GeoNode> neighbors = new();
		GeoNode? newNode;
		short newNodeX;
		short newNodeY;

		// Region for sure will change, we must read from correct file
		byte neighbor = pn[idx++]; // N
		if (neighbor > 0)
		{
			neighbor--;
			newNodeX = (short) nodeX;
			newNodeY = (short) (nodeY - 1);
			newNode = readNode(newNodeX, newNodeY, neighbor);
			if (newNode != null)
			{
				neighbors.Add(newNode);
			}
		}
		neighbor = pn[idx++]; // NE
		if (neighbor > 0)
		{
			neighbor--;
			newNodeX = (short) (nodeX + 1);
			newNodeY = (short) (nodeY - 1);
			newNode = readNode(newNodeX, newNodeY, neighbor);
			if (newNode != null)
			{
				neighbors.Add(newNode);
			}
		}
		neighbor = pn[idx++]; // E
		if (neighbor > 0)
		{
			neighbor--;
			newNodeX = (short) (nodeX + 1);
			newNodeY = (short) nodeY;
			newNode = readNode(newNodeX, newNodeY, neighbor);
			if (newNode != null)
			{
				neighbors.Add(newNode);
			}
		}
		neighbor = pn[idx++]; // SE
		if (neighbor > 0)
		{
			neighbor--;
			newNodeX = (short) (nodeX + 1);
			newNodeY = (short) (nodeY + 1);
			newNode = readNode(newNodeX, newNodeY, neighbor);
			if (newNode != null)
			{
				neighbors.Add(newNode);
			}
		}
		neighbor = pn[idx++]; // S
		if (neighbor > 0)
		{
			neighbor--;
			newNodeX = (short) nodeX;
			newNodeY = (short) (nodeY + 1);
			newNode = readNode(newNodeX, newNodeY, neighbor);
			if (newNode != null)
			{
				neighbors.Add(newNode);
			}
		}
		neighbor = pn[idx++]; // SW
		if (neighbor > 0)
		{
			neighbor--;
			newNodeX = (short) (nodeX - 1);
			newNodeY = (short) (nodeY + 1);
			newNode = readNode(newNodeX, newNodeY, neighbor);
			if (newNode != null)
			{
				neighbors.Add(newNode);
			}
		}
		neighbor = pn[idx++]; // W
		if (neighbor > 0)
		{
			neighbor--;
			newNodeX = (short) (nodeX - 1);
			newNodeY = (short) nodeY;
			newNode = readNode(newNodeX, newNodeY, neighbor);
			if (newNode != null)
			{
				neighbors.Add(newNode);
			}
		}
		neighbor = pn[idx++]; // NW
		if (neighbor > 0)
		{
			neighbor--;
			newNodeX = (short) (nodeX - 1);
			newNodeY = (short) (nodeY - 1);
			newNode = readNode(newNodeX, newNodeY, neighbor);
			if (newNode != null)
			{
				neighbors.Add(newNode);
			}
		}
		return neighbors.ToArray();
	}

	// Private

	private static short getShort(byte[] b, int index)
	{
		return (short)(b[index] + (b[index + 1] << 8));
	}

	private GeoNode? readNode(short nodeX, short nodeY, byte layer)
	{
		short regoffset = getRegionOffset(getRegionX(nodeX), getRegionY(nodeY));
		if (!pathNodesExist(regoffset))
			return null;

        short nbx = getNodeBlock(nodeX);
		short nby = getNodeBlock(nodeY);
		int idx = PATH_NODE_INDEX[regoffset][(nby << 8) + nbx];
		byte[] pn = PATH_NODES[regoffset];
		// Reading.
		byte nodes = pn[idx];
		idx += layer * 10 + 1; // byte + layer*10byte
		if (nodes < layer)
		{
			LOGGER.Warn("SmthWrong!");
		}
		short node_z = getShort(pn, idx);
		idx += 2;
		return new GeoNode(new GeoNodeLoc(nodeX, nodeY, node_z), idx);
	}

	private GeoNode? readNode(int gx, int gy, short z)
	{
		short nodeX = getNodePos(gx);
		short nodeY = getNodePos(gy);
		short regoffset = getRegionOffset(getRegionX(nodeX), getRegionY(nodeY));
		if (!pathNodesExist(regoffset))
			return null;

        short nbx = getNodeBlock(nodeX);
		short nby = getNodeBlock(nodeY);
		int idx = PATH_NODE_INDEX[regoffset][(nby << 8) + nbx];
		byte[] pn = PATH_NODES[regoffset];
		// Reading.
		byte nodes = pn[idx++];
		int idx2 = 0; // Create index to nearlest node by z.
		short lastZ = short.MinValue;
		while (nodes > 0)
		{
			short node_z = getShort(pn, idx);
			if (Math.Abs(lastZ - z) > Math.Abs(node_z - z))
			{
				lastZ = node_z;
				idx2 = idx + 2;
			}
			idx += 10; // short + 8 byte
			nodes--;
		}
		return new GeoNode(new GeoNodeLoc(nodeX, nodeY, lastZ), idx2);
	}

	protected GeoPathFinding()
	{
		for (int regionX = World.TILE_X_MIN; regionX <= World.TILE_X_MAX; regionX++)
		{
			for (int regionY = World.TILE_Y_MIN; regionY <= World.TILE_Y_MAX; regionY++)
			{
				loadPathNodeFile((byte) regionX, (byte) regionY);
			}
		}
	}

	private void loadPathNodeFile(byte rx, byte ry)
	{
		short regionoffset = getRegionOffset(rx, ry);
		string filePath = Path.Combine(Config.PATHNODE_PATH, rx + "_" + ry + ".pn");
		if (!File.Exists(filePath))
		{
			return;
		}

		// LOGGER.info("Path Engine: - Loading: " + file.getName() + " -> region offset: " + regionoffset + " X: " + rx + " Y: " + ry);

		int node = 0;
		int index = 0;

		// Create a read-only memory-mapped file.
		try
		{
			byte[] nodes = File.ReadAllBytes(filePath);

			// Indexing pathnode files, so we will know where each block starts.
			int[] indexs = new int[65536];

			while (node < 65536)
			{
				byte layer = nodes[index];
				indexs[node++] = index;
				index += layer * 10 + 1;
			}
			PATH_NODE_INDEX.put(regionoffset, indexs);
			PATH_NODES.put(regionoffset, nodes);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to Load PathNode File: " + filePath + " : " + e);
		}
	}

	public new static GeoPathFinding getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static GeoPathFinding INSTANCE = new GeoPathFinding();
	}
}