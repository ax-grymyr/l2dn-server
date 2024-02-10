using System.Reflection.Metadata;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author St3eT
 */
public class ClanHallData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanHallData));
	private static readonly Map<int, ClanHall> _clanHalls = new();
	
	protected ClanHallData()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackDirectory("data/residences/clanHalls", true);
		LOGGER.Info(GetType().Name + ": Succesfully loaded " + _clanHalls.size() + " clan halls.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		List<Door> doors = new();
		List<int> npcs = new();
		List<ClanHallTeleportHolder> teleports = new();
		StatSet @params = new StatSet();
		for (Node listNode = doc.getFirstChild(); listNode != null; listNode = listNode.getNextSibling())
		{
			if ("list".equals(listNode.getNodeName()))
			{
				for (Node clanHallNode = listNode.getFirstChild(); clanHallNode != null; clanHallNode = clanHallNode.getNextSibling())
				{
					if ("clanHall".equals(clanHallNode.getNodeName()))
					{
						@params.set("id", parseInteger(clanHallNode.getAttributes(), "id"));
						@params.set("name", parseString(clanHallNode.getAttributes(), "name", "None"));
						@params.set("grade", parseEnum(clanHallNode.getAttributes(), ClanHallGrade.class, "grade", ClanHallGrade.GRADE_NONE));
						@params.set("type", parseEnum(clanHallNode.getAttributes(), ClanHallType.class, "type", ClanHallType.OTHER));
						for (Node tpNode = clanHallNode.getFirstChild(); tpNode != null; tpNode = tpNode.getNextSibling())
						{
							switch (tpNode.getNodeName())
							{
								case "auction":
								{
									NamedNodeMap at = tpNode.getAttributes();
									@params.set("minBid", parseInteger(at, "minBid"));
									@params.set("lease", parseInteger(at, "lease"));
									@params.set("deposit", parseInteger(at, "deposit"));
									break;
								}
								case "npcs":
								{
									for (Node npcNode = tpNode.getFirstChild(); npcNode != null; npcNode = npcNode.getNextSibling())
									{
										if ("npc".equals(npcNode.getNodeName()))
										{
											NamedNodeMap np = npcNode.getAttributes();
											int npcId = parseInteger(np, "id");
											npcs.add(npcId);
										}
									}
									@params.set("npcList", npcs);
									break;
								}
								case "doorlist":
								{
									for (Node npcNode = tpNode.getFirstChild(); npcNode != null; npcNode = npcNode.getNextSibling())
									{
										if ("door".equals(npcNode.getNodeName()))
										{
											NamedNodeMap np = npcNode.getAttributes();
											int doorId = parseInteger(np, "id");
											Door door = DoorData.getInstance().getDoor(doorId);
											if (door != null)
											{
												doors.add(door);
											}
										}
									}
									@params.set("doorList", doors);
									break;
								}
								case "teleportList":
								{
									for (Node npcNode = tpNode.getFirstChild(); npcNode != null; npcNode = npcNode.getNextSibling())
									{
										if ("teleport".equals(npcNode.getNodeName()))
										{
											NamedNodeMap np = npcNode.getAttributes();
											int npcStringId = parseInteger(np, "npcStringId");
											int x = parseInteger(np, "x");
											int y = parseInteger(np, "y");
											int z = parseInteger(np, "z");
											int minFunctionLevel = parseInteger(np, "minFunctionLevel");
											int cost = parseInteger(np, "cost");
											teleports.add(new ClanHallTeleportHolder(npcStringId, x, y, z, minFunctionLevel, cost));
										}
									}
									@params.set("teleportList", teleports);
									break;
								}
								case "ownerRestartPoint":
								{
									NamedNodeMap ol = tpNode.getAttributes();
									@params.set("owner_loc", new Location(parseInteger(ol, "x"), parseInteger(ol, "y"), parseInteger(ol, "z")));
									break;
								}
								case "banishPoint":
								{
									NamedNodeMap bl = tpNode.getAttributes();
									@params.set("banish_loc", new Location(parseInteger(bl, "x"), parseInteger(bl, "y"), parseInteger(bl, "z")));
									break;
								}
							}
						}
					}
				}
			}
		}
		_clanHalls.put(@params.getInt("id"), new ClanHall(@params));
	}
	
	public ClanHall getClanHallById(int clanHallId)
	{
		return _clanHalls.get(clanHallId);
	}
	
	public ICollection<ClanHall> getClanHalls()
	{
		return _clanHalls.values();
	}
	
	public ClanHall getClanHallByNpcId(int npcId)
	{
		foreach (ClanHall ch in _clanHalls.values())
		{
			if (ch.getNpcs().Contains(npcId))
			{
				return ch;
			}
		}
		return null;
	}
	
	public ClanHall getClanHallByClan(Clan clan)
	{
		foreach (ClanHall ch in _clanHalls.values())
		{
			if (ch.getOwner() == clan)
			{
				return ch;
			}
		}
		return null;
	}
	
	public ClanHall getClanHallByDoorId(int doorId)
	{
		Door door = DoorData.getInstance().getDoor(doorId);
		foreach (ClanHall ch in _clanHalls.values())
		{
			if (ch.getDoors().Contains(door))
			{
				return ch;
			}
		}
		return null;
	}
	
	public List<ClanHall> getFreeAuctionableHall()
	{
		List<ClanHall> freeAuctionableHalls = new();
		foreach (ClanHall ch in _clanHalls.values())
		{
			if ((ch.getType() == ClanHallType.AUCTIONABLE) && (ch.getOwner() == null))
			{
				freeAuctionableHalls.add(ch);
			}
		}
		Collections.sort(freeAuctionableHalls, Comparator.comparingInt(ClanHall::getResidenceId));
		return freeAuctionableHalls;
	}
	
	/**
	 * Gets the single instance of ClanHallData.
	 * @return single instance of ClanHallData
	 */
	public static ClanHallData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ClanHallData INSTANCE = new();
	}
}
