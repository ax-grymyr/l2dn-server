using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.MapRegions;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class MapRegionData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(MapRegionData));

    private const string _defaultRespawnRegionName = "talking_island_town";
    private FrozenDictionary<string, MapRegion> _regions = FrozenDictionary<string, MapRegion>.Empty;
    private MapRegion? _defaultRespawnRegion;

    public static MapRegionData Instance => new();

    private MapRegionData()
    {
    }

    internal void Load()
    {
        Dictionary<string, MapRegion> regions = new Dictionary<string, MapRegion>();

        XmlFileReader.LoadXmlDocuments<XmlMapRegionList>("mapregion").
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

    public MapRegion? GetMapRegion(int locX, int locY)
    {
        int tileX = GetMapRegionX(locX);
        int tileY = GetMapRegionY(locY);
        foreach (MapRegion region in _regions.Values)
        {
            if (region.IsZoneInRegion(tileX, tileY))
                return region;
        }

        return null;
    }

    public static int GetMapRegionX(int posX) => (posX >> 15) + 9 + 11; // + centerTileX;

    public static int GetMapRegionY(int posY) => (posY >> 15) + 10 + 8; // + centerTileX;

    public MapRegion? GetMapRegionByName(string regionName) => _regions.GetValueOrDefault(regionName);

    public int GetBBs(Location2D location) =>
        (GetMapRegion(location.X, location.Y) ?? _defaultRespawnRegion)?.BBs ?? 0;

    public int GetMapRegionLocId(int locX, int locY) => GetMapRegion(locX, locY)?.LocationId ?? 0;
}