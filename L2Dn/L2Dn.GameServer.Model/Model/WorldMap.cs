namespace L2Dn.GameServer.Model;

public static class WorldMap
{
    /// <summary>
    /// Bit shift, defines number of regions note,
    /// shifting by 15 will result in regions corresponding
    /// to map tiles shifting by 11 divides one tile to 16x16 regions.
    /// </summary>
    public const int ShiftBy = 11;

    public const int TileSize = 32768;

    /** Map dimensions. */
    public const int TileXMin = 11;
    public const int TileYMin = 10;
    public const int TileXMax = 28;
    public const int TileYMax = 26;
    public const int TileZeroCoordX = 20;
    public const int TileZeroCoordY = 18;
    
    public const int WorldXMin = (TileXMin - TileZeroCoordX) * TileSize;
    public const int WorldYMin = (TileYMin - TileZeroCoordY) * TileSize;
	
    public const int WorldXMax = (TileXMax - TileZeroCoordX + 1) * TileSize;
    public const int WorldYMax = (TileYMax - TileZeroCoordY + 1) * TileSize;
	
    /** Calculated offset used so top left region is 0,0 */
    public const int OffsetX = -(WorldXMin >> ShiftBy);
    public const int OffsetY = -(WorldYMin >> ShiftBy);
	
    /** Number of regions. */
    private const int RegionsX = (WorldXMax >> ShiftBy) + OffsetX;
    private const int RegionsY = (WorldYMax >> ShiftBy) + OffsetY;
}