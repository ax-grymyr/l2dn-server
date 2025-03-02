using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Forms;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * This class manages the zones
 * @author durgus
 */
public class ZoneManager: DataReaderBase
{
    private const int ShiftBy = 15;
    private const int OffsetX = -WorldMap.WorldXMin >> ShiftBy;
    private const int OffsetY = -WorldMap.WorldYMin >> ShiftBy;

    private static readonly Logger _logger = LogManager.GetLogger(nameof(ZoneManager));

    private static readonly FrozenDictionary<string, ZoneTypeInfo> _zoneTypes =
        new ZoneTypeInfo[]
        {
            new ZoneTypeInfo<ArenaZone>((id, form) =>new ArenaZone(id, form)),
            new ZoneTypeInfo<CastleZone>((id, form) =>new CastleZone(id, form)),
            new ZoneTypeInfo<ClanHallZone>((id, form) =>new ClanHallZone(id, form)),
            new ZoneTypeInfo<ConditionZone>((id, form) =>new ConditionZone(id, form)),
            new ZoneTypeInfo<DamageZone>((id, form) =>new DamageZone(id, form)),
            new ZoneTypeInfo<DerbyTrackZone>((id, form) =>new DerbyTrackZone(id, form)),
            new ZoneTypeInfo<EffectZone>((id, form) =>new EffectZone(id, form)),
            new ZoneTypeInfo<FishingZone>((id, form) =>new FishingZone(id, form)),
            new ZoneTypeInfo<FortZone>((id, form) =>new FortZone(id, form)),
            new ZoneTypeInfo<HqZone>((id, form) =>new HqZone(id, form)),
            new ZoneTypeInfo<JailZone>((id, form) =>new JailZone(id, form)),
            new ZoneTypeInfo<LandingZone>((id, form) =>new LandingZone(id, form)),
            new ZoneTypeInfo<MotherTreeZone>((id, form) =>new MotherTreeZone(id, form)),
            new ZoneTypeInfo<NoLandingZone>((id, form) =>new NoLandingZone(id, form)),
            new ZoneTypeInfo<NoPvPZone>((id, form) =>new NoPvPZone(id, form)),
            new ZoneTypeInfo<NoRestartZone>((id, form) =>new NoRestartZone(id, form)),
            new ZoneTypeInfo<NoStoreZone>((id, form) =>new NoStoreZone(id, form)),
            new ZoneTypeInfo<NoSummonFriendZone>((id, form) =>new NoSummonFriendZone(id, form)),
            new ZoneTypeInfo<OlympiadStadiumZone>((id, form) =>new OlympiadStadiumZone(id, form)),
            new ZoneTypeInfo<PeaceZone>((id, form) =>new PeaceZone(id, form)),
            new ZoneTypeInfo<ResidenceHallTeleportZone>((id, form) =>new ResidenceHallTeleportZone(id, form)),
            new ZoneTypeInfo<ResidenceTeleportZone>((id, form) =>new ResidenceTeleportZone(id, form)),
            new ZoneTypeInfo<RespawnZone>((id, form) =>new RespawnZone(id, form)),
            new ZoneTypeInfo<SayuneZone>((id, form) =>new SayuneZone(id, form)),
            new ZoneTypeInfo<ScriptZone>((id, form) =>new ScriptZone(id, form)),
            new ZoneTypeInfo<SiegableHallZone>((id, form) =>new SiegableHallZone(id, form)),
            new ZoneTypeInfo<SiegeZone>((id, form) =>new SiegeZone(id, form)),
            new ZoneTypeInfo<SwampZone>((id, form) =>new SwampZone(id, form)),
            new ZoneTypeInfo<TaxZone>((id, form) =>new TaxZone(id, form)),
            new ZoneTypeInfo<TeleportZone>((id, form) =>new TeleportZone(id, form)),
            new ZoneTypeInfo<TimedHuntingZone>((id, form) =>new TimedHuntingZone(id, form)),
            new ZoneTypeInfo<UndyingZone>((id, form) =>new UndyingZone(id, form)),
            new ZoneTypeInfo<WaterZone>((id, form) =>new WaterZone(id, form)),
        }.ToFrozenDictionary(info => info.ZoneTypeName, StringComparer.OrdinalIgnoreCase);

