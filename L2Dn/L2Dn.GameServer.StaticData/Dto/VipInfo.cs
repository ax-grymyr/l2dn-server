using System.Collections.Immutable;

namespace L2Dn.GameServer.Dto;

public sealed record VipInfo(int Tier, long PointsRequired, long PointsDepreciated,
    ImmutableArray<VipBonusInfo> BonusList);