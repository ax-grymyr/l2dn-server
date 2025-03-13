using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Castle manor system.
 * @author malyelfik
 */
public class CastleManorManager: DataReaderBase, IStorable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CastleManorManager));

	// Current manor status
	private ManorMode _mode = ManorMode.APPROVED;
	// Temporary date
	private DateTime _nextModeChange = DateTime.MinValue;
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
			DateTime currentTime = DateTime.Now;
			int hour = currentTime.Hour;
			int min = currentTime.Minute;
			int maintenanceMin = Config.ALT_MANOR_REFRESH_MIN + Config.ALT_MANOR_MAINTENANCE_MIN;
			if ((hour >= Config.ALT_MANOR_REFRESH_TIME && min >= maintenanceMin) || hour < Config.ALT_MANOR_APPROVE_TIME || (hour == Config.ALT_MANOR_APPROVE_TIME && min <= Config.ALT_MANOR_APPROVE_MIN))
			{
				_mode = ManorMode.MODIFIABLE;
			}
			else if (hour == Config.ALT_MANOR_REFRESH_TIME && min >= Config.ALT_MANOR_REFRESH_MIN && min < maintenanceMin)
			{
				_mode = ManorMode.MAINTENANCE;
			}

			// Schedule mode change
			scheduleModeChange();

			// Schedule autosave
			if (!Config.ALT_MANOR_SAVE_ALL_ACTIONS)
			{
				ThreadPool.scheduleAtFixedRate(() => storeMe(), Config.ALT_MANOR_SAVE_PERIOD_RATE * 60 * 60 * 1000,
					Config.ALT_MANOR_SAVE_PERIOD_RATE * 60 * 60 * 1000);
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
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "Seeds.xml");
		document.Elements("list").Elements("castle").ForEach(parseElement);

		LOGGER.Info(GetType().Name +": Loaded " + _seeds.Count + " seeds.");
	}

	private void parseElement(XElement element)
	{
		int castleId = element.GetAttributeValueAsInt32("id");
		element.Elements("crop").ForEach(el =>
		{
			StatSet set = new StatSet(el);
			set.set("castleId", castleId);
			_seeds.put(set.getInt("seedId"), new Seed(set));
		});
	}

	private void loadDb()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			foreach (Castle castle in CastleManager.getInstance().getCastles())
			{
				int castleId = castle.getResidenceId();

				// Seed production
				List<SeedProduction> pCurrent = new();
				List<SeedProduction> pNext = new();
				foreach(DbCastleManorProduction production in ctx.CastleManorProduction.Where(p => p.CastleId == castleId))
				{
					int seedId = production.SeedId;
					if (_seeds.ContainsKey(seedId)) // Don't load unknown seeds
					{
						SeedProduction sp = new SeedProduction(seedId, production.Amount, production.Price, production.StartAmount);
						if (production.NextPeriod)
						{
							pNext.Add(sp);
						}
						else
						{
							pCurrent.Add(sp);
						}
					}
					else
					{
						LOGGER.Warn(GetType().Name + ": Unknown seed id: " + seedId + "!");
					}
				}

				_production.put(castleId, pCurrent);
				_productionNext.put(castleId, pNext);

				// Seed procure
				List<CropProcure> current = new();
				List<CropProcure> next = new();

				Set<int> cropIds = getCropIds();
				foreach (CastleManorProcure procure in ctx.CastleManorProcure.Where(p => p.CastleId == castleId))
				{
					int cropId = procure.CropId;
					if (cropIds.Contains(cropId)) // Don't load unknown crops
					{
						CropProcure cp = new CropProcure(cropId, procure.Amount, procure.RewardType,
							procure.StartAmount, procure.Price);

						if (procure.NextPeriod)
						{
							next.Add(cp);
						}
						else
						{
							current.Add(cp);
						}
					}
					else
					{
						LOGGER.Warn(GetType().Name + ": Unknown crop id: " + cropId + "!");
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
		DateTime time = DateTime.Now;
		time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);
		switch (_mode)
		{
			case ManorMode.MODIFIABLE:
			{
				time = new DateTime(time.Year, time.Month, time.Day, Config.ALT_MANOR_APPROVE_TIME, Config.ALT_MANOR_APPROVE_MIN, 0);
				if (time < DateTime.Now)
					time = time.AddDays(1);

				break;
			}
			case ManorMode.MAINTENANCE:
			{
				time = new DateTime(time.Year, time.Month, time.Day, Config.ALT_MANOR_REFRESH_TIME,
					Config.ALT_MANOR_REFRESH_MIN + Config.ALT_MANOR_MAINTENANCE_MIN, 0);

				break;
			}
			case ManorMode.APPROVED:
			{
				time = new DateTime(time.Year, time.Month, time.Day, Config.ALT_MANOR_REFRESH_TIME, Config.ALT_MANOR_REFRESH_MIN, 0);
				break;
			}
		}

		_nextModeChange = time;

		TimeSpan delay = time - DateTime.Now;
		if (delay < TimeSpan.Zero)
			delay = TimeSpan.Zero;

		// Schedule mode change
		ThreadPool.schedule(changeMode, delay);
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
					Clan? owner = castle.getOwner();
					if (owner == null)
					{
						continue;
					}

					int castleId = castle.getResidenceId();
					ItemContainer cwh = owner.getWarehouse();
					foreach (CropProcure crop in _procure[castleId])
					{
						if (crop.getStartAmount() > 0)
						{
							// Adding bought crops to clan warehouse
							if (crop.getStartAmount() != crop.getAmount())
							{
								long count = (long) ((crop.getStartAmount() - crop.getAmount()) * 0.9);
								if (count < 1 && Rnd.get(99) < 90)
								{
									count = 1;
								}

								if (count > 0)
                                {
                                    Seed? seed = getSeedByCrop(crop.getId());
                                    if (seed != null) // TODO: exception
									    cwh.addItem("Manor", seed.getMatureId(), count, null, null);
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
					List<SeedProduction> nextProduction = _productionNext[castleId];
					List<CropProcure> nextProcure = _procureNext[castleId];
					_production.put(castleId, nextProduction);
					_procure.put(castleId, nextProcure);

					if (castle.getTreasury() < getManorCost(castleId, false))
					{
						_productionNext.put(castleId, new());
						_procureNext.put(castleId, new());
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
					Clan? owner = castle.getOwner();
					if (owner != null)
					{
						ClanMember clanLeader = owner.getLeader();
                        Player? clanLeaderPlayer = clanLeader.getPlayer();
						if (clanLeader != null && clanLeader.isOnline() && clanLeaderPlayer != null)
						{
                            clanLeaderPlayer.sendPacket(SystemMessageId.THE_MANOR_INFORMATION_HAS_BEEN_UPDATED);
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
					Clan? owner = castle.getOwner();
					if (owner == null)
					{
						continue;
					}

					int slots = 0;
					int castleId = castle.getResidenceId();
					ItemContainer cwh = owner.getWarehouse();
					foreach (CropProcure crop in _procureNext[castleId])
                    {
                        Seed? seed = getSeedByCrop(crop.getId());
                        if (seed == null)
                            continue;

						if (crop.getStartAmount() > 0 && cwh.getAllItemsByItemId(seed.getMatureId()) == null)
						{
							slots++;
						}
					}

					long manorCost = getManorCost(castleId, true);
					if (!cwh.validateCapacity(slots) && castle.getTreasury() < manorCost)
					{
						_productionNext.get(castleId)?.Clear();
						_procureNext.get(castleId)?.Clear();

						// Notify clan leader
						ClanMember clanLeader = owner.getLeader();
                        Player? clanLeaderPlayer = clanLeader.getPlayer();
						if (clanLeader != null && clanLeader.isOnline() && clanLeaderPlayer != null)
						{
                            clanLeaderPlayer.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_FUNDS_IN_THE_CLAN_WAREHOUSE_FOR_THE_MANOR_TO_OPERATE);
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
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

				// Delete old data
				ctx.CastleManorProduction.Where(p => p.CastleId == castleId && p.NextPeriod).ExecuteDelete();

				// Insert new data
				if (list.Count != 0)
				{
					ctx.CastleManorProduction.AddRange(list.Select(sp => new DbCastleManorProduction()
					{
						CastleId = (short)castleId,
						SeedId = sp.getId(),
						Amount = sp.getAmount(),
						StartAmount = sp.getStartAmount(),
						Price = sp.getPrice(),
						NextPeriod = true
					}));

					ctx.SaveChanges();
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
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

				// Delete old data
				ctx.CastleManorProcure.Where(p => p.CastleId == castleId && p.NextPeriod).ExecuteDelete();

				// Insert new data
				if (list.Count != 0)
				{
					ctx.CastleManorProcure.AddRange(list.Select(cp => new CastleManorProcure()
					{
						CastleId = (short)castleId,
						CropId = cp.getId(),
						Amount = cp.getAmount(),
						StartAmount = cp.getStartAmount(),
						RewardType = cp.getReward(),
						Price = cp.getPrice(),
						NextPeriod = true
					}));

					ctx.SaveChanges();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Unable to store manor data!" + e);
			}
		}
	}

	public void updateCurrentProduction(int castleId, ICollection<SeedProduction> items)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (SeedProduction sp in items)
			{
				int seedId = sp.getId();
				long amount = sp.getAmount();
				ctx.CastleManorProduction.Where(p => p.CastleId == castleId && p.SeedId == seedId && !p.NextPeriod)
					.ExecuteUpdate(s => s.SetProperty(p => p.Amount, amount));
			}
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (CropProcure sp in items)
			{
				int seedId = sp.getId();
				long amount = sp.getAmount();
				ctx.CastleManorProcure.Where(p => p.CastleId == castleId && p.CropId == seedId && !p.NextPeriod)
					.ExecuteUpdate(s => s.SetProperty(p => p.Amount, amount));
			}
		}
		catch (Exception e)
		{
			LOGGER.Info(GetType().Name + ": Unable to store manor data!" + e);
		}
	}

	public List<SeedProduction> getSeedProduction(int castleId, bool nextPeriod)
	{
		return nextPeriod ? _productionNext[castleId] : _production[castleId];
	}

	public SeedProduction? getSeedProduct(int castleId, int seedId, bool nextPeriod)
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
		return nextPeriod ? _procureNext[castleId] : _procure[castleId];
	}

	public CropProcure? getCropProcure(int castleId, int cropId, bool nextPeriod)
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
			Seed? s = getSeed(seed.getId());
			total += s == null ? 1 : s.getSeedReferencePrice() * seed.getStartAmount();
		}
		foreach (CropProcure crop in procure)
		{
			total += crop.getPrice() * crop.getStartAmount();
		}
		return total;
	}

	public bool storeMe()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			// Delete old seeds
			ctx.CastleManorProduction.ExecuteDelete();

			// Delete old procure
			ctx.CastleManorProcure.ExecuteDelete();

			// Current production
			IEnumerable<(int CastleId, bool NextPeriod, SeedProduction Production)> currentProduction =
				_production.SelectMany(kvp =>
					kvp.Value.Select(sp => (CastleId: kvp.Key, NextPeriod: false, Production: sp)));

			// Next production
			IEnumerable<(int CastleId, bool NextPeriod, SeedProduction Production)> nextProduction =
				_productionNext.SelectMany(kvp =>
					kvp.Value.Select(sp => (CastleId: kvp.Key, NextPeriod: true, Production: sp)));

			ctx.CastleManorProduction.AddRange(currentProduction.Concat(nextProduction).Select(t => new DbCastleManorProduction
			{
				CastleId = (short)t.CastleId,
				SeedId = t.Production.getId(),
				Amount = t.Production.getAmount(),
				StartAmount = t.Production.getStartAmount(),
				Price = t.Production.getPrice(),
				NextPeriod = t.NextPeriod
			}));

			// Current procure
			IEnumerable<(int CastleId, bool NextPeriod, CropProcure Procure)> currentProcure =
				_procure.SelectMany(kvp =>
					kvp.Value.Select(sp => (CastleId: kvp.Key, NextPeriod: false, Procure: sp)));

			// Next procure
			IEnumerable<(int CastleId, bool NextPeriod, CropProcure Procure)> nextProcure =
				_procureNext.SelectMany(kvp =>
					kvp.Value.Select(sp => (CastleId: kvp.Key, NextPeriod: true, Procure: sp)));

			ctx.CastleManorProcure.AddRange(currentProcure.Concat(nextProcure).Select(t => new CastleManorProcure
			{
				CastleId = (short)t.CastleId,
				CropId = t.Procure.getId(),
				Amount = t.Procure.getAmount(),
				StartAmount = t.Procure.getStartAmount(),
				Price = t.Procure.getPrice(),
				RewardType = t.Procure.getReward(),
				NextPeriod = t.NextPeriod
			}));

			// Execute procure batch
			ctx.SaveChanges();

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

		_procure.get(castleId)?.Clear();
		_procureNext.get(castleId)?.Clear();
		_production.get(castleId)?.Clear();
		_productionNext.get(castleId)?.Clear();

		if (Config.ALT_MANOR_SAVE_ALL_ACTIONS)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

				// Delete seeds
				ctx.CastleManorProduction.Where(p => p.CastleId == castleId).ExecuteDelete();

				// Delete procure
				ctx.CastleManorProcure.Where(p => p.CastleId == castleId).ExecuteDelete();
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

	public string getCurrentModeName()
	{
		return _mode.ToString();
	}

	public string getNextModeChange()
	{
		return _nextModeChange.ToString("dd/MM HH:mm:ss");
	}

	// -------------------------------------------------------
	// Seed methods
	// -------------------------------------------------------
	public List<Seed> getCrops()
	{
		List<Seed> seeds = new();
		List<int> cropIds = new();
		foreach (Seed seed in _seeds.Values)
		{
			if (!cropIds.Contains(seed.getCropId()))
			{
				seeds.Add(seed);
				cropIds.Add(seed.getCropId());
			}
		}
		cropIds.Clear();
		return seeds;
	}

	public Set<Seed> getSeedsForCastle(int castleId)
	{
		Set<Seed> result = new();
		foreach (Seed seed in _seeds.Values)
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
		Set<int> set = new();
		set.addAll(_seeds.Keys);
		return set;
	}

	public Set<int> getCropIds()
	{
		Set<int> result = new();
		foreach (Seed seed in _seeds.Values)
		{
			result.add(seed.getCropId());
		}
		return result;
	}

	public Seed? getSeed(int seedId)
	{
		return _seeds.get(seedId);
	}

	public Seed? getSeedByCrop(int cropId, int castleId)
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

	public Seed? getSeedByCrop(int cropId)
	{
		foreach (Seed s in _seeds.Values)
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