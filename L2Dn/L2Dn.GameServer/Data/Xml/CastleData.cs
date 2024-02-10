using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author St3eT
 */
public class CastleData
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
		parseDatapackDirectory("data/residences/castles", true);
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node listNode = doc.getFirstChild(); listNode != null; listNode = listNode.getNextSibling())
		{
			if ("list".equals(listNode.getNodeName()))
			{
				for (Node castleNode = listNode.getFirstChild(); castleNode != null; castleNode = castleNode.getNextSibling())
				{
					if ("castle".equals(castleNode.getNodeName()))
					{
						int castleId = parseInteger(castleNode.getAttributes(), "id");
						for (Node tpNode = castleNode.getFirstChild(); tpNode != null; tpNode = tpNode.getNextSibling())
						{
							List<CastleSpawnHolder> spawns = new();
							if ("spawns".equals(tpNode.getNodeName()))
							{
								for (Node npcNode = tpNode.getFirstChild(); npcNode != null; npcNode = npcNode.getNextSibling())
								{
									if ("npc".equals(npcNode.getNodeName()))
									{
										NamedNodeMap np = npcNode.getAttributes();
										int npcId = parseInteger(np, "id");
										CastleSide side = parseEnum(np, CastleSide.class, "castleSide", CastleSide.NEUTRAL);
										int x = parseInteger(np, "x");
										int y = parseInteger(np, "y");
										int z = parseInteger(np, "z");
										int heading = parseInteger(np, "heading");
										spawns.add(new CastleSpawnHolder(npcId, side, x, y, z, heading));
									}
								}
								_spawns.put(castleId, spawns);
							}
							else if ("siegeGuards".equals(tpNode.getNodeName()))
							{
								List<SiegeGuardHolder> guards = new();
								for (Node npcNode = tpNode.getFirstChild(); npcNode != null; npcNode = npcNode.getNextSibling())
								{
									if ("guard".equals(npcNode.getNodeName()))
									{
										NamedNodeMap np = npcNode.getAttributes();
										int itemId = parseInteger(np, "itemId");
										SiegeGuardType type = parseEnum(tpNode.getAttributes(), SiegeGuardType.class, "type");
										bool stationary = parseBoolean(np, "stationary", false);
										int npcId = parseInteger(np, "npcId");
										int npcMaxAmount = parseInteger(np, "npcMaxAmount");
										guards.add(new SiegeGuardHolder(castleId, itemId, type, stationary, npcId, npcMaxAmount));
									}
								}
								_siegeGuards.put(castleId, guards);
							}
						}
					}
				}
			}
		}
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