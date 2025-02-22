using L2Dn.GameServer.Geo.PathFindings.CellNodes;
using L2Dn.GameServer.Geo.PathFindings.GeoNodes;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Geo.PathFindings;

/**
 * @author -Nemesiss-
 */
public abstract class PathFinding
{
	public static PathFinding getInstance()
	{
		return Config.PATHFINDING == 1 ? GeoPathFinding.getInstance() : CellPathFinding.getInstance();
	}

	public abstract bool pathNodesExist(short regionoffset);

	public abstract List<AbstractNodeLoc>? findPath(Location3D location, Location3D targetLocation, Instance? instance, bool playable);

	/**
	 * Convert geodata position to pathnode position
	 * @param geoPos
	 * @return pathnode position
	 */
	public short getNodePos(int geoPos)
	{
		return (short) (geoPos >> 3); // OK?
	}

	/**
	 * Convert node position to pathnode block position
	 * @param nodePos
	 * @return pathnode block position (0...255)
	 */
	public short getNodeBlock(int nodePos)
	{
		return (short) (nodePos % 256);
	}

	public byte getRegionX(int nodePos)
	{
		return (byte) ((nodePos >> 8) + WorldMap.TileXMin);
	}

	public byte getRegionY(int nodePos)
	{
		return (byte) ((nodePos >> 8) + WorldMap.TileYMin);
	}

	public short getRegionOffset(byte rx, byte ry)
	{
		return (short) ((rx << 5) + ry);
	}

	/**
	 * Convert pathnode x to World x position
	 * @param nodeX rx
	 * @return
	 */
	public int calculateWorldX(short nodeX)
	{
		return WorldMap.WorldXMin + (nodeX * 128) + 48;
	}

	/**
	 * Convert pathnode y to World y position
	 * @param nodeY
	 * @return
	 */
	public int calculateWorldY(short nodeY)
	{
		return WorldMap.WorldYMin + (nodeY * 128) + 48;
	}

	public virtual string[]? getStat()
	{
		return null;
	}
}