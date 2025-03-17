using System.Collections.Immutable;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Model;

public static class WorldMap
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(WorldMap));

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

    // Number of regions.
    public const int RegionCountX = (TileXMax - TileXMin + 1) * (TileSize / RegionSize);
    public const int RegionCountY = (TileYMax - TileYMin + 1) * (TileSize / RegionSize);

    // Gracia border Flying objects not allowed to the east of it.
    public const int GraciaMaxX = -166168;
    public const int GraciaMinZ = -895;
    public const int GraciaMaxZ = 6105;

    public static Location2D WorldLocationToTileCoordinates(Location2D worldLocation)
    {
        Location2D clampedLocation = new Location2D(Math.Clamp(worldLocation.X, WorldXMin, WorldXMax),
            Math.Clamp(worldLocation.Y, WorldYMin, WorldYMax));

        if (clampedLocation != worldLocation)
            _logger.Warn($"Location {worldLocation} is out of bounds. Clamped to {clampedLocation}.");

        return new Location2D(clampedLocation.X / TileSize + TileZeroCoordX,
            clampedLocation.Y / RegionSize + TileZeroCoordY);
    }

    public static Location2D WorldLocationToRegionCoordinates(Location2D worldLocation)
    {
        Location2D clampedLocation = new Location2D(Math.Clamp(worldLocation.X, WorldXMin, WorldXMax),
            Math.Clamp(worldLocation.Y, WorldYMin, WorldYMax));

        if (clampedLocation != worldLocation)
            _logger.Warn($"Location {worldLocation} is out of bounds. Clamped to {clampedLocation}.");

        return new Location2D((clampedLocation.X - WorldXMin) / RegionSize,
            (clampedLocation.Y - WorldYMin) / RegionSize);
    }

    public static Rectangle GetTileRectangle(Location2D tileCoordinates)
    {
        Location2D clampedCoordinates = new Location2D(Math.Clamp(tileCoordinates.X, TileXMin, TileXMax),
            Math.Clamp(tileCoordinates.Y, TileYMin, TileYMax));

        if (clampedCoordinates != tileCoordinates)
            _logger.Warn($"Tile coordinates {tileCoordinates} is out of bounds. Clamped to {clampedCoordinates}.");

        return new Rectangle((clampedCoordinates.X - TileZeroCoordX) * TileSize,
            (clampedCoordinates.Y - TileZeroCoordY) * TileSize, TileSize, TileSize);
    }

    public static Rectangle GetRegionRectangle(Location2D regionCoordinates)
    {
        Location2D clampedCoordinates = new Location2D(Math.Clamp(regionCoordinates.X, 0, RegionCountX - 1),
            Math.Clamp(regionCoordinates.Y, 0, RegionCountY - 1));

        if (clampedCoordinates != regionCoordinates)
            _logger.Warn($"Region coordinates {regionCoordinates} is out of bounds. Clamped to {clampedCoordinates}.");

        return new Rectangle(clampedCoordinates.X * RegionSize + WorldXMin,
            clampedCoordinates.Y * RegionSize + WorldYMin, RegionSize, RegionSize);
    }

    public static ImmutableArray<ImmutableArray<T>> CreateRegionGrid<T>(Func<Location2D, T> factory)
    {
        return (from regionX in Enumerable.Range(0, RegionCountX)
                select (from regionY in Enumerable.Range(0, RegionCountY)
                        select factory(new Location2D(regionX, regionY))).ToImmutableArray()).ToImmutableArray();
    }
}