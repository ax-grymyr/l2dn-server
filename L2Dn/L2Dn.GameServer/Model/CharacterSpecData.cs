using System.Collections.Immutable;

namespace L2Dn.GameServer.Model;

public sealed record CharacterSpecData(
    Location SpawnLocation,
    ImmutableArray<CharacterSpawnItem> SpawnItems,
    CharacterBaseStats BaseStats,
    CollisionDimensions MaleDimensions,
    CollisionDimensions FemaleDimensions,
    CharacterClassInfo BaseClass)
{
    public CollisionDimensions GetCollisionDimensions(Sex sex) => sex switch
    {
        Sex.Male => MaleDimensions,
        Sex.Female => FemaleDimensions,
        _ => throw new ArgumentOutOfRangeException(nameof(sex))
    };
}