    private static readonly Map<string, AbstractZoneSettings> _settings = new();

    private FrozenDictionary<Type, Map<int, ZoneType>> _classZones = FrozenDictionary<Type, Map<int, ZoneType>>.Empty;
    private readonly Map<string, SpawnTerritory> _spawnTerritories = new();
    private readonly AtomicInteger _lastDynamicId = new(300000);
    private List<Item> _debugItems = [];

    private readonly ZoneRegion[][] _zoneRegions;

    private ZoneManager()
    {
        _zoneRegions = new ZoneRegion[WorldMap.RegionCountX][];
        for (int x = 0; x < _zoneRegions.Length; x++)
        {
            ZoneRegion[] regions = _zoneRegions[x] = new ZoneRegion[WorldMap.RegionCountY];
            for (int y = 0; y < regions.Length; y++)
                regions[y] = new ZoneRegion(x, y);
        }

        _logger.Info(GetType().Name + " " + WorldMap.RegionCountX + " by " + WorldMap.RegionCountY +
            " Zone Region Grid set up.");

        load();
    }

    /**
     * Reload.
     */
    public void reload()
    {
        // Unload zones.
        unload();

        // Load the zones.
        load();

        // Re-validate all characters in zones.
        foreach (WorldObject obj in World.getInstance().getVisibleObjects())
        {
            if (obj.isCreature())
                ((Creature)obj).revalidateZone(true);
        }

        _settings.Clear();
    }

    public void unload()
    {
        // Get the world regions
        int count = 0;

        // Backup old zone settings
        foreach (Map<int, ZoneType> map in _classZones.Values)
        {
            foreach (ZoneType zone in map.Values)
            {
                AbstractZoneSettings? settings = zone.getSettings();
                if (settings != null)
                {
                    _settings.put(zone.getName(), settings);
                }
            }
        }

        // Clear zones
        foreach (ZoneRegion[] zoneRegions in _zoneRegions)
        {
            foreach (ZoneRegion zoneRegion in zoneRegions)
            {
                zoneRegion.getZones().Clear();
                count++;
            }
        }

        _logger.Info(GetType().Name + ": Removed zones in " + count + " regions.");
    }

