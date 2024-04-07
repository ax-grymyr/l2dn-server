namespace L2Dn.GameServer.Geo.GeoDataImpl;

/**
 * @author HorridoJoho
 */
public interface IRegion
{
	/** Blocks in a region on the x axis. */
	public const int REGION_BLOCKS_X = 256;
	/** Blocks in a region on the y axis. */
	public const int REGION_BLOCKS_Y = 256;
	/** Blocks in a region. */
	public const int REGION_BLOCKS = REGION_BLOCKS_X * REGION_BLOCKS_Y;
	
	/** Cells in a region on the x axis. */
	public const int REGION_CELLS_X = REGION_BLOCKS_X * IBlock.BLOCK_CELLS_X;
	/** Cells in a region on the y axis. */
	public const int REGION_CELLS_Y = REGION_BLOCKS_Y * IBlock.BLOCK_CELLS_Y;
	/** Cells in a region. */
	public const int REGION_CELLS = REGION_CELLS_X * REGION_CELLS_Y;
	
	bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe);
	
	void setNearestNswe(int geoX, int geoY, int worldZ, byte nswe);
	
	void unsetNearestNswe(int geoX, int geoY, int worldZ, byte nswe);
	
	int getNearestZ(int geoX, int geoY, int worldZ);
	
	int getNextLowerZ(int geoX, int geoY, int worldZ);
	
	int getNextHigherZ(int geoX, int geoY, int worldZ);
	
	bool hasGeo();

	bool saveToFile(string fileName);
}