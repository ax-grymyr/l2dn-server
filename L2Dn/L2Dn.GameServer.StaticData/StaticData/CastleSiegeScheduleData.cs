using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.CastleSiegeSchedules;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class CastleSiegeScheduleData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(CastleSiegeScheduleData));

    private FrozenDictionary<int, CastleSiegeSchedule> _scheduleData = FrozenDictionary<int, CastleSiegeSchedule>.Empty;

    private CastleSiegeScheduleData()
    {
    }

    public static CastleSiegeScheduleData Instance { get; } = new();

    public void Load()
    {
        _scheduleData = XmlLoader.
            LoadConfigXmlDocument<XmlCastleSiegeScheduleList>("SiegeSchedule.xml").Schedules.
            Select(xmlCastleSiegeSchedule => new CastleSiegeSchedule(xmlCastleSiegeSchedule.CastleId,
                xmlCastleSiegeSchedule.CastleName,
                Enum.Parse<DayOfWeek>(xmlCastleSiegeSchedule.Day, true), xmlCastleSiegeSchedule.Hour,
                xmlCastleSiegeSchedule.MaxConcurrent, xmlCastleSiegeSchedule.SiegeEnabled)).
            ToFrozenDictionary(x => x.CastleId);

        _logger.Info($"{nameof(CastleSiegeScheduleData)}: Loaded {_scheduleData.Count} siege schedules.");
    }

    public CastleSiegeSchedule? GetSchedule(int castleId) => _scheduleData.GetValueOrDefault(castleId);
}