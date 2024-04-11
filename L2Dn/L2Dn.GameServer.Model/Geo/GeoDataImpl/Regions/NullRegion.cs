namespace L2Dn.GameServer.Geo.GeoDataImpl.Regions;

/**
 * @author HorridoJoho
 */
public class NullRegion: IRegion
{
	public static readonly NullRegion INSTANCE = new NullRegion();
	
	public bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe)
	{
		return true;
	}
	
	public void setNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
	}
	
	public void unsetNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
	}
	
	public int getNearestZ(int geoX, int geoY, int worldZ)
	{
		return worldZ;
	}
	
	public int getNextLowerZ(int geoX, int geoY, int worldZ)
	{
		return worldZ;
	}
	
	public int getNextHigherZ(int geoX, int geoY, int worldZ)
	{
		return worldZ;
	}
	
	public bool hasGeo()
	{
		return false;
	}

	public bool saveToFile(string fileName)
	{
		return false;
	}
}