    private void parseElement(string filePath, XmlZone zone)
    {
        bool isNpcSpawnTerritory = string.Equals(zone.Type, "NpcSpawnTerritory", StringComparison.OrdinalIgnoreCase);
        int zoneId = zone.IdSpecified ? zone.Id : isNpcSpawnTerritory ? 0 : _lastDynamicId.incrementAndGet();
        string zoneName = zone.Name;

        // Check zone name for NpcSpawnTerritory. Must exist and to be unique
        if (isNpcSpawnTerritory)
        {
            if (string.IsNullOrEmpty(zoneName))
            {
                _logger.Error($"ZoneData: Missing name for NpcSpawnTerritory in file: {filePath}, skipping zone");
                return;
            }

            if (_spawnTerritories.ContainsKey(zoneName))
            {
                _logger.Error(
                    $"ZoneData: Name {zoneName} already used for another zone, check file: {filePath}. Skipping zone");

                return;
            }
        }

        int minZ = zone.MinZ;
        int maxZ = zone.MaxZ;
        string zoneShape = zone.Shape;

        // Get the zone shape from xml
        ZoneForm zoneForm;
        try
        {
            Location2D[] coords = zone.Nodes.Select(node => new Location2D(node.X, node.Y)).ToArray();
            if (coords.Length == 0)
            {
                _logger.Error(
                    GetType().Name + ": ZoneData: missing data for zone: " + zoneId + " XML file: " + filePath);

                return;
            }

            // Create this zone. Parsing for cuboids is a bit different than for other polygons cuboids need exactly 2 points to be defined.
            // Other polygons need at least 3 (one per vertex)
            if (string.Equals(zoneShape, "Cuboid", StringComparison.OrdinalIgnoreCase))
            {
                if (coords.Length == 2)
                {
                    zoneForm = new ZoneCuboid(coords[0].X, coords[1].X, coords[0].Y, coords[1].Y, minZ, maxZ);
                }
                else
                {
                    _logger.Error(GetType().Name + ": ZoneData: Missing cuboid vertex data for zone: " + zoneId +
                        " in file: " + filePath);

                    return;
                }
            }
            else if (string.Equals(zoneShape, "NPoly", StringComparison.OrdinalIgnoreCase))
            {
                // nPoly needs to have at least 3 vertices
                if (coords.Length > 2)
                {
                    int[] aX = new int[coords.Length];
                    int[] aY = new int[coords.Length];
                    for (int i = 0; i < coords.Length; i++)
                    {
                        aX[i] = coords[i].X;
                        aY[i] = coords[i].Y;
                    }

                    zoneForm = new ZoneNPoly(aX, aY, minZ, maxZ);
                }
                else
                {
                    _logger.Error(GetType().Name + ": ZoneData: Bad data for zone: " + zoneId + " in file: " +
                        filePath);

                    return;
                }
            }
            else if (string.Equals(zoneShape, "Cylinder", StringComparison.OrdinalIgnoreCase))
            {
                // A Cylinder zone requires a center point
                // at x,y and a radius
                int zoneRad = zone.Radius;
                if (coords.Length == 1 && zoneRad > 0)
                {
                    zoneForm = new ZoneCylinder(coords[0].X, coords[0].Y, minZ, maxZ, zoneRad);
                }
                else
                {
                    _logger.Error(GetType().Name + ": ZoneData: Bad data for zone: " + zoneId + " in file: " +
                        filePath);

                    return;
                }
            }
            else
            {
                _logger.Error(GetType().Name + ": ZoneData: Unknown shape: \"" + zoneShape + "\"  for zone: " + zoneId +
                    " in file: " + filePath);

                return;
            }
        }
        catch (Exception e)
        {
            _logger.Error(GetType().Name + ": ZoneData: Failed to load zone " + zoneId + " coordinates: " + e);
            return;
        }

        // No further parameters needed, if NpcSpawnTerritory is loading
        if (isNpcSpawnTerritory)
        {
            _spawnTerritories[zoneName] = new SpawnTerritory(zoneName, zoneForm);
            return;
        }

        // Create the zone
        if (!_zoneTypes.TryGetValue(zone.Type, out ZoneTypeInfo? zoneTypeInfo))
        {
            _logger.Error(GetType().Name + ": ZoneData: No such zone type: " + zone.Type + " in file: " + filePath);
            return;
        }

        ZoneType zoneType = zoneTypeInfo.Factory(zoneId, zoneForm);

        // Check for additional parameters
        zone.Stats.ForEach(stat => zoneType.setParameter(stat.Name, stat.Value));

        if (zoneType is ZoneRespawn zoneRespawn)
        {
            zone.Spawns.ForEach(spawn => zoneRespawn.parseLoc(new Location3D(spawn.X, spawn.Y, spawn.Z), spawn.Type));
        }

        if (zoneType is RespawnZone respawnZone)
        {
            zone.Races.ForEach(el => respawnZone.addRaceRespawnPoint(el.Race, el.Point));
        }

        if (!string.IsNullOrEmpty(zoneName))
        {
            zoneType.setName(zoneName);
        }

        if (!_classZones.TryGetValue(zoneTypeInfo.ZoneType, out Map<int, ZoneType>? zoneMap))
        {
            _logger.Warn(GetType().Name + ": Caution: Unknown zone type (" + zoneType.GetType() + ") from file: " +
                filePath + ".");

            return;
        }

        if (!zoneMap.TryAdd(zoneId, zoneType))
        {
            _logger.Warn(GetType().Name + ": Caution: Zone (" + zoneId + ") from file: " + filePath +
                " has duplicated definition.");

            return;
        }

        // Register the zone into any world region it
        // intersects with...
        // currently 11136 test for each zone :>
        for (int x = 0; x < _zoneRegions.Length; x++)
        {
            for (int y = 0; y < _zoneRegions[x].Length; y++)
            {
                int ax = (x - OffsetX) << ShiftBy;
                int bx = (x + 1 - OffsetX) << ShiftBy;
                int ay = (y - OffsetY) << ShiftBy;
                int by = (y + 1 - OffsetY) << ShiftBy;
                if (zoneType.getZone().intersectsRectangle(ax, bx, ay, by))
                {
                    _zoneRegions[x][y].getZones().put(zoneType.getId(), zoneType);
                }
            }
        }
    }

