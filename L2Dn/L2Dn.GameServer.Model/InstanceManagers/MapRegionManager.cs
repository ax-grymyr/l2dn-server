using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Model.Xml;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

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
        Player? player = creature.getActingPlayer();
		if (creature.isPlayer() && player != null)
		{
			Castle? castle;
            Clan? clan = player.getClan();
			if (clan != null && !player.isFlyingMounted() && !player.isFlying()) // flying players in gracia cant use teleports to aden continent
			{
				// If teleport to clan hall
				if (teleportWhere == TeleportWhereType.CLANHALL)
				{
					ClanHall? clanhall = ClanHallData.getInstance().getClanHallByClan(clan);
					if (clanhall != null && !player.isFlyingMounted())
					{
						return new Location(clanhall.getOwnerLocation(), 0);
					}
				}

				// If teleport to castle
				if (teleportWhere == TeleportWhereType.CASTLE)
				{
					castle = CastleManager.getInstance().getCastleByOwner(clan);
					// Otherwise check if player is on castle or fortress ground
					// and player's clan is defender
					if (castle == null)
					{
						castle = CastleManager.getInstance().getCastle(player);
						if (!(castle != null && castle.getSiege().isInProgress() && castle.getSiege().getDefenderClan(clan) != null))
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
					fort = FortManager.getInstance().getFortByOwner(clan);
					// Otherwise check if player is on castle or fortress ground
					// and player's clan is defender
					if (fort == null)
					{
						fort = FortManager.getInstance().getFort(player);
						if (!(fort != null && fort.getSiege().isInProgress() && fort.getOwnerClan() == clan))
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
							Set<Npc> flags = castle.getSiege().getFlag(clan);
							if (flags != null && !flags.isEmpty())
							{
								// Spawn to flag - Need more work to get player to the nearest flag
								return flags.First().Location;
							}
						}
					}
					else if (fort != null)
					{
						if (fort.getSiege().isInProgress())
						{
							// Check if player's clan is attacker
							Set<Npc> flags = fort.getSiege().getFlag(clan);
							if (flags != null && !flags.isEmpty())
							{
								// Spawn to flag - Need more work to get player to the nearest flag
								return flags.First().Location;
							}
						}
					}
				}
			}

			// Timed Hunting zones.
			TimedHuntingZoneHolder? timedHuntingZone = player.getTimedHuntingZone();
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
            if (castle != null && clan != null && castle.getSiege().isInProgress() &&
                (castle.getSiege().checkIsDefender(clan) || castle.getSiege().checkIsAttacker(clan)))
            {
                return new Location(castle.getResidenceZone().getOtherSpawnLoc(), player.getHeading());
            }

            // Checking if in an instance
			Instance? inst = player.getInstanceWorld();
			if (inst != null)
			{
				Location3D? loc = inst.getExitLocation(player);
				if (loc != null)
				{
					return new Location(loc.Value, 0);
				}
			}
		}

		if (Config.FactionSystem.FACTION_SYSTEM_ENABLED && Config.FactionSystem.FACTION_RESPAWN_AT_BASE)
		{
			if (player != null && player.isGood())
			{
				return Config.FactionSystem.FACTION_GOOD_BASE_LOCATION;
			}

			if (player != null && player.isEvil())
			{
				return Config.FactionSystem.FACTION_EVIL_BASE_LOCATION;
			}
		}

		// Get the nearest town
		return new Location(getNearestTownRespawn(creature), 0);
	}

	public Location3D getNearestKarmaRespawn(Player player)
	{
		try
		{
			RespawnZone? zone = ZoneManager.getInstance().getZone<RespawnZone>(player.Location.Location3D);
			if (zone != null)
            {
                string? respawnPoint = zone.getRespawnPoint(player);
                if (respawnPoint != null)
                {
                    MapRegion? restartRegion = getRestartRegion(player, respawnPoint);
                    if (restartRegion != null)
                        return restartRegion.getChaoticSpawnLoc();
                }
            }

			// Opposing race check
            MapRegion? mapRegion = getMapRegion(player);
            if (mapRegion != null)
            {
                if (mapRegion.getBannedRace().TryGetValue(player.getRace(), out string? value))
                {
                    MapRegion? restartRegion = _regions.GetValueOrDefault(value);
                    if (restartRegion != null)
                        return restartRegion.getChaoticSpawnLoc();
                }

                return mapRegion.getChaoticSpawnLoc();
            }

            MapRegion? defaultRegion = _regions.GetValueOrDefault(DefaultRespawn);
            if (defaultRegion != null)
                return defaultRegion.getChaoticSpawnLoc();

            throw new InvalidOperationException("No default respawn location found.");
		}
		catch (Exception e)
		{
            _logger.Error(e);

			if (player.isFlyingMounted())
            {
                MapRegion? restartRegion = _regions.GetValueOrDefault("union_base_of_kserth");
                if (restartRegion != null)
				    return restartRegion.getChaoticSpawnLoc();
			}

            MapRegion? defaultRegion = _regions.GetValueOrDefault(DefaultRespawn);
            if (defaultRegion != null)
                return defaultRegion.getChaoticSpawnLoc();

            throw new InvalidOperationException("No default respawn location found.");
		}
	}

	public Location3D getNearestTownRespawn(Creature creature)
	{
		try
		{
			RespawnZone? zone = ZoneManager.getInstance().getZone<RespawnZone>(creature.Location.Location3D);
			if (zone != null)
			{
                string? respawnPoint = zone.getRespawnPoint((Player)creature);
                if (respawnPoint != null)
                {
                    MapRegion? restartRegion = getRestartRegion(creature, respawnPoint);
                    if (restartRegion != null)
                        return restartRegion.getSpawnLoc();
                }
			}

			// Opposing race check.
            MapRegion? mapRegion = getMapRegion(creature);
            if (mapRegion != null)
            {
                if (mapRegion.getBannedRace().TryGetValue(creature.getRace(), out string? value))
                {
                    MapRegion? restartRegion = _regions.GetValueOrDefault(value);
                    if (restartRegion != null)
                        return restartRegion.getChaoticSpawnLoc();
                }

                return mapRegion.getSpawnLoc();
            }

            MapRegion? defaultRegion = _regions.GetValueOrDefault(DefaultRespawn);
            if (defaultRegion != null)
                return defaultRegion.getSpawnLoc();

            throw new InvalidOperationException("No default respawn location found.");
        }
		catch (Exception e)
        {
            _logger.Error(e);

			// Port to the default respawn if no closest town found.
            MapRegion? defaultRegion = _regions.GetValueOrDefault(DefaultRespawn);
            if (defaultRegion != null)
                return defaultRegion.getSpawnLoc();

            throw new InvalidOperationException("No default respawn location found.");
		}
	}

	/**
	 * @param creature
	 * @param point
	 * @return
	 */
	public MapRegion getRestartRegion(Creature creature, string point)
	{
		try
		{
			MapRegion? region = _regions.GetValueOrDefault(point);
            if (region != null)
            {
                if (region.getBannedRace().TryGetValue(creature.getRace(), out string? value))
                {
                    MapRegion? restartRegion = _regions.GetValueOrDefault(value);
                    if (restartRegion != null)
                        return restartRegion;
                }
            }

            MapRegion? defaultRegion = _regions.GetValueOrDefault(DefaultRespawn);
            if (defaultRegion != null)
                return defaultRegion;

            throw new InvalidOperationException("No default restart region found.");
		}
		catch (Exception e)
		{
            _logger.Error(e);

            MapRegion? defaultRegion = _regions.GetValueOrDefault(DefaultRespawn);
            if (defaultRegion != null)
                return defaultRegion;

            throw new InvalidOperationException("No default restart region found.");
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