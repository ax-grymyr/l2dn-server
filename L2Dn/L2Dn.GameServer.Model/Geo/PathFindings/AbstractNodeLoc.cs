using L2Dn.Geometry;

namespace L2Dn.GameServer.Geo.PathFindings;

/**
 * @author -Nemesiss-
 */
public abstract class AbstractNodeLoc
{
	public abstract Location3D Location { get; }
	public abstract int Z { get; }

	public abstract int getNodeX();

	public abstract int getNodeY();
}