using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Forms;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.DataPack;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * This class manages the zones
 * @author durgus
 */
public class ZoneManager: DataReaderBase
{
	private const int ShiftBy = 15;
	private const int OffsetX = -World.WORLD_X_MIN >> ShiftBy;
	private const int OffsetY = -World.WORLD_Y_MIN >> ShiftBy;
	private const int RegionCountX = (World.WORLD_X_MAX >> ShiftBy) + OffsetX + 1;
	private const int RegionCountY = (World.WORLD_Y_MAX >> ShiftBy) + OffsetY + 1;

	private static readonly Logger _logger = LogManager.GetLogger(nameof(ZoneManager));

	private static readonly FrozenDictionary<string, ZoneTypeInfo> _zoneTypes =
		new ZoneTypeInfo[]
		{
			new ZoneTypeInfo<ArenaZone>(id => new ArenaZone(id)),
			new ZoneTypeInfo<CastleZone>(id => new CastleZone(id)),
			new ZoneTypeInfo<ClanHallZone>(id => new ClanHallZone(id)),
			new ZoneTypeInfo<ConditionZone>(id => new ConditionZone(id)),
			new ZoneTypeInfo<DamageZone>(id => new DamageZone(id)),
			new ZoneTypeInfo<DerbyTrackZone>(id => new DerbyTrackZone(id)),
			new ZoneTypeInfo<EffectZone>(id => new EffectZone(id)),
			new ZoneTypeInfo<FishingZone>(id => new FishingZone(id)),
			new ZoneTypeInfo<FortZone>(id => new FortZone(id)),
			new ZoneTypeInfo<HqZone>(id => new HqZone(id)),
			new ZoneTypeInfo<JailZone>(id => new JailZone(id)),
			new ZoneTypeInfo<LandingZone>(id => new LandingZone(id)),
			new ZoneTypeInfo<MotherTreeZone>(id => new MotherTreeZone(id)),
			new ZoneTypeInfo<NoLandingZone>(id => new NoLandingZone(id)),
			new ZoneTypeInfo<NoPvPZone>(id => new NoPvPZone(id)),
			new ZoneTypeInfo<NoRestartZone>(id => new NoRestartZone(id)),
			new ZoneTypeInfo<NoStoreZone>(id => new NoStoreZone(id)),
			new ZoneTypeInfo<NoSummonFriendZone>(id => new NoSummonFriendZone(id)),
			new ZoneTypeInfo<OlympiadStadiumZone>(id => new OlympiadStadiumZone(id)),
			new ZoneTypeInfo<PeaceZone>(id => new PeaceZone(id)),
			new ZoneTypeInfo<ResidenceHallTeleportZone>(id => new ResidenceHallTeleportZone(id)),
			new ZoneTypeInfo<ResidenceTeleportZone>(id => new ResidenceTeleportZone(id)),
			new ZoneTypeInfo<RespawnZone>(id => new RespawnZone(id)),
			new ZoneTypeInfo<SayuneZone>(id => new SayuneZone(id)),
			new ZoneTypeInfo<ScriptZone>(id => new ScriptZone(id)),
			new ZoneTypeInfo<SiegableHallZone>(id => new SiegableHallZone(id)),
			new ZoneTypeInfo<SiegeZone>(id => new SiegeZone(id)),
			new ZoneTypeInfo<SwampZone>(id => new SwampZone(id)),
			new ZoneTypeInfo<TaxZone>(id => new TaxZone(id)),
			new ZoneTypeInfo<TeleportZone>(id => new TeleportZone(id)),
			new ZoneTypeInfo<TimedHuntingZone>(id => new TimedHuntingZone(id)),
			new ZoneTypeInfo<UndyingZone>(id => new UndyingZone(id)),
			new ZoneTypeInfo<WaterZone>(id => new WaterZone(id)),
		}.ToFrozenDictionary(info => info.ZoneTypeName, StringComparer.OrdinalIgnoreCase);
	
	private static readonly Map<string, AbstractZoneSettings> _settings = new();
	
	private FrozenDictionary<Type, Map<int, ZoneType>> _classZones = FrozenDictionary<Type, Map<int, ZoneType>>.Empty;
	private readonly Map<string, SpawnTerritory> _spawnTerritories = new();
	private readonly AtomicInteger _lastDynamicId = new(300000);
	private List<Item> _debugItems = [];
	
	private readonly ZoneRegion[][] _zoneRegions;
	
