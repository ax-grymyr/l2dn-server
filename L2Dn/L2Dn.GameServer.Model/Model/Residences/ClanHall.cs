using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author St3eT
 */
public class ClanHall: AbstractResidence
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanHall));

	// Static parameters
	private readonly ClanHallType _type;
	private readonly long _minBid;
	private readonly long _lease;
	private readonly long _deposit;
	private readonly Set<int> _npcs = new();
	private readonly Set<Door> _doors = new();
	private readonly Set<ClanHallTeleportHolder> _teleports = new();
	private readonly Location3D _ownerLocation;
	private readonly Location3D _banishLocation;

	// Dynamic parameters
	private Clan? _owner;
	private DateTime _paidUntil;
	protected ScheduledFuture? _checkPaymentTask;

	public ClanHall(int id, string name, ClanHallGrade grade, ClanHallType type, long minBid, long lease, long deposit,
		Location3D ownerLocation, Location3D banishLocation)
		: base(id, name)
	{
		_grade = grade;
		_type = type;
		_minBid = minBid;
		_lease = lease;
		_deposit = deposit;
		_ownerLocation = ownerLocation;
		_banishLocation = banishLocation;

		load();

		// Init Clan Hall zone and Functions
		initResidenceZone();
		initFunctions();
	}

	protected override void load()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int residenceId = getResidenceId();
			var record = ctx.ClanHalls.SingleOrDefault(r => r.Id == residenceId);
			if (record is not null)
			{
				setPaidUntil(record.PaidUntil);
				setOwner(record.OwnerId);
			}
			else
			{
				record = new DbClanHall();
				record.Id = residenceId;
				record.OwnerId = 0;
				record.PaidUntil = DateTime.MinValue;
				ctx.ClanHalls.Add(record);
				ctx.SaveChanges();

				LOGGER.Info("Clan Hall " + getName() + " (" + getResidenceId() + ") was sucessfully created.");
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed loading clan hall: " + e);
		}
	}

	public void updateDB()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int residenceId = getResidenceId();
			ctx.ClanHalls.Where(r => r.Id == residenceId).ExecuteUpdate(s =>
				s.SetProperty(r => r.OwnerId, getOwnerId()).SetProperty(r => r.PaidUntil, _paidUntil));
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
	}

	protected override void initResidenceZone()
	{
		foreach (ClanHallZone zone in ZoneManager.getInstance().getAllZones<ClanHallZone>())
		{
			if (zone.getResidenceId() == getResidenceId())
			{
				setResidenceZone(zone);
				break;
			}
		}
	}

	public int getCostFailDay()
	{
		TimeSpan failDay = _paidUntil - DateTime.UtcNow;
		return failDay < TimeSpan.Zero ? 0 : (int)failDay.TotalDays;
	}

	/**
	 * Teleport all non-owner players from {@link ClanHallZone} to {@link ClanHall#getBanishLocation()}.
	 */
	public void banishOthers()
	{
		getResidenceZone().banishForeigners(getOwnerId());
	}

	/**
	 * Open or close all {@link Door} related to this {@link ClanHall}.
	 * @param open {@code true} means open door, {@code false} means close door
	 */
	public void openCloseDoors(bool open)
	{
		_doors.ForEach(door => door.openCloseMe(open));
	}

	/**
	 * Gets all {@link Door} related to this {@link ClanHall}.
	 * @return all {@link Door} related to this {@link ClanHall}
	 */
	public Set<Door> getDoors()
	{
		return _doors;
	}

	/**
	 * Gets all {@link Npc} related to this {@link ClanHall}.
	 * @return all {@link Npc} related to this {@link ClanHall}
	 */
	public Set<int> getNpcs()
	{
		return _npcs;
	}

	/**
	 * Gets the {@link ClanHallType} of this {@link ClanHall}.
	 * @return {@link ClanHallType} of this {@link ClanHall} in {@link ClanHallGrade} enum.
	 */
	public ClanHallType getType()
	{
		return _type;
	}

	/**
	 * Gets the {@link Clan} which own this {@link ClanHall}.
	 * @return {@link Clan} which own this {@link ClanHall}
	 */
	public Clan getOwner()
	{
		return _owner;
	}

	/**
	 * Gets the {@link Clan} ID which own this {@link ClanHall}.
	 * @return the {@link Clan} ID which own this {@link ClanHall}
	 */
	public override int getOwnerId()
	{
		Clan owner = _owner;
		return (owner != null) ? owner.getId() : 0;
	}

	/**
	 * Set the owner of clan hall
	 * @param clanId the Id of the clan
	 */
	public void setOwner(int clanId)
	{
		setOwner(ClanTable.getInstance().getClan(clanId));
	}

	/**
	 * Set the clan as owner of clan hall
	 * @param clan the Clan object
	 */
	public void setOwner(Clan? clan)
	{
		if (clan != null)
		{
			_owner = clan;
			clan.setHideoutId(getResidenceId());
			clan.broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(clan));
			if (_paidUntil == DateTime.MinValue)
			{
				setPaidUntil(DateTime.UtcNow.AddDays(7));
			}

			int failDays = getCostFailDay();
			DateTime time = failDays > 0 ? (failDays > 8 ? DateTime.UtcNow : _paidUntil.AddDays(failDays + 1)) : _paidUntil;

			_checkPaymentTask = ThreadPool.schedule(new CheckPaymentTask(this), Algorithms.Max(TimeSpan.Zero, time - DateTime.UtcNow));
		}
		else
		{
			if (_owner != null)
			{
				_owner.setHideoutId(0);
				_owner.broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(_owner));
				removeFunctions();
			}
			_owner = null;
			setPaidUntil(DateTime.MinValue);
			if (_checkPaymentTask != null)
			{
				_checkPaymentTask.cancel(true);
				_checkPaymentTask = null;
			}
		}
		updateDB();
	}

	/**
	 * Gets the due date of clan hall payment
	 * @return the due date of clan hall payment
	 */
	public DateTime getPaidUntil()
	{
		return _paidUntil;
	}

	/**
	 * Set the due date of clan hall payment
	 * @param paidUntil the due date of clan hall payment
	 */
	public void setPaidUntil(DateTime paidUntil)
	{
		_paidUntil = paidUntil;
	}

	/**
	 * Gets the next date of clan hall payment
	 * @return the next date of clan hall payment
	 */
	public DateTime getNextPayment()
	{
		return (_checkPaymentTask != null) ? DateTime.UtcNow + _checkPaymentTask.getDelay() : DateTime.MinValue;
	}

	public Location3D getOwnerLocation()
	{
		return _ownerLocation;
	}

	public Location3D getBanishLocation()
	{
		return _banishLocation;
	}

	public Set<ClanHallTeleportHolder> getTeleportList()
	{
		return _teleports;
	}

	public List<ClanHallTeleportHolder> getTeleportList(int functionLevel)
	{
		List<ClanHallTeleportHolder> result = new();
		foreach (ClanHallTeleportHolder holder in _teleports)
		{
			if (holder.getMinFunctionLevel() <= functionLevel)
			{
				result.Add(holder);
			}
		}
		return result;
	}

	public long getMinBid()
	{
		return _minBid;
	}

	public long getLease()
	{
		return _lease;
	}

	public long getDeposit()
	{
		return _deposit;
	}

	private class CheckPaymentTask: Runnable
	{
		private readonly ClanHall _clanHall;

		public CheckPaymentTask(ClanHall clanHall)
		{
			_clanHall = clanHall;
		}

		public void run()
		{
			if (_clanHall._owner != null)
			{
				if (_clanHall._owner.getWarehouse().getAdena() < _clanHall._lease)
				{
					if (_clanHall.getCostFailDay() > 8)
					{
						_clanHall._owner.broadcastToOnlineMembers(new SystemMessagePacket(SystemMessageId.THE_CLAN_HALL_FEE_IS_ONE_WEEK_OVERDUE_THEREFORE_THE_CLAN_HALL_OWNERSHIP_HAS_BEEN_REVOKED));
						_clanHall.setOwner(null);
					}
					else
					{
						_clanHall._checkPaymentTask = ThreadPool.schedule(new CheckPaymentTask(_clanHall), TimeSpan.FromDays(1));
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_PAYMENT_FOR_YOUR_CLAN_HALL_HAS_NOT_BEEN_MADE_PLEASE_DEPOSIT_THE_NECESSARY_AMOUNT_OF_ADENA_TO_YOUR_CLAN_WAREHOUSE_BY_S1_TOMORROW);
						sm.Params.addLong(_clanHall._lease);
						_clanHall._owner.broadcastToOnlineMembers(sm);
					}
				}
				else
				{
					_clanHall._owner.getWarehouse().destroyItem("Clan Hall Lease", Inventory.ADENA_ID, _clanHall._lease, null, null);
					_clanHall.setPaidUntil(_clanHall._paidUntil.AddDays(7));
					_clanHall._checkPaymentTask = ThreadPool.schedule(new CheckPaymentTask(_clanHall), _clanHall._paidUntil - DateTime.UtcNow);
					_clanHall.updateDB();
				}
			}
		}
	}

	public override string ToString()
	{
		return GetType().Name + ":" + getName() + "[" + getResidenceId() + "]";
	}
}