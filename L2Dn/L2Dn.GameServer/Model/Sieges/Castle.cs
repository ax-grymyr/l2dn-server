using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Sieges;

public class Castle: AbstractResidence
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Castle));
	
	private readonly List<Door> _doors = new();
	private readonly List<Npc> _sideNpcs = new();
	int _ownerId = 0;
	private Siege _siege = null;
	private Calendar _siegeDate;
	private bool _isTimeRegistrationOver = true; // true if Castle Lords set the time, or 24h is elapsed after the siege
	private Calendar _siegeTimeRegistrationEndDate; // last siege end date + 1 day
	private CastleSide _castleSide = null;
	private long _treasury = 0;
	private bool _showNpcCrest = false;
	private SiegeZone _zone = null;
	private ResidenceTeleportZone _teleZone;
	private Clan _formerOwner = null;
	private readonly Set<Artefact> _artefacts = new(1);
	private readonly Map<int, CastleFunction> _function = new();
	private int _ticketBuyCount = 0;
	private bool _isFirstMidVictory = false;
	
	/** Castle Functions */
	public const int FUNC_TELEPORT = 1;
	public const int FUNC_RESTORE_HP = 2;
	public const int FUNC_RESTORE_MP = 3;
	public const int FUNC_RESTORE_EXP = 4;
	public const int FUNC_SUPPORT = 5;
	
	public class CastleFunction
	{
		private readonly int _type;
		private int _lvl;
		protected int _fee;
		protected int _tempFee;
		private readonly long _rate;
		long _endDate;
		protected bool _inDebt;
		public bool _cwh;
		
		public CastleFunction(int type, int lvl, int lease, int tempLease, long rate, long time, bool cwh)
		{
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
		
		public long getRate()
		{
			return _rate;
		}
		
		public long getEndTime()
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
		
		public void setEndTime(long time)
		{
			_endDate = time;
		}
		
		private void initializeTask(bool cwh)
		{
			if (_ownerId <= 0)
			{
				return;
			}
			
			long currentTime = System.currentTimeMillis();
			if (_endDate > currentTime)
			{
				ThreadPool.schedule(new FunctionTask(cwh), _endDate - currentTime);
			}
			else
			{
				ThreadPool.schedule(new FunctionTask(cwh), 0);
			}
		}
		
		private class FunctionTask: Runnable
		{
			public FunctionTask(bool cwh)
			{
				_cwh = cwh;
			}
			
			public void run()
			{
				try
				{
					if (_ownerId <= 0)
					{
						return;
					}
					if ((ClanTable.getInstance().getClan(getOwnerId()).getWarehouse().getAdena() >= _fee) || !_cwh)
					{
						int fee = _fee;
						if (_endDate == -1)
						{
							fee = _tempFee;
						}
						
						setEndTime(System.currentTimeMillis() + _rate);
						dbSave();
						if (_cwh)
						{
							ClanTable.getInstance().getClan(getOwnerId()).getWarehouse().destroyItemByItemId("CS_function_fee", Inventory.ADENA_ID, fee, null, null);
						}
						ThreadPool.schedule(new FunctionTask(true), _rate);
					}
					else
					{
						removeFunction(_type);
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
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement ps = con.prepareStatement(
					"REPLACE INTO castle_functions (castle_id, type, lvl, lease, rate, endTime) VALUES (?,?,?,?,?,?)");
				ps.setInt(1, getResidenceId());
				ps.setInt(2, _type);
				ps.setInt(3, _lvl);
				ps.setInt(4, _fee);
				ps.setLong(5, _rate);
				ps.setLong(6, _endDate);
				ps.execute();
			}
			catch (Exception e)
			{
				LOGGER.Error("Exception: Castle.updateFunctions(int type, int lvl, int lease, long rate, long time, bool addNew): " + e);
			}
		}
	}
	
	public Castle(int castleId): base(castleId)
	{
		load();
		initResidenceZone();
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
	public CastleFunction getCastleFunction(int type)
	{
		if (_function.containsKey(type))
		{
			return _function.get(type);
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void engrave(Clan clan, WorldObject target, CastleSide side)
	{
		if (!_artefacts.contains(target))
		{
			return;
		}
		setSide(side);
		setOwner(clan);
		SystemMessage msg = new SystemMessage(SystemMessageId.CLAN_S1_HAS_SUCCEEDED_IN_S2);
		msg.addString(clan.getName());
		msg.addString(getName());
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
				Castle rune = CastleManager.getInstance().getCastle("rune");
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
				Castle aden = CastleManager.getInstance().getCastle("aden");
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
		else if ((_treasury + amount) > Inventory.MAX_ADENA)
		{
			_treasury = Inventory.MAX_ADENA;
		}
		else
		{
			_treasury += amount;
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("UPDATE castle SET treasury = ? WHERE id = ?");
			ps.setLong(1, _treasury);
			ps.setInt(2, getResidenceId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
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
	public bool checkIfInZone(int x, int y, int z)
	{
		SiegeZone zone = getZone();
		return (zone != null) && zone.isInsideZone(x, y, z);
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
	
	public override CastleZone getResidenceZone()
	{
		return (CastleZone) super.getResidenceZone();
	}
	
	public ResidenceTeleportZone getTeleZone()
	{
		if (_teleZone == null)
		{
			foreach (ResidenceTeleportZone zone in ZoneManager.getInstance().getAllZones<ResidenceTeleportZone>())
			{
				if (zone.getResidenceId() == getResidenceId())
				{
					_teleZone = zone;
					break;
				}
			}
		}
		return _teleZone;
	}
	
	public void oustAllPlayers()
	{
		getTeleZone().oustAllPlayers();
	}
	
	/**
	 * Get the objects distance to this castle
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
		if ((player.getClanId() != _ownerId) && !player.canOverrideCond(PlayerCondOverride.CASTLE_CONDITIONS))
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
	
	public void openCloseDoor(Player player, String doorName, bool open)
	{
		if ((player.getClanId() != _ownerId) && !player.canOverrideCond(PlayerCondOverride.CASTLE_CONDITIONS))
		{
			return;
		}
		
		Door door = getDoor(doorName);
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
		_function.clear();
	}
	
	// This method updates the castle tax rate
	public void setOwner(Clan clan)
	{
		// Remove old owner
		if ((_ownerId > 0) && ((clan == null) || (clan.getId() != _ownerId)))
		{
			Clan oldOwner = ClanTable.getInstance().getClan(getOwnerId()); // Try to find clan instance
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
					Player oldleader = oldOwner.getLeader().getPlayer();
					if ((oldleader != null) && (oldleader.getMountType() == MountType.WYVERN))
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
		if ((clan != null) && (clan.getFortId() > 0))
		{
			FortManager.getInstance().getFortByOwner(clan).removeOwner(true);
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
			clan.broadcastToOnlineMembers(new PledgeShowInfoUpdate(clan));
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
		_function.clear();
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
				door.setCurrentHp((isDoorWeak) ? (door.getMaxHp() / 2) : (door.getMaxHp()));
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps1 = con.prepareStatement("SELECT * FROM castle WHERE id = ?");
			PreparedStatement ps2 = con.prepareStatement("SELECT clan_id FROM clan_data WHERE hasCastle = ?");
			ps1.setInt(1, getResidenceId());
			{
				ResultSet rs = ps1.executeQuery();
				while (rs.next())
				{
					setName(rs.getString("name"));
					// _OwnerId = rs.getInt("ownerId");
					_siegeDate = Calendar.getInstance();
					_siegeDate.setTimeInMillis(rs.getLong("siegeDate"));
					_siegeTimeRegistrationEndDate = Calendar.getInstance();
					_siegeTimeRegistrationEndDate.setTimeInMillis(rs.getLong("regTimeEnd"));
					_isTimeRegistrationOver = rs.getBoolean("regTimeOver");
					_castleSide = Enum.valueOf(CastleSide.class, rs.getString("side"));
					_treasury = rs.getLong("treasury");
					_showNpcCrest = rs.getBoolean("showNpcCrest");
					_ticketBuyCount = rs.getInt("ticketBuyCount");
				}
			}
			
			ps2.setInt(1, getResidenceId());
			{
				ResultSet rs = ps2.executeQuery();
				while (rs.next())
				{
					_ownerId = rs.getInt("clan_id");
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: loadCastleData(): " + e);
		}
	}
	
	/** Load All Functions */
	private void loadFunctions()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM castle_functions WHERE castle_id = ?");
			ps.setInt(1, getResidenceId());

			{
				ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					_function.put(rs.getInt("type"), new CastleFunction(rs.getInt("type"), rs.getInt("lvl"), rs.getInt("lease"), 0, rs.getLong("rate"), rs.getLong("endTime"), true));
				}
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("DELETE FROM castle_functions WHERE castle_id=? AND type=?");
			ps.setInt(1, getResidenceId());
			ps.setInt(2, functionType);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: Castle.removeFunctions(int functionType): " + e);
		}
	}
	
	public bool updateFunctions(Player player, int type, int lvl, int lease, long rate, bool addNew)
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
			_function.put(type, new CastleFunction(type, lvl, lease, 0, rate, 0, false));
		}
		else if ((lvl == 0) && (lease == 0))
		{
			removeFunction(type);
		}
		else
		{
			int diffLease = lease - _function.get(type).getLease();
			if (diffLease > 0)
			{
				_function.remove(type);
				_function.put(type, new CastleFunction(type, lvl, lease, 0, rate, -1, false));
			}
			else
			{
				_function.get(type).setLease(lease);
				_function.get(type).setLvl(lvl);
				_function.get(type).dbSave();
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
			if ((door.getCastle() != null) && (door.getCastle().getResidenceId() == getResidenceId()))
			{
				_doors.add(door);
			}
		}
	}
	
	// This method loads castle door upgrade data from database
	private void loadDoorUpgrade()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM castle_doorupgrade WHERE castleId=?");
			ps.setInt(1, getResidenceId());
			{
				ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					setDoorUpgrade(rs.getInt("doorId"), rs.getInt("ratio"), false);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: loadCastleDoorUpgrade(): " + e);
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("DELETE FROM castle_doorupgrade WHERE castleId=?");
			ps.setInt(1, getResidenceId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: removeDoorUpgrade(): " + e);
		}
	}
	
	public void setDoorUpgrade(int doorId, int ratio, bool save)
	{
		Door door = (getDoors().isEmpty()) ? DoorData.getInstance().getDoor(doorId) : getDoor(doorId);
		if (door == null)
		{
			return;
		}
		
		door.getStat().setUpgradeHpRatio(ratio);
		door.setCurrentHp(door.getMaxHp());
		
		if (save)
		{
			try 
			{
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement ps =
					con.prepareStatement("REPLACE INTO castle_doorupgrade (doorId, ratio, castleId) values (?,?,?)");
				ps.setInt(1, doorId);
				ps.setInt(2, ratio);
				ps.setInt(3, getResidenceId());
				ps.execute();
			}
			catch (Exception e)
			{
				LOGGER.Warn("Exception: setDoorUpgrade(int doorId, int ratio, int castleId): " + e);
			}
		}
	}
	
	private void updateOwnerInDB(Clan clan)
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
			Connection con = DatabaseFactory.getConnection();
			// Need to remove has castle flag from clan_data, should be checked from castle table.
			{
				PreparedStatement ps = con.prepareStatement("UPDATE clan_data SET hasCastle = 0 WHERE hasCastle = ?");
				ps.setInt(1, getResidenceId());
				ps.execute();
			}

			{
				PreparedStatement ps = con.prepareStatement("UPDATE clan_data SET hasCastle = ? WHERE clan_id = ?");
				ps.setInt(1, getResidenceId());
				ps.setInt(2, _ownerId);
				ps.execute();
			}
			
			// Announce to clan members
			if (clan != null)
			{
				clan.setCastleId(getResidenceId()); // Set has castle flag for new owner
				clan.broadcastToOnlineMembers(new PledgeShowInfoUpdate(clan));
				clan.broadcastToOnlineMembers(new PlaySound(1, "Siege_Victory", 0, 0, 0, 0, 0));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: updateOwnerInDB(Pledge clan): " + e);
		}
	}
	
	public Door getDoor(int doorId)
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
	
	public Door getDoor(String doorName)
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
	
	public Clan getOwner()
	{
		return (_ownerId != 0) ? ClanTable.getInstance().getClan(_ownerId) : null;
	}
	
	public Siege getSiege()
	{
		if (_siege == null)
		{
			_siege = new Siege(this);
		}
		return _siege;
	}
	
	public Calendar getSiegeDate()
	{
		return _siegeDate;
	}
	
	public bool isTimeRegistrationOver()
	{
		return _isTimeRegistrationOver;
	}
	
	public void setTimeRegistrationOver(bool value)
	{
		_isTimeRegistrationOver = value;
	}
	
	public Calendar getTimeRegistrationOverDate()
	{
		if (_siegeTimeRegistrationEndDate == null)
		{
			_siegeTimeRegistrationEndDate = Calendar.getInstance();
		}
		return _siegeTimeRegistrationEndDate;
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
				Clan owner = ClanTable.getInstance().getClan(getOwnerId());
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
			Clan owner = ClanTable.getInstance().getClan(getOwnerId());
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("UPDATE castle SET showNpcCrest = ? WHERE id = ?");
			ps.setString(1, String.valueOf(_showNpcCrest));
			ps.setInt(2, getResidenceId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Info("Error saving showNpcCrest for castle " + getName() + ": " + e);
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("UPDATE castle SET ticketBuyCount = ? WHERE id = ?");
			ps.setInt(1, _ticketBuyCount);
			ps.setInt(2, getResidenceId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}
	
	public int getTrapUpgradeLevel(int towerIndex)
	{
		TowerSpawn spawn = SiegeManager.getInstance().getFlameTowers(getResidenceId()).get(towerIndex);
		return (spawn != null) ? spawn.getUpgradeLevel() : 0;
	}
	
	public void setTrapUpgrade(int towerIndex, int level, bool save)
	{
		if (save)
		{
			try 
			{
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement ps =
					con.prepareStatement(
						"REPLACE INTO castle_trapupgrade (castleId, towerIndex, level) values (?,?,?)");
				ps.setInt(1, getResidenceId());
				ps.setInt(2, towerIndex);
				ps.setInt(3, level);
				ps.execute();
			}
			catch (Exception e)
			{
				LOGGER.Warn("Exception: setTrapUpgradeLevel(int towerIndex, int level, int castleId): " + e);
			}
		}
		TowerSpawn spawn = SiegeManager.getInstance().getFlameTowers(getResidenceId()).get(towerIndex);
		if (spawn != null)
		{
			spawn.setUpgradeLevel(level);
		}
	}
	
	private void removeTrapUpgrade()
	{
		foreach (TowerSpawn ts in SiegeManager.getInstance().getFlameTowers(getResidenceId()))
		{
			ts.setUpgradeLevel(0);
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("DELETE FROM castle_trapupgrade WHERE castleId=?");
			ps.setInt(1, getResidenceId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: removeDoorUpgrade(): " + e);
		}
	}
	
	protected override void initResidenceZone()
	{
		foreach (CastleZone zone in ZoneManager.getInstance().getAllZones<CastleZone>())
		{
			if (zone.getResidenceId() == getResidenceId())
			{
				setResidenceZone(zone);
				break;
			}
		}
	}
	
	public void giveResidentialSkills(Player player)
	{
		base.giveResidentialSkills(player);
		Skill skill = _castleSide == CastleSide.DARK ? CommonSkill.ABILITY_OF_DARKNESS.getSkill() : CommonSkill.ABILITY_OF_LIGHT.getSkill();
		player.addSkill(skill);
	}
	
	public void removeResidentialSkills(Player player)
	{
		base.removeResidentialSkills(player);
		player.removeSkill(CommonSkill.ABILITY_OF_DARKNESS.getId());
		player.removeSkill(CommonSkill.ABILITY_OF_LIGHT.getId());
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
				spawn.setXYZ(holder);
				spawn.setHeading(holder.getHeading());
				Npc npc = spawn.doSpawn(false);
				spawn.stopRespawn();
				npc.broadcastInfo();
				_sideNpcs.add(npc);
			}
		}
	}
	
	public List<CastleSpawnHolder> getSideSpawns()
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("UPDATE castle SET side = ? WHERE id = ?");
			ps.setString(1, side.ToString());
			ps.setInt(2, getResidenceId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
		_castleSide = side;
		Broadcast.toAllOnlinePlayers(new ExCastleState(this));
		spawnSideNpcs();
	}
	
	public CastleSide getSide()
	{
		return _castleSide;
	}
}