namespace L2Dn.GameServer.Geo.GeoDataImpl;

/**
 * @author HorridoJoho
 */
public interface IBlock
{
	public const int TYPE_FLAT = 0;
	public const int TYPE_COMPLEX = 1;
	public const int TYPE_MULTILAYER = 2;
	
	/** Cells in a block on the x axis */
	public const int BLOCK_CELLS_X = 8;
	/** Cells in a block on the y axis */
	public const int BLOCK_CELLS_Y = 8;
	/** Cells in a block */
	public const int BLOCK_CELLS = BLOCK_CELLS_X * BLOCK_CELLS_Y;
	
	bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe);
	
	int getNearestZ(int geoX, int geoY, int worldZ);
	
	int getNextLowerZ(int geoX, int geoY, int worldZ);
	
	int getNextHigherZ(int geoX, int geoY, int worldZ);
}