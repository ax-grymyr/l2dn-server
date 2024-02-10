using L2Dn.GameServer.Geo.GeoDataImpl.Blocks;

namespace L2Dn.GameServer.Geo.GeoDataImpl.Regions;

/**
 * @author HorridoJoho
 */
public class Region: IRegion
{
	private readonly IBlock[] _blocks = new IBlock[IRegion.REGION_BLOCKS];
	
	internal Region(GeoReader reader)
	{
		for (int blockOffset = 0; blockOffset < IRegion.REGION_BLOCKS; blockOffset++)
		{
			int blockType = reader.ReadByte();
			switch (blockType)
			{
				case IBlock.TYPE_FLAT:
				{
					_blocks[blockOffset] = new FlatBlock(reader);
					break;
				}
				case IBlock.TYPE_COMPLEX:
				{
					_blocks[blockOffset] = new ComplexBlock(reader);
					break;
				}
				case IBlock.TYPE_MULTILAYER:
				{
					_blocks[blockOffset] = new MultilayerBlock(reader);
					break;
				}
				default:
				{
					throw new InvalidOperationException("Invalid block type " + blockType + "!");
				}
			}
		}
	}
	
	private IBlock getBlock(int geoX, int geoY)
	{
		return _blocks[(((geoX / IBlock.BLOCK_CELLS_X) % IRegion.REGION_BLOCKS_X) * IRegion.REGION_BLOCKS_Y) + ((geoY / IBlock.BLOCK_CELLS_Y) % IRegion.REGION_BLOCKS_Y)];
	}
	
	public bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe)
	{
		return getBlock(geoX, geoY).checkNearestNswe(geoX, geoY, worldZ, nswe);
	}
	
	public int getNearestZ(int geoX, int geoY, int worldZ)
	{
		return getBlock(geoX, geoY).getNearestZ(geoX, geoY, worldZ);
	}
	
	public int getNextLowerZ(int geoX, int geoY, int worldZ)
	{
		return getBlock(geoX, geoY).getNextLowerZ(geoX, geoY, worldZ);
	}
	
	public int getNextHigherZ(int geoX, int geoY, int worldZ)
	{
		return getBlock(geoX, geoY).getNextHigherZ(geoX, geoY, worldZ);
	}
	
	public bool hasGeo()
	{
		return true;
	}
}