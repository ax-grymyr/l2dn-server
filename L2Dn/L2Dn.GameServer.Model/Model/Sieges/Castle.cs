using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Events;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using FortManager = L2Dn.GameServer.InstanceManagers.FortManager;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Sieges;

public class Castle: AbstractResidence, IEventContainerProvider
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Castle));

	private readonly EventContainer _eventContainer;
	private readonly List<Door> _doors = new();
	private readonly List<Npc> _sideNpcs = new();
	int _ownerId;
	private Siege? _siege;
	private DateTime _siegeDate;
	private bool _isTimeRegistrationOver = true; // true if Castle Lords set the time, or 24h is elapsed after the siege
	private DateTime? _siegeTimeRegistrationEndDate; // last siege end date + 1 day
	private CastleSide _castleSide;
	private long _treasury;
	private bool _showNpcCrest;
	private readonly SiegeZone _zone;
	private readonly ResidenceTeleportZone _teleZone;
	private Clan? _formerOwner;
	private readonly Set<Artefact> _artefacts = new();
	private readonly Map<int, CastleFunction> _function = new();
	private int _ticketBuyCount;
	private bool _isFirstMidVictory;

	/** Castle Functions */
	public const int FUNC_TELEPORT = 1;
	public const int FUNC_RESTORE_HP = 2;
	public const int FUNC_RESTORE_MP = 3;
	public const int FUNC_RESTORE_EXP = 4;
	public const int FUNC_SUPPORT = 5;

	public EventContainer Events => _eventContainer;

	public class CastleFunction
	{
		private readonly Castle _castle;
		private readonly int _type;
		private int _lvl;
		protected int _fee;
		protected int _tempFee;
		private readonly TimeSpan _rate;
		private DateTime? _endDate;
		protected bool _inDebt;
		public bool _cwh;

		public CastleFunction(Castle castle, int type, int lvl, int lease, int tempLease, TimeSpan rate, DateTime? time, bool cwh)
		{
			_castle = castle;
			_type = type;
			_lvl = lvl;
			_fee = lease;
			_tempFee = tempLease;
			_rate = rate;
			_endDate = time;
			initializeTask(cwh);
		}

		public int getType()
		{
			return _type;
		}

		public int getLvl()
		{
			return _lvl;
		}

		public int getLease()
		{
			return _fee;
		}

		public TimeSpan getRate()
		{
			return _rate;
		}

		public DateTime? getEndTime()
		{
			return _endDate;
		}

		public void setLvl(int lvl)
		{
			_lvl = lvl;
		}

		public void setLease(int lease)
		{
			_fee = lease;
		}

		public void setEndTime(DateTime time)
		{
			_endDate = time;
		}

		private void initializeTask(bool cwh)
		{
			if (_castle._ownerId <= 0)
			{
				return;
			}

			DateTime currentTime = DateTime.UtcNow;
			if (_endDate > currentTime)
			{
				ThreadPool.schedule(new FunctionTask(_castle, this, cwh), _endDate.Value - currentTime);
			}
			else
			{
				ThreadPool.schedule(new FunctionTask(_castle, this, cwh), 0);
			}
		}

		private class FunctionTask: Runnable
		{
			private readonly Castle _castle;
			private readonly CastleFunction _castleFunction;

			public FunctionTask(Castle castle, CastleFunction castleFunction, bool cwh)
			{
				_castle = castle;
				_castleFunction = castleFunction;
				_castleFunction._cwh = cwh;
			}

			public void run()
			{
				try
                {
                    Clan? ownerClan = ClanTable.getInstance().getClan(_castle.getOwnerId());
					if (_castle._ownerId <= 0 || ownerClan == null)
						return;

					if (ownerClan.getWarehouse().getAdena() >=
					    _castleFunction._fee || !_castleFunction._cwh)
					{
						int fee = _castleFunction._fee;
						if (_castleFunction._endDate is null)
						{
							fee = _castleFunction._tempFee;
						}

						_castleFunction.setEndTime(DateTime.UtcNow + _castleFunction._rate);
						_castleFunction.dbSave();
						if (_castleFunction._cwh)
						{
                            ownerClan.getWarehouse()
								.destroyItemByItemId("CS_function_fee", Inventory.ADENA_ID, fee, null, null);
						}

						ThreadPool.schedule(new FunctionTask(_castle, _castleFunction, true), _castleFunction._rate);
					}
					else
					{
						_castle.removeFunction(_castleFunction._type);
					}
				}
				catch (Exception e)
				{
					LOGGER.Error(e);
				}
			}
		}

		public void dbSave()
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int castleId = _castle.getResidenceId();
				var record = ctx.CastleFunctions.SingleOrDefault(r => r.CastleId == castleId && r.Type == _type);
				if (record is null)
				{
					record = new DbCastleFunction();
					record.CastleId = (byte)castleId;
					record.Type = (byte)_type;
					ctx.CastleFunctions.Add(record);
				}

				record.Level = (short)_lvl;
				record.Lease = _fee;
				record.Rate = _rate;
				record.EndTime = _endDate;
				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error("Exception: Castle.updateFunctions(int type, int lvl, int lease, long rate, long time, bool addNew): " + e);
			}
		}
	}

    public Castle(int castleId, string castleName): base(castleId, castleName, FindResidenceZone<CastleZone>(castleId))
    {
        _eventContainer = new EventContainer($"Castle template {castleId}", GlobalEvents.Global);
        _zone = FindSiegeZone(castleId);
        _teleZone = FindResidenceTeleportZone(castleId);

        load();
        // initFunctions();
        spawnSideNpcs();
        if (_ownerId != 0)
        {
            loadFunctions();
            loadDoorUpgrade();
        }
    }

    /**
     * Return function with id
     * @param type
     * @return
     */
	public CastleFunction? getCastleFunction(int type)
	{
		return _function.GetValueOrDefault(type);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void engrave(Clan clan, WorldObject target, CastleSide side)
	{
		if (!_artefacts.Contains(target))
		{
			return;
		}
		setSide(side);
		setOwner(clan);
		SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.CLAN_S1_HAS_SUCCEEDED_IN_S2);
		msg.Params.addString(clan.getName());
		msg.Params.addString(getName());
		getSiege().announceToPlayer(msg, true);
	}

	// This method add to the treasury
	/**
	 * Add amount to castle instance's treasury (warehouse).
	 * @param amountValue
	 */
	public void addToTreasury(long amountValue)
	{
		// check if owned
		if (_ownerId <= 0)
		{
			return;
		}

		long amount = amountValue;
		switch (getName().ToLower())
		{
			case "schuttgart":
			case "goddard":
			{
				Castle? rune = CastleManager.getInstance().getCastle("rune");
				if (rune != null)
				{
					long runeTax = (long) (amount * rune.getTaxRate(TaxType.BUY));
					if (rune.getOwnerId() > 0)
					{
						rune.addToTreasury(runeTax);
					}
					amount -= runeTax;
				}
				break;
			}
			case "dion":
			case "giran":
			case "gludio":
			case "innadril":
			case "oren":
			{
				Castle? aden = CastleManager.getInstance().getCastle("aden");
				if (aden != null)
				{
					long adenTax = (long) (amount * aden.getTaxRate(TaxType.BUY)); // Find out what Aden gets from the current castle instance's income
					if (aden.getOwnerId() > 0)
					{
						aden.addToTreasury(adenTax); // Only bother to really add the tax to the treasury if not npc owned
					}
					amount -= adenTax; // Subtract Aden's income from current castle instance's income
				}
				break;
			}
		}

		addToTreasuryNoTax(amount);
	}

	/**
	 * Add amount to castle instance's treasury (warehouse), no tax paying.
	 * @param amountValue
	 * @return
	 */
	public bool addToTreasuryNoTax(long amountValue)
	{
		if (_ownerId <= 0)
		{
			return false;
		}

		long amount = amountValue;
		if (amount < 0)
		{
			amount *= -1;
			if (_treasury < amount)
			{
				return false;
			}
			_treasury -= amount;
		}
		else if (_treasury + amount > Inventory.MAX_ADENA)
		{
			_treasury = Inventory.MAX_ADENA;
		}
		else
		{
			_treasury += amount;
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			ctx.Castles.Where(r => r.Id == castleId).ExecuteUpdate(s => s.SetProperty(r => r.Treasury, _treasury));
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}

		return true;
	}

	/**
	 * Move non clan members off castle area and to nearest town.
	 */
	public void banishForeigners()
	{
		getResidenceZone().banishForeigners(_ownerId);
	}

	/**
	 * Return true if object is inside the zone
	 * @param x
	 * @param y
	 * @param z
	 * @return
	 */
	public bool checkIfInZone(Location3D location)
	{
		SiegeZone? zone = getZone();
		return zone != null && zone.isInsideZone(location);
	}

	public SiegeZone getZone() => _zone;

    public override CastleZone getResidenceZone()
	{
		return (CastleZone)base.getResidenceZone();
	}

	public ResidenceTeleportZone getTeleZone()
	{
		return _teleZone;
	}

	public void oustAllPlayers()
	{
		getTeleZone()?.oustAllPlayers();
	}

	/**
	 * Get the objects distance to this castle
	 * @param obj
	 * @return
	 */
	public double getDistance(WorldObject obj)
	{
		return getZone()?.getDistanceToZone(obj) ?? double.MaxValue; // TODO: verify
	}

	public void closeDoor(Player player, int doorId)
	{
		openCloseDoor(player, doorId, false);
	}

	public void openDoor(Player player, int doorId)
	{
		openCloseDoor(player, doorId, true);
	}

	public void openCloseDoor(Player player, int doorId, bool open)
	{
		if (player.getClanId() != _ownerId && !player.canOverrideCond(PlayerCondOverride.CASTLE_CONDITIONS))
		{
			return;
		}

		Door? door = getDoor(doorId);
		if (door != null)
		{
			if (open)
			{
				door.openMe();
			}
			else
			{
				door.closeMe();
			}
		}
	}

	public void openCloseDoor(Player player, string doorName, bool open)
	{
		if (player.getClanId() != _ownerId && !player.canOverrideCond(PlayerCondOverride.CASTLE_CONDITIONS))
		{
			return;
		}

		Door? door = getDoor(doorName);
		if (door != null)
		{
			if (open)
			{
				door.openMe();
			}
			else
			{
				door.closeMe();
			}
		}
	}

	// This method is used to begin removing all castle upgrades
	public void removeUpgrade()
	{
		removeDoorUpgrade();
		removeTrapUpgrade();
		foreach (int fc in _function.Keys)
		{
			removeFunction(fc);
		}
		_function.Clear();
	}

	// This method updates the castle tax rate
	public void setOwner(Clan? clan)
	{
		// Remove old owner
		if (_ownerId > 0 && (clan == null || clan.getId() != _ownerId))
		{
			Clan? oldOwner = ClanTable.getInstance().getClan(getOwnerId()); // Try to find clan instance
			if (oldOwner != null)
			{
				if (_formerOwner == null)
				{
					_formerOwner = oldOwner;
					if (Config.REMOVE_CASTLE_CIRCLETS)
					{
						CastleManager.getInstance().removeCirclet(_formerOwner, getResidenceId());
					}
				}
				try
				{
					Player? oldleader = oldOwner.getLeader().getPlayer();
					if (oldleader != null && oldleader.getMountType() == MountType.WYVERN)
					{
						oldleader.dismount();
					}
				}
				catch (Exception e)
				{
					LOGGER.Warn("Exception in setOwner: " + e);
				}
				oldOwner.setCastleId(0); // Unset has castle flag for old owner
				foreach (Player member in oldOwner.getOnlineMembers(0))
				{
					removeResidentialSkills(member);
					member.sendSkillList();
					member.broadcastUserInfo();
				}
			}
		}

		updateOwnerInDB(clan); // Update in database
		setShowNpcCrest(false);

		// if clan have fortress, remove it
		if (clan != null && clan.getFortId() > 0)
		{
			FortManager.getInstance().getFortByOwner(clan)?.removeOwner(true);
		}

		if (getSiege().isInProgress())
		{
			getSiege().midVictory(); // Mid victory phase of siege
		}

		if (clan != null)
		{
			foreach (Player member in clan.getOnlineMembers(0))
			{
				giveResidentialSkills(member);
				member.sendSkillList();
			}
		}
	}

	public void removeOwner(Clan clan)
	{
		if (clan != null)
		{
			_formerOwner = clan;
			if (Config.REMOVE_CASTLE_CIRCLETS)
			{
				CastleManager.getInstance().removeCirclet(_formerOwner, getResidenceId());
			}
			foreach (Player member in clan.getOnlineMembers(0))
			{
				removeResidentialSkills(member);
				member.sendSkillList();
			}
			clan.setCastleId(0);
			clan.broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(clan));
		}

		setSide(CastleSide.NEUTRAL);
		updateOwnerInDB(null);
		if (getSiege().isInProgress())
		{
			getSiege().midVictory();
		}

		foreach (int fc in _function.Keys)
		{
			removeFunction(fc);
		}
		_function.Clear();
	}

	/**
	 * Respawn all doors on castle grounds.
	 */
	public void spawnDoor()
	{
		spawnDoor(false);
	}

	/**
	 * Respawn all doors on castle grounds
	 * @param isDoorWeak
	 */
	public void spawnDoor(bool isDoorWeak)
	{
		foreach (Door door in _doors)
		{
			if (door.isDead())
			{
				door.doRevive();
				door.setCurrentHp(isDoorWeak ? door.getMaxHp() / 2 : door.getMaxHp());
			}

			if (door.isOpen())
			{
				door.closeMe();
			}
		}
	}

	// This method loads castle
	protected override void load()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();

			var record = ctx.Castles.SingleOrDefault(r => r.Id == castleId);
			if (record != null)
			{
				_siegeDate = record.SiegeTime;
				_siegeTimeRegistrationEndDate = record.RegistrationEndTime;
				_isTimeRegistrationOver = record.RegistrationTimeOver;
				_castleSide = record.Side;
				_treasury = record.Treasury;
				_showNpcCrest = record.ShowNpcCrest;
				_ticketBuyCount = record.TicketBuyCount;
			}

			var record2 = ctx.Clans.SingleOrDefault(r => r.Castle == castleId);
			if (record2 != null)
			{
				_ownerId = record2.Id;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: loadCastleData(): " + e);
		}
	}

	/** Load All Functions */
	private void loadFunctions()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			var query = ctx.CastleFunctions.Where(r => r.CastleId == castleId);
			foreach (var record in query)
			{
				_function.put(record.Type,
					new CastleFunction(this, record.Type, record.Level, record.Lease, 0, record.Rate,
						record.EndTime, true));
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: Castle.loadFunctions(): " + e);
		}
	}

	/**
	 * Remove function In List and in DB
	 * @param functionType
	 */
	public void removeFunction(int functionType)
	{
		_function.remove(functionType);
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			ctx.CastleFunctions.Where(r => r.CastleId == castleId && r.Type == functionType).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: Castle.removeFunctions(int functionType): " + e);
		}
	}

	public bool updateFunctions(Player player, int type, int lvl, int lease, TimeSpan rate, bool addNew)
	{
		if (player == null)
			return false;

        if (lease > 0 && !player.destroyItemByItemId("Consume", Inventory.ADENA_ID, lease, null, true))
			return false;

        if (addNew)
			_function.put(type, new CastleFunction(this, type, lvl, lease, 0, rate, null, false));
        else if (lvl == 0 && lease == 0)
			removeFunction(type);
		else
        {
            CastleFunction func = _function.get(type) ?? throw new InvalidOperationException($"Function type={type} not found");
			int diffLease = lease - func.getLease();
			if (diffLease > 0)
			{
				_function.remove(type);
				_function.put(type, new CastleFunction(this, type, lvl, lease, 0, rate, null, false));
			}
			else
			{
                func.setLease(lease);
                func.setLvl(lvl);
                func.dbSave();
			}
		}

		return true;
	}

	public void activateInstance()
	{
		loadDoor();
	}

	// This method loads castle door data from database
	private void loadDoor()
	{
		foreach (Door door in DoorData.getInstance().getDoors())
        {
            Castle? castle = door.getCastle();
			if (castle != null && castle.getResidenceId() == getResidenceId())
			{
				_doors.Add(door);
			}
		}
	}

	// This method loads castle door upgrade data from database
	private void loadDoorUpgrade()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			var query = ctx.CastleDoorUpgrades.Where(r => r.CastleId == castleId);
			foreach (var record in query)
			{
				setDoorUpgrade(record.DoorId, record.Ratio, false);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: loadCastleDoorUpgrade(): " + e);
		}
	}

	private void removeDoorUpgrade()
	{
		foreach (Door door in _doors)
		{
			door.getStat().setUpgradeHpRatio(1);
			door.setCurrentHp(door.getCurrentHp());
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			ctx.CastleDoorUpgrades.Where(r => r.CastleId == castleId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: removeDoorUpgrade(): " + e);
		}
	}

	public void setDoorUpgrade(int doorId, int ratio, bool save)
	{
		Door? door = getDoors().Count == 0 ? DoorData.getInstance().getDoor(doorId) : getDoor(doorId);
		if (door == null)
			return;

		door.getStat().setUpgradeHpRatio(ratio);
		door.setCurrentHp(door.getMaxHp());

		if (save)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				var record = ctx.CastleDoorUpgrades.SingleOrDefault(r => r.DoorId == doorId);
				if (record is null)
				{
					record = new DbCastleDoorUpgrade();
					record.DoorId = doorId;
					ctx.CastleDoorUpgrades.Add(record);
				}

				record.CastleId = (byte)getResidenceId();
				record.Ratio = (short)ratio;
				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error("Exception: setDoorUpgrade(int doorId, int ratio, int castleId): " + e);
			}
		}
	}

	private void updateOwnerInDB(Clan? clan)
	{
		if (clan != null)
		{
			_ownerId = clan.getId(); // Update owner id property
		}
		else
		{
			_ownerId = 0; // Remove owner
			CastleManorManager.getInstance().resetManorData(getResidenceId());
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();

			// Need to remove has castle flag from clan_data, should be checked from castle table.
			ctx.Clans.Where(c => c.Castle == castleId).ExecuteUpdate(s => s.SetProperty(r => r.Castle, (short?)null));
			ctx.Clans.Where(c => c.Id == _ownerId).ExecuteUpdate(s => s.SetProperty(r => r.Castle, (short)castleId));

			// Announce to clan members
			if (clan != null)
			{
				clan.setCastleId(getResidenceId()); // Set has castle flag for new owner
				clan.broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(clan));
				clan.broadcastToOnlineMembers(new PlaySoundPacket(1, "Siege_Victory", 0, 0, 0, 0, 0));
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: updateOwnerInDB(Pledge clan): " + e);
		}
	}

	public Door? getDoor(int doorId)
	{
		foreach (Door door in _doors)
		{
			if (door.getId() == doorId)
			{
				return door;
			}
		}
		return null;
	}

	public Door? getDoor(string doorName)
	{
		foreach (Door door in _doors)
		{
			if (door.getTemplate().getName().equals(doorName))
			{
				return door;
			}
		}
		return null;
	}

	public List<Door> getDoors()
	{
		return _doors;
	}

	public bool isFirstMidVictory()
	{
		return _isFirstMidVictory;
	}

	public void setFirstMidVictory(bool value)
	{
		_isFirstMidVictory = value;
	}

	public override int getOwnerId()
	{
		return _ownerId;
	}

	public Clan? getOwner()
	{
		return _ownerId != 0 ? ClanTable.getInstance().getClan(_ownerId) : null;
	}

	public Siege getSiege()
	{
		if (_siege == null)
		{
			_siege = new Siege(this);
		}
		return _siege;
	}

	public DateTime getSiegeDate()
	{
		return _siegeDate;
	}

	public void setSiegeDate(DateTime siegeDate)
	{
		_siegeDate = siegeDate;
	}

	public bool isTimeRegistrationOver()
	{
		return _isTimeRegistrationOver;
	}

	public void setTimeRegistrationOver(bool value)
	{
		_isTimeRegistrationOver = value;
	}

	public DateTime getTimeRegistrationOverDate()
	{
		if (_siegeTimeRegistrationEndDate == null)
		{
			_siegeTimeRegistrationEndDate = DateTime.UtcNow;
		}

		return _siegeTimeRegistrationEndDate.Value;
	}

	public void setTimeRegistrationOverDate(DateTime time)
	{
		_siegeTimeRegistrationEndDate = time;
	}

	public int getTaxPercent(TaxType type)
	{
		int taxPercent;
		switch (_castleSide)
		{
			case CastleSide.LIGHT:
			{
				taxPercent = type == TaxType.BUY ? Config.CASTLE_BUY_TAX_LIGHT : Config.CASTLE_SELL_TAX_LIGHT;
				break;
			}
			case CastleSide.DARK:
			{
				taxPercent = type == TaxType.BUY ? Config.CASTLE_BUY_TAX_DARK : Config.CASTLE_SELL_TAX_DARK;
				break;
			}
			default:
			{
				taxPercent = type == TaxType.BUY ? Config.CASTLE_BUY_TAX_NEUTRAL : Config.CASTLE_SELL_TAX_NEUTRAL;
				break;
			}
		}
		return taxPercent;
	}

	public double getTaxRate(TaxType taxType)
	{
		return getTaxPercent(taxType) / 100.0;
	}

	public long getTreasury()
	{
		return _treasury;
	}

	public bool getShowNpcCrest()
	{
		return _showNpcCrest;
	}

	public void setShowNpcCrest(bool showNpcCrest)
	{
		if (_showNpcCrest != showNpcCrest)
		{
			_showNpcCrest = showNpcCrest;
			updateShowNpcCrest();
		}
	}

	public void updateClansReputation()
	{
		if (_formerOwner != null)
		{
			if (_formerOwner != ClanTable.getInstance().getClan(getOwnerId()))
			{
				int maxreward = Math.Max(0, _formerOwner.getReputationScore());
				_formerOwner.takeReputationScore(Config.LOOSE_CASTLE_POINTS);
				Clan? owner = ClanTable.getInstance().getClan(getOwnerId());
				if (owner != null)
				{
					owner.addReputationScore(Math.Min(Config.TAKE_CASTLE_POINTS, maxreward));
				}
			}
			else
			{
				_formerOwner.addReputationScore(Config.CASTLE_DEFENDED_POINTS);
			}
		}
		else
		{
			Clan? owner = ClanTable.getInstance().getClan(getOwnerId());
			if (owner != null)
			{
				owner.addReputationScore(Config.TAKE_CASTLE_POINTS);
			}
		}
	}

	public void updateShowNpcCrest()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			ctx.Castles.Where(r => r.Id == castleId)
				.ExecuteUpdate(s => s.SetProperty(r => r.ShowNpcCrest, _showNpcCrest));
		}
		catch (Exception e)
		{
			LOGGER.Error("Error saving showNpcCrest for castle " + getName() + ": " + e);
		}
	}

	/**
	 * Register Artefact to castle
	 * @param artefact
	 */
	public void registerArtefact(Artefact artefact)
	{
		_artefacts.add(artefact);
	}

	public Set<Artefact> getArtefacts()
	{
		return _artefacts;
	}

	/**
	 * @return the tickets exchanged for this castle
	 */
	public int getTicketBuyCount()
	{
		return _ticketBuyCount;
	}

	/**
	 * Set the exchanged tickets count.<br>
	 * Performs database update.
	 * @param count the ticket count to set
	 */
	public void setTicketBuyCount(int count)
	{
		_ticketBuyCount = count;

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			ctx.Castles.Where(r => r.Id == castleId)
				.ExecuteUpdate(s => s.SetProperty(r => r.TicketBuyCount, _ticketBuyCount));
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
	}

	public int getTrapUpgradeLevel(int towerIndex)
	{
		TowerSpawn? spawn = SiegeManager.getInstance().getFlameTowers(getResidenceId())?[towerIndex];
		return spawn != null ? spawn.getUpgradeLevel() : 0;
	}

	public void setTrapUpgrade(int towerIndex, int level, bool save)
	{
		if (save)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int castleId = getResidenceId();
				var record = ctx.CastleTrapUpgrades.SingleOrDefault(r => r.CastleId == castleId && r.TowerIndex == towerIndex);
				if (record is null)
				{
					record = new CastleTrapUpgrade();
					record.CastleId = (short)castleId;
					record.TowerIndex = (short)towerIndex;
					ctx.CastleTrapUpgrades.Add(record);
				}

				record.Level = (short)level;
				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error("Exception: setTrapUpgradeLevel(int towerIndex, int level, int castleId): " + e);
			}
		}
		TowerSpawn? spawn = SiegeManager.getInstance().getFlameTowers(getResidenceId())?[towerIndex];
		if (spawn != null)
		{
			spawn.setUpgradeLevel(level);
		}
	}

	private void removeTrapUpgrade()
    {
        List<TowerSpawn>? towers = SiegeManager.getInstance().getFlameTowers(getResidenceId());
        if (towers != null)
        {
		    foreach (TowerSpawn ts in towers)
		    {
			    ts.setUpgradeLevel(0);
		    }
        }

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			ctx.CastleTrapUpgrades.Where(r => r.CastleId == castleId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: removeDoorUpgrade(): " + e);
		}
	}

	public override void giveResidentialSkills(Player player)
	{
		base.giveResidentialSkills(player);
		Skill skill = _castleSide == CastleSide.DARK ? CommonSkill.ABILITY_OF_DARKNESS.getSkill() : CommonSkill.ABILITY_OF_LIGHT.getSkill();
		player.addSkill(skill);
	}

	public override void removeResidentialSkills(Player player)
	{
		base.removeResidentialSkills(player);
		player.removeSkill((int)CommonSkill.ABILITY_OF_DARKNESS);
		player.removeSkill((int)CommonSkill.ABILITY_OF_LIGHT);
	}

	public void spawnSideNpcs()
	{
		foreach (Npc npc in _sideNpcs)
		{
			if (npc != null)
			{
				npc.deleteMe();
			}
		}
		_sideNpcs.Clear();

		foreach (CastleSpawnHolder holder in getSideSpawns())
		{
			if (holder != null)
			{
				Spawn spawn;
				try
				{
					spawn = new Spawn(holder.getNpcId());
				}
				catch (Exception e)
				{
					LOGGER.Warn(nameof(Castle) + ": " + e);
					return;
				}
				spawn.Location = holder.Location;
				Npc? npc = spawn.doSpawn(false);
				spawn.stopRespawn();
                if (npc != null)
                {
                    npc.broadcastInfo();
                    _sideNpcs.Add(npc);
                }
            }
		}
	}

	public ImmutableArray<CastleSpawnHolder> getSideSpawns()
	{
		return CastleData.getInstance().getSpawnsForSide(getResidenceId(), getSide());
	}

	public void setSide(CastleSide side)
	{
		if (_castleSide == side)
		{
			return;
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = getResidenceId();
			ctx.Castles.Where(r => r.Id == castleId)
				.ExecuteUpdate(s => s.SetProperty(r => r.Side, side));
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
		_castleSide = side;
		Broadcast.toAllOnlinePlayers(new ExCastleStatePacket(this));
		spawnSideNpcs();
	}

	public CastleSide getSide()
	{
		return _castleSide;
	}
}