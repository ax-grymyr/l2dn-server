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

	public void setNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		throw new InvalidOperationException("Cannot set NSWE on a flat block!");
	}
	
	public void unsetNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		throw new InvalidOperationException("Cannot unset NSWE on a flat block!");
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
	
	public short getHeight()
	{
		return _height;
	}
}