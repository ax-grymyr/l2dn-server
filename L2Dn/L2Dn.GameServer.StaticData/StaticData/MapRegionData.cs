using System.Collections.Frozen;
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

        _defaultRespawnRegion = regions.GetValueOrDefault(_defaultRespawnRegionName);
        if (_defaultRespawnRegion is null)
            _logger.Error($"{nameof(MapRegionData)}: Default respawn region '{_defaultRespawnRegionName}' not defined");

        _logger.Info($"{nameof(MapRegionData)}: Loaded {regions.Count} map regions.");
    }

    public MapRegion? DefaultRespawnRegion => _defaultRespawnRegion;

    public MapRegion? GetMapRegion(Location2D location)
    {
        Location2D tileCoordinates = WorldMap.WorldLocationToTileCoordinates(location);
        foreach (MapRegion region in _regions.Values)
        {
            if (region.IsZoneInRegion(tileCoordinates))
                return region;
        }

        return null;
    }

    public MapRegion? GetMapRegionByName(string regionName) => _regions.GetValueOrDefault(regionName);

    public int GetBBs(Location2D location) => (GetMapRegion(location) ?? _defaultRespawnRegion)?.BBs ?? 0;

    public int GetMapRegionLocId(Location2D location) => GetMapRegion(location)?.LocationId ?? 0;
}