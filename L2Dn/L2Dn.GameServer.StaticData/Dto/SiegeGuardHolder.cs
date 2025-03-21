using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Dto;

public sealed record SiegeGuardHolder(int CastleId, int ItemId, SiegeGuardType GuardType, bool IsStationary, int NpcId,
    int MaxNpcAmount);