namespace L2Dn.GameServer.Geo.PathFindings.GeoNodes;

/**
 * @author -Nemesiss-
 */
public class GeoNode: AbstractNode<GeoNodeLoc>
{
	private readonly int _neighborsIdx;
	private short _cost;
	private GeoNode[]? _neighbors;

	public GeoNode(GeoNodeLoc loc, int neighborsIdx): base(loc)
	{
		_neighborsIdx = neighborsIdx;
	}

	public short getCost()
	{
		return _cost;
	}

	public void setCost(int cost)
	{
		_cost = (short) cost;
	}

	public GeoNode[]? getNeighbors()
	{
		return _neighbors;
	}

	public void attachNeighbors(GeoNode[] neighbors)
	{
		_neighbors = neighbors;
	}

	public int getNeighborsIdx()
	{
		return _neighborsIdx;
	}
}