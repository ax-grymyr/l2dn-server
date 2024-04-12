using System.IO.Compression;
using L2Dn.GameServer.Geo.GeoDataImpl.Regions;

namespace L2Dn.GameServer.Geo.GeoDataImpl;

/**
 * @author HorridoJoho
 */
public class GeoData
{
	// World dimensions: 1048576 * 1048576 = 1099511627776
	private const int WORLD_MIN_X = -655360;
	private const int WORLD_MIN_Y = -589824;
	//private const int WORLD_MAX_X = 393215;
	//private const int WORLD_MAX_Y = 458751;
	
	/** Regions in the world on the x axis */
	public const int GEO_REGIONS_X = 32;
	/** Regions in the world on the y axis */
	public const int GEO_REGIONS_Y = 32;
	/** Region in the world */
	public const int GEO_REGIONS = GEO_REGIONS_X * GEO_REGIONS_Y;
	
	/** Blocks in the world on the x axis */
	//public const int GEO_BLOCKS_X = GEO_REGIONS_X * IRegion.REGION_BLOCKS_X;
	/** Blocks in the world on the y axis */
	//public const int GEO_BLOCKS_Y = GEO_REGIONS_Y * IRegion.REGION_BLOCKS_Y;
	/** Blocks in the world */
	//public const int GEO_BLOCKS = GEO_REGIONS * IRegion.REGION_BLOCKS;
	
	/** Cells in the world on the x axis */
	//public const int GEO_CELLS_X = GEO_BLOCKS_X * IBlock.BLOCK_CELLS_X;
	/** Cells in the world in the y axis */
	//public const int GEO_CELLS_Y = GEO_BLOCKS_Y * IBlock.BLOCK_CELLS_Y;
	
	/** The regions array */
	private readonly IRegion[] _regions = new IRegion[GEO_REGIONS];
	
	public GeoData()
	{
		for (int i = 0; i < _regions.Length; i++)
		{
			_regions[i] = NullRegion.INSTANCE;
		}
	}
	
	// private void checkGeoX(int geoX)
	// {
	// 	if ((geoX < 0) || (geoX >= GEO_CELLS_X))
	// 	{
	// 		throw new ArgumentException();
	// 	}
	// }
	//
	// private void checkGeoY(int geoY)
	// {
	// 	if ((geoY < 0) || (geoY >= GEO_CELLS_Y))
	// 	{
	// 		throw new ArgumentException();
	// 	}
	// }
	
	public IRegion getRegion(int geoX, int geoY)
	{
		// checkGeoX(geoX);
		// checkGeoY(geoY);
		return _regions[geoX / IRegion.REGION_CELLS_X * GEO_REGIONS_Y + geoY / IRegion.REGION_CELLS_Y];
	}
	
	public void setRegion(int regionX, int regionY, Region region)
	{
		int regionOffset = regionX * GEO_REGIONS_Y + regionY;
		_regions[regionOffset] = region;
	}
	
	public void loadRegion(string filePath, int regionX, int regionY)
	{
		int regionOffset = regionX * GEO_REGIONS_Y + regionY;
		
		Stream stream;
		if (filePath.EndsWith(".gz"))
		{
			using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using GZipStream gZipStream = new(fileStream, CompressionMode.Decompress);
			stream = new MemoryStream();
			gZipStream.CopyTo(stream);
			stream.Position = 0;
		}
		else 
			stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

		using (stream)
		{
			GeoReader reader = new(stream);
			_regions[regionOffset] = new Region(reader);
		}
	}
	
	public void unloadRegion(int regionX, int regionY)
	{
		_regions[regionX * GEO_REGIONS_Y + regionY] = NullRegion.INSTANCE;
	}
	
	public bool hasGeoPos(int geoX, int geoY)
	{
		return getRegion(geoX, geoY).hasGeo();
	}
	
	public bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe)
	{
		return getRegion(geoX, geoY).checkNearestNswe(geoX, geoY, worldZ, nswe);
	}
	
	public void setNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		getRegion(geoX, geoY).setNearestNswe(geoX, geoY, worldZ, nswe);
	}
	
	public void unsetNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		getRegion(geoX, geoY).unsetNearestNswe(geoX, geoY, worldZ, nswe);
	}
	
	public int getNearestZ(int geoX, int geoY, int worldZ)
	{
		return getRegion(geoX, geoY).getNearestZ(geoX, geoY, worldZ);
	}
	
	public int getNextLowerZ(int geoX, int geoY, int worldZ)
	{
		return getRegion(geoX, geoY).getNextLowerZ(geoX, geoY, worldZ);
	}
	
	public int getNextHigherZ(int geoX, int geoY, int worldZ)
	{
		return getRegion(geoX, geoY).getNextHigherZ(geoX, geoY, worldZ);
	}
	
	public static int getGeoX(int worldX)
	{
		// if ((worldX < WORLD_MIN_X) || (worldX > WORLD_MAX_X))
		// {
		// 	throw new ArgumentException();
		// }
		return (worldX - WORLD_MIN_X) / 16;
	}
	
	public static int getGeoY(int worldY)
	{
		// if ((worldY < WORLD_MIN_Y) || (worldY > WORLD_MAX_Y))
		// {
		// 	throw new ArgumentException();
		// }
		return (worldY - WORLD_MIN_Y) / 16;
	}
	
	public static int getWorldX(int geoX)
	{
		//checkGeoX(geoX);
		return geoX * 16 + WORLD_MIN_X + 8;
	}
	
	public static int getWorldY(int geoY)
	{
		//checkGeoY(geoY);
		return geoY * 16 + WORLD_MIN_Y + 8;
	}
}