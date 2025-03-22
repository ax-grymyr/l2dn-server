namespace L2Dn.GameServer.Dto;

public sealed record CastleSiegeSchedule(int CastleId, string CastleName, DayOfWeek Day, int Hour, int MaxConcurrent,
    bool SiegeEnabled);