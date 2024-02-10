using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * This class manages the zones
 * @author durgus
 */
public class ZoneManager: IXmlReader
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ZoneManager));
	
	private static readonly Map<String, AbstractZoneSettings> SETTINGS = new();
	
	private const int SHIFT_BY = 15;
	private static readonly int OFFSET_X = Math.Abs(World.WORLD_X_MIN >> SHIFT_BY);
	private static readonly int OFFSET_Y = Math.Abs(World.WORLD_Y_MIN >> SHIFT_BY);
	
	private readonly Map<Type, Map<int, ZoneType>> _classZones = new();
	private readonly Map<String, SpawnTerritory> _spawnTerritories = new();
	private readonly AtomicInteger _lastDynamicId = new AtomicInteger(300000);
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
	
	public void parseDocument(Document doc, File f)
	{
		NamedNodeMap attrs;
		Node attribute;
		String zoneName;
		int[][] coords;
		int zoneId;
		int minZ;
		int maxZ;
		String zoneType;
		String zoneShape;
		List<int[]> rs = new();
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				attrs = n.getAttributes();
				attribute = attrs.getNamedItem("enabled");
				if ((attribute != null) && !Boolean.parseBoolean(attribute.getNodeValue()))
				{
					continue;
				}
				
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("zone".equalsIgnoreCase(d.getNodeName()))
					{
						attrs = d.getAttributes();
						attribute = attrs.getNamedItem("type");
						if (attribute != null)
						{
							zoneType = attribute.getNodeValue();
						}
						else
						{
							LOGGER.Warn("ZoneData: Missing type for zone in file: " + f.getName());
							continue;
						}
						
						attribute = attrs.getNamedItem("id");
						if (attribute != null)
						{
							zoneId = int.Parse(attribute.getNodeValue());
						}
						else
						{
							zoneId = zoneType.equalsIgnoreCase("NpcSpawnTerritory") ? 0 : _lastDynamicId.incrementAndGet();
						}
						
						attribute = attrs.getNamedItem("name");
						if (attribute != null)
						{
							zoneName = attribute.getNodeValue();
						}
						else
						{
							zoneName = null;
						}
						
						// Check zone name for NpcSpawnTerritory. Must exist and to be unique
						if (zoneType.equalsIgnoreCase("NpcSpawnTerritory"))
						{
							if (zoneName == null)
							{
								LOGGER.Warn("ZoneData: Missing name for NpcSpawnTerritory in file: " + f.getName() + ", skipping zone");
								continue;
							}
							else if (_spawnTerritories.containsKey(zoneName))
							{
								LOGGER.Warn("ZoneData: Name " + zoneName + " already used for another zone, check file: " + f.getName() + ". Skipping zone");
								continue;
							}
						}
						
						minZ = parseInteger(attrs, "minZ");
						maxZ = parseInteger(attrs, "maxZ");
						zoneType = parseString(attrs, "type");
						zoneShape = parseString(attrs, "shape");
						
						// Get the zone shape from xml
						ZoneForm zoneForm = null;
						try
						{
							for (Node cd = d.getFirstChild(); cd != null; cd = cd.getNextSibling())
							{
								if ("node".equalsIgnoreCase(cd.getNodeName()))
								{
									attrs = cd.getAttributes();
									int[] point = new int[2];
									point[0] = parseInteger(attrs, "X");
									point[1] = parseInteger(attrs, "Y");
									rs.add(point);
								}
							}
							
							coords = rs.toArray(new int[rs.size()][2]);
							rs.clear();
							
							if ((coords == null) || (coords.length == 0))
							{
								LOGGER.Warn(GetType().Name + ": ZoneData: missing data for zone: " + zoneId + " XML file: " + f.getName());
								continue;
							}
							
							// Create this zone. Parsing for cuboids is a bit different than for other polygons cuboids need exactly 2 points to be defined.
							// Other polygons need at least 3 (one per vertex)
							if (zoneShape.equalsIgnoreCase("Cuboid"))
							{
								if (coords.length == 2)
								{
									zoneForm = new ZoneCuboid(coords[0][0], coords[1][0], coords[0][1], coords[1][1], minZ, maxZ);
								}
								else
								{
									LOGGER.Warn(GetType().Name + ": ZoneData: Missing cuboid vertex data for zone: " + zoneId + " in file: " + f.getName());
									continue;
								}
							}
							else if (zoneShape.equalsIgnoreCase("NPoly"))
							{
								// nPoly needs to have at least 3 vertices
								if (coords.length > 2)
								{
									int[] aX = new int[coords.length];
									int[] aY = new int[coords.length];
									for (int i = 0; i < coords.length; i++)
									{
										aX[i] = coords[i][0];
										aY[i] = coords[i][1];
									}
									zoneForm = new ZoneNPoly(aX, aY, minZ, maxZ);
								}
								else
								{
									LOGGER.Warn(GetType().Name + ": ZoneData: Bad data for zone: " + zoneId + " in file: " + f.getName());
									continue;
								}
							}
							else if (zoneShape.equalsIgnoreCase("Cylinder"))
							{
								// A Cylinder zone requires a center point
								// at x,y and a radius
								attrs = d.getAttributes();
								int zoneRad = int.Parse(attrs.getNamedItem("rad").getNodeValue());
								if ((coords.length == 1) && (zoneRad > 0))
								{
									zoneForm = new ZoneCylinder(coords[0][0], coords[0][1], minZ, maxZ, zoneRad);
								}
								else
								{
									LOGGER.Warn(GetType().Name + ": ZoneData: Bad data for zone: " + zoneId + " in file: " + f.getName());
									continue;
								}
							}
							else
							{
								LOGGER.Warn(GetType().Name + ": ZoneData: Unknown shape: \"" + zoneShape + "\"  for zone: " + zoneId + " in file: " + f.getName());
								continue;
							}
						}
						catch (Exception e)
						{
							LOGGER.Warn(GetType().Name + ": ZoneData: Failed to load zone " + zoneId + " coordinates: " + e.getMessage(), e);
						}
						
						// No further parameters needed, if NpcSpawnTerritory is loading
						if (zoneType.equalsIgnoreCase("NpcSpawnTerritory"))
						{
							_spawnTerritories.put(zoneName, new SpawnTerritory(zoneName, zoneForm));
							continue;
						}
						
						// Create the zone
						Class<?> newZone = null;
						Constructor<?> zoneConstructor = null;
						ZoneType temp;
						try
						{
							newZone = Class.forName("org.l2jmobius.gameserver.model.zone.type." + zoneType);
							zoneConstructor = newZone.getConstructor(int));
							temp = (ZoneType) zoneConstructor.newInstance(zoneId);
							temp.setZone(zoneForm);
						}
						catch (Exception e)
						{
							LOGGER.Warn(GetType().Name + ": ZoneData: No such zone type: " + zoneType + " in file: " + f.getName());
							continue;
						}
						
						// Check for additional parameters
						for (Node cd = d.getFirstChild(); cd != null; cd = cd.getNextSibling())
						{
							if ("stat".equalsIgnoreCase(cd.getNodeName()))
							{
								attrs = cd.getAttributes();
								String name = attrs.getNamedItem("name").getNodeValue();
								String val = attrs.getNamedItem("val").getNodeValue();
								temp.setParameter(name, val);
							}
							else if ("spawn".equalsIgnoreCase(cd.getNodeName()) && (temp instanceof ZoneRespawn))
							{
								attrs = cd.getAttributes();
								int spawnX = int.Parse(attrs.getNamedItem("X").getNodeValue());
								int spawnY = int.Parse(attrs.getNamedItem("Y").getNodeValue());
								int spawnZ = int.Parse(attrs.getNamedItem("Z").getNodeValue());
								Node val = attrs.getNamedItem("type");
								((ZoneRespawn) temp).parseLoc(spawnX, spawnY, spawnZ, val == null ? null : val.getNodeValue());
							}
							else if ("race".equalsIgnoreCase(cd.getNodeName()) && (temp instanceof RespawnZone))
							{
								attrs = cd.getAttributes();
								String race = attrs.getNamedItem("name").getNodeValue();
								String point = attrs.getNamedItem("point").getNodeValue();
								((RespawnZone) temp).addRaceRespawnPoint(race, point);
							}
						}
						if (checkId(zoneId))
						{
							LOGGER.config(getClass().getSimpleName() + ": Caution: Zone (" + zoneId + ") from file: " + f.getName() + " overrides previous definition.");
						}
						
						if ((zoneName != null) && !zoneName.isEmpty())
						{
							temp.setName(zoneName);
						}
						
						addZone(zoneId, temp);
						
						// Register the zone into any world region it
						// intersects with...
						// currently 11136 test for each zone :>
						for (int x = 0; x < _zoneRegions.length; x++)
						{
							for (int y = 0; y < _zoneRegions[x].length; y++)
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
		parseDatapackDirectory("data/zones", false);
		LOGGER.Info(GetType().Name +": Loaded " + _classZones.size() + " zone classes and " + getSize() + " zones.");
		OptionalInt maxId = _classZones.values().stream().flatMap(map => map.keySet().stream()).mapToInt(int)::cast).filter(value => value < 300000).max();
		LOGGER.Info(GetType().Name +": Last static id " + maxId.getAsInt() + ".");
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