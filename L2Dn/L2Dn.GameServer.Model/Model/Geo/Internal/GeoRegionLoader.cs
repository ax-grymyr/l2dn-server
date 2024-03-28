namespace L2Dn.GameServer.Model.Geo.Internal;

internal static class GeoRegionLoader
{
    internal static GeoRegion LoadRegion(string filePath)
    {
        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        GeoReader reader = new(fileStream);

        // Cells data
        List<GeoCell> cells = new((int)(fileStream.Length / 2));
        List<MultilayerCellInfo> multilayerCells = new();
        
        // Load blocks
        BlockInfo[] blockInfos = new BlockInfo[Constants.BlocksInRegion];
        for (int blockIndex = 0; blockIndex < blockInfos.Length; blockIndex++)
        {
            BlockType blockType = (BlockType)reader.ReadByte();
            blockInfos[blockIndex].Type = blockType;
            
            switch (blockType)
            {
                case BlockType.Flat:
                    blockInfos[blockIndex].Height = reader.ReadInt16();
                    break;
                
                case BlockType.Complex:
                    blockInfos[blockIndex].StartCellIndex = cells.Count;
                    for (int cellIndex = 0; cellIndex < Constants.CellsInBlock; cellIndex++)
                        cells.Add(new GeoCell(reader.ReadInt16()));
                    
                    break;
                
                case BlockType.Multilayer:
                    multilayerCells.EnsureCapacity(multilayerCells.Count + Constants.CellsInBlock);
                    blockInfos[blockIndex].StartCellIndex = multilayerCells.Count;
                    for (int cellIndex = 0; cellIndex < Constants.CellsInBlock; cellIndex++)
                    {
                        byte layerCount = reader.ReadByte();
                        if (layerCount is 0 or > 125)
                            throw new InvalidOperationException("GeoData: Geo file corrupted! Invalid layer count!");

                        multilayerCells.Add(new MultilayerCellInfo
                        {
                            LayerCount = layerCount,
                            StartCellIndex = cells.Count
                        });
                        
                        for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
                            cells.Add(new GeoCell(reader.ReadInt16()));
                    }

                    break;
                
                default:
                    throw new InvalidOperationException("Invalid block type");
            }
        }
        
        GeoCell[] cellArray = cells.ToArray();
        ReadOnlyMemory<GeoCell>[] multilayerCellArray =
            multilayerCells.Select(c => new ReadOnlyMemory<GeoCell>(cellArray, c.StartCellIndex, c.LayerCount))
                .ToArray();
            
        // Create blocks
        IGeoBlock[] blocks = new IGeoBlock[Constants.BlocksInRegion];
        for (int blockIndex = 0; blockIndex < blockInfos.Length; blockIndex++)
        {
            BlockInfo blockInfo = blockInfos[blockIndex]; 
            blocks[blockIndex] = blockInfo.Type switch
            {
                BlockType.Flat => new FlatBlock(blockInfo.Height),
                BlockType.Complex => new ComplexBlock(cellArray.AsMemory(blockInfo.StartCellIndex, Constants.CellsInBlock)),
                BlockType.Multilayer => new MultilayerBlock(multilayerCellArray.AsMemory(blockInfo.StartCellIndex, Constants.CellsInBlock)),
                _ => throw new InvalidOperationException("Invalid block type")
            };
        }

        return new GeoRegion(blocks);
    }

    private struct BlockInfo
    {
        public BlockType Type;
        
        // Flat block
        public short Height;
        
        // Complex block (contains Constants.CellsInBlock cells)
        // Multilayer block
        public int StartCellIndex;
    }

    private struct MultilayerCellInfo
    {
        public int StartCellIndex;
        public int LayerCount;
    }
}
