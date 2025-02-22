namespace L2Dn.GameServer.Model;

public static class WorldMap
{
    // Map tile is the square area corresponding to one map file in the client, for example 26_16.unr.
    // The tiles are arranged in a grid, with the 0,0 world coordinate at the left top corner of the tile 20,18.
    // The tile numbers are counted with the X axis increasing to the right and the Y axis increasing downwards.
    public const int TileXMin = 11;
    public const int TileYMin = 10;
    public const int TileXMax = 28;
    public const int TileYMax = 26;
    public const int TileZeroCoordX = 20;
    public const int TileZeroCoordY = 18;

    /// <summary>
    /// Each tile is 32768x32768 units in size.
    /// </summary>
    public const int TileSize = 32768;

    // World coordinates of the map.
    public const int WorldXMin = (TileXMin - TileZeroCoordX) * TileSize;
    public const int WorldYMin = (TileYMin - TileZeroCoordY) * TileSize;
    public const int WorldXMax = (TileXMax - TileZeroCoordX + 1) * TileSize - 1;
    public const int WorldYMax = (TileYMax - TileZeroCoordY + 1) * TileSize - 1;

    // Each tile is divided into regions, and consists of 16x16 regions,
    // thus each region is 2048x2048 units in size.
    public const int RegionSize = TileSize / 16; // = 2048

    // The top left region has coordinates 0,0.
    // The offset is used to convert the world coordinates to the region coordinates.
    public const int RegionOffsetX = -(WorldXMin / RegionSize);
    public const int RegionOffsetY = -(WorldYMin / RegionSize);

    // Number of regions.
    public const int RegionCountX = (TileXMax - TileXMin + 1) * (TileSize / RegionSize);
    public const int RegionCountY = (TileYMax - TileYMin + 1) * (TileSize / RegionSize);

    // Gracia border Flying objects not allowed to the east of it.
    public const int GraciaMaxX = -166168;
    public const int GraciaMinZ = -895;
    public const int GraciaMaxZ = 6105;
}