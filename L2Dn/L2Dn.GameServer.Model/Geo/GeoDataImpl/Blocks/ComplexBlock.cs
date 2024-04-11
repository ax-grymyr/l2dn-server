namespace L2Dn.GameServer.Geo.GeoDataImpl.Blocks;

/**
 * @author HorridoJoho
 */
public class ComplexBlock: IBlock
{
	private readonly short[] _data;
	
	internal ComplexBlock(GeoReader reader)
	{
		_data = new short[IBlock.BLOCK_CELLS];
		for (int cellOffset = 0; cellOffset < IBlock.BLOCK_CELLS; cellOffset++)
		{
			_data[cellOffset] = reader.ReadInt16();
		}
	}
	
	private short getCellData(int geoX, int geoY)
	{
		return _data[((geoX % IBlock.BLOCK_CELLS_X) * IBlock.BLOCK_CELLS_Y) + (geoY % IBlock.BLOCK_CELLS_Y)];
	}
	
	private byte getCellNSWE(int geoX, int geoY)
	{
		return (byte) (getCellData(geoX, geoY) & 0x000F);
	}
	
	private int getCellHeight(int geoX, int geoY)
	{
		return (short) (getCellData(geoX, geoY) & 0x0FFF0) >> 1;
	}
	
	public bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe)
	{
		return (getCellNSWE(geoX, geoY) & nswe) == nswe;
	}
	
	public void setNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		byte currentNswe = getCellNSWE(geoX, geoY);
		if ((currentNswe & nswe) == 0)
		{
			short currentHeight = (short) getCellHeight(geoX, geoY);
			short encodedHeight = (short) (currentHeight << 1); // Shift left by 1 bit.
			short newNswe = (short) (currentNswe | nswe); // Add NSWE.
			short newCombinedData = (short) (encodedHeight | newNswe); // Combine height and NSWE.
			_data[((geoX % IBlock.BLOCK_CELLS_X) * IBlock.BLOCK_CELLS_Y) + (geoY % IBlock.BLOCK_CELLS_Y)] = (short) (newCombinedData & 0xffff);
		}
	}
	
	public void unsetNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		byte currentNswe = getCellNSWE(geoX, geoY);
		if ((currentNswe & nswe) != 0)
		{
			short currentHeight = (short) getCellHeight(geoX, geoY);
			short encodedHeight = (short) (currentHeight << 1); // Shift left by 1 bit.
			short newNswe = (short) (currentNswe & ~nswe); // Subtract NSWE.
			short newCombinedData = (short) (encodedHeight | newNswe); // Combine height and NSWE.
			_data[((geoX % IBlock.BLOCK_CELLS_X) * IBlock.BLOCK_CELLS_Y) + (geoY % IBlock.BLOCK_CELLS_Y)] = (short) (newCombinedData & 0xffff);
		}
	}
	
	public int getNearestZ(int geoX, int geoY, int worldZ)
	{
		return getCellHeight(geoX, geoY);
	}
	
	public int getNextLowerZ(int geoX, int geoY, int worldZ)
	{
		int cellHeight = getCellHeight(geoX, geoY);
		return cellHeight <= worldZ ? cellHeight : worldZ;
	}
	
	public int getNextHigherZ(int geoX, int geoY, int worldZ)
	{
		int cellHeight = getCellHeight(geoX, geoY);
		return cellHeight >= worldZ ? cellHeight : worldZ;
	}
	
	public short[] getData()
	{
		return _data;
	}
}