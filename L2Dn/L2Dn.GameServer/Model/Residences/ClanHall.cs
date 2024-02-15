using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author St3eT
 */
public class ClanHall: AbstractResidence
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanHall));
	
	// Static parameters
	private readonly ClanHallType _type;
	private readonly int _minBid;
	private readonly int _lease;
	private readonly int _deposit;
	private readonly Set<int> _npcs = new();
	private readonly Set<Door> _doors = new();
	private readonly Set<ClanHallTeleportHolder> _teleports = new();
	private readonly Location _ownerLocation;
	private readonly Location _banishLocation;
	// Dynamic parameters
	Clan _owner = null;
	long _paidUntil = 0;
	protected ScheduledFuture _checkPaymentTask = null;
	// Other
	private const String INSERT_CLANHALL = "INSERT INTO clanhall (id, ownerId, paidUntil) VALUES (?,?,?)";
	private const String LOAD_CLANHALL = "SELECT * FROM clanhall WHERE id=?";
	private const String UPDATE_CLANHALL = "UPDATE clanhall SET ownerId=?,paidUntil=? WHERE id=?";

	public ClanHall(int id, ClanHallGrade grade, ClanHallType type, int minBid, int lease, int deposit,
		Location ownerLocation, Location banishLocation)
		: base(id)
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

	public ClanHall(StatSet @params): base(@params.getInt("id"))
	{
		// Set static parameters
		setName(@params.getString("name"));
		_grade = @params.getEnum<ClanHallGrade>("grade");
		_type = @params.getEnum<ClanHallType>("type");
		_minBid = @params.getInt("minBid");
		_lease = @params.getInt("lease");
		_deposit = @params.getInt("deposit");
		List<int> npcs = @params.getList<int>("npcList");
		if (npcs != null)
		{
			_npcs.addAll(npcs);
		}
		List<Door> doors = @params.getList<Door>("doorList");
		if (doors != null)
		{
			_doors.addAll(doors);
		}
		List<ClanHallTeleportHolder> teleports = @params.getList<ClanHallTeleportHolder>("teleportList");
		if (teleports != null)
		{
			_teleports.addAll(teleports);
		}
		_ownerLocation = @params.getLocation("owner_loc");
		_banishLocation = @params.getLocation("banish_loc");
		// Set dynamic parameters (from DB)
		load();
		// Init Clan Hall zone and Functions
		initResidenceZone();
		initFunctions();
	}
	
	protected override void load()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement loadStatement = con.prepareStatement(LOAD_CLANHALL);
			PreparedStatement insertStatement = con.prepareStatement(INSERT_CLANHALL);
			loadStatement.setInt(1, getResidenceId());

			{
				ResultSet rset = loadStatement.executeQuery();
				if (rset.next())
				{
					setPaidUntil(rset.getLong("paidUntil"));
					setOwner(rset.getInt("ownerId"));
				}
				else
				{
					insertStatement.setInt(1, getResidenceId());
					insertStatement.setInt(2, 0); // New clanhall should not have owner
					insertStatement.setInt(3, 0); // New clanhall should not have paid until
					if (insertStatement.execute())
					{
						LOGGER.Info("Clan Hall " + getName() + " (" + getResidenceId() + ") was sucessfully created.");
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Info("Failed loading clan hall: " + e);
		}
	}
	
	public void updateDB()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(UPDATE_CLANHALL);
			statement.setInt(1, getOwnerId());
			statement.setLong(2, _paidUntil);
			statement.setInt(3, getResidenceId());
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}
	
	protected void initResidenceZone()
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
		Duration failDay = Duration.between(Instant.ofEpochMilli(_paidUntil), Instant.now());
		return failDay.isNegative() ? 0 : (int) failDay.toDays();
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
		_doors.forEach(door => door.Key.openCloseMe(open));
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
	public int getOwnerId()
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
	public void setOwner(Clan clan)
	{
		if (clan != null)
		{
			_owner = clan;
			clan.setHideoutId(getResidenceId());
			clan.broadcastToOnlineMembers(new PledgeShowInfoUpdate(clan));
			if (_paidUntil == 0)
			{
				setPaidUntil(Instant.now().plus(Duration.ofDays(7)).toEpochMilli());
			}
			
			int failDays = getCostFailDay();
			long time = failDays > 0 ? (failDays > 8 ? Instant.now().toEpochMilli() : Instant.ofEpochMilli(_paidUntil).plus(Duration.ofDays(failDays + 1)).toEpochMilli()) : _paidUntil;
			_checkPaymentTask = ThreadPool.schedule(new CheckPaymentTask(), Math.max(0, time - System.currentTimeMillis()));
		}
		else
		{
			if (_owner != null)
			{
				_owner.setHideoutId(0);
				_owner.broadcastToOnlineMembers(new PledgeShowInfoUpdate(_owner));
				removeFunctions();
			}
			_owner = null;
			setPaidUntil(0);
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
	public long getPaidUntil()
	{
		return _paidUntil;
	}
	
	/**
	 * Set the due date of clan hall payment
	 * @param paidUntil the due date of clan hall payment
	 */
	public void setPaidUntil(long paidUntil)
	{
		_paidUntil = paidUntil;
	}
	
	/**
	 * Gets the next date of clan hall payment
	 * @return the next date of clan hall payment
	 */
	public long getNextPayment()
	{
		return (_checkPaymentTask != null) ? System.currentTimeMillis() + _checkPaymentTask.getDelay(TimeUnit.MILLISECONDS) : 0;
	}
	
	public Location getOwnerLocation()
	{
		return _ownerLocation;
	}
	
	public Location getBanishLocation()
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
				result.add(holder);
			}
		}
		return result;
	}
	
	public int getMinBid()
	{
		return _minBid;
	}
	
	public int getLease()
	{
		return _lease;
	}
	
	public int getDeposit()
	{
		return _deposit;
	}
	
	class CheckPaymentTask: Runnable
	{
		public void run()
		{
			if (_owner != null)
			{
				if (_owner.getWarehouse().getAdena() < _lease)
				{
					if (getCostFailDay() > 8)
					{
						_owner.broadcastToOnlineMembers(new SystemMessage(SystemMessageId.THE_CLAN_HALL_FEE_IS_ONE_WEEK_OVERDUE_THEREFORE_THE_CLAN_HALL_OWNERSHIP_HAS_BEEN_REVOKED));
						setOwner(null);
					}
					else
					{
						_checkPaymentTask = ThreadPool.schedule(new CheckPaymentTask(), 24 * 60 * 60 * 1000); // 1 day
						SystemMessage sm = new SystemMessage(SystemMessageId.THE_PAYMENT_FOR_YOUR_CLAN_HALL_HAS_NOT_BEEN_MADE_PLEASE_DEPOSIT_THE_NECESSARY_AMOUNT_OF_ADENA_TO_YOUR_CLAN_WAREHOUSE_BY_S1_TOMORROW);
						sm.addInt(_lease);
						_owner.broadcastToOnlineMembers(sm);
					}
				}
				else
				{
					_owner.getWarehouse().destroyItem("Clan Hall Lease", Inventory.ADENA_ID, _lease, null, null);
					setPaidUntil(Instant.ofEpochMilli(_paidUntil).plus(Duration.ofDays(7)).toEpochMilli());
					_checkPaymentTask = ThreadPool.schedule(new CheckPaymentTask(), _paidUntil - System.currentTimeMillis());
					updateDB();
				}
			}
		}
	}
	
	public override String ToString()
	{
		return GetType().Name + ":" + getName() + "[" + getResidenceId() + "]";
	}
}