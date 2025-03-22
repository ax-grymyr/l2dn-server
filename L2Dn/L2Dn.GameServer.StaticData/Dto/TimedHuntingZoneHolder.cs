using System.Collections.Immutable;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.StaticData.Xml.TimedHuntingZones;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto;

public sealed class TimedHuntingZoneHolder
{
    public TimedHuntingZoneHolder(XmlTimedHuntingZone xmlTimedHuntingZone)
    {
        ZoneId = xmlTimedHuntingZone.Id;
        ZoneName = xmlTimedHuntingZone.Name;
        InitialTime = TimeSpan.FromSeconds(xmlTimedHuntingZone.InitialTimeSeconds);
        MaximumAddedTime = TimeSpan.FromSeconds(xmlTimedHuntingZone.MaxAddedTimeSeconds);
        ResetDelay = TimeSpan.FromSeconds(xmlTimedHuntingZone.ResetDelaySeconds);
        EntryItemId = xmlTimedHuntingZone.EntryItemId;
        EntryFee = xmlTimedHuntingZone.EntryFee;
        MinLevel = xmlTimedHuntingZone.MinLevel;
        MaxLevel = xmlTimedHuntingZone.MaxLevel;
        RemainRefillTime = TimeSpan.FromSeconds(xmlTimedHuntingZone.RemainRefillTimeSeconds);
        RefillTimeMax = TimeSpan.FromSeconds(xmlTimedHuntingZone.RefillTimeMaxSeconds);
        IsPvpZone = xmlTimedHuntingZone.PvpZone;
        IsNoPvpZone = xmlTimedHuntingZone.NoPvpZone;
        InstanceId = xmlTimedHuntingZone.InstanceId;
        IsSoloInstance = xmlTimedHuntingZone.SoloInstance;
        IsWeekly = xmlTimedHuntingZone.Weekly;
        UseWorldPrefix = false;
        PremiumUsersOnly = false;

        EnterLocations = xmlTimedHuntingZone.EnterLocations.Select(ParseLocation).ToImmutableArray();
        if (!string.IsNullOrEmpty(xmlTimedHuntingZone.ExitLocation))
            ExitLocation = ParseLocation(xmlTimedHuntingZone.ExitLocation);

        TileCoordinates = EnterLocations.
            Select(loc => WorldMap.WorldLocationToTileCoordinates(loc.Location2D)).ToImmutableArray();
    }

    public int ZoneId { get; }
    public string ZoneName { get; }
    public TimeSpan InitialTime { get; }
    public TimeSpan MaximumAddedTime { get; }
    public TimeSpan ResetDelay { get; }
    public int EntryItemId { get; }
    public int EntryFee { get; }
    public int MinLevel { get; }
    public int MaxLevel { get; }
    public TimeSpan RemainRefillTime { get; }
    public TimeSpan RefillTimeMax { get; }
    public bool IsPvpZone { get; }
    public bool IsNoPvpZone { get; }
    public int InstanceId { get; }
    public bool IsSoloInstance { get; }
    public bool IsWeekly { get; }
    public bool UseWorldPrefix { get; }
    public bool PremiumUsersOnly { get; }
    public ImmutableArray<Location3D> EnterLocations { get; }
    public ImmutableArray<Location2D> TileCoordinates { get; }
    public Location3D EnterLocation => EnterLocations[0];
    public Location3D? ExitLocation { get; }

    private static Location3D ParseLocation(string location)
    {
        ImmutableArray<int> coordinates = ParseUtil.ParseList<int>(location, ',');
        return new Location3D(coordinates[0], coordinates[1], coordinates[2]);
    }
}