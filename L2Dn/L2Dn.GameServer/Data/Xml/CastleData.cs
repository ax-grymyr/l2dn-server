using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author St3eT
 */
public class CastleData: DataReaderBase
{
	private readonly Map<int, List<CastleSpawnHolder>> _spawns = new();
	private static readonly Map<int, List<SiegeGuardHolder>> _siegeGuards = new();
	
	protected CastleData()
	{
		load();
	}
	
	public void load()
	{
		_spawns.clear();
		_siegeGuards.clear();
		
		LoadXmlDocuments(DataFileLocation.Data, "residences/castles", true).ForEach(t =>
		{
			t.Document.Elements("list").Elements("castle").ForEach(x => loadElement(t.FilePath, x));
		});
	}

	private void loadElement(string filePath, XElement element)
	{
		int castleId = element.Attribute("id").GetInt32();

		element.Elements("spawns").ForEach(el =>
		{
			List<CastleSpawnHolder> spawns = new();

			el.Elements("npc").ForEach(e =>
			{
				int npcId = e.Attribute("id").GetInt32();
				CastleSide side = e.Attribute("castleSide").GetEnum(CastleSide.NEUTRAL);
				int x = e.Attribute("x").GetInt32();
				int y = e.Attribute("y").GetInt32();
				int z = e.Attribute("z").GetInt32();
				int heading = e.Attribute("heading").GetInt32();
				spawns.add(new CastleSpawnHolder(npcId, side, x, y, z, heading));
			});

			_spawns.put(castleId, spawns);
		});

		element.Elements("siegeGuards").ForEach(el =>
		{
			List<SiegeGuardHolder> guards = new();

			el.Elements("guard").ForEach(e =>
			{
				int itemId = e.Attribute("itemId").GetInt32();
				SiegeGuardType type = e.Attribute("type").GetEnum<SiegeGuardType>();
				bool stationary = e.Attribute("stationary").GetBoolean(false);
				int npcId = e.Attribute("npcId").GetInt32();
				int npcMaxAmount = e.Attribute("npcMaxAmount").GetInt32();
				guards.add(new SiegeGuardHolder(castleId, itemId, type, stationary, npcId, npcMaxAmount));
			});

			_siegeGuards.put(castleId, guards);
		});
	}

	public List<CastleSpawnHolder> getSpawnsForSide(int castleId, CastleSide side)
	{
		List<CastleSpawnHolder> result = new();
		if (_spawns.containsKey(castleId))
		{
			foreach (CastleSpawnHolder spawn in _spawns.get(castleId))
			{
				if (spawn.getSide() == side)
				{
					result.add(spawn);
				}
			}
		}
		return result;
	}
	
	public List<SiegeGuardHolder> getSiegeGuardsForCastle(int castleId)
	{
		return _siegeGuards.getOrDefault(castleId, new());
	}
	
	public Map<int, List<SiegeGuardHolder>> getSiegeGuards()
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