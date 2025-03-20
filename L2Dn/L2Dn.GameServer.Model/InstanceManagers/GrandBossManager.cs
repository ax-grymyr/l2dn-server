using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.InstanceManagers.Tasks;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Grand Boss manager.
 * @author DaRkRaGe Revised by Emperorc
 */
public class GrandBossManager: IStorable
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(GrandBossManager));
	protected static Map<int, GrandBoss> _bosses = new();
	protected static Map<int, StatSet> _storedInfo = new();
	private readonly Map<int, int> _bossStatus = new();

	protected GrandBossManager()
	{
		init();
	}

	private void init()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			// Read all info from DB, and store it for AI to read and decide what to do faster than accessing DB in real time
			foreach (DbGrandBoss boss in ctx.GrandBosses)
			{
				int bossId = boss.Id;
                NpcTemplate? bossTemplate = NpcData.getInstance().getTemplate(bossId);
				if (bossTemplate != null)
				{
					StatSet info = new StatSet();
					info.set("loc_x", boss.X);
					info.set("loc_y", boss.Y);
					info.set("loc_z", boss.Z);
					info.set("heading", boss.Heading);
					info.set("respawn_time", boss.RespawnTime);
					info.set("currentHP", boss.CurrentHp);
					info.set("currentMP", boss.CurrentMp);
					int status = boss.Status;
					_bossStatus.put(bossId, status);
					_storedInfo.put(bossId, info);
                    LOGGER.Info($"{GetType().Name}: {bossTemplate.getName()}({bossId}) status is {status}");

					if (status > 0)
                    {
                        LOGGER.Info($"{GetType().Name}: Next spawn date of {bossTemplate.getName()} is {boss.RespawnTime}");
                    }
				}
				else
				{
					LOGGER.Error(GetType().Name + ": Could not find GrandBoss NPC template for " + bossId);
				}
			}

			LOGGER.Info(GetType().Name + ": Loaded " + _storedInfo.Count + " instances.");
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error while initializing GrandBossManager: " + e);
		}

		ThreadPool.scheduleAtFixedRate(new GrandBossManagerStoreTask(), 5 * 60 * 1000, 5 * 60 * 1000);
	}

	public int getStatus(int bossId)
	{
		return _bossStatus.GetValueOrDefault(bossId, -1);
	}

	public void setStatus(int bossId, int status)
	{
        NpcTemplate? bossTemplate = NpcData.getInstance().getTemplate(bossId);
        if (bossTemplate == null)
        {
            LOGGER.Warn(GetType().Name + ": Could not find GrandBoss NPC template for " + bossId);
            return;
        }

		_bossStatus.put(bossId, status);
		LOGGER.Info(GetType().Name +": Updated " + bossTemplate.getName() + "(" + bossId + ") status to " + status + ".");
		updateDb(bossId, true);
	}

	/**
	 * Adds a GrandBoss to the list of bosses.
	 * @param boss
	 */
	public void addBoss(GrandBoss boss)
	{
		if (boss != null)
		{
			_bosses.put(boss.Id, boss);
		}
	}

	public GrandBoss? getBoss(int bossId)
	{
		return _bosses.get(bossId);
	}

	public StatSet? getStatSet(int bossId)
	{
		return _storedInfo.get(bossId);
	}

	public void setStatSet(int bossId, StatSet info)
	{
		_storedInfo.put(bossId, info);
		updateDb(bossId, false);
	}

	public bool storeMe()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (var e in _storedInfo)
			{
				int bossId = e.Key;
				GrandBoss? boss = _bosses.get(bossId);
				StatSet info = e.Value;
				int status = _bossStatus.get(bossId);
				if (boss == null || info == null)
				{
					ctx.GrandBosses.Where(b => b.Id == bossId).ExecuteUpdate(s => s.SetProperty(b => b.Status, status));
				}
				else
				{
					DateTime respawnTime = info.getDateTime("respawn_time");
					double hp = boss.isDead() ? boss.getMaxHp() : boss.getCurrentHp();
					double mp = boss.isDead() ? boss.getMaxMp() : boss.getCurrentMp();
					ctx.GrandBosses.Where(b => b.Id == bossId).ExecuteUpdate(s =>
						s.SetProperty(b => b.Status, status).SetProperty(b => b.X, boss.getX())
							.SetProperty(b => b.Y, boss.getY()).SetProperty(b => b.Z, boss.getZ())
							.SetProperty(b => b.Heading, boss.getHeading()).SetProperty(b => b.RespawnTime, respawnTime)
							.SetProperty(b => b.CurrentHp, hp).SetProperty(b => b.CurrentMp, mp));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Couldn't store grandbosses to database: " + e);
			return false;
		}
		return true;
	}

	private void updateDb(int bossId, bool statusOnly)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			GrandBoss? boss = _bosses.get(bossId);
			StatSet? info = _storedInfo.get(bossId);
			int status = _bossStatus.get(bossId);
			if (statusOnly || boss == null || info == null)
			{
				ctx.GrandBosses.Where(b => b.Id == bossId).ExecuteUpdate(s => s.SetProperty(b => b.Status, status));
			}
			else
			{
				DateTime respawnTime = info.getDateTime("respawn_time");
				double hp = boss.isDead() ? boss.getMaxHp() : boss.getCurrentHp();
				double mp = boss.isDead() ? boss.getMaxMp() : boss.getCurrentMp();
				ctx.GrandBosses.Where(b => b.Id == bossId).ExecuteUpdate(s =>
					s.SetProperty(b => b.Status, status).SetProperty(b => b.X, boss.getX())
						.SetProperty(b => b.Y, boss.getY()).SetProperty(b => b.Z, boss.getZ())
						.SetProperty(b => b.Heading, boss.getHeading()).SetProperty(b => b.RespawnTime, respawnTime)
						.SetProperty(b => b.CurrentHp, hp).SetProperty(b => b.CurrentMp, mp));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Couldn't update grandbosses to database:" + e);
		}
	}

	/**
	 * Saves all Grand Boss info and then clears all info from memory, including all schedules.
	 */
	public void cleanUp()
	{
		storeMe();

		_bosses.Clear();
		_storedInfo.Clear();
		_bossStatus.Clear();
	}

	/**
	 * Gets the single instance of {@code GrandBossManager}.
	 * @return single instance of {@code GrandBossManager}
	 */
	public static GrandBossManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly GrandBossManager INSTANCE = new GrandBossManager();
	}
}