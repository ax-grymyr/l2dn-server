namespace L2Dn.GameServer.Geo.PathFindings;

/**
 * @author -Nemesiss-
 */
public abstract class AbstractNodeLoc
{
	public abstract int getX();
	
	public abstract int getY();
	
	public abstract int getZ();
	
	public abstract void setZ(short z);
	
	public abstract int getNodeX();
	
	public abstract int getNodeY();
}