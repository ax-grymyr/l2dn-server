using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.TimedHuntingZones;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class TimedHuntingZoneData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(TimedHuntingZoneData));

    private FrozenDictionary<int, TimedHuntingZoneHolder> _timedHuntingZoneData =
        FrozenDictionary<int, TimedHuntingZoneHolder>.Empty;

    private TimedHuntingZoneData()
    {
    }

    public static TimedHuntingZoneData Instance { get; } = new();

    public void Load()
    {
        XmlTimedHuntingZoneList xmlTimedHuntingZoneList =
            XmlLoader.LoadXmlDocument<XmlTimedHuntingZoneList>("TimedHuntingZoneData.xml");

        _timedHuntingZoneData = xmlTimedHuntingZoneList.Enabled
            ? xmlTimedHuntingZoneList.Zones.
                Select(xmlTimedHuntingZone => new TimedHuntingZoneHolder(xmlTimedHuntingZone)).
                Where(holder =>
                {
                    if (!holder.EnterLocations.IsDefaultOrEmpty)
                        return true;

                    _logger.Error($"{nameof(TimedHuntingZoneData)}: No enter location for timed hunting " +
                        $"zone {holder.ZoneId} {holder.ZoneName}");

                    return false;

                }).ToFrozenDictionary(z => z.ZoneId)
            : FrozenDictionary<int, TimedHuntingZoneHolder>.Empty;

        _logger.Info($"{nameof(TimedHuntingZoneData)}: Loaded {_timedHuntingZoneData.Count} timed hunting zones.");
    }

    public TimedHuntingZoneHolder? GetHuntingZone(int zoneId) => _timedHuntingZoneData.GetValueOrDefault(zoneId);

    public ImmutableArray<TimedHuntingZoneHolder> HuntingZones => _timedHuntingZoneData.Values;
}