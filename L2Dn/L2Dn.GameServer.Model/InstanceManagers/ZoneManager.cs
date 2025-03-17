using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/// <summary>
/// This class manages the zones.
/// </summary>
public sealed class ZoneManager
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ZoneManager));

    private readonly ImmutableArray<ImmutableArray<ZoneRegion>> _zoneRegions;
    private readonly List<Item> _debugItems = [];
    private int _lastDynamicId = 300000;

    private FrozenDictionary<string, SpawnTerritory> _spawnTerritories =
        FrozenDictionary<string, SpawnTerritory>.Empty;

    private FrozenDictionary<int, Zone> _zonesById = FrozenDictionary<int, Zone>.Empty;
    private FrozenDictionary<string, Zone> _zonesByName = FrozenDictionary<string, Zone>.Empty;
    private FrozenDictionary<Type, ZoneListOfType> _zonesByTypeAndId = FrozenDictionary<Type, ZoneListOfType>.Empty;

    private FrozenDictionary<string, AbstractZoneSettings> _zoneSettings =
        FrozenDictionary<string, AbstractZoneSettings>.Empty;

    private ZoneManager()
    {
        _zoneRegions = WorldMap.CreateRegionGrid(coords => new ZoneRegion(coords.X, coords.Y));
        _logger.Info($"{nameof(ZoneManager)}: {WorldMap.RegionCountX}x{WorldMap.RegionCountY} " +
            "zone region grid set up.");
    }

    public static ZoneManager Instance { get; } = new();

    public void Reload()
    {
        Unload();
        Load();

        // Re-validate all characters in zones.
        foreach (WorldObject obj in World.getInstance().getVisibleObjects())
        {
            if (obj.isCreature())
                ((Creature)obj).revalidateZone(true);
        }

        _zoneSettings = FrozenDictionary<string, AbstractZoneSettings>.Empty;
    }

    public void Unload()
    {
        // Get the world regions
        int count = 0;

        // Backup old zone settings
        _zoneSettings = _zonesByName.
            Select(pair => new KeyValuePair<string, AbstractZoneSettings>(pair.Key, pair.Value.getSettings())).
            ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        // Clear zones
        foreach (ImmutableArray<ZoneRegion> zoneRegions in _zoneRegions)
        foreach (ZoneRegion zoneRegion in zoneRegions)
        {
            zoneRegion.getZones().Clear();
            count++;
        }

        _logger.Info($"{nameof(ZoneManager)}: Removed zones in {count} regions.");
    }

    public void Load()
    {
        ZoneLoader loader = new(_lastDynamicId);
        loader.Load();

        _lastDynamicId = loader.LastDynamicId;
        _zonesById = loader.GetZonesById();
        _zonesByName = loader.GetZonesByName();
        _zonesByTypeAndId = loader.GetZonesByTypeAndId();
        _spawnTerritories = loader.GetSpawnTerritories();


        // Register the zones into any world region they intersect with...
        foreach (Zone zone in _zonesById.Values)
        {
            ZoneForm form = zone.getZone();
            Rectangle bounds = form.Bounds;
            Location2D leftTop = WorldMap.WorldLocationToRegionCoordinates(bounds.LeftTop);
            Location2D rightBottom = WorldMap.WorldLocationToRegionCoordinates(bounds.RightBottom);
            for (int x = leftTop.X; x <= rightBottom.X; x++)
            {
                ImmutableArray<ZoneRegion> regions = _zoneRegions[x];
                for (int y = leftTop.Y; y <= rightBottom.Y; y++)
                {
                    Rectangle regionRectangle = WorldMap.GetRegionRectangle(new Location2D(x, y));
                    if (zone.getZone().IntersectsRectangle(regionRectangle))
                        regions[y].getZones()[zone.getId()] = zone;
                }
            }
        }

        int maxId = _zonesById.Count == 0 ? 0 : _zonesById.Keys.Where(value => value < 300000).Max();

        _logger.Info($"{nameof(ZoneManager)}: Loaded {_zonesByTypeAndId.Count} zone classes and {_zonesById.Count} " +
            $"zones. The last static zone id is {maxId}; the last dynamic zone id is {_lastDynamicId}.");
    }

    /// <summary>
    /// Return all zones by class type.
    /// </summary>
    public ImmutableArray<T> getAllZones<T>()
        where T: Zone =>
        _zonesByTypeAndId.TryGetValue(typeof(T), out ZoneListOfType? zoneListOfType)
            ? ((ZoneListOfType<T>)zoneListOfType).Array
            : ImmutableArray<T>.Empty;

    /// <summary>
    /// Get zone by ID.
    /// </summary>
    public Zone? getZoneById(int id) => _zonesById.GetValueOrDefault(id);

    /// <summary>
    /// Get zone by name.
    /// </summary>
    public Zone? getZoneByName(string name) => _zonesByName.GetValueOrDefault(name);

    /// <summary>
    /// Get zone by ID and zone class.
    /// </summary>
    public T? getZoneById<T>(int id)
        where T: Zone =>
        _zonesById.GetValueOrDefault(id) as T;

    /// <summary>
    /// Get zone by type and name.
    /// </summary>
    public T? getZoneByName<T>(string name)
        where T: Zone =>
        _zonesByName.GetValueOrDefault(name) as T;

    /// <summary>
    /// Returns all zones where the object is located.
    /// </summary>
    public List<Zone> getZones(Location3D location)
    {
        List<Zone> temp = new();
        ZoneRegion region = getRegion(location.Location2D);
        foreach (Zone zone in region.getZones().Values)
        {
            if (zone.isInsideZone(location))
                temp.Add(zone);
        }

        return temp;
    }

    /// <summary>
    /// Returns all zones by the given coordinates.
    /// </summary>
    public List<Zone> getZones(Location2D location)
    {
        List<Zone> temp = new();
        ZoneRegion region = getRegion(location);
        foreach (Zone zone in region.getZones().Values)
        {
            if (zone.isInsideZone(location))
                temp.Add(zone);
        }

        return temp;
    }

    /// <summary>
    /// Returns a zone by the given coordinates.
    /// </summary>
    public T? getZone<T>(Location3D location)
        where T: Zone
    {
        ZoneRegion region = getRegion(location.Location2D);
        return region.getZones().Values.OfType<T>().FirstOrDefault(zone => zone.isInsideZone(location));
    }

    /// <summary>
    /// Returns a zone by the given coordinates.
    /// </summary>
    public T? getZone<T>(Location2D location)
        where T: Zone
    {
        ZoneRegion region = getRegion(location);
        foreach (T zone in region.getZones().Values.OfType<T>())
        {
            if (zone.isInsideZone(location))
                return zone;
        }

        return null;
    }

    /**
     * Get spawm territory by name
     * @param name name of territory to search
     * @return link to zone form
     */
    public SpawnTerritory? getSpawnTerritory(string name) => _spawnTerritories.GetValueOrDefault(name);

    /**
     * Returns all spawm territories from where the object is located
     * @param object
     * @return zones
     */
    public List<SpawnTerritory> getSpawnTerritories(WorldObject obj)
    {
        List<SpawnTerritory> temp = new();
        foreach (SpawnTerritory territory in _spawnTerritories.Values)
        {
            if (territory.isInsideZone(obj.getX(), obj.getY(), obj.getZ()))
                temp.Add(territory);
        }

        return temp;
    }

    /**
     * Gets the olympiad stadium.
     * @param creature the creature
     * @return the olympiad stadium
     */
    public OlympiadStadiumZone? getOlympiadStadium(Creature creature)
    {
        if (creature == null)
            return null;

        foreach (Zone temp in getZones(new Location3D(creature.getX(), creature.getY(), creature.getZ())))
        {
            if (temp is OlympiadStadiumZone zone && zone.isCharacterInZone(creature))
                return zone;
        }

        return null;
    }

    public void DropDebugItem(int itemId, int num, int x, int y, int z)
    {
        Item item = new Item(IdManager.getInstance().getNextId(), itemId);
        item.setCount(num);
        item.spawnMe(new Location3D(x, y, z + 5));
        getDebugItems().Add(item);
    }

    /**
     * General storage for debug items used for visualizing zones.
     * @return list of items
     */
    public List<Item> getDebugItems() => _debugItems;

    /**
     * Remove all debug items from the world.
     */
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void clearDebugItems()
    {
        foreach (Item item in _debugItems)
            item.decayMe();

        _debugItems.Clear();
    }

    public ZoneRegion getRegion(Location2D location)
    {
        Location2D regionLocation = WorldMap.WorldLocationToRegionCoordinates(location);
        return _zoneRegions[regionLocation.X][regionLocation.Y];
    }

    /// <summary>
    /// Gets the settings.
    /// </summary>
    public AbstractZoneSettings? getSettings(string name) => _zoneSettings.GetValueOrDefault(name);

    private abstract class ZoneListOfType
    {
        public static ZoneListOfType Create(Type type, List<Zone> zones)
        {
            return (ZoneListOfType?)Activator.CreateInstance(typeof(ZoneListOfType<>).MakeGenericType(type), zones) ??
                throw new InvalidOperationException($"Failed to create ZoneListOfType<{type.Name}> object");
        }
    }

    private sealed class ZoneListOfType<T>(List<Zone> zones): ZoneListOfType
        where T: Zone
    {
        public ImmutableArray<T> Array { get; } = zones.Cast<T>().ToImmutableArray();
    }

    private sealed class ZoneLoader(int lastDynamicId)
    {
        private readonly Dictionary<string, SpawnTerritory> _spawnTerritories = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, Zone> _zonesById = [];
        private readonly Dictionary<string, Zone> _zonesByName = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Type, List<Zone>> _zonesByTypeAndId = [];
        private int _lastDynamicId = lastDynamicId;

        public int LastDynamicId => _lastDynamicId;

        public FrozenDictionary<string, SpawnTerritory> GetSpawnTerritories() =>
            _spawnTerritories.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        public FrozenDictionary<int, Zone> GetZonesById() => _zonesById.ToFrozenDictionary();

        public FrozenDictionary<string, Zone> GetZonesByName() =>
            _zonesByName.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        public FrozenDictionary<Type, ZoneListOfType> GetZonesByTypeAndId() =>
            _zonesByTypeAndId.ToFrozenDictionary(pair => pair.Key, pair => ZoneListOfType.Create(pair.Key, pair.Value));

        public void Load()
        {
            IEnumerable<XmlZoneList> xmlZoneLists = XmlLoader.LoadXmlDocuments<XmlZoneList>("zones").
                Where(xmlZoneList => xmlZoneList.Enabled);

            foreach (XmlZoneList xmlZoneList in xmlZoneLists)
            foreach (XmlZone xmlZone in xmlZoneList.Zones)
                CreateZone(xmlZoneList.FilePath, xmlZone);
        }

        private void CreateZone(string filePath, XmlZone xmlZone)
        {
            bool isNpcSpawnTerritory = xmlZone.Type == ZoneType.NpcSpawnTerritory;
            int zoneId = xmlZone.Id;
            if (!xmlZone.IdSpecified && !isNpcSpawnTerritory)
                zoneId = xmlZone.Id = ++_lastDynamicId;

            string zoneName = xmlZone.Name;

            try
            {
                if (isNpcSpawnTerritory)
                {
                    SpawnTerritory spawnTerritory = new(xmlZone);
                    if (!_spawnTerritories.TryAdd(zoneName, spawnTerritory))
                    {
                        _logger.Error($"{nameof(ZoneManager)}: Duplicated spawn territory zone name={zoneName} " +
                            $"in file '{filePath}'");
                    }

                    return;
                }

                Zone zone = ZoneFactory.Create(xmlZone);
                if (!_zonesById.TryAdd(zoneId, zone))
                {
                    _logger.Error($"{nameof(ZoneManager)}: Duplicated zone id={zoneId} in file '{filePath}'");
                    return;
                }

                if (!string.IsNullOrEmpty(zoneName) && !_zonesByName.TryAdd(zoneName, zone))
                {
                    _logger.Error($"{nameof(ZoneManager)}: Duplicated zone id={zoneId}, name={zoneName} in " +
                        $"file '{filePath}'");

                    return;
                }

                _zonesByTypeAndId.GetOrAdd(zone.GetType(), _ => []).Add(zone);
            }
            catch (Exception exception)
            {
                _logger.Error($"{nameof(ZoneManager)}: Invalid data for zone id={zoneId}, name={zoneName} " +
                    $"in file '{filePath}': {exception.Message}");
            }
        }
    }
}