using System.Collections.Immutable;

namespace L2Dn.GameServer.Dto;

public sealed record LuckyGameDataHolder(int Index, int TurningPoints, ImmutableArray<ItemChanceHolder> CommonRewards,
    ImmutableArray<ItemPointHolder> UniqueRewards, int MinModifyRewardGame, int MaxModifyRewardGame,
    ImmutableArray<ItemChanceHolder> ModifyRewards);