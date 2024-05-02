using System.Collections.Frozen;
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
using L2Dn.Geometry;
using L2Dn.Model.DataPack;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Map Region Manager.
 * @author Nyaran
 */
public class MapRegionManager: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(MapRegionManager));

	private static FrozenDictionary<string, MapRegion> _regions = FrozenDictionary<string, MapRegion>.Empty;
	private const string DefaultRespawn = "talking_island_town";
	
	private MapRegionManager()
	{
		load();
	}
	
	public void load()
	{
		Dictionary<string, MapRegion> regions = new Dictionary<string, MapRegion>();
		LoadXmlDocuments<XmlMapRegionList>(DataFileLocation.Data, "mapregion")
			.Where(t => t.Document.Enabled)
			.SelectMany(t => t.Document.Regions)
			.Select(xmlRegion =>
			{
				MapRegion region = new(xmlRegion.Name, xmlRegion.Town, xmlRegion.LocationId, xmlRegion.Bbs);
				foreach (XmlMapRegionRespawnPoint respawnPoint in xmlRegion.RespawnPoints)
				{
					if (respawnPoint.IsOther)
						region.addOtherSpawn(respawnPoint.X, respawnPoint.Y, respawnPoint.Z);
					else if (respawnPoint.IsChaotic)
						region.addChaoticSpawn(respawnPoint.X, respawnPoint.Y, respawnPoint.Z);
					else if (respawnPoint.IsBanish)
						region.addBanishSpawn(respawnPoint.X, respawnPoint.Y, respawnPoint.Z);
					else
						region.addSpawn(respawnPoint.X, respawnPoint.Y, respawnPoint.Z);
				}

				foreach (XmlMapRegionMap map in xmlRegion.Maps)
					region.addMap(map.X, map.Y);

				foreach (XmlMapRegionBanned banned in xmlRegion.Banned)
					region.addBannedRace(banned.Race, banned.Point);

				return region;
			})
			.ForEach(region =>
			{
				if (!regions.TryAdd(region.getName(), region))
					_logger.Error(nameof(MapRegionManager) + $": Duplicated region name '{region.getName()}'");
			});

		_regions = regions.ToFrozenDictionary();
		
		_logger.Info(GetType().Name +": Loaded " + regions.Count + " map regions.");
	}
	
	/**
	 * @param locX
	 * @param locY
	 * @return
	 */
	public MapRegion? getMapRegion(int locX, int locY)
	{
		foreach (MapRegion region in _regions.Values)
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
		return getMapRegion(locX, locY)?.getLocId() ?? 0;
	}
	
	/**
	 * @param obj
	 * @return
	 */
	public MapRegion? getMapRegion(WorldObject obj)
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
	public string getClosestTownName(Creature creature)
	{
		return getMapRegion(creature)?.getTown() ?? "Aden Castle Town";
	}
	
	/**
	 * @param creature
	 * @param teleportWhere
	 * @return
	 */
	public Location getTeleToLocation(Creature creature, TeleportWhereType teleportWhere)
	{
		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			Castle castle;
			if (player.getClan() != null && !player.isFlyingMounted() && !player.isFlying()) // flying players in gracia cant use teleports to aden continent
			{
				// If teleport to clan hall
				if (teleportWhere == TeleportWhereType.CLANHALL)
				{
					ClanHall? clanhall = ClanHallData.getInstance().getClanHallByClan(player.getClan());
					if (clanhall != null && !player.isFlyingMounted())
					{
						return new Location(clanhall.getOwnerLocation(), 0);
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
						if (!(castle != null && castle.getSiege().isInProgress() && castle.getSiege().getDefenderClan(player.getClan()) != null))
						{
							castle = null;
						}
					}
					
					if (castle != null && castle.getResidenceId() > 0)
					{
						if (player.getReputation() < 0)
						{
							return new Location(castle.getResidenceZone().getChaoticSpawnLoc(), player.getHeading());
						}

						return new Location(castle.getResidenceZone().getSpawnLoc(), player.getHeading());
					}
				}
				
				// If teleport to fortress
				Fort? fort;
				if (teleportWhere == TeleportWhereType.FORTRESS)
				{
					fort = FortManager.getInstance().getFortByOwner(player.getClan());
					// Otherwise check if player is on castle or fortress ground
					// and player's clan is defender
					if (fort == null)
					{
						fort = FortManager.getInstance().getFort(player);
						if (!(fort != null && fort.getSiege().isInProgress() && fort.getOwnerClan() == player.getClan()))
						{
							fort = null;
						}
					}
					
					if (fort != null && fort.getResidenceId() > 0)
					{
						if (player.getReputation() < 0)
						{
							return new Location(fort.getResidenceZone().getChaoticSpawnLoc(), player.getHeading());
						}
						return new Location(fort.getResidenceZone().getSpawnLoc(), player.getHeading());
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
							if (flags != null && !flags.isEmpty())
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
							if (flags != null && !flags.isEmpty())
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
				Location3D? exitLocation = timedHuntingZone.getExitLocation();
				if (exitLocation != null)
				{
					return new Location(exitLocation.Value, 0);
				}
			}
			
			// Karma player land out of city
			if (player.getReputation() < 0)
			{
				return new Location(getNearestKarmaRespawn(player), 0);
			}
			
			// Checking if needed to be respawned in "far" town from the castle;
			// Check if player's clan is participating
			castle = CastleManager.getInstance().getCastle(player);
			if (castle != null && castle.getSiege().isInProgress() && (castle.getSiege().checkIsDefender(player.getClan()) || castle.getSiege().checkIsAttacker(player.getClan())))
			{
				return new Location(castle.getResidenceZone().getOtherSpawnLoc(), player.getHeading());
			}
			
			// Checking if in an instance
			Instance inst = player.getInstanceWorld();
			if (inst != null)
			{
				Location3D? loc = inst.getExitLocation(player);
				if (loc != null)
				{
					return new Location(loc.Value, 0);
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
		return new Location(getNearestTownRespawn(creature), 0);
	}
	
	public Location3D getNearestKarmaRespawn(Player player)
	{
		try
		{
			RespawnZone? zone = ZoneManager.getInstance().getZone<RespawnZone>(player.getLocation().Location3D);
			if (zone != null)
			{
				return getRestartRegion(player, zone.getRespawnPoint(player)).getChaoticSpawnLoc();
			}
			// Opposing race check.
			if (getMapRegion(player).getBannedRace().containsKey(player.getRace()))
			{
				return _regions.GetValueOrDefault(getMapRegion(player).getBannedRace().get(player.getRace())).getChaoticSpawnLoc();
			}
			return getMapRegion(player).getChaoticSpawnLoc();
		}
		catch (Exception e)
		{
			if (player.isFlyingMounted())
			{
				return _regions.GetValueOrDefault("union_base_of_kserth").getChaoticSpawnLoc();
			}
			
			return _regions.GetValueOrDefault(DefaultRespawn).getChaoticSpawnLoc();
		}
	}
	
	public Location3D getNearestTownRespawn(Creature creature)
	{
		try
		{
			RespawnZone? zone = ZoneManager.getInstance().getZone<RespawnZone>(creature.getLocation().Location3D);
			if (zone != null)
			{
				return getRestartRegion(creature, zone.getRespawnPoint((Player) creature)).getSpawnLoc();
			}
			// Opposing race check.
			if (getMapRegion(creature).getBannedRace().containsKey(creature.getRace()))
			{
				return _regions.GetValueOrDefault(getMapRegion(creature).getBannedRace().get(creature.getRace())).getChaoticSpawnLoc();
			}
			return getMapRegion(creature).getSpawnLoc();
		}
		catch (Exception e)
		{
			// Port to the default respawn if no closest town found.
			return _regions.GetValueOrDefault(DefaultRespawn).getSpawnLoc();
		}
	}
	
	/**
	 * @param creature
	 * @param point
	 * @return
	 */
	public MapRegion? getRestartRegion(Creature creature, string point)
	{
		try
		{
			Player player = (Player) creature;
			MapRegion? region = _regions.GetValueOrDefault(point);
			if (region.getBannedRace().containsKey(player.getRace()))
			{
				getRestartRegion(player, region.getBannedRace().get(player.getRace()));
			}
			return region;
		}
		catch (Exception e)
		{
			return _regions.GetValueOrDefault(DefaultRespawn);
		}
	}
	
	/**
	 * @param regionName the map region name.
	 * @return if exists the map region identified by that name, null otherwise.
	 */
	public MapRegion? getMapRegionByName(string regionName)
	{
		return _regions.GetValueOrDefault(regionName);
	}
	
	public int getBBs(Location2D location)
	{
		return getMapRegion(location.X, location.Y)?.getBbs() ??
		       _regions.GetValueOrDefault(DefaultRespawn)?.getBbs() ?? 0;
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
		public static readonly MapRegionManager INSTANCE = new();
	}
}