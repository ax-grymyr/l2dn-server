using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Castle manor system.
 * @author malyelfik
 */
public class CastleManorManager: IXmlReader, IStorable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CastleManorManager));
	
	// SQL queries
	private const string INSERT_PRODUCT = "INSERT INTO castle_manor_production VALUES (?, ?, ?, ?, ?, ?)";
	private const string INSERT_CROP = "INSERT INTO castle_manor_procure VALUES (?, ?, ?, ?, ?, ?, ?)";
	
	// Current manor status
	private ManorMode _mode = ManorMode.APPROVED;
	// Temporary date
	private Calendar _nextModeChange = null;
	// Seeds holder
	private static readonly Map<int, Seed> _seeds = new();
	// Manor period settings
	private readonly Map<int, List<CropProcure>> _procure = new();
	private readonly Map<int, List<CropProcure>> _procureNext = new();
	private readonly Map<int, List<SeedProduction>> _production = new();
	private readonly Map<int, List<SeedProduction>> _productionNext = new();
	
	public CastleManorManager()
	{
		if (Config.ALLOW_MANOR)
		{
			load(); // Load seed data (XML)
			loadDb(); // Load castle manor data (DB)
			
			// Set mode and start timer
			Calendar currentTime = Calendar.getInstance();
			int hour = currentTime.get(Calendar.HOUR_OF_DAY);
			int min = currentTime.get(Calendar.MINUTE);
			int maintenanceMin = Config.ALT_MANOR_REFRESH_MIN + Config.ALT_MANOR_MAINTENANCE_MIN;
			if (((hour >= Config.ALT_MANOR_REFRESH_TIME) && (min >= maintenanceMin)) || (hour < Config.ALT_MANOR_APPROVE_TIME) || ((hour == Config.ALT_MANOR_APPROVE_TIME) && (min <= Config.ALT_MANOR_APPROVE_MIN)))
			{
				_mode = ManorMode.MODIFIABLE;
			}
			else if ((hour == Config.ALT_MANOR_REFRESH_TIME) && ((min >= Config.ALT_MANOR_REFRESH_MIN) && (min < maintenanceMin)))
			{
				_mode = ManorMode.MAINTENANCE;
			}
			
			// Schedule mode change
			scheduleModeChange();
			
			// Schedule autosave
			if (!Config.ALT_MANOR_SAVE_ALL_ACTIONS)
			{
				ThreadPool.scheduleAtFixedRate(this::storeMe, Config.ALT_MANOR_SAVE_PERIOD_RATE * 60 * 60 * 1000, Config.ALT_MANOR_SAVE_PERIOD_RATE * 60 * 60 * 1000);
			}
		}
		else
		{
			_mode = ManorMode.DISABLED;
			LOGGER.Info(GetType().Name +": Manor system is deactivated.");
		}
	}
	
	public void load()
	{
		parseDatapackFile("data/Seeds.xml");
		LOGGER.Info(GetType().Name +": Loaded " + _seeds.size() + " seeds.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		StatSet set;
		NamedNodeMap attrs;
		Node att;
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("castle".equalsIgnoreCase(d.getNodeName()))
					{
						int castleId = parseInteger(d.getAttributes(), "id");
						for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
						{
							if ("crop".equalsIgnoreCase(c.getNodeName()))
							{
								set = new StatSet();
								set.set("castleId", castleId);
								attrs = c.getAttributes();
								for (int i = 0; i < attrs.getLength(); i++)
								{
									att = attrs.item(i);
									set.set(att.getNodeName(), att.getNodeValue());
								}
								_seeds.put(set.getInt("seedId"), new Seed(set));
							}
						}
					}
				}
			}
		}
	}
	
	private void loadDb()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement stProduction = con.prepareStatement("SELECT * FROM castle_manor_production WHERE castle_id=?");
			PreparedStatement stProcure = con.prepareStatement("SELECT * FROM castle_manor_procure WHERE castle_id=?");
			foreach (Castle castle in CastleManager.getInstance().getCastles())
			{
				int castleId = castle.getResidenceId();
				
				// Clear params
				stProduction.clearParameters();
				stProcure.clearParameters();
				
				// Seed production
				List<SeedProduction> pCurrent = new();
				List<SeedProduction> pNext = new();
				stProduction.setInt(1, castleId);

				{
					ResultSet rs = stProduction.executeQuery();
					while (rs.next())
					{
						int seedId = rs.getInt("seed_id");
						if (_seeds.containsKey(seedId)) // Don't load unknown seeds
						{
							SeedProduction sp = new SeedProduction(seedId, rs.getLong("amount"), rs.getLong("price"), rs.getInt("start_amount"));
							if (rs.getBoolean("next_period"))
							{
								pNext.add(sp);
							}
							else
							{
								pCurrent.add(sp);
							}
						}
						else
						{
							LOGGER.Warn(GetType().Name + ": Unknown seed id: " + seedId + "!");
						}
					}
				}
				_production.put(castleId, pCurrent);
				_productionNext.put(castleId, pNext);
				
				// Seed procure
				List<CropProcure> current = new();
				List<CropProcure> next = new();
				stProcure.setInt(1, castleId);
				try
				{
					ResultSet rs = stProcure.executeQuery();
					Set<int> cropIds = getCropIds();
					while (rs.next())
					{
						int cropId = rs.getInt("crop_id");
						if (cropIds.Contains(cropId)) // Don't load unknown crops
						{
							CropProcure cp = new CropProcure(cropId, rs.getLong("amount"), rs.getInt("reward_type"), rs.getLong("start_amount"), rs.getLong("price"));
							if (rs.getBoolean("next_period"))
							{
								next.add(cp);
							}
							else
							{
								current.add(cp);
							}
						}
						else
						{
							LOGGER.Warn(GetType().Name + ": Unknown crop id: " + cropId + "!");
						}
					}
				}
				_procure.put(castleId, current);
				_procureNext.put(castleId, next);
			}
			LOGGER.Info(GetType().Name +": Manor data loaded.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Unable to load manor data! " + e);
		}
	}
	
	// -------------------------------------------------------
	// Manor methods
	// -------------------------------------------------------
	private void scheduleModeChange()
	{
		// Calculate next mode change
		_nextModeChange = Calendar.getInstance();
		_nextModeChange.set(Calendar.SECOND, 0);
		switch (_mode)
		{
			case ManorMode.MODIFIABLE:
			{
				_nextModeChange.set(Calendar.HOUR_OF_DAY, Config.ALT_MANOR_APPROVE_TIME);
				_nextModeChange.set(Calendar.MINUTE, Config.ALT_MANOR_APPROVE_MIN);
				if (_nextModeChange.before(Calendar.getInstance()))
				{
					_nextModeChange.add(Calendar.DATE, 1);
				}
				break;
			}
			case ManorMode.MAINTENANCE:
			{
				_nextModeChange.set(Calendar.HOUR_OF_DAY, Config.ALT_MANOR_REFRESH_TIME);
				_nextModeChange.set(Calendar.MINUTE, Config.ALT_MANOR_REFRESH_MIN + Config.ALT_MANOR_MAINTENANCE_MIN);
				break;
			}
			case ManorMode.APPROVED:
			{
				_nextModeChange.set(Calendar.HOUR_OF_DAY, Config.ALT_MANOR_REFRESH_TIME);
				_nextModeChange.set(Calendar.MINUTE, Config.ALT_MANOR_REFRESH_MIN);
				break;
			}
		}
		// Schedule mode change
		ThreadPool.schedule(this::changeMode, Math.Max(0, _nextModeChange.getTimeInMillis() - System.currentTimeMillis()));
	}
	
	public void changeMode()
	{
		switch (_mode)
		{
			case ManorMode.APPROVED:
			{
				// Change mode
				_mode = ManorMode.MAINTENANCE;
				
				// Update manor period
				foreach (Castle castle in CastleManager.getInstance().getCastles())
				{
					Clan owner = castle.getOwner();
					if (owner == null)
					{
						continue;
					}
					
					int castleId = castle.getResidenceId();
					ItemContainer cwh = owner.getWarehouse();
					foreach (CropProcure crop in _procure.get(castleId))
					{
						if (crop.getStartAmount() > 0)
						{
							// Adding bought crops to clan warehouse
							if (crop.getStartAmount() != crop.getAmount())
							{
								long count = (long) ((crop.getStartAmount() - crop.getAmount()) * 0.9);
								if ((count < 1) && (Rnd.get(99) < 90))
								{
									count = 1;
								}
								
								if (count > 0)
								{
									cwh.addItem("Manor", getSeedByCrop(crop.getId()).getMatureId(), count, null, null);
								}
							}
							// Reserved and not used money giving back to treasury
							if (crop.getAmount() > 0)
							{
								castle.addToTreasuryNoTax(crop.getAmount() * crop.getPrice());
							}
						}
					}
					
					// Change next period to current and prepare next period data
					List<SeedProduction> nextProduction = _productionNext.get(castleId);
					List<CropProcure> nextProcure = _procureNext.get(castleId);
					_production.put(castleId, nextProduction);
					_procure.put(castleId, nextProcure);
					
					if (castle.getTreasury() < getManorCost(castleId, false))
					{
						_productionNext.put(castleId, Collections.emptyList());
						_procureNext.put(castleId, Collections.emptyList());
					}
					else
					{
						List<SeedProduction> production = new();
						foreach (SeedProduction s in production)
						{
							s.setAmount(s.getStartAmount());
						}
						_productionNext.put(castleId, production);
						
						List<CropProcure> procure = new();
						foreach (CropProcure cr in procure)
						{
							cr.setAmount(cr.getStartAmount());
						}
						_procureNext.put(castleId, procure);
					}
				}
				
				// Save changes
				storeMe();
				break;
			}
			case ManorMode.MAINTENANCE:
			{
				// Notify clan leader about manor mode change
				foreach (Castle castle in CastleManager.getInstance().getCastles())
				{
					Clan owner = castle.getOwner();
					if (owner != null)
					{
						ClanMember clanLeader = owner.getLeader();
						if ((clanLeader != null) && clanLeader.isOnline())
						{
							clanLeader.getPlayer().sendPacket(SystemMessageId.THE_MANOR_INFORMATION_HAS_BEEN_UPDATED);
						}
					}
				}
				_mode = ManorMode.MODIFIABLE;
				break;
			}
			case ManorMode.MODIFIABLE:
			{
				_mode = ManorMode.APPROVED;
				foreach (Castle castle in CastleManager.getInstance().getCastles())
				{
					Clan owner = castle.getOwner();
					if (owner == null)
					{
						continue;
					}
					
					int slots = 0;
					int castleId = castle.getResidenceId();
					ItemContainer cwh = owner.getWarehouse();
					foreach (CropProcure crop in _procureNext.get(castleId))
					{
						if ((crop.getStartAmount() > 0) && (cwh.getAllItemsByItemId(getSeedByCrop(crop.getId()).getMatureId()) == null))
						{
							slots++;
						}
					}
					
					long manorCost = getManorCost(castleId, true);
					if (!cwh.validateCapacity(slots) && (castle.getTreasury() < manorCost))
					{
						_productionNext.get(castleId).Clear();
						_procureNext.get(castleId).Clear();
						
						// Notify clan leader
						ClanMember clanLeader = owner.getLeader();
						if ((clanLeader != null) && clanLeader.isOnline())
						{
							clanLeader.getPlayer().sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_FUNDS_IN_THE_CLAN_WAREHOUSE_FOR_THE_MANOR_TO_OPERATE);
						}
					}
					else
					{
						castle.addToTreasuryNoTax(-manorCost);
					}
				}
				
				// Store changes
				if (Config.ALT_MANOR_SAVE_ALL_ACTIONS)
				{
					storeMe();
				}
				break;
			}
		}
		scheduleModeChange();
	}
	
	public void setNextSeedProduction(List<SeedProduction> list, int castleId)
	{
		_productionNext.put(castleId, list);
		if (Config.ALT_MANOR_SAVE_ALL_ACTIONS)
		{
			try 
			{
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement dps = con.prepareStatement("DELETE FROM castle_manor_production WHERE castle_id = ? AND next_period = 1");
				PreparedStatement ips = con.prepareStatement(INSERT_PRODUCT);
				// Delete old data
				dps.setInt(1, castleId);
				dps.executeUpdate();
				
				// Insert new data
				if (!list.isEmpty())
				{
					foreach (SeedProduction sp in list)
					{
						ips.setInt(1, castleId);
						ips.setInt(2, sp.getId());
						ips.setLong(3, sp.getAmount());
						ips.setLong(4, sp.getStartAmount());
						ips.setLong(5, sp.getPrice());
						ips.setBoolean(6, true);
						ips.addBatch();
					}
					ips.executeBatch();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Unable to store manor data!" + e);
			}
		}
	}
	
	public void setNextCropProcure(List<CropProcure> list, int castleId)
	{
		_procureNext.put(castleId, list);
		if (Config.ALT_MANOR_SAVE_ALL_ACTIONS)
		{
			try 
			{
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement dps = con.prepareStatement("DELETE FROM castle_manor_procure WHERE castle_id = ? AND next_period = 1");
				PreparedStatement ips = con.prepareStatement(INSERT_CROP);
				// Delete old data
				dps.setInt(1, castleId);
				dps.executeUpdate();
				
				// Insert new data
				if (!list.isEmpty())
				{
					foreach (CropProcure cp in list)
					{
						ips.setInt(1, castleId);
						ips.setInt(2, cp.getId());
						ips.setLong(3, cp.getAmount());
						ips.setLong(4, cp.getStartAmount());
						ips.setLong(5, cp.getPrice());
						ips.setInt(6, cp.getReward());
						ips.setBoolean(7, true);
						ips.addBatch();
					}
					ips.executeBatch();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Unable to store manor data!" + e);
			}
		}
	}
	
	public void updateCurrentProduction(int castleId, Collection<SeedProduction> items)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(
				"UPDATE castle_manor_production SET amount = ? WHERE castle_id = ? AND seed_id = ? AND next_period = 0");
			foreach (SeedProduction sp in items)
			{
				ps.setLong(1, sp.getAmount());
				ps.setInt(2, castleId);
				ps.setInt(3, sp.getId());
				ps.addBatch();
			}
			ps.executeBatch();
		}
		catch (Exception e)
		{
			LOGGER.Info(GetType().Name + ": Unable to store manor data!" + e);
		}
	}
	
	public void updateCurrentProcure(int castleId, ICollection<CropProcure> items)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(
				"UPDATE castle_manor_procure SET amount = ? WHERE castle_id = ? AND crop_id = ? AND next_period = 0");
			foreach (CropProcure sp in items)
			{
				ps.setLong(1, sp.getAmount());
				ps.setInt(2, castleId);
				ps.setInt(3, sp.getId());
				ps.addBatch();
			}
			ps.executeBatch();
		}
		catch (Exception e)
		{
			LOGGER.Info(GetType().Name + ": Unable to store manor data!" + e);
		}
	}
	
	public List<SeedProduction> getSeedProduction(int castleId, bool nextPeriod)
	{
		return (nextPeriod) ? _productionNext.get(castleId) : _production.get(castleId);
	}
	
	public SeedProduction getSeedProduct(int castleId, int seedId, bool nextPeriod)
	{
		foreach (SeedProduction sp in getSeedProduction(castleId, nextPeriod))
		{
			if (sp.getId() == seedId)
			{
				return sp;
			}
		}
		return null;
	}
	
	public List<CropProcure> getCropProcure(int castleId, bool nextPeriod)
	{
		return (nextPeriod) ? _procureNext.get(castleId) : _procure.get(castleId);
	}
	
	public CropProcure getCropProcure(int castleId, int cropId, bool nextPeriod)
	{
		foreach (CropProcure cp in getCropProcure(castleId, nextPeriod))
		{
			if (cp.getId() == cropId)
			{
				return cp;
			}
		}
		return null;
	}
	
	public long getManorCost(int castleId, bool nextPeriod)
	{
		List<CropProcure> procure = getCropProcure(castleId, nextPeriod);
		List<SeedProduction> production = getSeedProduction(castleId, nextPeriod);
		long total = 0;
		foreach (SeedProduction seed in production)
		{
			Seed s = getSeed(seed.getId());
			total += (s == null) ? 1 : (s.getSeedReferencePrice() * seed.getStartAmount());
		}
		foreach (CropProcure crop in procure)
		{
			total += (crop.getPrice() * crop.getStartAmount());
		}
		return total;
	}
	
	public bool storeMe()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ds = con.prepareStatement("DELETE FROM castle_manor_production");
			PreparedStatement is1 = con.prepareStatement(INSERT_PRODUCT);
			PreparedStatement dp = con.prepareStatement("DELETE FROM castle_manor_procure");
			PreparedStatement ip = con.prepareStatement(INSERT_CROP);
			// Delete old seeds
			ds.executeUpdate();
			
			// Current production
			foreach (var entry in _production)
			{
				foreach (SeedProduction sp in entry.Value)
				{
					is1.setInt(1, entry.Key);
					is1.setInt(2, sp.getId());
					is1.setLong(3, sp.getAmount());
					is1.setLong(4, sp.getStartAmount());
					is1.setLong(5, sp.getPrice());
					is1.setBoolean(6, false);
					is1.addBatch();
				}
			}
			
			// Next production
			foreach (var entry in _productionNext)
			{
				foreach (SeedProduction sp in entry.Value)
				{
					is1.setInt(1, entry.Key);
					is1.setInt(2, sp.getId());
					is1.setLong(3, sp.getAmount());
					is1.setLong(4, sp.getStartAmount());
					is1.setLong(5, sp.getPrice());
					is1.setBoolean(6, true);
					is1.addBatch();
				}
			}
			
			// Execute production batch
			is1.executeBatch();
			
			// Delete old procure
			dp.executeUpdate();
			
			// Current procure
			foreach (var entry in _procure)
			{
				foreach (CropProcure cp in entry.Value)
				{
					ip.setInt(1, entry.Key);
					ip.setInt(2, cp.getId());
					ip.setLong(3, cp.getAmount());
					ip.setLong(4, cp.getStartAmount());
					ip.setLong(5, cp.getPrice());
					ip.setInt(6, cp.getReward());
					ip.setBoolean(7, false);
					ip.addBatch();
				}
			}
			
			// Next procure
			foreach (var entry in _procureNext)
			{
				foreach (CropProcure cp in entry.Value)
				{
					ip.setInt(1, entry.Key);
					ip.setInt(2, cp.getId());
					ip.setLong(3, cp.getAmount());
					ip.setLong(4, cp.getStartAmount());
					ip.setLong(5, cp.getPrice());
					ip.setInt(6, cp.getReward());
					ip.setBoolean(7, true);
					ip.addBatch();
				}
			}
			
			// Execute procure batch
			ip.executeBatch();
			
			return true;
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Unable to store manor data! " + e);
			return false;
		}
	}
	
	public void resetManorData(int castleId)
	{
		if (!Config.ALLOW_MANOR)
		{
			return;
		}
		
		_procure.get(castleId).Clear();
		_procureNext.get(castleId).Clear();
		_production.get(castleId).Clear();
		_productionNext.get(castleId).Clear();
		
		if (Config.ALT_MANOR_SAVE_ALL_ACTIONS)
		{
			try 
			{
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement ds = con.prepareStatement("DELETE FROM castle_manor_production WHERE castle_id = ?");
				PreparedStatement dc = con.prepareStatement("DELETE FROM castle_manor_procure WHERE castle_id = ?");
				// Delete seeds
				ds.setInt(1, castleId);
				ds.executeUpdate();
				
				// Delete procure
				dc.setInt(1, castleId);
				dc.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Unable to store manor data!" + e);
			}
		}
	}
	
	public bool isUnderMaintenance()
	{
		return _mode == ManorMode.MAINTENANCE;
	}
	
	public bool isManorApproved()
	{
		return _mode == ManorMode.APPROVED;
	}
	
	public bool isModifiablePeriod()
	{
		return _mode == ManorMode.MODIFIABLE;
	}
	
	public String getCurrentModeName()
	{
		return _mode.ToString();
	}
	
	public String getNextModeChange()
	{
		return new SimpleDateFormat("dd/MM HH:mm:ss").format(_nextModeChange.getTime());
	}
	
	// -------------------------------------------------------
	// Seed methods
	// -------------------------------------------------------
	public List<Seed> getCrops()
	{
		List<Seed> seeds = new();
		List<int> cropIds = new();
		foreach (Seed seed in _seeds.values())
		{
			if (!cropIds.Contains(seed.getCropId()))
			{
				seeds.add(seed);
				cropIds.add(seed.getCropId());
			}
		}
		cropIds.Clear();
		return seeds;
	}
	
	public Set<Seed> getSeedsForCastle(int castleId)
	{
		Set<Seed> result = new();
		foreach (Seed seed in _seeds.values())
		{
			if (seed.getCastleId() == castleId)
			{
				result.add(seed);
			}
		}
		return result;
	}
	
	public Set<int> getSeedIds()
	{
		return _seeds.Keys;
	}
	
	public Set<int> getCropIds()
	{
		Set<int> result = new();
		foreach (Seed seed in _seeds.values())
		{
			result.add(seed.getCropId());
		}
		return result;
	}
	
	public Seed getSeed(int seedId)
	{
		return _seeds.get(seedId);
	}
	
	public Seed getSeedByCrop(int cropId, int castleId)
	{
		foreach (Seed s in getSeedsForCastle(castleId))
		{
			if (s.getCropId() == cropId)
			{
				return s;
			}
		}
		return null;
	}
	
	public Seed getSeedByCrop(int cropId)
	{
		foreach (Seed s in _seeds.values())
		{
			if (s.getCropId() == cropId)
			{
				return s;
			}
		}
		return null;
	}
	
	// -------------------------------------------------------
	// Static methods
	// -------------------------------------------------------
	public static CastleManorManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CastleManorManager INSTANCE = new CastleManorManager();
	}
}