    public void load()
    {
        _classZones = _zoneTypes.Values.ToFrozenDictionary(info => info.ZoneType, _ => new Map<int, ZoneType>());

        _spawnTerritories.Clear();

        LoadXmlDocuments<XmlZones>(DataFileLocation.Data, "zones").Where(tuple => tuple.Document.Enabled).
            ForEach(t => t.Document.Zones.ForEach(zone => parseElement(t.FilePath, zone)));

        _logger.Info(GetType().Name + ": Loaded " + _classZones.Count + " zone classes and " + getSize() + " zones.");
        int maxId = _classZones.Values.SelectMany(map => map.Keys).Where(value => value < 300000).Max();
        _logger.Info(GetType().Name + ": Last static id " + maxId + ".");
    }

    /**
     * Gets the size.
     * @return the size
     */
    public int getSize()
    {
        return _classZones.Sum(pair => pair.Value.Count);
    }

    /**
     * Return all zones by class type.
     * @param <T> the generic type
     * @param zoneType Zone class
     * @return Collection of zones
     */
    public ImmutableArray<T> getAllZones<T>()
        where T: ZoneType
    {
        if (!_classZones.TryGetValue(typeof(T), out Map<int, ZoneType>? map))
            return ImmutableArray<T>.Empty;

        return map.Values.Cast<T>().ToImmutableArray();
    }

    /**
     * Get zone by ID.
     * @param id the id
     * @return the zone by id
     * @see #getZoneById(int, Class)
     */
    public ZoneType? getZoneById(int id)
    {
        foreach (Map<int, ZoneType> map in _classZones.Values)
        {
            if (map.TryGetValue(id, out ZoneType? zone))
                return zone;
        }

        return null;
    }

    /**
     * Get zone by name.
     * @param name the zone name
     * @return the zone by name
     */
    public ZoneType? getZoneByName(string name)
    {
        return _classZones.SelectMany(pair => pair.Value.Values).
            FirstOrDefault(zone => string.Equals(zone.getName(), name));
    }

    /**
     * Get zone by ID and zone class.
     * @param <T> the generic type
     * @param id the id
     * @param zoneType the zone type
     * @return zone
     */
    public T? getZoneById<T>(int id)
        where T: ZoneType
    {
        return (T?)_classZones.GetValueOrDefault(typeof(T))?.GetValueOrDefault(id);
    }

    /**
     * Get zone by name.
     * @param <T> the generic type
     * @param name the zone name
     * @param zoneType the zone type
     * @return
     */
    public T? getZoneByName<T>(string name)
        where T: ZoneType
    {
        if (!_classZones.TryGetValue(typeof(T), out Map<int, ZoneType>? map))
            return null;

        return map.Values.OfType<T>().FirstOrDefault(zone => string.Equals(zone.getName(), name));
    }

    /**
     * Returns all zones from where the object is located.
     * @param locational the locational
     * @return zones
     */
    public List<ZoneType> getZones(Location3D location)
    {
        List<ZoneType> temp = new();
        ZoneRegion? region = getRegion(location.Location2D);
        if (region != null)
        {
            foreach (ZoneType zone in region.getZones().Values)
            {
                if (zone.isInsideZone(location))
                    temp.Add(zone);
            }
        }

        return temp;
    }

