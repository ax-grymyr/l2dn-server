using System.Runtime.InteropServices.JavaScript;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers.Tasks;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Grand Boss manager.
 * @author DaRkRaGe Revised by Emperorc
 */
public class GrandBossManager: IStorable
{
	// SQL queries
	private const string UPDATE_GRAND_BOSS_DATA = "UPDATE grandboss_data set loc_x = ?, loc_y = ?, loc_z = ?, heading = ?, respawn_time = ?, currentHP = ?, currentMP = ?, status = ? where boss_id = ?";
	private const string UPDATE_GRAND_BOSS_DATA2 = "UPDATE grandboss_data set status = ? where boss_id = ?";
	
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
			using GameServerDbContext ctx = new();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery("SELECT * from grandboss_data ORDER BY boss_id");
			// Read all info from DB, and store it for AI to read and decide what to do faster than accessing DB in real time
			while (rs.next())
			{
				int bossId = rs.getInt("boss_id");
				if (NpcData.getInstance().getTemplate(bossId) != null)
				{
					StatSet info = new StatSet();
					info.set("loc_x", rs.getInt("loc_x"));
					info.set("loc_y", rs.getInt("loc_y"));
					info.set("loc_z", rs.getInt("loc_z"));
					info.set("heading", rs.getInt("heading"));
					info.set("respawn_time", rs.getLong("respawn_time"));
					info.set("currentHP", rs.getDouble("currentHP"));
					info.set("currentMP", rs.getDouble("currentMP"));
					int status = rs.getInt("status");
					_bossStatus.put(bossId, status);
					_storedInfo.put(bossId, info);
					LOGGER.Info(GetType().Name +": " + NpcData.getInstance().getTemplate(bossId).getName() + "(" + bossId + ") status is " + status);
					if (status > 0)
					{
						LOGGER.Info(GetType().Name +": Next spawn date of " + NpcData.getInstance().getTemplate(bossId).getName() + " is " + new JSType.Date(info.getLong("respawn_time")));
					}
				}
				else
				{
					LOGGER.Warn(GetType().Name + ": Could not find GrandBoss NPC template for " + bossId);
				}
			}
			LOGGER.Info(GetType().Name +": Loaded " + _storedInfo.size() + " instances.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while initializing GrandBossManager: " + e);
		}
		ThreadPool.scheduleAtFixedRate(new GrandBossManagerStoreTask(), 5 * 60 * 1000, 5 * 60 * 1000);
	}
	
	public int getStatus(int bossId)
	{
		if (!_bossStatus.containsKey(bossId))
		{
			return -1;
		}
		return _bossStatus.get(bossId);
	}
	
	public void setStatus(int bossId, int status)
	{
		_bossStatus.put(bossId, status);
		LOGGER.Info(GetType().Name +": Updated " + NpcData.getInstance().getTemplate(bossId).getName() + "(" + bossId + ") status to " + status + ".");
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
			_bosses.put(boss.getId(), boss);
		}
	}
	
	public GrandBoss getBoss(int bossId)
	{
		return _bosses.get(bossId);
	}
	
	public StatSet getStatSet(int bossId)
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
			using GameServerDbContext ctx = new();
			foreach (var e in _storedInfo)
			{
				GrandBoss boss = _bosses.get(e.Key);
				StatSet info = e.Value;
				if ((boss == null) || (info == null))
				{

					{
						PreparedStatement update = con.prepareStatement(UPDATE_GRAND_BOSS_DATA2);
						update.setInt(1, _bossStatus.get(e.Key));
						update.setInt(2, e.Key);
						update.executeUpdate();
						update.clearParameters();
					}
				}
				else
				{
					{
						PreparedStatement update = con.prepareStatement(UPDATE_GRAND_BOSS_DATA);
						update.setInt(1, boss.getX());
						update.setInt(2, boss.getY());
						update.setInt(3, boss.getZ());
						update.setInt(4, boss.getHeading());
						update.setLong(5, info.getLong("respawn_time"));
						double hp = boss.getCurrentHp();
						double mp = boss.getCurrentMp();
						if (boss.isDead())
						{
							hp = boss.getMaxHp();
							mp = boss.getMaxMp();
						}
						update.setDouble(6, hp);
						update.setDouble(7, mp);
						update.setInt(8, _bossStatus.get(e.Key));
						update.setInt(9, e.Key);
						update.executeUpdate();
						update.clearParameters();
					}
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
			using GameServerDbContext ctx = new();
			GrandBoss boss = _bosses.get(bossId);
			StatSet info = _storedInfo.get(bossId);
			if (statusOnly || (boss == null) || (info == null))
			{

				{
					PreparedStatement ps = con.prepareStatement(UPDATE_GRAND_BOSS_DATA2);
					ps.setInt(1, _bossStatus.get(bossId));
					ps.setInt(2, bossId);
					ps.executeUpdate();
				}
			}
			else
			{
				{
					PreparedStatement ps = con.prepareStatement(UPDATE_GRAND_BOSS_DATA);
					ps.setInt(1, boss.getX());
					ps.setInt(2, boss.getY());
					ps.setInt(3, boss.getZ());
					ps.setInt(4, boss.getHeading());
					ps.setLong(5, info.getLong("respawn_time"));
					double hp = boss.getCurrentHp();
					double mp = boss.getCurrentMp();
					if (boss.isDead())
					{
						hp = boss.getMaxHp();
						mp = boss.getMaxMp();
					}
					ps.setDouble(6, hp);
					ps.setDouble(7, mp);
					ps.setInt(8, _bossStatus.get(bossId));
					ps.setInt(9, bossId);
					ps.executeUpdate();
				}
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
		
		_bosses.clear();
		_storedInfo.clear();
		_bossStatus.clear();
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