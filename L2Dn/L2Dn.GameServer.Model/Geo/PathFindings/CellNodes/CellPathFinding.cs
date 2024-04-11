using System.Text;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Geo.PathFindings.CellNodes;

/**
 * @author Sami, DS Credits to Diamond
 */
public class CellPathFinding: PathFinding
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CellPathFinding));
	
	private BufferInfo[] _allBuffers;
	private int _findSuccess = 0;
	private int _findFails = 0;
	private int _postFilterUses = 0;
	private int _postFilterPlayableUses = 0;
	private int _postFilterPasses = 0;
	private TimeSpan _postFilterElapsed;
	
	private List<Item> _debugItems = null;
	
	protected CellPathFinding()
	{
		try
		{
			String[] array = Config.PATHFIND_BUFFERS.Split(";");
			_allBuffers = new BufferInfo[array.Length];
			
			String buf;
			String[] args;
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
	
	public override List<AbstractNodeLoc> findPath(int x, int y, int z, int tx, int ty, int tz, Instance instance, bool playable)
	{
		int gx = GeoEngine.getGeoX(x);
		int gy = GeoEngine.getGeoY(y);
		if (!GeoEngine.getInstance().hasGeo(x, y))
		{
			return null;
		}
		int gz = GeoEngine.getInstance().getHeight(x, y, z);
		int gtx = GeoEngine.getGeoX(tx);
		int gty = GeoEngine.getGeoY(ty);
		if (!GeoEngine.getInstance().hasGeo(tx, ty))
		{
			return null;
		}
		int gtz = GeoEngine.getInstance().getHeight(tx, ty, tz);
		CellNodeBuffer buffer = alloc(64 + (2 * Math.Max(Math.Abs(gx - gtx), Math.Abs(gy - gty))), playable);
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
		
		List<AbstractNodeLoc> path = null;
		try
		{
			CellNode result = buffer.findPath(gx, gy, gz, gtx, gty, gtz);
			
			if (debug)
			{
				foreach (CellNode n in buffer.debugPath())
				{
					if (n.getCost() < 0)
					{
						dropDebugItem(1831, (int) (-n.getCost() * 10), n.getLoc());
					}
					else
					{
						// Known nodes.
						dropDebugItem(57, (int) (n.getCost() * 10), n.getLoc());
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
		
		if ((path.size() < 3) || (Config.MAX_POSTFILTER_PASSES <= 0))
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
		
		int middlePoint;
		int currentX;
		int currentY;
		int currentZ;
		int pass = 0;
		bool remove;
		do
		{
			pass++;
			_postFilterPasses++;
			
			remove = false;
			middlePoint = 0;
			currentX = x;
			currentY = y;
			currentZ = z;
			
			while (middlePoint < path.Count)
			{
				AbstractNodeLoc locMiddle = path[middlePoint];
				middlePoint++;
				if (middlePoint >= path.Count)
				{
					break;
				}
				
				AbstractNodeLoc locEnd = path[middlePoint];
				if (GeoEngine.getInstance().canMoveToTarget(currentX, currentY, currentZ, locEnd.getX(), locEnd.getY(), locEnd.getZ(), instance))
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
					currentX = locMiddle.getX();
					currentY = locMiddle.getY();
					currentZ = locMiddle.getZ();
				}
			}
		}
		// Only one postfilter pass for AI.
		while (playable && remove && (path.size() > 2) && (pass < Config.MAX_POSTFILTER_PASSES));
		
		if (debug)
		{
			path.forEach(n => dropDebugItem(1061, 1, n));
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
		int directionX;
		int directionY;
		
		AbstractNode<NodeLoc> tempNode = node;
		while (tempNode.getParent() != null)
		{
			if (!Config.ADVANCED_DIAGONAL_STRATEGY && (tempNode.getParent().getParent() != null))
			{
				int tmpX = tempNode.getLoc().getNodeX() - tempNode.getParent().getParent().getLoc().getNodeX();
				int tmpY = tempNode.getLoc().getNodeY() - tempNode.getParent().getParent().getLoc().getNodeY();
				if (Math.Abs(tmpX) == Math.Abs(tmpY))
				{
					directionX = tmpX;
					directionY = tmpY;
				}
				else
				{
					directionX = tempNode.getLoc().getNodeX() - tempNode.getParent().getLoc().getNodeX();
					directionY = tempNode.getLoc().getNodeY() - tempNode.getParent().getLoc().getNodeY();
				}
			}
			else
			{
				directionX = tempNode.getLoc().getNodeX() - tempNode.getParent().getLoc().getNodeX();
				directionY = tempNode.getLoc().getNodeY() - tempNode.getParent().getLoc().getNodeY();
			}
			
			// Only add a new route point if moving direction changes.
			if ((directionX != previousDirectionX) || (directionY != previousDirectionY))
			{
				previousDirectionX = directionX;
				previousDirectionY = directionY;
				
				path.Insert(0, tempNode.getLoc()); // TODO: very inefficient
				tempNode.setLoc(null);
			}
			
			tempNode = tempNode.getParent();
		}
		
		return path;
	}
	
	private CellNodeBuffer alloc(int size, bool playable)
	{
		CellNodeBuffer current = null;
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
				if (i.bufs.size() < i.count)
				{
					i.bufs.add(current);
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
		item.spawnMe(loc.getX(), loc.getY(), loc.getZ());
		_debugItems.add(item);
	}
	
	private class BufferInfo
	{
		public int mapSize;
		public int count;
		public List<CellNodeBuffer> bufs;
		public int uses = 0;
		public int playableUses = 0;
		public int overflows = 0;
		public int playableOverflows = 0;
		public TimeSpan elapsed;
		
		public BufferInfo(int size, int cnt)
		{
			mapSize = size;
			count = cnt;
			bufs = new(count);
		}
		
		public override String ToString()
		{
			StringBuilder stat = new StringBuilder(100);
			StringUtil.append(stat, mapSize.ToString(), "x", mapSize.ToString(), " num:", bufs.size().ToString(), "/", count.ToString(), " uses:", uses.ToString(), "/", playableUses.ToString());
			if (uses > 0)
			{
				StringUtil.append(stat, " total/avg(ms):", elapsed.ToString(), "/", (elapsed / uses).ToString());
			}
			
			StringUtil.append(stat, " ovf:", overflows.ToString(), "/", playableOverflows.ToString());
			
			return stat.ToString();
		}
	}
	
	public override String[] getStat()
	{
		String[] result = new String[_allBuffers.Length + 1];
		for (int i = 0; i < _allBuffers.Length; i++)
		{
			result[i] = _allBuffers[i].ToString();
		}
		
		StringBuilder stat = new StringBuilder(100);
		StringUtil.append(stat, "LOS postfilter uses:", _postFilterUses.ToString(), "/", _postFilterPlayableUses.ToString());
		if (_postFilterUses > 0)
		{
			StringUtil.append(stat, " total/avg(ms):", _postFilterElapsed.ToString(), "/", (_postFilterElapsed / _postFilterUses).ToString(), 
				" passes total/avg:", _postFilterPasses.ToString(), "/", ((double) _postFilterPasses / _postFilterUses).ToString(), Environment.NewLine);
		}
		StringUtil.append(stat, "Pathfind success/fail:", _findSuccess.ToString(), "/", _findFails.ToString());
		result[result.Length - 1] = stat.ToString();
		
		return result;
	}
	
	public static CellPathFinding getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static CellPathFinding INSTANCE = new CellPathFinding();
	}
}