using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Geometry;
using L2Dn.Model.DataPack;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author St3eT
 */
public class CastleData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(CastleData));
    
	private FrozenDictionary<int, ImmutableArray<CastleSpawnHolder>> _spawns =
		FrozenDictionary<int, ImmutableArray<CastleSpawnHolder>>.Empty;

	private FrozenDictionary<int, ImmutableArray<SiegeGuardHolder>> _siegeGuards =
		FrozenDictionary<int, ImmutableArray<SiegeGuardHolder>>.Empty;
	
	protected CastleData()
	{
		load();
	}
	
	public void load()
	{
		Dictionary<int, List<CastleSpawnHolder>> spawns = new();
		Dictionary<int, List<SiegeGuardHolder>> siegeGuards = new();

		LoadXmlDocuments<XmlCastleList>(DataFileLocation.Data, "residences/castles", true)
			.SelectMany(t => t.Document.Castles)
			.ForEach(castle =>
			{
				List<CastleSpawnHolder> spawnList = castle.Spawns
					.Select(s => new CastleSpawnHolder(s.Id, s.CastleSide, new Location(s.X, s.Y, s.Z, s.Heading)))
					.ToList();

				if (!spawns.TryAdd(castle.Id, spawnList))
					_logger.Warn(nameof(CastleData) + $": Duplicated castle definition id={castle.Id}");

				List<SiegeGuardHolder> guardList = castle.SiegeGuards
					.Select(g => new SiegeGuardHolder(castle.Id, g.ItemId, g.Type, g.Stationary, g.NpcId,
						g.NpcMaxAmount))
					.ToList();

				if (!siegeGuards.TryAdd(castle.Id, guardList))
					_logger.Warn(nameof(CastleData) + $": Duplicated castle definition id={castle.Id}");
			});

		_spawns = spawns.ToFrozenDictionary(t => t.Key, t => t.Value.ToImmutableArray());
		_siegeGuards = siegeGuards.ToFrozenDictionary(t => t.Key, t => t.Value.ToImmutableArray());
	}

	public ImmutableArray<CastleSpawnHolder> getSpawnsForSide(int castleId, CastleSide side)
	{
		if (_spawns.TryGetValue(castleId, out ImmutableArray<CastleSpawnHolder> spawns))
			return spawns.Where(spawn => spawn.getSide() == side).ToImmutableArray();
		
		return ImmutableArray<CastleSpawnHolder>.Empty;
	}
	
	public ImmutableArray<SiegeGuardHolder> getSiegeGuardsForCastle(int castleId)
	{
		return _siegeGuards.GetValueOrDefault(castleId, ImmutableArray<SiegeGuardHolder>.Empty);
	}
	
	public FrozenDictionary<int, ImmutableArray<SiegeGuardHolder>> getSiegeGuards()
	{
		return _siegeGuards;
	}
	
	/**
	 * Gets the single instance of CastleData.
	 * @return single instance of CastleData
	 */
	public static CastleData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CastleData INSTANCE = new();
	}
}