	private ZoneManager()
	{
		_zoneRegions = new ZoneRegion[RegionCountX][];
		for (int x = 0; x < _zoneRegions.Length; x++)
		{
			_zoneRegions[x] = new ZoneRegion[RegionCountY];
			for (int y = 0; y < _zoneRegions[x].Length; y++)
			{
				_zoneRegions[x][y] = new ZoneRegion(x, y);
			}
		}

		_logger.Info(GetType().Name + " " + RegionCountX + " by " + RegionCountY + " Zone Region Grid set up.");
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
			{
				((Creature)obj).revalidateZone(true);
			}
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
			foreach (ZoneType zone in map.values())
			{
				if (zone.getSettings() != null)
				{
					_settings.put(zone.getName(), zone.getSettings());
				}
			}
		}
		
		// Clear zones
		foreach (ZoneRegion[] zoneRegions in _zoneRegions)
		{
			foreach (ZoneRegion zoneRegion in zoneRegions)
			{
				zoneRegion.getZones().clear();
				count++;
			}
		}
		_logger.Info(GetType().Name +": Removed zones in " + count + " regions.");
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
				_logger.Error($"ZoneData: Name {zoneName} already used for another zone, check file: {filePath}. Skipping zone");
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
			Point2D[] coords = zone.Nodes.Select(node => new Point2D(node.X, node.Y)).ToArray();
			if (coords.Length == 0)
			{
				_logger.Error(GetType().Name + ": ZoneData: missing data for zone: " + zoneId + " XML file: " + filePath);
				return;
			}
			
