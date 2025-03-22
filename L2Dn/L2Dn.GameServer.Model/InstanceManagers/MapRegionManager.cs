using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Map Region Manager.
 * @author Nyaran
 */
public static class MapRegionManager
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(MapRegionManager));

    public static Location GetTeleToLocation(Creature creature, TeleportWhereType teleportWhere)
    {
        Player? player = creature.getActingPlayer();
        if (creature.isPlayer() && player != null)
        {
            Castle? castle;
            Clan? clan = player.getClan();
            if (clan != null && !player.isFlyingMounted() &&
                !player.isFlying()) // flying players in gracia cant use teleports to aden continent
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
                        if (!(castle != null && castle.getSiege().isInProgress() &&
                                castle.getSiege().getDefenderClan(clan) != null))
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
                Location3D? exitLocation = timedHuntingZone.ExitLocation;
                if (exitLocation != null)
                {
                    return new Location(exitLocation.Value, 0);
                }
            }

            // Karma player land out of city
            if (player.getReputation() < 0)
            {
                return new Location(GetNearestKarmaRespawn(player), 0);
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
        return new Location(GetNearestTownRespawn(creature), 0);
    }

    public static Location3D GetNearestKarmaRespawn(Player player)
    {
        try
        {
            RespawnZone? zone = ZoneManager.Instance.getZone<RespawnZone>(player.Location.Location3D);
            if (zone != null)
            {
                string? respawnPoint = zone.getRespawnPoint(player);
                if (respawnPoint != null)
                {
                    MapRegion? restartRegion = GetRestartRegion(player, respawnPoint);
                    if (restartRegion != null)
                        return restartRegion.GetChaoticSpawnLocation();
                }
            }

            // Opposing race check
            MapRegion? mapRegion = MapRegionData.Instance.GetMapRegion(player);
            if (mapRegion != null)
            {
                if (mapRegion.BannedRaces.TryGetValue(player.getRace(), out string? value))
                {
                    MapRegion? restartRegion = MapRegionData.Instance.GetMapRegionByName(value);
                    if (restartRegion != null)
                        return restartRegion.GetChaoticSpawnLocation();
                }

                return mapRegion.GetChaoticSpawnLocation();
            }

            MapRegion? defaultRegion = MapRegionData.Instance.DefaultRespawnRegion;
            if (defaultRegion != null)
                return defaultRegion.GetChaoticSpawnLocation();

            throw new InvalidOperationException("No default respawn location found.");
        }
        catch (Exception e)
        {
            _logger.Error(e);

            if (player.isFlyingMounted())
            {
                MapRegion? restartRegion = MapRegionData.Instance.GetMapRegionByName("union_base_of_kserth");
                if (restartRegion != null)
                    return restartRegion.GetChaoticSpawnLocation();
            }

            MapRegion? defaultRegion = MapRegionData.Instance.DefaultRespawnRegion;
            if (defaultRegion != null)
                return defaultRegion.GetChaoticSpawnLocation();

            throw new InvalidOperationException("No default respawn location found.");
        }
    }

    public static Location3D GetNearestTownRespawn(Creature creature)
    {
        try
        {
            RespawnZone? zone = ZoneManager.Instance.getZone<RespawnZone>(creature.Location.Location3D);
            if (zone != null)
            {
                string? respawnPoint = zone.getRespawnPoint((Player)creature);
                if (respawnPoint != null)
                {
                    MapRegion? restartRegion = GetRestartRegion(creature, respawnPoint);
                    if (restartRegion != null)
                        return restartRegion.GetSpawnLocation();
                }
            }

            // Opposing race check.
            MapRegion? mapRegion = MapRegionData.Instance.GetMapRegion(creature);
            if (mapRegion != null)
            {
                if (mapRegion.BannedRaces.TryGetValue(creature.getRace(), out string? value))
                {
                    MapRegion? restartRegion = MapRegionData.Instance.GetMapRegionByName(value);
                    if (restartRegion != null)
                        return restartRegion.GetChaoticSpawnLocation();
                }

                return mapRegion.GetSpawnLocation();
            }

            MapRegion? defaultRegion = MapRegionData.Instance.DefaultRespawnRegion;
            if (defaultRegion != null)
                return defaultRegion.GetSpawnLocation();

            throw new InvalidOperationException("No default respawn location found.");
        }
        catch (Exception e)
        {
            _logger.Error(e);

            // Port to the default respawn if no closest town found.
            MapRegion? defaultRegion = MapRegionData.Instance.DefaultRespawnRegion;
            if (defaultRegion != null)
                return defaultRegion.GetSpawnLocation();

            throw new InvalidOperationException("No default respawn location found.");
        }
    }

    public static MapRegion GetRestartRegion(Creature creature, string point)
    {
        try
        {
            MapRegion? region = MapRegionData.Instance.GetMapRegionByName(point);
            if (region != null)
            {
                if (region.BannedRaces.TryGetValue(creature.getRace(), out string? value))
                {
                    MapRegion? restartRegion = MapRegionData.Instance.GetMapRegionByName(value);
                    if (restartRegion != null)
                        return restartRegion;
                }
            }

            MapRegion? defaultRegion = MapRegionData.Instance.DefaultRespawnRegion;
            if (defaultRegion != null)
                return defaultRegion;

            throw new InvalidOperationException("No default restart region found.");
        }
        catch (Exception e)
        {
            _logger.Error(e);

            MapRegion? defaultRegion = MapRegionData.Instance.DefaultRespawnRegion;
            if (defaultRegion != null)
                return defaultRegion;

            throw new InvalidOperationException("No default restart region found.");
        }
    }
}