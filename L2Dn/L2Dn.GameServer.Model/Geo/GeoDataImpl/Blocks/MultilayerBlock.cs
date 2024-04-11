namespace L2Dn.GameServer.Geo.GeoDataImpl.Blocks;

/**
 * @author HorridoJoho
 */
public class MultilayerBlock: IBlock
{
	private readonly byte[] _data;
	
	/**
	 * Initializes a new instance of this block reading the specified buffer.
	 * @param bb the buffer
	 */
	internal MultilayerBlock(GeoReader reader)
	{
		long start = reader.Position;
		
		for (int blockCellOffset = 0; blockCellOffset < IBlock.BLOCK_CELLS; blockCellOffset++)
		{
			byte nLayers = reader.ReadByte();
			if ((nLayers <= 0) || (nLayers > 125))
			{
				throw new InvalidOperationException("L2JGeoDriver: Geo file corrupted! Invalid layers count!");
			}
			
			reader.SeekToPosition(reader.Position + (nLayers * 2));
		}
		
		_data = new byte[reader.Position - start];
		reader.SeekToPosition(start);
		reader.ReadBytes(_data);
	}
	
	private short getNearestLayer(int geoX, int geoY, int worldZ)
	{
		int startOffset = getCellDataOffset(geoX, geoY);
		byte nLayers = _data[startOffset];
		int endOffset = startOffset + 1 + (nLayers * 2);
		
		// One layer at least was required on loading so this is set at least once on the loop below.
		int nearestDZ = 0;
		short nearestData = 0;
		for (int offset = startOffset + 1; offset < endOffset; offset += 2)
		{
			short layerData = extractLayerData(offset);
			int layerZ = extractLayerHeight(layerData);
			if (layerZ == worldZ)
			{
				// Exact z.
				return layerData;
			}
			
			int layerDZ = Math.Abs(layerZ - worldZ);
			if ((offset == (startOffset + 1)) || (layerDZ < nearestDZ))
			{
				nearestDZ = layerDZ;
				nearestData = layerData;
			}
		}
		
		return nearestData;
	}
	
	private int getCellDataOffset(int geoX, int geoY)
	{
		int cellLocalOffset = ((geoX % IBlock.BLOCK_CELLS_X) * IBlock.BLOCK_CELLS_Y) + (geoY % IBlock.BLOCK_CELLS_Y);
		int cellDataOffset = 0;
		// Move index to cell, we need to parse on each request, OR we parse on creation and save indexes.
		for (int i = 0; i < cellLocalOffset; i++)
		{
			cellDataOffset += 1 + (_data[cellDataOffset] * 2);
		}
		// Now the index points to the cell we need.
		
		return cellDataOffset;
	}
	
	private short extractLayerData(int dataOffset)
	{
		return (short) ((_data[dataOffset] & 0xFF) | (_data[dataOffset + 1] << 8));
	}
	
	private int getNearestNSWE(int geoX, int geoY, int worldZ)
	{
		return extractLayerNswe(getNearestLayer(geoX, geoY, worldZ));
	}
	
	private int extractLayerNswe(short layer)
	{
		return (byte) (layer & 0x000F);
	}
	
	private int extractLayerHeight(short layer)
	{
		return (short) (layer & 0x0fff0) >> 1;
	}
	
