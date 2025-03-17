using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.MapRegions;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Dto;

public sealed class MapRegion
{
    internal MapRegion(XmlMapRegion xmlMapRegion)
    {
        Name = xmlMapRegion.Name;
        Town = xmlMapRegion.Town;
        LocationId = xmlMapRegion.LocationId;
        BBs = xmlMapRegion.Bbs;

        MapTiles = xmlMapRegion.MapTiles.Select(tile => new Location2D(tile.X, tile.Y)).ToImmutableArray();

        SpawnLocations = xmlMapRegion.RespawnPoints.Where(point => !(point.IsOther || point.IsChaotic || point.IsBanish)).
            Select(point => new Location3D(point.X, point.Y, point.Z)).ToImmutableArray();

        OtherSpawnLocations = xmlMapRegion.RespawnPoints.Where(point => point.IsOther).
            Select(point => new Location3D(point.X, point.Y, point.Z)).ToImmutableArray();

        ChaoticSpawnLocations = xmlMapRegion.RespawnPoints.Where(point => point.IsChaotic).
            Select(point => new Location3D(point.X, point.Y, point.Z)).ToImmutableArray();

        BanishSpawnLocations = xmlMapRegion.RespawnPoints.Where(point => point.IsBanish).
            Select(point => new Location3D(point.X, point.Y, point.Z)).ToImmutableArray();

        BannedRaces = xmlMapRegion.Banned.ToFrozenDictionary(banned => banned.Race, banned => banned.Point);
    }

    public string Name { get; }
    public string Town { get; }
    public int LocationId { get; }
    public int BBs { get; }
    public ImmutableArray<Location2D> MapTiles { get; }
    public ImmutableArray<Location3D> SpawnLocations { get; }
    public ImmutableArray<Location3D> OtherSpawnLocations { get; }
    public ImmutableArray<Location3D> ChaoticSpawnLocations { get; }
    public ImmutableArray<Location3D> BanishSpawnLocations { get; }
    public FrozenDictionary<Race, string> BannedRaces { get; }

    public bool IsZoneInRegion(Location2D tileCoordinates)
    {
        foreach (Location2D tile in MapTiles)
        {
            if (tile == tileCoordinates)
                return true;
        }

        return false;
    }

    public Location3D GetSpawnLocation() =>
        Config.Character.RANDOM_RESPAWN_IN_TOWN_ENABLED ? SpawnLocations.GetRandomElement() : SpawnLocations[0];

    public Location3D GetOtherSpawnLocation()
    {
        if (!OtherSpawnLocations.IsDefaultOrEmpty)
        {
            return Config.Character.RANDOM_RESPAWN_IN_TOWN_ENABLED
                ? OtherSpawnLocations.GetRandomElement()
                : OtherSpawnLocations[0];
        }

        return GetSpawnLocation();
    }

    public Location3D GetChaoticSpawnLocation()
    {
        if (!ChaoticSpawnLocations.IsDefaultOrEmpty)
        {
            return Config.Character.RANDOM_RESPAWN_IN_TOWN_ENABLED
                ? ChaoticSpawnLocations.GetRandomElement()
                : ChaoticSpawnLocations[0];
        }

        return GetSpawnLocation();
    }

    public Location3D GetBanishSpawnLocation()
    {
        if (!BanishSpawnLocations.IsDefaultOrEmpty)
        {
            return Config.Character.RANDOM_RESPAWN_IN_TOWN_ENABLED
                ? BanishSpawnLocations.GetRandomElement()
                : BanishSpawnLocations[0];
        }

        return GetSpawnLocation();
    }
}