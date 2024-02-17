using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Map Region Manager.
 * @author Nyaran
 */
public class MapRegionManager: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MapRegionManager));
	
	private static readonly Map<String, MapRegion> REGIONS = new();
	private const string DEFAULT_RESPAWN = "talking_island_town";
	
	protected MapRegionManager()
	{
		load();
	}
	
	public void load()
	{
		REGIONS.clear();
		
		LoadXmlDocuments(DataFileLocation.Data, "mapregion").ForEach(t =>
		{
			t.Document.Elements("").ForEach(el => parseElement(t.FilePath, el));
		});
		
		LOGGER.Info(GetType().Name +": Loaded " + REGIONS.size() + " map regions.");
	}
	
	private void parseElement(string filePath, XElement element)
	{
		string name = element.Attribute("name").GetString();
		string town = element.Attribute("town").GetString();
		int locId = element.Attribute("locId").GetInt32();
		int bbs = element.Attribute("bbs").GetInt32();
						
		MapRegion region = new MapRegion(name, town, locId, bbs);
		foreach (XElement c in element.Elements())
		{
			string nodeName = c.Name.LocalName;
			if ("respawnPoint".equalsIgnoreCase(nodeName))
			{
				int spawnX = c.Attribute("X").GetInt32();
				int spawnY = c.Attribute("Y").GetInt32();
				int spawnZ = c.Attribute("Z").GetInt32();
				bool other = c.Attribute("isOther").GetBoolean(false);
				bool chaotic = c.Attribute("isChaotic").GetBoolean(false);
				bool banish = c.Attribute("isBanish").GetBoolean(false);
				if (other)
				{
					region.addOtherSpawn(spawnX, spawnY, spawnZ);
				}
				else if (chaotic)
				{
					region.addChaoticSpawn(spawnX, spawnY, spawnZ);
				}
				else if (banish)
				{
					region.addBanishSpawn(spawnX, spawnY, spawnZ);
				}
				else
				{
					region.addSpawn(spawnX, spawnY, spawnZ);
				}
			}
			else if ("map".equalsIgnoreCase(nodeName))
			{
				region.addMap(c.Attribute("X").GetInt32(), c.Attribute("Y").GetInt32());
			}
			else if ("banned".equalsIgnoreCase(nodeName))
			{
				region.addBannedRace(c.Attribute("race").GetString(), c.Attribute("point").GetString());
			}
		}
		REGIONS.put(name, region);
	}
	
	/**
	 * @param locX
	 * @param locY
	 * @return
	 */
	public MapRegion getMapRegion(int locX, int locY)
	{
		foreach (MapRegion region in REGIONS.values())
		{
			if (region.isZoneInRegion(getMapRegionX(locX), getMapRegionY(locY)))
			{
				return region;
			}
		}
		return null;
	}
	
	/**
	 * @param locX
	 * @param locY
	 * @return
	 */
	public int getMapRegionLocId(int locX, int locY)
	{
		MapRegion region = getMapRegion(locX, locY);
		if (region != null)
		{
			return region.getLocId();
		}
		return 0;
	}
	
	/**
	 * @param obj
	 * @return
	 */
	public MapRegion getMapRegion(WorldObject obj)
	{
		return getMapRegion(obj.getX(), obj.getY());
	}
	
	/**
	 * @param obj
	 * @return
	 */
	public int getMapRegionLocId(WorldObject obj)
	{
		return getMapRegionLocId(obj.getX(), obj.getY());
	}
	
	/**
	 * @param posX
	 * @return
	 */
	public int getMapRegionX(int posX)
	{
		return (posX >> 15) + 9 + 11; // + centerTileX;
	}
	
	/**
	 * @param posY
	 * @return
	 */
	public int getMapRegionY(int posY)
	{
		return (posY >> 15) + 10 + 8; // + centerTileX;
	}
	
	/**
	 * Get town name by character position
	 * @param creature
	 * @return
	 */
	public String getClosestTownName(Creature creature)
	{
		MapRegion region = getMapRegion(creature);
		return region == null ? "Aden Castle Town" : region.getTown();
	}
	
	/**
	 * @param creature
	 * @param teleportWhere
	 * @return
	 */
	public ILocational getTeleToLocation(Creature creature, TeleportWhereType teleportWhere)
	{
		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			Castle castle = null;
			Fort fort = null;
			ClanHall clanhall = null;
			if ((player.getClan() != null) && !player.isFlyingMounted() && !player.isFlying()) // flying players in gracia cant use teleports to aden continent
			{
				// If teleport to clan hall
				if (teleportWhere == TeleportWhereType.CLANHALL)
				{
					clanhall = ClanHallData.getInstance().getClanHallByClan(player.getClan());
					if ((clanhall != null) && !player.isFlyingMounted())
					{
						return clanhall.getOwnerLocation();
					}
				}
				
				// If teleport to castle
				if (teleportWhere == TeleportWhereType.CASTLE)
				{
					castle = CastleManager.getInstance().getCastleByOwner(player.getClan());
					// Otherwise check if player is on castle or fortress ground
					// and player's clan is defender
					if (castle == null)
					{
						castle = CastleManager.getInstance().getCastle(player);
						if (!((castle != null) && castle.getSiege().isInProgress() && (castle.getSiege().getDefenderClan(player.getClan()) != null)))
						{
							castle = null;
						}
					}
					
					if ((castle != null) && (castle.getResidenceId() > 0))
					{
						if (player.getReputation() < 0)
						{
							return castle.getResidenceZone().getChaoticSpawnLoc();
						}
						return castle.getResidenceZone().getSpawnLoc();
					}
				}
				
				// If teleport to fortress
				if (teleportWhere == TeleportWhereType.FORTRESS)
				{
					fort = FortManager.getInstance().getFortByOwner(player.getClan());
					// Otherwise check if player is on castle or fortress ground
					// and player's clan is defender
					if (fort == null)
					{
						fort = FortManager.getInstance().getFort(player);
						if (!((fort != null) && fort.getSiege().isInProgress() && (fort.getOwnerClan() == player.getClan())))
						{
							fort = null;
						}
					}
					
					if ((fort != null) && (fort.getResidenceId() > 0))
					{
						if (player.getReputation() < 0)
						{
							return fort.getResidenceZone().getChaoticSpawnLoc();
						}
						return fort.getResidenceZone().getSpawnLoc();
					}
				}
				
				// If teleport to SiegeHQ
				if (teleportWhere == TeleportWhereType.SIEGEFLAG)
				{
					castle = CastleManager.getInstance().getCastle(player);
					fort = FortManager.getInstance().getFort(player);
					if (castle != null)
					{
						if (castle.getSiege().isInProgress())
						{
							// Check if player's clan is attacker
							Set<Npc> flags = castle.getSiege().getFlag(player.getClan());
							if ((flags != null) && !flags.isEmpty())
							{
								// Spawn to flag - Need more work to get player to the nearest flag
								return flags.First().getLocation();
							}
						}
					}
					else if (fort != null)
					{
						if (fort.getSiege().isInProgress())
						{
							// Check if player's clan is attacker
							Set<Npc> flags = fort.getSiege().getFlag(player.getClan());
							if ((flags != null) && !flags.isEmpty())
							{
								// Spawn to flag - Need more work to get player to the nearest flag
								return flags.First().getLocation();
							}
						}
					}
				}
			}
			
			// Timed Hunting zones.
			TimedHuntingZoneHolder timedHuntingZone = player.getTimedHuntingZone();
			if (timedHuntingZone != null)
			{
				Location exitLocation = timedHuntingZone.getExitLocation();
				if (exitLocation != null)
				{
					return exitLocation;
				}
			}
			
			// Karma player land out of city
			if (player.getReputation() < 0)
			{
				return getNearestKarmaRespawn(player);
			}
			
			// Checking if needed to be respawned in "far" town from the castle;
			// Check if player's clan is participating
			castle = CastleManager.getInstance().getCastle(player);
			if ((castle != null) && castle.getSiege().isInProgress() && (castle.getSiege().checkIsDefender(player.getClan()) || castle.getSiege().checkIsAttacker(player.getClan())))
			{
				return castle.getResidenceZone().getOtherSpawnLoc();
			}
			
			// Checking if in an instance
			Instance inst = player.getInstanceWorld();
			if (inst != null)
			{
				Location loc = inst.getExitLocation(player);
				if (loc != null)
				{
					return loc;
				}
			}
		}
		
		if (Config.FACTION_SYSTEM_ENABLED && Config.FACTION_RESPAWN_AT_BASE)
		{
			if (creature.getActingPlayer().isGood())
			{
				return Config.FACTION_GOOD_BASE_LOCATION;
			}
			if (creature.getActingPlayer().isEvil())
			{
				return Config.FACTION_EVIL_BASE_LOCATION;
			}
		}
		
		// Get the nearest town
		return getNearestTownRespawn(creature);
	}
	
	public Location getNearestKarmaRespawn(Player player)
	{
		try
		{
			RespawnZone zone = ZoneManager.getInstance().getZone<RespawnZone>(player);
			if (zone != null)
			{
				return getRestartRegion(player, zone.getRespawnPoint(player)).getChaoticSpawnLoc();
			}
			// Opposing race check.
			if (getMapRegion(player).getBannedRace().containsKey(player.getRace()))
			{
				return REGIONS.get(getMapRegion(player).getBannedRace().get(player.getRace())).getChaoticSpawnLoc();
			}
			return getMapRegion(player).getChaoticSpawnLoc();
		}
		catch (Exception e)
		{
			if (player.isFlyingMounted())
			{
				return REGIONS.get("union_base_of_kserth").getChaoticSpawnLoc();
			}
			return REGIONS.get(DEFAULT_RESPAWN).getChaoticSpawnLoc();
		}
	}
	
	public Location getNearestTownRespawn(Creature creature)
	{
		try
		{
			RespawnZone zone = ZoneManager.getInstance().getZone<RespawnZone>(creature);
			if (zone != null)
			{
				return getRestartRegion(creature, zone.getRespawnPoint((Player) creature)).getSpawnLoc();
			}
			// Opposing race check.
			if (getMapRegion(creature).getBannedRace().containsKey(creature.getRace()))
			{
				return REGIONS.get(getMapRegion(creature).getBannedRace().get(creature.getRace())).getChaoticSpawnLoc();
			}
			return getMapRegion(creature).getSpawnLoc();
		}
		catch (Exception e)
		{
			// Port to the default respawn if no closest town found.
			return REGIONS.get(DEFAULT_RESPAWN).getSpawnLoc();
		}
	}
	
	/**
	 * @param creature
	 * @param point
	 * @return
	 */
	public MapRegion getRestartRegion(Creature creature, String point)
	{
		try
		{
			Player player = (Player) creature;
			MapRegion region = REGIONS.get(point);
			if (region.getBannedRace().containsKey(player.getRace()))
			{
				getRestartRegion(player, region.getBannedRace().get(player.getRace()));
			}
			return region;
		}
		catch (Exception e)
		{
			return REGIONS.get(DEFAULT_RESPAWN);
		}
	}
	
	/**
	 * @param regionName the map region name.
	 * @return if exists the map region identified by that name, null otherwise.
	 */
	public MapRegion getMapRegionByName(String regionName)
	{
		return REGIONS.get(regionName);
	}
	
	public int getBBs(ILocational loc)
	{
		MapRegion region = getMapRegion(loc.getX(), loc.getY());
		return region != null ? region.getBbs() : REGIONS.get(DEFAULT_RESPAWN).getBbs();
	}
	
	/**
	 * Gets the single instance of {@code MapRegionManager}.
	 * @return single instance of {@code MapRegionManager}
	 */
	public static MapRegionManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MapRegionManager INSTANCE = new MapRegionManager();
	}
}