			// Create this zone. Parsing for cuboids is a bit different than for other polygons cuboids need exactly 2 points to be defined.
			// Other polygons need at least 3 (one per vertex)
			if (string.Equals(zoneShape, "Cuboid", StringComparison.OrdinalIgnoreCase))
			{
				if (coords.Length == 2)
				{
					zoneForm = new ZoneCuboid(coords[0].getX(), coords[1].getX(), coords[0].getY(), coords[1].getY(), minZ, maxZ);
				}
				else
				{
					_logger.Error(GetType().Name + ": ZoneData: Missing cuboid vertex data for zone: " + zoneId + " in file: " + filePath);
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
						aX[i] = coords[i].getX();
						aY[i] = coords[i].getY();
					}
					zoneForm = new ZoneNPoly(aX, aY, minZ, maxZ);
				}
				else
				{
					_logger.Error(GetType().Name + ": ZoneData: Bad data for zone: " + zoneId + " in file: " + filePath);
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
					zoneForm = new ZoneCylinder(coords[0].getX(), coords[0].getY(), minZ, maxZ, zoneRad);
				}
				else
				{
					_logger.Error(GetType().Name + ": ZoneData: Bad data for zone: " + zoneId + " in file: " + filePath);
					return;
				}
			}
			else
			{
				_logger.Error(GetType().Name + ": ZoneData: Unknown shape: \"" + zoneShape + "\"  for zone: " + zoneId + " in file: " + filePath);
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
			
		ZoneType zoneType = zoneTypeInfo.Factory(zoneId);
		zoneType.setZone(zoneForm);
		
		// Check for additional parameters
		zone.Stats.ForEach(stat => zoneType.setParameter(stat.Name, stat.Value));

		if (zoneType is ZoneRespawn zoneRespawn)
		{
			zone.Spawns.ForEach(spawn => zoneRespawn.parseLoc(spawn.X, spawn.Y, spawn.Z, spawn.Type));
		}

		if (zoneType is RespawnZone respawnZone)
		{
			zone.Races.ForEach(el => respawnZone.addRaceRespawnPoint(el.Name, el.Point));
		}

		if (zoneName != null && !zoneName.isEmpty())
		{
			zoneType.setName(zoneName);
		}

		if (!_classZones.TryGetValue(zoneTypeInfo.ZoneType, out Map<int, ZoneType>? zoneMap))
		{
			_logger.Warn(GetType().Name + ": Caution: Unknown zone type (" + zoneType.GetType() + ") from file: " + filePath + ".");
			return;
		}

		if (!zoneMap.TryAdd(zoneId, zoneType))
		{
			_logger.Warn(GetType().Name + ": Caution: Zone (" + zoneId + ") from file: " + filePath + " has duplicated definition.");
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
		
		_spawnTerritories.clear();

		LoadXmlDocuments<XmlZones>(DataFileLocation.Data, "zones")
			.Where(tuple => tuple.Document.Enabled)
			.ForEach(t => t.Document.Zones.ForEach(zone => parseElement(t.FilePath, zone)));
		
		_logger.Info(GetType().Name +": Loaded " + _classZones.Count + " zone classes and " + getSize() + " zones.");
		int maxId = _classZones.Values.SelectMany(map => map.Keys).Where(value => value < 300000).Max();
		_logger.Info(GetType().Name +": Last static id " + maxId + ".");
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
		return _classZones.SelectMany(pair => pair.Value.Values)
			.FirstOrDefault(zone => string.Equals(zone.getName(), name));
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
	public List<ZoneType> getZones(ILocational locational)
	{
		return getZones(locational.getX(), locational.getY(), locational.getZ());
	}
	
	/**
	 * Gets the zone.
	 * @param <T> the generic type
	 * @param locational the locational
	 * @param type the type
	 * @return zone from where the object is located by type
	 */
	public T getZone<T>(ILocational locational)
		where T: ZoneType
	{
		if (locational == null)
			return null;
		
		return getZone<T>(locational.getX(), locational.getY(), locational.getZ());
	}
	
	/**
	 * Returns all zones from given coordinates (plane).
	 * @param x the x
	 * @param y the y
	 * @return zones
	 */
	public List<ZoneType> getZones(int x, int y)
	{
		List<ZoneType> temp = new();
		foreach (ZoneType zone in getRegion(x, y).getZones().values())
		{
			if (zone.isInsideZone(x, y))
			{
				temp.add(zone);
			}
		}
		return temp;
	}
	
	/**
	 * Returns all zones from given coordinates.
	 * @param x the x
	 * @param y the y
	 * @param z the z
	 * @return zones
	 */
	public List<ZoneType> getZones(int x, int y, int z)
	{
		List<ZoneType> temp = new();
		foreach (ZoneType zone in getRegion(x, y).getZones().values())
		{
			if (zone.isInsideZone(x, y, z))
			{
				temp.add(zone);
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
	public T getZone<T>(int x, int y, int z)
		where T: ZoneType	
	{
		foreach (ZoneType zone in getRegion(x, y).getZones().values())
		{
			if (zone.isInsideZone(x, y, z) && zone is T)
			{
				return (T) zone;
			}
		}
		return null;
	}
	
	/**
	 * Get spawm territory by name
	 * @param name name of territory to search
	 * @return link to zone form
	 */
	public SpawnTerritory getSpawnTerritory(string name)
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
		foreach (SpawnTerritory territory in _spawnTerritories.values())
		{
			if (territory.isInsideZone(obj.getX(), obj.getY(), obj.getZ()))
			{
				temp.add(territory);
			}
		}
		return temp;
	}
	
	/**
	 * Gets the olympiad stadium.
	 * @param creature the creature
	 * @return the olympiad stadium
	 */
	public OlympiadStadiumZone getOlympiadStadium(Creature creature)
	{
		if (creature == null)
		{
			return null;
		}
		
		foreach (ZoneType temp in getInstance().getZones(creature.getX(), creature.getY(), creature.getZ()))
		{
			if (temp is OlympiadStadiumZone && temp.isCharacterInZone(creature))
			{
				return (OlympiadStadiumZone) temp;
			}
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
	
	public ZoneRegion getRegion(int x, int y)
	{
		try
		{
			return _zoneRegions[(x >> ShiftBy) + OffsetX][(y >> ShiftBy) + OffsetY];
		}
		catch (IndexOutOfRangeException e)
		{
			// LOGGER.Warn(GetType().Name + ": Incorrect zone region X: " + ((x >> SHIFT_BY) + OFFSET_X) + " Y: " + ((y >> SHIFT_BY) + OFFSET_Y) + " for coordinates x: " + x + " y: " + y);
			return null;
		}
	}
	
	public ZoneRegion getRegion(ILocational point)
	{
		return getRegion(point.getX(), point.getY());
	}
	
	/**
	 * Gets the settings.
	 * @param name the name
	 * @return the settings
	 */
	public static AbstractZoneSettings getSettings(string name)
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
		public string ZoneTypeName => ZoneType.Name;
		public abstract Func<int, ZoneType> Factory { get; }
	}

	private sealed class ZoneTypeInfo<T>(Func<int, T> factory): ZoneTypeInfo(typeof(T))
		where T: ZoneType
	{
		public override Func<int, ZoneType> Factory => factory;
	}
}