using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Geo.PathFindings.CellNodes;

/**
 * @author DS Credits to Diamond
 */
public class CellNodeBuffer
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(CellNodeBuffer));
	private const int MAX_ITERATIONS = 3500;

	private readonly object _lock = new();
	private readonly int _mapSize;
	private readonly CellNode?[][] _buffer;

	private int _baseX;
	private int _baseY;

	private int _targetX;
	private int _targetY;
	private int _targetZ;

	private DateTime _timeStamp;
	private TimeSpan _lastElapsedTime;

	private CellNode? _current;

	public CellNodeBuffer(int size)
	{
		_mapSize = size;
		_buffer = new CellNode[_mapSize][];
		for (int i = 0; i < _buffer.Length; i++)
		{
			_buffer[i] = new CellNode[_mapSize];
		}
	}

	public bool @lock()
	{
		return Monitor.TryEnter(_lock);
	}

	public CellNode? findPath(int x, int y, int z, int tx, int ty, int tz)
	{
		_timeStamp = DateTime.UtcNow;
		_baseX = x + (tx - x - _mapSize) / 2; // Middle of the line (x,y) - (tx,ty).
		_baseY = y + (ty - y - _mapSize) / 2; // Will be in the center of the buffer.
		_targetX = tx;
		_targetY = ty;
		_targetZ = tz;
		_current = getNode(x, y, z);
		_current!.setCost(getCost(x, y, z, Config.HIGH_WEIGHT)); // TODO

		for (int count = 0; count < MAX_ITERATIONS; count++)
        {
            NodeLoc currentLoc = _current?.getLoc() ?? throw new InvalidOperationException();
            if (currentLoc.getNodeX() == _targetX && currentLoc.getNodeY() == _targetY &&
                Math.Abs(currentLoc.Z - _targetZ) < 64)
            {
                return _current; // Found.
            }

            getNeighbors();
			if (_current.getNext() == null)
			{
				return null; // No more ways.
			}

			_current = _current.getNext();
		}
		return null;
	}

	public void free()
	{
		_current = null;

		CellNode? node;
		for (int i = 0; i < _mapSize; i++)
		{
			for (int j = 0; j < _mapSize; j++)
			{
				node = _buffer[i][j];
				if (node != null)
				{
					node.free();
				}
			}
		}

		if (Monitor.IsEntered(_lock))
		{
			Monitor.Exit(_lock);
		}

		_lastElapsedTime = DateTime.UtcNow - _timeStamp;
	}

	public TimeSpan getElapsedTime()
	{
		return _lastElapsedTime;
	}

	public List<CellNode> debugPath()
	{
		List<CellNode> result = new();
		for (CellNode? n = _current; n?.getParent() != null; n = (CellNode?)n.getParent())
		{
			result.Add(n);
			n.setCost(-n.getCost());
		}

		for (int i = 0; i < _mapSize; i++)
		{
			for (int j = 0; j < _mapSize; j++)
			{
				CellNode? n = _buffer[i][j];
				if (n == null || !n.isInUse() || n.getCost() <= 0)
				{
					continue;
				}

				result.Add(n);
			}
		}
		return result;
	}

	private void getNeighbors()
    {
        NodeLoc currentLoc = _current?.getLoc() ?? throw new InvalidOperationException();
		if (currentLoc.canGoNone())
		{
			return;
		}

		int x = currentLoc.getNodeX();
		int y = currentLoc.getNodeY();
		int z = currentLoc.Z;

		CellNode? nodeE = null;
		CellNode? nodeS = null;
		CellNode? nodeW = null;
		CellNode? nodeN = null;

		// East
		if (currentLoc.canGoEast())
		{
			nodeE = addNode(x + 1, y, z, false);
		}

		// South
		if (currentLoc.canGoSouth())
		{
			nodeS = addNode(x, y + 1, z, false);
		}

		// West
		if (currentLoc.canGoWest())
		{
			nodeW = addNode(x - 1, y, z, false);
		}

		// North
		if (currentLoc.canGoNorth())
		{
			nodeN = addNode(x, y - 1, z, false);
		}

		if (!Config.ADVANCED_DIAGONAL_STRATEGY)
		{
			return;
		}

		// SouthEast
		if (nodeE != null && nodeS != null && nodeE.getLoc()?.canGoSouth() == true && nodeS.getLoc()?.canGoEast() == true)
		{
			addNode(x + 1, y + 1, z, true);
		}

		// SouthWest
		if (nodeS != null && nodeW != null && nodeW.getLoc()?.canGoSouth() == true && nodeS.getLoc()?.canGoWest() == true)
		{
			addNode(x - 1, y + 1, z, true);
		}

		// NorthEast
		if (nodeN != null && nodeE != null && nodeE.getLoc()?.canGoNorth() == true && nodeN.getLoc()?.canGoEast() == true)
		{
			addNode(x + 1, y - 1, z, true);
		}

		// NorthWest
		if (nodeN != null && nodeW != null && nodeW.getLoc()?.canGoNorth() == true && nodeN.getLoc()?.canGoWest() == true)
		{
			addNode(x - 1, y - 1, z, true);
		}
	}

	private CellNode? getNode(int x, int y, int z)
	{
		int aX = x - _baseX;
		if (aX < 0 || aX >= _mapSize)
		{
			return null;
		}

		int aY = y - _baseY;
		if (aY < 0 || aY >= _mapSize)
		{
			return null;
		}

		CellNode? result = _buffer[aX][aY];
		if (result == null)
		{
			result = new CellNode(new NodeLoc(x, y, z));
			_buffer[aX][aY] = result;
		}
		else if (!result.isInUse())
		{
			result.setInUse();
			// Re-init node if needed.
            NodeLoc? resultLoc = result.getLoc();
			if (resultLoc != null)
			{
                resultLoc.set(x, y, z);
			}
			else
			{
				result.setLoc(new NodeLoc(x, y, z));
			}
		}

		return result;
	}

	private CellNode? addNode(int x, int y, int z, bool diagonal)
	{
		CellNode? newNode = getNode(x, y, z);
		if (newNode == null)
		{
			return null;
		}
		if (newNode.getCost() >= 0)
		{
			return newNode;
		}

        NodeLoc newNodeLoc = newNode.getLoc() ?? throw new InvalidOperationException();
		int geoZ = newNodeLoc.Z;

        NodeLoc currentLoc = _current?.getLoc() ?? throw new InvalidOperationException();
		int stepZ = Math.Abs(geoZ - currentLoc.Z);
		float weight = diagonal ? Config.DIAGONAL_WEIGHT : Config.LOW_WEIGHT;

		if (!newNodeLoc.canGoAll() || stepZ > 16)
		{
			weight = Config.HIGH_WEIGHT;
		}
		else if (isHighWeight(x + 1, y, geoZ))
		{
			weight = Config.MEDIUM_WEIGHT;
		}
		else if (isHighWeight(x - 1, y, geoZ))
		{
			weight = Config.MEDIUM_WEIGHT;
		}
		else if (isHighWeight(x, y + 1, geoZ))
		{
			weight = Config.MEDIUM_WEIGHT;
		}
		else if (isHighWeight(x, y - 1, geoZ))
		{
			weight = Config.MEDIUM_WEIGHT;
		}

		newNode.setParent(_current);
		newNode.setCost(getCost(x, y, geoZ, weight));

		CellNode node = _current;
		int count = 0;
		while (node.getNext() != null && count < MAX_ITERATIONS * 4)
		{
			count++;
			if (node.getNext()!.getCost() > newNode.getCost())
			{
				// Insert node into a chain.
				newNode.setNext(node.getNext());
				break;
			}
			node = node.getNext()!;
		}
		if (count == MAX_ITERATIONS * 4)
		{
			_logger.Error("Pathfinding: too long loop detected, cost:" + newNode.getCost());
		}

		node.setNext(newNode); // Add last.

		return newNode;
	}

	private bool isHighWeight(int x, int y, int z)
	{
		CellNode? result = getNode(x, y, z);
        NodeLoc? resultLoc = result?.getLoc();
		return result == null || resultLoc == null || !resultLoc.canGoAll() || Math.Abs(resultLoc.Z - z) > 16;
	}

	private double getCost(int x, int y, int z, float weight)
	{
		int dX = x - _targetX;
		int dY = y - _targetY;
		int dZ = z - _targetZ;
		// Math.abs(dx) + Math.abs(dy) + Math.abs(dz) / 16
		double result = Math.Sqrt(dX * dX + dY * dY + dZ * dZ / 256.0);
		if (result > weight)
		{
			result += weight;
		}

		if (result > float.MaxValue)
		{
			result = float.MaxValue;
		}

		return result;
	}
}