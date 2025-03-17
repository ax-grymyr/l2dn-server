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
        _worldRegions = WorldMap.CreateRegionGrid(coords => new WorldRegion(this, coords.X, coords.Y));

        // Set surrounding regions
        foreach (ImmutableArray<WorldRegion> regions in _worldRegions)
        foreach (WorldRegion region in regions)
            region.CalculateSurroundingRegions();

        _logger.Info($"{nameof(WorldRegionCollection)}: {WorldMap.RegionCountX}x{WorldMap.RegionCountY} " +
            "world region grid set up.");
    }

    public WorldRegion this[int regionX, int regionY] => _worldRegions[regionX][regionY];

    public WorldRegion GetRegion(Location2D location)
    {
        Location2D regionLocation = WorldMap.WorldLocationToRegionCoordinates(location);
        return _worldRegions[regionLocation.X][regionLocation.Y];
    }
}