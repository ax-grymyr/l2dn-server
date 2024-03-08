using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author St3eT
 */
public class ClanHallData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanHallData));
	private static readonly Map<int, ClanHall> _clanHalls = new();
	
	protected ClanHallData()
	{
		load();
	}
	
	public void load()
	{
		LoadXmlDocuments(DataFileLocation.Data, "residences/clanHalls", true).ForEach(t =>
		{
			t.Document.Elements("list").Elements("clanHall").ForEach(x => loadElement(t.FilePath, x));
		});
        
		LOGGER.Info(GetType().Name + ": Succesfully loaded " + _clanHalls.size() + " clan halls.");
	}
	
	private void loadElement(string filePath, XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		string name = element.Attribute("name").GetString("None");
		ClanHallGrade grade = element.Attribute("grade").GetEnum(ClanHallGrade.GRADE_NONE);
		ClanHallType type = element.Attribute("type").GetEnum(ClanHallType.OTHER);

		XElement auctionEl = element.Elements("auction").Single();
		int minBid = auctionEl.GetAttributeValueAsInt32("minBid");
		int lease = auctionEl.GetAttributeValueAsInt32("lease");
		int deposit = auctionEl.GetAttributeValueAsInt32("deposit");

		XElement rpEl = element.Elements("ownerRestartPoint").Single();
		Location ownerRestartPoint = new Location(rpEl.GetAttributeValueAsInt32("x"), rpEl.GetAttributeValueAsInt32("z"),
			rpEl.GetAttributeValueAsInt32("z"));
		
		XElement bpEl = element.Elements("banishPoint").Single();
		Location banishPoint = new Location(bpEl.GetAttributeValueAsInt32("x"), bpEl.GetAttributeValueAsInt32("z"),
			bpEl.GetAttributeValueAsInt32("z"));

		ClanHall clanHall = new ClanHall(id, grade, type, minBid, lease, deposit, ownerRestartPoint, banishPoint);
		clanHall.setName(name);
		
		element.Elements("npcs").Elements("npc").ForEach(el =>
		{
			int npcId = el.GetAttributeValueAsInt32("id");
			clanHall.getNpcs().add(npcId);
		});
		
		element.Elements("doorlist").Elements("door").ForEach(el =>
		{
			int doorId = el.GetAttributeValueAsInt32("id");
			Door door = DoorData.getInstance().getDoor(doorId);
			if (door != null)
			{
				clanHall.getDoors().add(door);
			}
		});
		
		element.Elements("teleportList").Elements("teleport").ForEach(el =>
		{
			NpcStringId npcStringId = (NpcStringId)el.GetAttributeValueAsInt32("npcStringId");
			int x = el.GetAttributeValueAsInt32("x");
			int y = el.GetAttributeValueAsInt32("y");
			int z = el.GetAttributeValueAsInt32("z");
			int minFunctionLevel = el.GetAttributeValueAsInt32("minFunctionLevel");
			int cost = el.GetAttributeValueAsInt32("cost");
			clanHall.getTeleportList().add(new ClanHallTeleportHolder(npcStringId, x, y, z, minFunctionLevel, cost));
		});

		_clanHalls.put(id, clanHall);
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

		freeAuctionableHalls.Sort((x, y) => x.getResidenceId().CompareTo(y.getResidenceId()));
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
