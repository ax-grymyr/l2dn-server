using System.Text;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Geo.PathFindings.CellNodes;

/**
 * @author Sami, DS Credits to Diamond
 */
public class CellPathFinding: PathFinding
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CellPathFinding));

	private BufferInfo[] _allBuffers;
	private int _findSuccess;
	private int _findFails;
	private int _postFilterUses;
	private int _postFilterPlayableUses;
	private int _postFilterPasses;
	private TimeSpan _postFilterElapsed;

	private List<Item> _debugItems = [];

	protected CellPathFinding()
	{
		try
		{
			string[] array = Config.PATHFIND_BUFFERS.Split(";");
			_allBuffers = new BufferInfo[array.Length];

			string buf;
			string[] args;
			for (int i = 0; i < array.Length; i++)
			{
				buf = array[i];
				args = buf.Split("x");
				if (args.Length != 2)
				{
					throw new Exception("Invalid buffer definition: " + buf);
				}

				_allBuffers[i] = new BufferInfo(int.Parse(args[0]), int.Parse(args[1]));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("CellPathFinding: Problem during buffer init: " + e);
			throw new Exception("CellPathFinding: load aborted");
		}
	}

	public override bool pathNodesExist(short regionoffset)
	{
		return false;
	}

	public override List<AbstractNodeLoc>? findPath(Location3D location, Location3D targetLocation, Instance? instance, bool playable)
	{
		int gx = GeoEngine.getGeoX(location.X);
		int gy = GeoEngine.getGeoY(location.Y);
		if (!GeoEngine.getInstance().hasGeo(location.X, location.Y))
		{
			return null;
		}
		int gz = GeoEngine.getInstance().getHeight(location);
		int gtx = GeoEngine.getGeoX(targetLocation.X);
		int gty = GeoEngine.getGeoY(targetLocation.Y);
		if (!GeoEngine.getInstance().hasGeo(targetLocation.X, targetLocation.Y))
		{
			return null;
		}
		int gtz = GeoEngine.getInstance().getHeight(targetLocation);
		CellNodeBuffer? buffer = alloc(64 + (2 * Math.Max(Math.Abs(gx - gtx), Math.Abs(gy - gty))), playable);
		if (buffer == null)
		{
			return null;
		}

		bool debug = Config.DEBUG_PATH && playable;

		if (debug)
		{
			if (_debugItems == null)
			{
				_debugItems = new();
			}
			else
			{
				foreach (Item item in _debugItems)
				{
					item.decayMe();
				}

				_debugItems.Clear();
			}
		}

		List<AbstractNodeLoc>? path = null;
		try
		{
			CellNode? result = buffer.findPath(gx, gy, gz, gtx, gty, gtz);

			if (debug)
			{
				foreach (CellNode n in buffer.debugPath())
				{
					if (n.getCost() < 0)
					{
                        NodeLoc loc = n.getLoc() ?? throw new InvalidOperationException();
						dropDebugItem(1831, (int) (-n.getCost() * 10), loc);
					}
					else
					{
						// Known nodes.
                        NodeLoc loc = n.getLoc() ?? throw new InvalidOperationException();
						dropDebugItem(57, (int) (n.getCost() * 10), loc);
					}
				}
			}

			if (result == null)
			{
				_findFails++;
				return null;
			}

			path = constructPath(result);
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
			return null;
		}
		finally
		{
			buffer.free();
		}

		if ((path.Count < 3) || (Config.MAX_POSTFILTER_PASSES <= 0))
		{
			_findSuccess++;
			return path;
		}

		DateTime timeStamp = DateTime.UtcNow;
		_postFilterUses++;
		if (playable)
		{
			_postFilterPlayableUses++;
		}

		int pass = 0;
		bool remove;
		do
		{
			pass++;
			_postFilterPasses++;

			remove = false;
			int middlePoint = 0;
			Location3D currentLoc = location;

			while (middlePoint < path.Count)
			{
				AbstractNodeLoc locMiddle = path[middlePoint];
				middlePoint++;
				if (middlePoint >= path.Count)
				{
					break;
				}

				AbstractNodeLoc locEnd = path[middlePoint];
				if (GeoEngine.getInstance().canMoveToTarget(currentLoc, locEnd.Location, instance))
				{
					path.RemoveAt(middlePoint);
					remove = true;
					if (debug)
					{
						dropDebugItem(735, 1, locMiddle);
					}
				}
				else
				{
					currentLoc = locMiddle.Location;
				}
			}
		}

		// Only one postfilter pass for AI.
		while (playable && remove && (path.Count > 2) && (pass < Config.MAX_POSTFILTER_PASSES));

		if (debug)
		{
			path.ForEach(n => dropDebugItem(1061, 1, n));
		}

		_findSuccess++;
		_postFilterElapsed += DateTime.UtcNow - timeStamp;
		return path;
	}

	private List<AbstractNodeLoc> constructPath(AbstractNode<NodeLoc> node)
	{
		List<AbstractNodeLoc> path = new();
		int previousDirectionX = int.MinValue;
		int previousDirectionY = int.MinValue;

        AbstractNode<NodeLoc>? tempNode = node;
		while (tempNode?.getParent() != null)
		{
            int directionX;
            int directionY;
            NodeLoc tempNodeLoc = tempNode.getLoc() ?? throw new InvalidOperationException();
            NodeLoc tempNodeParentLoc = tempNode.getParent()?.getLoc() ?? throw new InvalidOperationException();
            AbstractNode<NodeLoc>? tempNodeParentParent = tempNode.getParent()?.getParent();
            if (!Config.ADVANCED_DIAGONAL_STRATEGY && (tempNodeParentParent != null))
			{
                NodeLoc tempNodeParentParentLoc = tempNodeParentParent.getLoc() ?? throw new InvalidOperationException();
				int tmpX = tempNodeLoc.getNodeX() - tempNodeParentParentLoc.getNodeX();
				int tmpY = tempNodeLoc.getNodeY() - tempNodeParentParentLoc.getNodeY();
				if (Math.Abs(tmpX) == Math.Abs(tmpY))
				{
					directionX = tmpX;
					directionY = tmpY;
				}
				else
				{
					directionX = tempNodeLoc.getNodeX() - tempNodeParentLoc.getNodeX();
					directionY = tempNodeLoc.getNodeY() - tempNodeParentLoc.getNodeY();
				}
			}
			else
			{
				directionX = tempNodeLoc.getNodeX() - tempNodeParentLoc.getNodeX();
				directionY = tempNodeLoc.getNodeY() - tempNodeParentLoc.getNodeY();
			}

			// Only add a new route point if moving direction changes.
			if ((directionX != previousDirectionX) || (directionY != previousDirectionY))
			{
				previousDirectionX = directionX;
				previousDirectionY = directionY;

				path.Insert(0, tempNodeLoc); // TODO: very inefficient
				tempNode.setLoc(null);
			}

			tempNode = tempNode.getParent();
		}

		return path;
	}

	private CellNodeBuffer? alloc(int size, bool playable)
	{
		CellNodeBuffer? current = null;
		foreach (BufferInfo i in _allBuffers)
		{
			if (i.mapSize >= size)
			{
				foreach (CellNodeBuffer buf in i.bufs)
				{
					if (buf.@lock())
					{
						i.uses++;
						if (playable)
						{
							i.playableUses++;
						}
						i.elapsed += buf.getElapsedTime();
						current = buf;
						break;
					}
				}
				if (current != null)
				{
					break;
				}

				// Not found, allocate temporary buffer.
				current = new CellNodeBuffer(i.mapSize);
				current.@lock();
				if (i.bufs.Count < i.count)
				{
					i.bufs.Add(current);
					i.uses++;
					if (playable)
					{
						i.playableUses++;
					}
					break;
				}

				i.overflows++;
				if (playable)
				{
					i.playableOverflows++;
					// System.err.println("Overflow, size requested: " + size + " playable:"+playable);
				}
			}
		}

		return current;
	}

	private void dropDebugItem(int itemId, int num, AbstractNodeLoc loc)
	{
		Item item = new Item(IdManager.getInstance().getNextId(), itemId);
		item.setCount(num);
		item.spawnMe(loc.Location);
		_debugItems.Add(item);
	}

	private class BufferInfo
	{
		public int mapSize;
		public int count;
		public List<CellNodeBuffer> bufs;
		public int uses;
		public int playableUses;
		public int overflows;
		public int playableOverflows;
		public TimeSpan elapsed;

		public BufferInfo(int size, int cnt)
		{
			mapSize = size;
			count = cnt;
			bufs = new(count);
		}

		public override string ToString()
		{
			StringBuilder stat = new(100);
			stat.Append(mapSize);
			stat.Append('x');
			stat.Append(mapSize);
			stat.Append(" num:");
			stat.Append(bufs.Count);
			stat.Append('/');
			stat.Append(count);
			stat.Append(" uses:");
			stat.Append(uses);
			stat.Append('/');
			stat.Append(playableUses);
			if (uses > 0)
			{
				stat.Append(" total/avg(ms):");
				stat.Append(elapsed.ToString());
				stat.Append('/');
				stat.Append((elapsed / uses).ToString());
			}

			stat.Append(" ovf:");
			stat.Append(overflows);
			stat.Append('/');
			stat.Append(playableOverflows);

			return stat.ToString();
		}
	}

	public override string[] getStat()
	{
		string[] result = new string[_allBuffers.Length + 1];
		for (int i = 0; i < _allBuffers.Length; i++)
		{
			result[i] = _allBuffers[i].ToString();
		}

		StringBuilder stat = new(100);
		stat.Append("LOS postfilter uses:");
		stat.Append(_postFilterUses);
		stat.Append('/');
		stat.Append(_postFilterPlayableUses);
		if (_postFilterUses > 0)
		{
			stat.Append(" total/avg(ms):");
			stat.Append(_postFilterElapsed.ToString());
			stat.Append('/');
			stat.Append((_postFilterElapsed / _postFilterUses).ToString());
			stat.Append(" passes total/avg:");
			stat.Append(_postFilterPasses);
			stat.Append('/');
			stat.Append((double)_postFilterPasses / _postFilterUses);
			stat.AppendLine();
		}

		stat.Append("Pathfind success/fail:");
		stat.Append(_findSuccess);
		stat.Append('/');
		stat.Append(_findFails);
		result[^1] = stat.ToString();

		return result;
	}

	public new static CellPathFinding getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static CellPathFinding INSTANCE = new CellPathFinding();
	}
}