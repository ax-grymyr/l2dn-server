using L2Dn.Events;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using FortManager = L2Dn.GameServer.InstanceManagers.FortManager;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Sieges;

public class Fort: AbstractResidence, IEventContainerProvider
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Fort));

	private readonly EventContainer _eventContainer;
	private readonly List<Door> _doors = new();
	private StaticObject _flagPole = null;
	private FortSiege _siege = null;
	private DateTime _siegeDate;
	private DateTime? _lastOwnedTime;
	private SiegeZone _zone;
	private Clan? _fortOwner = null;
	private int _fortType = 0;
	private int _state = 0;
	private int _castleId = 0;
	private int _supplyLvL = 0;
	private readonly Map<int, FortFunction> _function = new();
	private readonly ScheduledFuture[] _fortUpdater = new ScheduledFuture[2];
	
	// Spawn Data
	private bool _isSuspiciousMerchantSpawned = false;
	private readonly Set<Spawn> _siegeNpcs = new();
	private readonly Set<Spawn> _npcCommanders = new();
	private readonly Set<Spawn> _specialEnvoys = new();
	
	private readonly Map<int, int> _envoyCastles = new();
	private readonly Set<int> _availableCastles = new();
	
	/** Fortress Functions */
	public const int FUNC_TELEPORT = 1;
	public const int FUNC_RESTORE_HP = 2;
	public const int FUNC_RESTORE_MP = 3;
	public const int FUNC_RESTORE_EXP = 4;
	public const int FUNC_SUPPORT = 5;

	public EventContainer Events => _eventContainer;
	
	public class FortFunction
	{
		private readonly Fort _fort;
		private readonly int _type;
		private int _level;
		protected int _fee;
		protected int _tempFee;
		private readonly TimeSpan _rate;
		DateTime? _endDate;
		protected bool _inDebt;
		public bool _cwh;
		
		public FortFunction(Fort fort, int type, int level, int lease, int tempLease, TimeSpan rate, DateTime? time, bool cwh)
		{
			_fort = fort;
			_type = type;
			_level = level;
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
		
		public int getLevel()
		{
			return _level;
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
			_level = lvl;
		}
		
		public void setLease(int lease)
		{
			_fee = lease;
		}
		
		public void setEndTime(DateTime? time)
		{
			_endDate = time;
		}
		
		private void initializeTask(bool cwh)
		{
			if (_fort._fortOwner == null)
			{
				return;
			}
			
			DateTime currentTime = DateTime.UtcNow;
			if (_endDate > currentTime)
			{
				ThreadPool.schedule(new FunctionTask(_fort, this, cwh), _endDate.Value - currentTime);
			}
			else
			{
				ThreadPool.schedule(new FunctionTask(_fort, this, cwh), 0);
			}
		}
		
		private class FunctionTask: Runnable
		{
			private readonly Fort _fort;
			private readonly FortFunction _fortFunction;

			public FunctionTask(Fort fort, FortFunction fortFunction, bool cwh)
			{
				_fort = fort;
				_fortFunction = fortFunction;
				_fortFunction._cwh = cwh;
			}
			
			public void run()
			{
				try
				{
					if (_fort._fortOwner == null)
					{
						return;
					}
					if ((_fort._fortOwner.getWarehouse().getAdena() >= _fortFunction._fee) || !_fortFunction._cwh)
					{
						int fee = _fortFunction._endDate is null ? _fortFunction._tempFee : _fortFunction._fee;
						_fortFunction.setEndTime(DateTime.UtcNow + _fortFunction._rate);
						_fortFunction.dbSave();
						if (_fortFunction._cwh)
						{
							_fort._fortOwner.getWarehouse().destroyItemByItemId("CS_function_fee", Inventory.ADENA_ID, fee, null, null);
						}
						ThreadPool.schedule(new FunctionTask(_fort, _fortFunction, true), _fortFunction._rate);
					}
					else
					{
						_fort.removeFunction(_fortFunction._type);
					}
				}
				catch (Exception t)
				{
					// Ignore. 
					// TODO: log
				}
			}
		}
		
		public void dbSave()
		{
			try
			{
				int fortId = _fort.getResidenceId();
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				var record = ctx.FortFunctions.SingleOrDefault(r => r.FortId == fortId && r.Type == _type);
				if (record is null)
				{
					record = new DbFortFunction();
					record.FortId = (byte)fortId;
					record.Type = (byte)_type;
					ctx.FortFunctions.Add(record);
				}

				record.Level = (short)_level;
				record.Lease = _fee;
				record.Rate = _rate;
				record.EndTime = _endDate;
				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error("Exception: Fort.updateFunctions(int type, int lvl, int lease, long rate, long time, bool addNew): " + e);
			}
		}
	}
	
	public Fort(int fortId, string fortName): base(fortId, fortName)
	{
		_eventContainer = new($"Fort template {fortId}", GlobalEvents.Global);
		load();
		loadFlagPoles();
		if (_fortOwner != null)
		{
			setVisibleFlag(true);
			loadFunctions();
		}
		
		initResidenceZone();
		// initFunctions();
		initNpcs(); // load and spawn npcs (Always spawned)
		initSiegeNpcs(); // load suspicious merchants (Despawned 10mins before siege)
		// spawnSuspiciousMerchant(); // spawn suspicious merchants
		initNpcCommanders(); // npc Commanders (not monsters) (Spawned during siege)
		spawnNpcCommanders(); // spawn npc Commanders
		initSpecialEnvoys(); // envoys from castles (Spawned after fort taken)
		if ((_fortOwner != null) && (_state == 0))
		{
			spawnSpecialEnvoys();
		}
	}
	
	/**
	 * Return function with id
	 * @param type
	 * @return
	 */
	public FortFunction getFortFunction(int type)
	{
		return _function.get(type);
	}
	
	public void endOfSiege(Clan clan)
	{
		ThreadPool.execute(new endFortressSiege(this, clan));
	}
	
	/**
	 * Move non clan members off fort area and to nearest town.
	 */
	public void banishForeigners()
	{
		getResidenceZone().banishForeigners(_fortOwner.getId());
	}
	
	/**
	 * @param x
	 * @param y
	 * @param z
	 * @return true if object is inside the zone
	 */
	public bool checkIfInZone(Location3D location)
	{
		SiegeZone zone = getZone();
		return (zone != null) && zone.isInsideZone(location);
	}
	
	public SiegeZone getZone()
	{
		if (_zone == null)
		{
			foreach (SiegeZone zone in ZoneManager.getInstance().getAllZones<SiegeZone>())
			{
				if (zone.getSiegeObjectId() == getResidenceId())
				{
					_zone = zone;
					break;
				}
			}
		}
		return _zone;
	}
	
	public override FortZone getResidenceZone()
	{
		return (FortZone) base.getResidenceZone();
	}
	
	/**
	 * Get the objects distance to this fort
	 * @param obj
	 * @return
	 */
	public double getDistance(WorldObject obj)
	{
		return getZone().getDistanceToZone(obj);
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
		if (player.getClan() != _fortOwner)
		{
			return;
		}
		
		Door door = getDoor(doorId);
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
	
	// This method is used to begin removing all fort upgrades
	public void removeUpgrade()
	{
		removeDoorUpgrade();
	}
	
	/**
	 * This method will set owner for Fort
	 * @param clan
	 * @param updateClansReputation
	 * @return
	 */
	public bool setOwner(Clan clan, bool updateClansReputation)
	{
		if (clan == null)
		{
			LOGGER.Warn(GetType().Name + ": Updating Fort owner with null clan!!!");
			return false;
		}
		
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_OF_S1_HAS_FINISHED);
		sm.Params.addCastleId(getResidenceId());
		getSiege().announceToPlayer(sm);
		
		Clan oldowner = _fortOwner;
		if ((oldowner != null) && (clan != oldowner))
		{
			// Remove points from old owner
			this.updateClansReputation(oldowner, true);
			try
			{
				Player oldleader = oldowner.getLeader().getPlayer();
				if ((oldleader != null) && (oldleader.getMountType() == MountType.WYVERN))
				{
					oldleader.dismount();
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn("Exception in setOwner: " + e);
			}
			if (getSiege().isInProgress())
			{
				getSiege().updatePlayerSiegeStateFlags(true);
			}
			removeOwner(true);
		}
		setFortState(0, 0); // initialize fort state
		
		// if clan already have castle, don't store him in fortress
		if (clan.getCastleId() > 0)
		{
			getSiege().announceToPlayer(new SystemMessagePacket(SystemMessageId.THE_REBEL_ARMY_RECAPTURED_THE_FORTRESS));
			return false;
		}
		
		// Give points to new owner
		if (updateClansReputation)
		{
			this.updateClansReputation(clan, false);
		}
		
		spawnSpecialEnvoys();
		// if clan have already fortress, remove it
		if (clan.getFortId() > 0)
		{
			FortManager.getInstance().getFortByOwner(clan).removeOwner(true);
		}
		
		setSupplyLevel(0);
		setOwnerClan(clan);
		updateOwnerInDB(); // Update in database
		saveFortVariables();
		
		if (getSiege().isInProgress())
		{
			getSiege().endSiege();
		}
		
		foreach (Player member in clan.getOnlineMembers(0))
		{
			giveResidentialSkills(member);
			member.sendSkillList();
		}
		return true;
	}
	
	public void removeOwner(bool updateDB)
	{
		Clan clan = _fortOwner;
		if (clan != null)
		{
			foreach (Player member in clan.getOnlineMembers(0))
			{
				removeResidentialSkills(member);
				member.sendSkillList();
			}
			clan.setFortId(0);
			clan.broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(clan));
			setOwnerClan(null);
			setSupplyLevel(0);
			saveFortVariables();
			removeAllFunctions();
			if (updateDB)
			{
				updateOwnerInDB();
			}
		}
	}
	
	public void raiseSupplyLvL()
	{
		_supplyLvL++;
		if (_supplyLvL > Config.FS_MAX_SUPPLY_LEVEL)
		{
			_supplyLvL = Config.FS_MAX_SUPPLY_LEVEL;
		}
	}
	
	public void setSupplyLevel(int value)
	{
		if (value <= Config.FS_MAX_SUPPLY_LEVEL)
		{
			_supplyLvL = value;
		}
	}
	
	public int getSupplyLevel()
	{
		return _supplyLvL;
	}
	
	public void saveFortVariables()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			ctx.Forts.Where(r => r.Id == fortId).ExecuteUpdate(s => s.SetProperty(r => r.SupplyLevel, _supplyLvL));
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: saveFortVariables(): " + e);
		}
	}
	
	/**
	 * Show or hide flag inside flag pole.
	 * @param value
	 */
	public void setVisibleFlag(bool value)
	{
		StaticObject flagPole = _flagPole;
		if (flagPole != null)
		{
			flagPole.setMeshIndex(value ? 1 : 0);
		}
	}
	
	/**
	 * Respawn all doors on fort grounds.
	 */
	public void resetDoors()
	{
		foreach (Door door in _doors)
		{
			if (door.isOpen())
			{
				door.closeMe();
			}
			// Orc Fortress
			if (!door.isOpen())
			{
				door.openMe();
			}
			if (door.isDead())
			{
				door.doRevive();
			}
			if (door.getCurrentHp() < door.getMaxHp())
			{
				door.setCurrentHp(door.getMaxHp());
			}
		}
		loadDoorUpgrade(); // Check for any upgrade the doors may have
	}
	
	public void OpenOrcFortressDoors()
	{
		foreach (Door door in _doors)
		{
			if (!door.isOpen())
			{
				door.openMe();
				
			}
		}
	}
	
	public void CloseOrcFortressDoors()
	{
		foreach (Door door in _doors)
		{
			if (door.isOpen())
			{
				door.closeMe();
			}
		}
	}
	
	// Orc Fortress
	public void SetOrcFortressOwnerNpcs(bool val)
	{
		SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_fortress_owner_npcs").forEach(holder =>
		{
			if (val)
			{
				holder.spawnAll();
			}
			else
			{
				holder.despawnAll();
			}
		}));
	}
	
	// This method upgrade door
	public void upgradeDoor(int doorId, int hp, int pDef, int mDef)
	{
		Door door = getDoor(doorId);
		if (door != null)
		{
			door.setCurrentHp(door.getMaxHp() + hp);
			saveDoorUpgrade(doorId, hp, pDef, mDef);
		}
	}
	
	// This method loads fort
	protected override void load()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			int? ownerId = null;
			var record = ctx.Forts.SingleOrDefault(r => r.Id == fortId);
			if (record is not null)
			{
				_siegeDate = record.SiegeDate;
				_lastOwnedTime = record.LastOwnedTime;
				ownerId = record.OwnerId;
				_fortType = record.Type;
				_state = record.State;
				_castleId = record.CastleId;
				_supplyLvL = record.SupplyLevel;
			}

			if (ownerId != null)
			{
				Clan clan = ClanTable.getInstance().getClan(ownerId.Value); // Try to find clan instance
				clan.setFortId(getResidenceId());
				setOwnerClan(clan);
				TimeSpan period = TimeSpan.FromSeconds(Config.FS_UPDATE_FRQ * 60);
				int runCount = (int)((getOwnedTime() ?? TimeSpan.Zero) / period);
				TimeSpan initial = _lastOwnedTime is null ? TimeSpan.Zero : DateTime.UtcNow - _lastOwnedTime.Value;
				if (initial > period)
				{
					initial = TimeSpan.FromTicks(initial.Ticks % period.Ticks);
				}

				initial = period - initial;
				if ((Config.FS_MAX_OWN_TIME <= 0) || (getOwnedTime() < TimeSpan.FromSeconds(Config.FS_MAX_OWN_TIME * 3600)))
				{
					_fortUpdater[0] = ThreadPool.scheduleAtFixedRate(
						new FortUpdater(this, clan, runCount, FortUpdaterType.PERIODIC_UPDATE), initial,
						period); // Schedule owner tasks to start running
					if (Config.FS_MAX_OWN_TIME > 0)
					{
						_fortUpdater[1] = ThreadPool.scheduleAtFixedRate(
							new FortUpdater(this, clan, runCount, FortUpdaterType.MAX_OWN_TIME), 3600000,
							3600000); // Schedule owner tasks to remove owner
					}
				}
				else
				{
					_fortUpdater[1] = ThreadPool.schedule(new FortUpdater(this, clan, 0, FortUpdaterType.MAX_OWN_TIME),
						60000); // Schedule owner tasks to remove owner
				}
			}
			else
			{
				setOwnerClan(null);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: loadFortData(): " + e);
		}
	}
	
	/** Load All Functions */
	private void loadFunctions()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			var query = ctx.FortFunctions.Where(r => r.FortId == fortId);
			foreach (var record in query)
			{
				_function.put(record.Type, new FortFunction(this, record.Type, record.Level, record.Lease, 0, record.Rate, record.EndTime, true));
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: Fort.loadFunctions(): " + e);
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
			int fortId = getResidenceId();
			ctx.FortFunctions.Where(r => r.FortId == fortId && r.Type == functionType).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: Fort.removeFunctions(int functionType): " + e);
		}
	}
	
	/**
	 * Remove all fort functions.
	 */
	private void removeAllFunctions()
	{
		foreach (int id in _function.Keys)
		{
			removeFunction(id);
		}
	}
	
	public bool updateFunctions(Player player, int type, int lvl, int lease, TimeSpan rate, bool addNew)
	{
		if (player == null)
		{
			return false;
		}
		if ((lease > 0) && !player.destroyItemByItemId("Consume", Inventory.ADENA_ID, lease, null, true))
		{
			return false;
		}
		if (addNew)
		{
			_function.put(type, new FortFunction(this, type, lvl, lease, 0, rate, null, false));
		}
		else if ((lvl == 0) && (lease == 0))
		{
			removeFunction(type);
		}
		else if ((lease - _function.get(type).getLease()) > 0)
		{
			_function.remove(type);
			_function.put(type, new FortFunction(this, type, lvl, lease, 0, rate, null, false));
		}
		else
		{
			_function.get(type).setLease(lease);
			_function.get(type).setLvl(lvl);
			_function.get(type).dbSave();
		}
		return true;
	}
	
	public void activateInstance()
	{
		loadDoor();
	}
	
	// This method loads fort door data from database
	private void loadDoor()
	{
		foreach (Door door in DoorData.getInstance().getDoors())
		{
			if ((door.getFort() != null) && (door.getFort().getResidenceId() == getResidenceId()))
			{
				_doors.add(door);
			}
		}
	}
	
	private void loadFlagPoles()
	{
		foreach (StaticObject obj in StaticObjectData.getInstance().getStaticObjects())
		{
			if ((obj.getType() == 3) && obj.getName().startsWith(getName()))
			{
				_flagPole = obj;
				break;
			}
		}
		if (_flagPole == null)
		{
			throw new InvalidOperationException("Can't find flagpole for Fort " + this);
		}
	}
	
	// This method loads fort door upgrade data from database
	private void loadDoorUpgrade()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			var query = ctx.FortDoorUpgrades.Where(r => r.FortId == fortId);
			foreach (var record in query)
			{
				upgradeDoor(record.DoorId, record.Hp, record.PDef, record.MDef);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: loadFortDoorUpgrade(): " + e);
		}
	}
	
	private void removeDoorUpgrade()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			ctx.FortDoorUpgrades.Where(r => r.FortId == fortId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: removeDoorUpgrade(): " + e);
		}
	}
	
	private void saveDoorUpgrade(int doorId, int hp, int pDef, int mDef)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.FortDoorUpgrades.Add(new DbFortDoorUpgrade()
			{
				DoorId = doorId,
				FortId = (byte)getResidenceId(),
				Hp = hp,
				PDef = pDef,
				MDef = mDef
			});

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: saveDoorUpgrade(int doorId, int hp, int pDef, int mDef): " + e);
		}
	}
	
	private void updateOwnerInDB()
	{
		Clan clan = _fortOwner;
		int? clanId = null;
		if (clan != null)
		{
			clanId = clan.getId();
			_lastOwnedTime = DateTime.UtcNow;;
		}
		else
		{
			_lastOwnedTime = null;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			var record = ctx.Forts.SingleOrDefault(r => r.Id == fortId);
			if (record == null)
			{
				record = new DbFort();
				record.Id = fortId;
				ctx.Forts.Add(record);
			}

			record.OwnerId = clanId;
			record.LastOwnedTime = _lastOwnedTime;
			record.State = 0;
			record.CastleId = 0;
			ctx.SaveChanges();
			
			// Announce to clan members
			if (clan != null)
			{
				clan.setFortId(getResidenceId()); // Set has fort flag for new owner
				SystemMessagePacket sm;
				sm = new SystemMessagePacket(SystemMessageId.S1_IS_VICTORIOUS_IN_THE_FORTRESS_BATTLE_OF_S2);
				sm.Params.addString(clan.getName());
				sm.Params.addCastleId(getResidenceId());
				World.getInstance().getPlayers().forEach(p => p.sendPacket(sm));
				clan.broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(clan));
				clan.broadcastToOnlineMembers(new PlaySoundPacket(1, "Siege_Victory", 0, 0, 0, 0, 0));
				if (_fortUpdater[0] != null)
				{
					_fortUpdater[0].cancel(false);
				}
				if (_fortUpdater[1] != null)
				{
					_fortUpdater[1].cancel(false);
				}
				_fortUpdater[0] = ThreadPool.scheduleAtFixedRate(new FortUpdater(this, clan, 0, FortUpdaterType.PERIODIC_UPDATE), Config.FS_UPDATE_FRQ * 60000, Config.FS_UPDATE_FRQ * 60000); // Schedule owner tasks to start running
				if (Config.FS_MAX_OWN_TIME > 0)
				{
					_fortUpdater[1] = ThreadPool.scheduleAtFixedRate(new FortUpdater(this, clan, 0, FortUpdaterType.MAX_OWN_TIME), 3600000, 3600000); // Schedule owner tasks to remove owner
				}
			}
			else
			{
				if (_fortUpdater[0] != null)
				{
					_fortUpdater[0].cancel(false);
				}
				_fortUpdater[0] = null;
				if (_fortUpdater[1] != null)
				{
					_fortUpdater[1].cancel(false);
				}
				_fortUpdater[1] = null;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: updateOwnerInDB(Pledge clan): " + e);
		}
	}
	
	public override int getOwnerId()
	{
		Clan clan = _fortOwner;
		return clan != null ? clan.getId() : -1;
	}
	
	public Clan getOwnerClan()
	{
		return _fortOwner;
	}
	
	public void setOwnerClan(Clan clan)
	{
		setVisibleFlag(clan != null);
		_fortOwner = clan;
	}
	
	public Door getDoor(int doorId)
	{
		if (doorId <= 0)
		{
			return null;
		}
		
		foreach (Door door in _doors)
		{
			if (door.getId() == doorId)
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
	
	public StaticObject getFlagPole()
	{
		return _flagPole;
	}
	
	public FortSiege getSiege()
	{
		if (_siege == null)
		{
			lock (this)
			{
				if (_siege == null)
				{
					_siege = new FortSiege(this);
				}
			}
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
	
	/// <summary>
	/// Seconds
	/// </summary>
	/// <returns></returns>
	public TimeSpan? getOwnedTime()
	{
		return _lastOwnedTime is null ? null : DateTime.UtcNow - _lastOwnedTime.Value;
	}
	
	public TimeSpan? getTimeTillRebelArmy()
	{
		return _lastOwnedTime is null
			? null
			: _lastOwnedTime + TimeSpan.FromMilliseconds(Config.FS_MAX_OWN_TIME * 3600000) - DateTime.UtcNow;
	}
	
	public TimeSpan getTimeTillNextFortUpdate()
	{
		return _fortUpdater[0] == null ? TimeSpan.Zero : _fortUpdater[0].getDelay();
	}
	
	public void updateClansReputation(Clan owner, bool removePoints)
	{
		if (owner != null)
		{
			if (removePoints)
			{
				owner.takeReputationScore(Config.LOOSE_FORT_POINTS);
			}
			else
			{
				owner.addReputationScore(Config.TAKE_FORT_POINTS);
			}
		}
	}
	
	private class endFortressSiege: Runnable
	{
		private Fort _f;
		private Clan _clan;
		
		public endFortressSiege(Fort f, Clan clan)
		{
			_f = f;
			_clan = clan;
		}
		
		public void run()
		{
			try
			{
				_f.setOwner(_clan, true);
			}
			catch (Exception e)
			{
				LOGGER.Error("Exception in endFortressSiege " + e);
			}
		}
	}
	
	/**
	 * @return Returns state of fortress.<br>
	 *         0 - not decided yet<br>
	 *         1 - independent<br>
	 *         2 - contracted with castle
	 */
	public int getFortState()
	{
		return _state;
	}
	
	/**
	 * @param state
	 *            <ul>
	 *            <li>0 - not decided yet</li>
	 *            <li>1 - independent</li>
	 *            <li>2 - contracted with castle</li>
	 *            </ul>
	 * @param castleId the Id of the contracted castle (0 if no contract with any castle)
	 */
	public void setFortState(int state, int castleId)
	{
		_state = state;
		_castleId = castleId;
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			var record = ctx.Forts.SingleOrDefault(r => r.Id == fortId);
			if (record == null)
			{
				record = new DbFort();
				record.Id = fortId;
				ctx.Forts.Add(record);
			}

			record.LastOwnedTime = _lastOwnedTime;
			record.State = 0;
			record.CastleId = 0;
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: setFortState(int state, int castleId): " + e);
		}
	}
	
	/**
	 * @return the fortress type (0 - small (3 commanders), 1 - big (4 commanders + control room))
	 */
	public int getFortType()
	{
		return _fortType;
	}
	
	/**
	 * @param npcId the Id of the ambassador NPC
	 * @return the Id of the castle this ambassador represents
	 */
	public int getCastleIdByAmbassador(int npcId)
	{
		if ((_envoyCastles == null) || !_envoyCastles.containsKey(npcId))
		{
			return -1;
		}
		return _envoyCastles.get(npcId);
	}
	
	/**
	 * @param npcId the Id of the ambassador NPC
	 * @return the castle this ambassador represents
	 */
	public Castle getCastleByAmbassador(int npcId)
	{
		return CastleManager.getInstance().getCastleById(getCastleIdByAmbassador(npcId));
	}
	
	/**
	 * @return the Id of the castle contracted with this fortress
	 */
	public int getContractedCastleId()
	{
		return _castleId;
	}
	
	/**
	 * @return the castle contracted with this fortress ({@code null} if no contract with any castle)
	 */
	public Castle getContractedCastle()
	{
		return CastleManager.getInstance().getCastleById(getContractedCastleId());
	}
	
	/**
	 * Check if this is a border fortress (associated with multiple castles).
	 * @return {@code true} if this is a border fortress (associated with more than one castle), {@code false} otherwise
	 */
	public bool isBorderFortress()
	{
		return _availableCastles.size() > 1;
	}
	
	/**
	 * @return the amount of barracks in this fortress
	 */
	public int getFortSize()
	{
		return _fortType == 0 ? 3 : 5;
	}
	
	public void spawnSuspiciousMerchant()
	{
		if (_isSuspiciousMerchantSpawned)
		{
			return;
		}
		
		_isSuspiciousMerchantSpawned = true;
		foreach (Spawn spawnDat in _siegeNpcs)
		{
			spawnDat.doSpawn(false);
			spawnDat.startRespawn();
		}
	}
	
	public void despawnSuspiciousMerchant()
	{
		if (!_isSuspiciousMerchantSpawned)
		{
			return;
		}
		_isSuspiciousMerchantSpawned = false;
		foreach (Spawn spawnDat in _siegeNpcs)
		{
			spawnDat.stopRespawn();
			spawnDat.getLastSpawn().deleteMe();
		}
	}
	
	public void spawnNpcCommanders()
	{
		foreach (Spawn spawnDat in _npcCommanders)
		{
			spawnDat.doSpawn(false);
			spawnDat.startRespawn();
		}
	}
	
	public void despawnNpcCommanders()
	{
		foreach (Spawn spawnDat in _npcCommanders)
		{
			spawnDat.stopRespawn();
			spawnDat.getLastSpawn().deleteMe();
		}
	}
	
	public void spawnSpecialEnvoys()
	{
		foreach (Spawn spawnDat in _specialEnvoys)
		{
			spawnDat.doSpawn(false);
			spawnDat.startRespawn();
		}
	}
	
	private void initNpcs()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			var query = ctx.FortSpawns.Where(r => r.FortId == fortId && r.Type == 0);
			foreach (var record in query)
			{
				Spawn spawnDat = new Spawn(record.NpcId);
				spawnDat.setAmount(1);
				spawnDat.Location = new Location(record.X, record.Y, record.Z, record.Heading);
				spawnDat.setRespawnDelay(TimeSpan.FromSeconds(60));
				SpawnTable.getInstance().addNewSpawn(spawnDat, false);
				spawnDat.doSpawn(false);
				spawnDat.startRespawn();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Fort " + getResidenceId() + " initNpcs: Spawn could not be initialized: " + e);
		}
	}
	
	private void initSiegeNpcs()
	{
		_siegeNpcs.clear();
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			var query = ctx.FortSpawns.Where(r => r.FortId == fortId && r.Type == 2);
			foreach (var record in query)
			{
				Spawn spawnDat = new Spawn(record.NpcId);
				spawnDat.setAmount(1);
				spawnDat.Location = new Location(record.X, record.Y, record.Z, record.Heading);
				spawnDat.setRespawnDelay(TimeSpan.FromSeconds(60));
				_siegeNpcs.add(spawnDat);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Fort " + getResidenceId() + " initSiegeNpcs: Spawn could not be initialized: " + e);
		}
	}
	
	private void initNpcCommanders()
	{
		_npcCommanders.clear();
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			var query = ctx.FortSpawns.Where(r => r.FortId == fortId && r.Type == 1);
			foreach (var record in query)
			{
				Spawn spawnDat = new Spawn(record.NpcId);
				spawnDat.setAmount(1);
				spawnDat.Location = new Location(record.X, record.Y, record.Z, record.Heading);
				spawnDat.setRespawnDelay(TimeSpan.FromSeconds(60));
				_npcCommanders.add(spawnDat);
			}
		}
		catch (Exception e)
		{
			// problem with initializing spawn, go to next one
			LOGGER.Error("Fort " + getResidenceId() + " initNpcCommanders: Spawn could not be initialized: " + e);
		}
	}
	
	private void initSpecialEnvoys()
	{
		_specialEnvoys.clear();
		_envoyCastles.clear();
		_availableCastles.clear();
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int fortId = getResidenceId();
			var query = ctx.FortSpawns.Where(r => r.FortId == fortId && r.Type == 3);
			foreach (var record in query)
			{
				Spawn spawnDat = new Spawn(record.NpcId);
				spawnDat.setAmount(1);
				spawnDat.Location = new Location(record.X, record.Y, record.Z, record.Heading);
				spawnDat.setRespawnDelay(TimeSpan.FromSeconds(60));

				_specialEnvoys.add(spawnDat);
				_envoyCastles.put(record.NpcId, record.CastleId);
				_availableCastles.add(record.CastleId);
			}
		}
		catch (Exception e)
		{
			// problem with initializing spawn, go to next one
			LOGGER.Error("Fort " + getResidenceId() + " initSpecialEnvoys: Spawn could not be initialized: " + e);
		}
	}
	
	protected override void initResidenceZone()
	{
		foreach (FortZone zone in ZoneManager.getInstance().getAllZones<FortZone>())
		{
			if (zone.getResidenceId() == getResidenceId())
			{
				setResidenceZone(zone);
				break;
			}
		}
	}
}