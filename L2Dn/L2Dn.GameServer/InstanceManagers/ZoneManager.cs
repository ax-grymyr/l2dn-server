using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
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
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * This class manages the zones
 * @author durgus
 */
public class ZoneManager: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ZoneManager));
	
	private static readonly Map<string, AbstractZoneSettings> SETTINGS = new();
	
	private const int SHIFT_BY = 15;
	private static readonly int OFFSET_X = Math.Abs(World.WORLD_X_MIN >> SHIFT_BY);
	private static readonly int OFFSET_Y = Math.Abs(World.WORLD_Y_MIN >> SHIFT_BY);
	
	private readonly Map<Type, Map<int, ZoneType>> _classZones = new();
	private readonly Map<string, SpawnTerritory> _spawnTerritories = new();
	private readonly AtomicInteger _lastDynamicId = new(300000);
	private List<Item> _debugItems;
	
	private readonly ZoneRegion[][] _zoneRegions;
	
	/**
	 * Instantiates a new zone manager.
	 */
	protected ZoneManager()
	{
		_zoneRegions = new ZoneRegion[(World.WORLD_X_MAX >> SHIFT_BY) + OFFSET_X + 1][];
		for (int x = 0; x < _zoneRegions.Length; x++)
		{
			_zoneRegions[x] = new ZoneRegion[(World.WORLD_Y_MAX >> SHIFT_BY) + OFFSET_Y + 1];
			for (int y = 0; y < _zoneRegions[x].Length; y++)
			{
				_zoneRegions[x][y] = new ZoneRegion(x, y);
			}
		}
		
		LOGGER.Info(GetType().Name +" " + _zoneRegions.Length + " by " + _zoneRegions[0].Length + " Zone Region Grid set up.");
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
				((Creature) obj).revalidateZone(true);
			}
		}
		
		SETTINGS.clear();
	}
	
	public void unload()
	{
		// Get the world regions
		int count = 0;
		
		// Backup old zone settings
		foreach (var map in _classZones.values())
		{
			foreach (ZoneType zone in map.values())
			{
				if (zone.getSettings() != null)
				{
					SETTINGS.put(zone.getName(), zone.getSettings());
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
		LOGGER.Info(GetType().Name +": Removed zones in " + count + " regions.");
	}
	
	private void parseElement(string filePath, XElement element)
	{
		string zoneType = element.Attribute("type").GetString();
		int zoneId = element.Attribute("id").GetInt32(-1);
		if (zoneId == -1)
			zoneId = zoneType.equalsIgnoreCase("NpcSpawnTerritory") ? 0 : _lastDynamicId.incrementAndGet();

		string? zoneName = element.Attribute("name")?.GetString(); 
		
		// Check zone name for NpcSpawnTerritory. Must exist and to be unique
		if (zoneType.equalsIgnoreCase("NpcSpawnTerritory"))
		{
			if (zoneName == null)
			{
				LOGGER.Error("ZoneData: Missing name for NpcSpawnTerritory in file: " + filePath + ", skipping zone");
				return;
			}
			
			if (_spawnTerritories.containsKey(zoneName))
			{
				LOGGER.Error("ZoneData: Name " + zoneName + " already used for another zone, check file: " + filePath + ". Skipping zone");
				return;
			}
		}

		Point2D[] coords;
		
		int minZ = element.Attribute("minZ").GetInt32();
		int maxZ = element.Attribute("maxZ").GetInt32();
		string zoneShape = element.Attribute("shape").GetString();
						
		// Get the zone shape from xml
		ZoneForm zoneForm = null;
		try
		{
			List<Point2D> rs = new();
			element.Elements("node").ForEach(el =>
			{
				
				int x = el.Attribute("X").GetInt32();
				int y = el.Attribute("Y").GetInt32();
				rs.Add(new Point2D(x, y));
			});

			coords = rs.ToArray();
			if (coords.Length == 0)
			{
				LOGGER.Error(GetType().Name + ": ZoneData: missing data for zone: " + zoneId + " XML file: " + filePath);
				return;
			}
			
			// Create this zone. Parsing for cuboids is a bit different than for other polygons cuboids need exactly 2 points to be defined.
			// Other polygons need at least 3 (one per vertex)
			if (zoneShape.equalsIgnoreCase("Cuboid"))
			{
				if (coords.Length == 2)
				{
					zoneForm = new ZoneCuboid(coords[0].getX(), coords[1].getX(), coords[0].getY(), coords[1].getY(), minZ, maxZ);
				}
				else
				{
					LOGGER.Error(GetType().Name + ": ZoneData: Missing cuboid vertex data for zone: " + zoneId + " in file: " + filePath);
					return;
				}
			}
			else if (zoneShape.equalsIgnoreCase("NPoly"))
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
					LOGGER.Error(GetType().Name + ": ZoneData: Bad data for zone: " + zoneId + " in file: " + filePath);
					return;
				}
			}
			else if (zoneShape.equalsIgnoreCase("Cylinder"))
			{
				// A Cylinder zone requires a center point
				// at x,y and a radius
				int zoneRad = element.Attribute("rad").GetInt32();
				if ((coords.Length == 1) && (zoneRad > 0))
				{
					zoneForm = new ZoneCylinder(coords[0].getX(), coords[0].getY(), minZ, maxZ, zoneRad);
				}
				else
				{
					LOGGER.Error(GetType().Name + ": ZoneData: Bad data for zone: " + zoneId + " in file: " + filePath);
					return;
				}
			}
			else
			{
				LOGGER.Error(GetType().Name + ": ZoneData: Unknown shape: \"" + zoneShape + "\"  for zone: " + zoneId + " in file: " + filePath);
				return;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": ZoneData: Failed to load zone " + zoneId + " coordinates: " + e);
			return;
		}
		
		// No further parameters needed, if NpcSpawnTerritory is loading
		if (zoneType.equalsIgnoreCase("NpcSpawnTerritory"))
		{
			_spawnTerritories.put(zoneName, new SpawnTerritory(zoneName, zoneForm));
			return;
		}
		
		// Create the zone
		ZoneType temp;
		try
		{
			// TODO: create factory
			string ns = typeof(ArenaZone).Namespace; 
			string typeName = ns + "." + zoneType;
			Type zoneClass = Assembly.GetExecutingAssembly().GetType(typeName);
			temp = (ZoneType)Activator.CreateInstance(zoneClass, zoneId);
			temp.setZone(zoneForm);
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": ZoneData: No such zone type: " + zoneType + " in file: " + filePath);
			return;
		}
		
		// Check for additional parameters
		element.Elements("stat").ForEach(el =>
		{
			string name = el.Attribute("name").GetString();
			string val = el.Attribute("val").GetString();
			temp.setParameter(name, val);
		});

		if (temp is ZoneRespawn zoneRespawn)
		{
			element.Elements("spawn").ForEach(el =>
			{
				int spawnX = el.Attribute("X").GetInt32();
				int spawnY = el.Attribute("Y").GetInt32();
				int spawnZ = el.Attribute("Z").GetInt32();
				string? val = el.Attribute("type")?.GetString();
				zoneRespawn.parseLoc(spawnX, spawnY, spawnZ, val);
			});
		}

		if (temp is RespawnZone respawnZone)
		{
			element.Elements("race").ForEach(el =>
			{
				string race = el.Attribute("name").GetString();
				string point = el.Attribute("point").GetString();
				respawnZone.addRaceRespawnPoint(race, point);
			});
		}

		if (checkId(zoneId))
		{
			LOGGER.Warn(GetType().Name + ": Caution: Zone (" + zoneId + ") from file: " + filePath + " overrides previous definition.");
		}
		
		if ((zoneName != null) && !zoneName.isEmpty())
		{
			temp.setName(zoneName);
		}
		
		addZone(zoneId, temp);
		
		// Register the zone into any world region it
		// intersects with...
		// currently 11136 test for each zone :>
		for (int x = 0; x < _zoneRegions.Length; x++)
		{
			for (int y = 0; y < _zoneRegions[x].Length; y++)
			{
				int ax = (x - OFFSET_X) << SHIFT_BY;
				int bx = ((x + 1) - OFFSET_X) << SHIFT_BY;
				int ay = (y - OFFSET_Y) << SHIFT_BY;
				int by = ((y + 1) - OFFSET_Y) << SHIFT_BY;
				if (temp.getZone().intersectsRectangle(ax, bx, ay, by))
				{
					_zoneRegions[x][y].getZones().put(temp.getId(), temp);
				}
			}
		}
	}
	
	public void load()
	{
		_classZones.clear();
		_classZones.put(typeof(ArenaZone), new());
		_classZones.put(typeof(CastleZone), new());
		_classZones.put(typeof(ClanHallZone), new());
		_classZones.put(typeof(ConditionZone), new());
		_classZones.put(typeof(DamageZone), new());
		_classZones.put(typeof(DerbyTrackZone), new());
		_classZones.put(typeof(EffectZone), new());
		_classZones.put(typeof(FishingZone), new());
		_classZones.put(typeof(FortZone), new());
		_classZones.put(typeof(HqZone), new());
		_classZones.put(typeof(JailZone), new());
		_classZones.put(typeof(LandingZone), new());
		_classZones.put(typeof(MotherTreeZone), new());
		_classZones.put(typeof(NoLandingZone), new());
		_classZones.put(typeof(NoRestartZone), new());
		_classZones.put(typeof(NoStoreZone), new());
		_classZones.put(typeof(NoSummonFriendZone), new());
		_classZones.put(typeof(OlympiadStadiumZone), new());
		_classZones.put(typeof(PeaceZone), new());
		_classZones.put(typeof(ResidenceHallTeleportZone), new());
		_classZones.put(typeof(ResidenceTeleportZone), new());
		_classZones.put(typeof(ResidenceZone), new());
		_classZones.put(typeof(RespawnZone), new());
		_classZones.put(typeof(SayuneZone), new());
		_classZones.put(typeof(ScriptZone), new());
		_classZones.put(typeof(SiegableHallZone), new());
		_classZones.put(typeof(SiegeZone), new());
		_classZones.put(typeof(SwampZone), new());
		_classZones.put(typeof(TaxZone), new());
		_classZones.put(typeof(TeleportZone), new());
		_classZones.put(typeof(TimedHuntingZone), new());
		_classZones.put(typeof(UndyingZone), new());
		_classZones.put(typeof(WaterZone), new());
		
		_spawnTerritories.clear();
		
		LoadXmlDocuments(DataFileLocation.Data, "zones").ForEach(t =>
		{
			t.Document.Elements("list").Where(e => e.Attribute("enabled").GetBoolean(true)).Elements("zone")
				.ForEach(e => parseElement(t.FilePath, e));
		});
		
		LOGGER.Info(GetType().Name +": Loaded " + _classZones.size() + " zone classes and " + getSize() + " zones.");
		int maxId = _classZones.values().SelectMany(map => map.Keys).Where(value => value < 300000).Max();
		LOGGER.Info(GetType().Name +": Last static id " + maxId + ".");
	}
	
	/**
	 * Gets the size.
	 * @return the size
	 */
	public int getSize()
	{
		int i = 0;
		foreach (var map in _classZones.values())
		{
			i += map.size();
		}
		return i;
	}
	
	/**
	 * Check id.
	 * @param id the id
	 * @return true, if successful
	 */
	private bool checkId(int id)
	{
		foreach (var map in _classZones.values())
		{
			if (map.containsKey(id))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Add new zone.
	 * @param <T> the generic type
	 * @param id the id
	 * @param zone the zone
	 */
	private void addZone(int id, ZoneType zone)
	{
		var map = _classZones.get(zone.GetType());
		if (map == null)
		{
			_classZones.put(zone.GetType(), map = new());
		}
		
		map.put(id, zone);
	}
	
	/**
	 * Return all zones by class type.
	 * @param <T> the generic type
	 * @param zoneType Zone class
	 * @return Collection of zones
	 */
	public ICollection<T> getAllZones<T>()
		where T: ZoneType
	{
		return _classZones.get(typeof(T)).values().Cast<T>().ToList();
	}
	
	/**
	 * Get zone by ID.
	 * @param id the id
	 * @return the zone by id
	 * @see #getZoneById(int, Class)
	 */
	public ZoneType getZoneById(int id)
	{
		foreach (var map in _classZones.values())
		{
			if (map.containsKey(id))
			{
				return map.get(id);
			}
		}
		return null;
	}
	
	/**
	 * Get zone by name.
	 * @param name the zone name
	 * @return the zone by name
	 */
	public ZoneType getZoneByName(String name)
	{
		foreach (var map in _classZones.values())
		{
			foreach (ZoneType zone in map.values())
			{
				if ((zone.getName() != null) && zone.getName().equals(name))
				{
					return zone;
				}
			}
		}
		return null;
	}
	
	/**
	 * Get zone by ID and zone class.
	 * @param <T> the generic type
	 * @param id the id
	 * @param zoneType the zone type
	 * @return zone
	 */
	public T getZoneById<T>(int id)
		where T: ZoneType
	{
		return (T) _classZones.get(typeof(T)).get(id);
	}
	
	/**
	 * Get zone by name.
	 * @param <T> the generic type
	 * @param name the zone name
	 * @param zoneType the zone type
	 * @return
	 */
	public T getZoneByName<T>(String name)
		where T: ZoneType
	{
		if (_classZones.containsKey(typeof(T)))
		{
			foreach (ZoneType zone in _classZones.get(typeof(T)).values())
			{
				if ((zone.getName() != null) && zone.getName().equals(name))
				{
					return (T) zone;
				}
			}
		}
		return null;
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
		{
			return null;
		}
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
	public SpawnTerritory getSpawnTerritory(String name)
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
			if ((temp is OlympiadStadiumZone) && temp.isCharacterInZone(creature))
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
			return _zoneRegions[(x >> SHIFT_BY) + OFFSET_X][(y >> SHIFT_BY) + OFFSET_Y];
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
	public static AbstractZoneSettings getSettings(String name)
	{
		return SETTINGS.get(name);
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
		public static readonly ZoneManager INSTANCE = new ZoneManager();
	}
}