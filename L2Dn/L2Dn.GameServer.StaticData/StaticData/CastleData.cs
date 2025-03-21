using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.Castles;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class CastleData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(CastleData));

    private FrozenDictionary<(int, CastleSide), ImmutableArray<CastleSpawnHolder>> _spawns =
        FrozenDictionary<(int, CastleSide), ImmutableArray<CastleSpawnHolder>>.Empty;

    private FrozenDictionary<int, ImmutableArray<SiegeGuardHolder>> _siegeGuards =
        FrozenDictionary<int, ImmutableArray<SiegeGuardHolder>>.Empty;

    private CastleData()
    {
    }

    public static CastleData Instance { get; } = new();

    public void Load()
    {
        List<XmlCastle> xmlCastleList = XmlLoader.LoadXmlDocuments<XmlCastleList>("residences/castles", true).
            SelectMany(doc => doc.Castles).ToList();

        List<int> duplicateCastleIds = xmlCastleList.Select(c => c.Id).Duplicates().ToList();
        if (duplicateCastleIds.Count != 0)
            _logger.Warn($"{nameof(CastleData)}: Duplicate castle definitions {string.Join(",", duplicateCastleIds)}.");

        _spawns = xmlCastleList.SelectMany(xmlCastle => xmlCastle.Spawns.Select(xmlCastleSpawn =>
                (Key: (xmlCastle.Id, xmlCastleSpawn.CastleSide), Spawn: xmlCastleSpawn))).
            GroupBy(tuple => tuple.Key, tuple => tuple.Spawn).
            ToFrozenDictionary(g => g.Key,
                g => g.Select(s => new CastleSpawnHolder(s.Id, s.CastleSide, new Location(s.X, s.Y, s.Z, s.Heading))).
                    ToImmutableArray());

        _siegeGuards = xmlCastleList.Select(xmlCastle => KeyValuePair.Create(xmlCastle.Id,
            xmlCastle.SiegeGuards.Select(g =>
                    new SiegeGuardHolder(xmlCastle.Id, g.ItemId, g.Type, g.Stationary, g.NpcId, g.NpcMaxAmount)).
                ToImmutableArray())).ToFrozenDictionary();
    }

    public ImmutableArray<CastleSpawnHolder> GetSpawnsForSide(int castleId, CastleSide side) =>
        _spawns.GetValueOrDefault((castleId, side), ImmutableArray<CastleSpawnHolder>.Empty);

    public ImmutableArray<SiegeGuardHolder> GetSiegeGuardsForCastle(int castleId) =>
        _siegeGuards.GetValueOrDefault(castleId, ImmutableArray<SiegeGuardHolder>.Empty);

    public FrozenDictionary<int, ImmutableArray<SiegeGuardHolder>> GetSiegeGuards() => _siegeGuards;
}