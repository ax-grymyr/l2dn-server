using System.Collections.Immutable;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Model;

public sealed class WorldRegionCollection
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(WorldRegionCollection));
    private readonly ImmutableArray<ImmutableArray<WorldRegion>> _worldRegions;

    public WorldRegionCollection()
    {
        // Create regions
        _worldRegions =
            (from regionX in Enumerable.Range(0, WorldMap.RegionCountX)
             select
                 (from regionY in Enumerable.Range(0, WorldMap.RegionCountY)
                  select new WorldRegion(this, regionX, regionY)).ToImmutableArray()).ToImmutableArray();

        // Set surrounding regions
        foreach (ImmutableArray<WorldRegion> regions in _worldRegions)
        foreach (WorldRegion region in regions)
            region.CalculateSurroundingRegions();

        _logger.Info($"{nameof(WorldRegionCollection)}: ({WorldMap.RegionCountX} by {WorldMap.RegionCountY}) World Region Grid set up.");
    }

    public WorldRegion this[int regionX, int regionY] => _worldRegions[regionX][regionY];

    public WorldRegion GetRegion(Location2D location)
    {
        int x = Math.Clamp(location.X, WorldMap.WorldXMin, WorldMap.WorldXMax);
        int y = Math.Clamp(location.Y, WorldMap.WorldYMin, WorldMap.WorldYMax);
        if (x != location.X || y != location.Y)
            _logger.Warn($"Location {location} is out of bounds. Clamped to ({x}, {y}).");

        int regionX = (x - WorldMap.WorldXMin) / WorldMap.RegionSize + WorldMap.RegionOffsetX;
        int regionY = (y - WorldMap.WorldYMin) / WorldMap.RegionSize + WorldMap.RegionOffsetY;
        return _worldRegions[regionX][regionY];
    }
}