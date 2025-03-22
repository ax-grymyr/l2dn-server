using System.Collections.Frozen;
using System.Collections.Immutable;

namespace L2Dn.GameServer.Dto;

// Npcs: npcId, points
// Items: itemId, rate
public sealed record SubjugationHolder(int Category, ImmutableArray<SubjugationHotTime> HotTimes,
    FrozenDictionary<int, int> Npcs, FrozenDictionary<int, double> Items);