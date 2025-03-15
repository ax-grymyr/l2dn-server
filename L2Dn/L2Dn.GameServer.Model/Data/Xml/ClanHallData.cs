using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author St3eT
 */
public class ClanHallData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ClanHallData));
	private static FrozenDictionary<int, ClanHall> _clanHalls = FrozenDictionary<int, ClanHall>.Empty;

	private ClanHallData()
	{
		load();
	}

	public void load()
	{
		Dictionary<int, ClanHall> clanHalls = new Dictionary<int, ClanHall>();
		LoadXmlDocuments<XmlClanHallList>(DataFileLocation.Data, "residences/clanHalls", true)
			.SelectMany(t => t.Document.ClanHalls)
			.Select(LoadClanHall)
			.Where(c => c is not null)
			.ForEach(clanHall =>
			{
				if (!clanHalls.TryAdd(clanHall!.getResidenceId(), clanHall))
					_logger.Error(nameof(ClanHallData) + $": Duplicated clan hall id={clanHall.getResidenceId()}");
			});

		_clanHalls = clanHalls.ToFrozenDictionary();

		_logger.Info(GetType().Name + ": Succesfully loaded " + clanHalls.Count + " clan halls.");
	}

	private static ClanHall? LoadClanHall(XmlClanHall xmlClanHall)
	{
		string name = string.IsNullOrEmpty(xmlClanHall.Name) ? "None" : xmlClanHall.Name;
		ClanHallGrade grade = xmlClanHall.GradeSpecified ? xmlClanHall.Grade : ClanHallGrade.GRADE_NONE;
		ClanHallType type = xmlClanHall.TypeSpecified ? xmlClanHall.Type : ClanHallType.OTHER;

		XmlClanHallAuction? auction = xmlClanHall.Auction;
		XmlLocation? ownerRestartPoint = xmlClanHall.OwnerRestartPoint;
		XmlLocation? banishPoint = xmlClanHall.BanishPoint;

		if (auction is null)
		{
			_logger.Error(nameof(ClanHallData) + $": auction is null for clan hall id={xmlClanHall.Id}");
			return null;
		}

		if (ownerRestartPoint is null)
		{
			_logger.Error(nameof(ClanHallData) + $": ownerRestartPoint is null for clan hall id={xmlClanHall.Id}");
			return null;
		}

		if (banishPoint is null)
		{
			_logger.Error(nameof(ClanHallData) + $": banishPoint is null for clan hall id={xmlClanHall.Id}");
			return null;
		}

		Location3D ownerRestartPointLoc = new(ownerRestartPoint.X, ownerRestartPoint.Y, ownerRestartPoint.Z);
		Location3D banishPointLoc = new(banishPoint.X, banishPoint.Y, banishPoint.Z);

		ClanHall clanHall = new(xmlClanHall.Id, name, grade, type, auction.MinBid, auction.Lease, auction.Deposit,
			ownerRestartPointLoc, banishPointLoc);

		foreach (XmlClanHallNpc clanHallNpc in xmlClanHall.NpcList)
			clanHall.getNpcs().add(clanHallNpc.Id);

		foreach (XmlClanHallDoor clanHallDoor in xmlClanHall.DoorList)
		{
			Door? door = DoorData.getInstance().getDoor(clanHallDoor.Id);
			if (door is null)
				_logger.Warn(nameof(ClanHallData) + $": Door id={clanHallDoor.Id} not exists");
			else
				clanHall.getDoors().add(door);
		}

		foreach (XmlClanHallTeleport teleport in xmlClanHall.TeleportList)
		{
			clanHall.getTeleportList().add(new ClanHallTeleportHolder((NpcStringId)teleport.NpcStringId, new Location3D(
				teleport.X, teleport.Y, teleport.Z), teleport.MinFunctionLevel, teleport.Cost));
		}

		return clanHall;
	}

	public ClanHall? getClanHallById(int clanHallId)
	{
		return _clanHalls.GetValueOrDefault(clanHallId);
	}

	public ImmutableArray<ClanHall> getClanHalls()
	{
		return _clanHalls.Values;
	}

	public ClanHall? getClanHallByNpcId(int npcId)
	{
		foreach (ClanHall ch in _clanHalls.Values)
		{
			if (ch.getNpcs().Contains(npcId))
			{
				return ch;
			}
		}
		return null;
	}

	public ClanHall? getClanHallByClan(Clan clan)
	{
		foreach (ClanHall ch in _clanHalls.Values)
		{
			if (ch.getOwner() == clan)
			{
				return ch;
			}
		}

		return null;
	}

	public ClanHall? getClanHallByDoorId(int doorId)
	{
		Door? door = DoorData.getInstance().getDoor(doorId);
        if (door is null)
            return null;

		foreach (ClanHall ch in _clanHalls.Values)
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
		foreach (ClanHall ch in _clanHalls.Values)
		{
			if (ch.getType() == ClanHallType.AUCTIONABLE && ch.getOwner() == null)
			{
				freeAuctionableHalls.Add(ch);
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