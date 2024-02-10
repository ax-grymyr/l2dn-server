namespace L2Dn.GameServer.Geo.GeoDataImpl.Blocks;

/**
 * @author HorridoJoho
 */
public class FlatBlock: IBlock
{
	private readonly short _height;
	
	internal FlatBlock(GeoReader reader)
	{
		_height = reader.ReadInt16();
	}
	
	public bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe)
	{
		return true;
	}
	
	public int getNearestZ(int geoX, int geoY, int worldZ)
	{
		return _height;
	}
	
	public int getNextLowerZ(int geoX, int geoY, int worldZ)
	{
		return _height <= worldZ ? _height : worldZ;
	}
	
	public int getNextHigherZ(int geoX, int geoY, int worldZ)
	{
		return _height >= worldZ ? _height : worldZ;
	}
}