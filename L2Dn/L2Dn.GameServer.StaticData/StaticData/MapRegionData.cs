using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.StaticData.Xml.MapRegions;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class MapRegionData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(MapRegionData));

    private const string _defaultRespawnRegionName = "talking_island_town";
    private FrozenDictionary<string, MapRegion> _regions = FrozenDictionary<string, MapRegion>.Empty;

    private FrozenDictionary<Location2D, ImmutableArray<MapRegion>> _regionsByTile =
        FrozenDictionary<Location2D, ImmutableArray<MapRegion>>.Empty;

    private MapRegion? _defaultRespawnRegion;

    public static MapRegionData Instance { get; } = new();

    private MapRegionData()
    {
    }

    public void Load()
    {
        Dictionary<string, MapRegion> regions = new Dictionary<string, MapRegion>();

        XmlLoader.LoadXmlDocuments<XmlMapRegionList>("mapregion").
            Where(xmlRegionList => xmlRegionList.Enabled).
            SelectMany(xmlRegionList => xmlRegionList.Regions).
            Select(xmlRegion => new MapRegion(xmlRegion)).
            ForEach(region =>
            {
                if (!regions.TryAdd(region.Name, region))
                    _logger.Error($"{nameof(MapRegionData)}: Duplicate region name '{region.Name}'");
            });

        _regions = regions.ToFrozenDictionary();
        _regionsByTile = regions.SelectMany(r => r.Value.MapTiles.Select(t => (Tile: t, Region: r.Value))).
            GroupBy(t => t.Tile).ToFrozenDictionary(g => g.Key, g => g.Select(t => t.Region).ToImmutableArray());

        _defaultRespawnRegion = regions.GetValueOrDefault(_defaultRespawnRegionName);
        if (_defaultRespawnRegion is null)
            _logger.Error($"{nameof(MapRegionData)}: Default respawn region '{_defaultRespawnRegionName}' not defined");

        _logger.Info($"{nameof(MapRegionData)}: Loaded {regions.Count} map regions.");
    }

    public MapRegion? DefaultRespawnRegion => _defaultRespawnRegion;

    public MapRegion? GetMapRegion(Location2D location)
    {
        Location2D tileCoordinates = WorldMap.WorldLocationToTileCoordinates(location);
        ImmutableArray<MapRegion> regions = _regionsByTile.GetValueOrDefault(tileCoordinates);
        return regions.IsDefaultOrEmpty ? null : regions[0];
    }

    public MapRegion? GetMapRegion(IHasLocation location) => GetMapRegion(location.Location.Location2D);

    public MapRegion? GetMapRegionByName(string regionName) => _regions.GetValueOrDefault(regionName);

    public int GetBBs(Location2D location) => (GetMapRegion(location) ?? _defaultRespawnRegion)?.BBs ?? 0;

    public int GetMapRegionLocationId(Location2D location) => GetMapRegion(location)?.LocationId ?? 0;
    public int GetMapRegionLocationId(IHasLocation location) => GetMapRegionLocationId(location.Location.Location2D);

    /// <summary>
    /// Get town name by location.
    /// </summary>
    public string GetClosestTownName(Location2D location) => GetMapRegion(location)?.Town ?? "Aden Castle Town";

    /// <summary>
    /// Get town name by location.
    /// </summary>
    public string GetClosestTownName(IHasLocation location) => GetClosestTownName(location.Location.Location2D);
}