    /**
     * Returns all zones from given coordinates (plane).
     * @param x the x
     * @param y the y
     * @return zones
     */
    public List<ZoneType> getZones(Location2D location)
    {
        List<ZoneType> temp = new();
        ZoneRegion? region = getRegion(location);
        if (region != null)
        {
            foreach (ZoneType zone in region.getZones().Values)
            {
                if (zone.isInsideZone(location))
                    temp.Add(zone);
            }
        }

        return temp;
    }

    /**
     * Gets the zone.
     * @param <T> the generic type
     * @param x the x
     * @param y the y
     * @param z the z
     * @param type the type
     * @return zone from given coordinates
     */
    public T? getZone<T>(Location3D location)
        where T: ZoneType
    {
        ZoneRegion? region = getRegion(location.Location2D);
        if (region != null)
        {
            foreach (ZoneType zone in region.getZones().Values)
            {
                if (zone.isInsideZone(location) && zone is T zoneType)
                    return zoneType;
            }
        }

        return null;
    }

    /**
     * Gets the zone.
     * @param <T> the generic type
     * @param x the x
     * @param y the y
     * @param z the z
     * @param type the type
     * @return zone from given coordinates
     */
    public T? getZone<T>(Location2D location)
        where T: ZoneType
    {
        ZoneRegion? region = getRegion(location);
        if (region != null)
        {
            foreach (ZoneType zone in region.getZones().Values)
            {
                if (zone.isInsideZone(location) && zone is T zoneType)
                    return zoneType;
            }
        }

        return null;
    }

    /**
     * Get spawm territory by name
     * @param name name of territory to search
     * @return link to zone form
     */
    public SpawnTerritory? getSpawnTerritory(string name)
    {
        return _spawnTerritories.get(name);
    }

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
            {
                temp.Add(territory);
            }
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

        foreach (ZoneType temp in getInstance().
                     getZones(new Location3D(creature.getX(), creature.getY(), creature.getZ())))
        {
            if (temp is OlympiadStadiumZone zone && zone.isCharacterInZone(creature))
                return zone;
        }

        return null;
    }

    /**
     * General storage for debug items used for visualizing zones.
     * @return list of items
     */
    public List<Item> getDebugItems()
    {
        if (_debugItems == null)
        {
            _debugItems = new();
        }

        return _debugItems;
    }

    /**
     * Remove all debug items from l2world.
     */
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void clearDebugItems()
    {
        if (_debugItems != null)
        {
            foreach (Item item in _debugItems)
            {
                if (item != null)
                {
                    item.decayMe();
                }
            }

            _debugItems.Clear();
        }
    }

    public ZoneRegion? getRegion(Location2D location)
    {
        int index = (location.X >> ShiftBy) + OffsetX;
        if (index < 0 || index >= _zoneRegions.Length)
            return null;

        ZoneRegion[] regions = _zoneRegions[index];
        index = (location.Y >> ShiftBy) + OffsetY;
        if (index < 0 || index >= regions.Length)
            return null;

        return regions[index];
    }

    /**
     * Gets the settings.
     * @param name the name
     * @return the settings
     */
    public static AbstractZoneSettings? getSettings(string? name)
    {
        if (name == null)
            return null;

        return _settings.get(name);
    }

    /**
     * Gets the single instance of ZoneManager.
     * @return single instance of ZoneManager
     */
    public static ZoneManager getInstance()
    {
        return SingletonHolder.INSTANCE;
    }

    private static class SingletonHolder
    {
        public static readonly ZoneManager INSTANCE = new();
    }

    private abstract class ZoneTypeInfo(Type type)
    {
        public Type ZoneType => type;
        public string ZoneTypeName => type.Name;
        public abstract Func<int, ZoneForm, ZoneType> Factory { get; }
    }

    private sealed class ZoneTypeInfo<T>(Func<int, ZoneForm, T> factory): ZoneTypeInfo(typeof(T))
        where T: ZoneType
    {
        public override Func<int, ZoneForm, ZoneType> Factory => factory;
    }
}