	public bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe)
	{
		return (getNearestNSWE(geoX, geoY, worldZ) & nswe) == nswe;
	}
	
	public void setNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		int startOffset = getCellDataOffset(geoX, geoY);
		byte nLayers = _data[startOffset];
		int endOffset = startOffset + 1 + (nLayers * 2);
		
		int nearestDZ = 0;
		int nearestLayerZ = 0;
		int nearestOffset = 0;
		short nearestLayerData = 0;
		for (int offset = startOffset + 1; offset < endOffset; offset += 2)
		{
			short layerData = extractLayerData(offset);
			int layerZ = extractLayerHeight(layerData);
			if (layerZ == worldZ)
			{
				nearestLayerZ = layerZ;
				nearestOffset = offset;
				nearestLayerData = layerData;
				break;
			}
			
			int layerDZ = Math.Abs(layerZ - worldZ);
			if ((offset == (startOffset + 1)) || (layerDZ < nearestDZ))
			{
				nearestDZ = layerDZ;
				nearestLayerZ = layerZ;
				nearestOffset = offset;
			}
		}
		
		short currentNswe = (short) extractLayerNswe(nearestLayerData);
		if ((currentNswe & nswe) == 0)
		{
			short encodedHeight = (short) (nearestLayerZ << 1); // Shift left by 1 bit.
			short newNswe = (short) (currentNswe | nswe); // Combine NSWE.
			short newCombinedData = (short) (encodedHeight | newNswe); // Combine height and NSWE.
			_data[nearestOffset] = (byte) (newCombinedData & 0xff); // Update the first byte at offset.
			_data[nearestOffset + 1] = (byte) ((newCombinedData >> 8) & 0xff); // Update the second byte at offset + 1.
		}
	}
	
	public void unsetNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		int startOffset = getCellDataOffset(geoX, geoY);
		byte nLayers = _data[startOffset];
		int endOffset = startOffset + 1 + (nLayers * 2);
		
		int nearestDZ = 0;
		int nearestLayerZ = 0;
		int nearestOffset = 0;
		short nearestLayerData = 0;
		for (int offset = startOffset + 1; offset < endOffset; offset += 2)
		{
			short layerData = extractLayerData(offset);
			int layerZ = extractLayerHeight(layerData);
			if (layerZ == worldZ)
			{
				nearestLayerZ = layerZ;
				nearestOffset = offset;
				nearestLayerData = layerData;
				break;
			}
			
			int layerDZ = Math.Abs(layerZ - worldZ);
			if ((offset == (startOffset + 1)) || (layerDZ < nearestDZ))
			{
				nearestDZ = layerDZ;
				nearestLayerZ = layerZ;
				nearestOffset = offset;
			}
		}
		
		short currentNswe = (short) extractLayerNswe(nearestLayerData);
		if ((currentNswe & nswe) != 0)
		{
			short encodedHeight = (short) (nearestLayerZ << 1); // Shift left by 1 bit.
			short newNswe = (short) (currentNswe & ~nswe); // Subtract NSWE.
			short newCombinedData = (short) (encodedHeight | newNswe); // Combine height and NSWE.
			_data[nearestOffset] = (byte) (newCombinedData & 0xff); // Update the first byte at offset.
			_data[nearestOffset + 1] = (byte) ((newCombinedData >> 8) & 0xff); // Update the second byte at offset + 1.
		}
	}
	
	public int getNearestZ(int geoX, int geoY, int worldZ)
	{
		return extractLayerHeight(getNearestLayer(geoX, geoY, worldZ));
	}
	
	public int getNextLowerZ(int geoX, int geoY, int worldZ)
	{
		int startOffset = getCellDataOffset(geoX, geoY);
		byte nLayers = _data[startOffset];
		int endOffset = startOffset + 1 + (nLayers * 2);
		
		int lowerZ = int.MinValue;
		for (int offset = startOffset + 1; offset < endOffset; offset += 2)
		{
			short layerData = extractLayerData(offset);
			
			int layerZ = extractLayerHeight(layerData);
			if (layerZ == worldZ)
			{
				// Exact z.
				return layerZ;
			}
			
			if ((layerZ < worldZ) && (layerZ > lowerZ))
			{
				lowerZ = layerZ;
			}
		}
		
		return lowerZ == int.MinValue ? worldZ : lowerZ;
	}
	
	public int getNextHigherZ(int geoX, int geoY, int worldZ)
	{
		int startOffset = getCellDataOffset(geoX, geoY);
		byte nLayers = _data[startOffset];
		int endOffset = startOffset + 1 + (nLayers * 2);
		
		int higherZ = int.MaxValue;
		for (int offset = startOffset + 1; offset < endOffset; offset += 2)
		{
			short layerData = extractLayerData(offset);
			
			int layerZ = extractLayerHeight(layerData);
			if (layerZ == worldZ)
			{
				// Exact z.
				return layerZ;
			}
			
			if ((layerZ > worldZ) && (layerZ < higherZ))
			{
				higherZ = layerZ;
			}
		}
		
		return higherZ == int.MaxValue ? worldZ : higherZ;
	}
	
	public byte[] getData()
	{
		return _data;
	}
}