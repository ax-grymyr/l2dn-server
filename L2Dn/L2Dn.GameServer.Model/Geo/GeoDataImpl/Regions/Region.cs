using L2Dn.Conversion;
using L2Dn.GameServer.Geo.GeoDataImpl.Blocks;
using L2Dn.IO;
using Microsoft.Extensions.Logging;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Geo.GeoDataImpl.Regions;

/**
 * @author HorridoJoho
 */
public class Region: IRegion
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(Region));
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

	public void setNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		IBlock block = getBlock(geoX, geoY);

		// Flat block cells are enabled by default on all directions.
		if (block is FlatBlock)
		{
			// convertFlatToComplex(block, geoX, geoY);
			return;
		}

		getBlock(geoX, geoY).setNearestNswe(geoX, geoY, worldZ, nswe);
	}

	public void unsetNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		IBlock block = getBlock(geoX, geoY);

		// Flat blocks are by default enabled on all locations.
		if (block is FlatBlock)
		{
			convertFlatToComplex(block, geoX, geoY);
		}

		getBlock(geoX, geoY).unsetNearestNswe(geoX, geoY, worldZ, nswe);
	}

	private void convertFlatToComplex(IBlock block, int geoX, int geoY)
	{
		short currentHeight = ((FlatBlock)block).getHeight();
		short encodedHeight = (short) ((currentHeight << 1) & 0xffff);
		short combinedData = (short) (encodedHeight | Cell.NSWE_ALL);

		using MemoryStream memoryStream = new MemoryStream();
		BinaryWriter<LittleEndianBitConverter> writer = new(memoryStream);
		for (int i = 0; i < IBlock.BLOCK_CELLS; i++)
			writer.WriteInt16(combinedData);

		memoryStream.Position = 0;
		GeoReader reader = new GeoReader(memoryStream);

		_blocks[
			(((geoX / IBlock.BLOCK_CELLS_X) % IRegion.REGION_BLOCKS_X) * IRegion.REGION_BLOCKS_Y) +
			((geoY / IBlock.BLOCK_CELLS_Y) % IRegion.REGION_BLOCKS_Y)] = new ComplexBlock(reader);
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
	/**
	 * Saves this region to a file.
	 * @param fileName the target file name.
	 * @return true if the file was saved successfully, false otherwise.
	 */
	public bool saveToFile(string fileName)
	{
		string filePath = Path.Combine(Config.GeoEngine.GEOEDIT_PATH, fileName);
		if (File.Exists(filePath))
		{
			try
			{
				File.Delete(filePath);
			}
			catch (IOException exception)
			{
                _logger.Trace("Could not delete file: " + filePath + ", exception: " + exception);
				return false;
			}
		}

		try
		{
			using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
			BinaryWriter<LittleEndianBitConverter> writer = new(fileStream);
			foreach (IBlock block in _blocks)
			{
				if (block is FlatBlock)
				{
					writer.WriteByte(IBlock.TYPE_FLAT);
					writer.WriteInt16(((FlatBlock) block).getHeight());
				}
				else if (block is ComplexBlock)
				{
					short[] data = ((ComplexBlock)block).getData();
					writer.WriteByte(IBlock.TYPE_COMPLEX);
					foreach (short info in data)
						writer.WriteInt16(info);
				}
				else if (block is MultilayerBlock)
				{
					byte[] data = ((MultilayerBlock)block).getData();
					writer.WriteByte(IBlock.TYPE_MULTILAYER);
					foreach (byte info in data)
						writer.WriteByte(info);
				}
			}
		}
		catch (IOException exception)
		{
            _logger.Trace(exception);
			return false;
		}

		return true;
	}
}