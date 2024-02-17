using System.Runtime.InteropServices.JavaScript;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Database spawn manager.
 * @author godson, UnAfraid
 */
public class DBSpawnManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(DBSpawnManager));
	
	protected readonly Map<int, Npc> _npcs = new();
	protected readonly Map<int, Spawn> _spawns = new();
	protected readonly Map<int, StatSet> _storedInfo = new();
	protected readonly Map<int, ScheduledFuture> _schedules = new();
	
	/**
	 * Instantiates a new raid npc spawn manager.
	 */
	protected DBSpawnManager()
	{
		load();
	}
	
	/**
	 * Load.
	 */
	public void load()
	{
		if (Config.ALT_DEV_NO_SPAWNS)
		{
			return;
		}
		
		if (!_spawns.isEmpty())
		{
			foreach (Spawn spawn in _spawns.values())
			{
				deleteSpawn(spawn, false);
			}
		}
		
		_npcs.clear();
		_spawns.clear();
		_storedInfo.clear();
		_schedules.clear();
		
		try 
		{
			using GameServerDbContext ctx = new();
			foreach (NpcRespawn record in ctx.NpcRespawns)
			{
				int id = record.Id;
				NpcTemplate template = getValidTemplate(id);
				if (template != null)
				{
					Spawn spawn = new Spawn(template);
					spawn.setXYZ(record.X, record.Y, record.Z);
					spawn.setAmount(1);
					spawn.setHeading(record.Heading);
					
					List<NpcSpawnTemplate> spawns = SpawnData.getInstance().getNpcSpawns(npc => (npc.getId() == template.getId()) && npc.hasDBSave());
					if (spawns.isEmpty())
					{
						LOGGER.Warn(GetType().Name + ": Couldn't find spawn declaration for npc: " + template.getId() + " - " + template.getName());
						deleteSpawn(spawn, true);
						continue;
					}
					
					if (spawns.size() > 1)
					{
						LOGGER.Warn(GetType().Name + ": Found multiple database spawns for npc: " + template.getId() + " - " + template.getName() + " " + spawns);
						continue;
					}
					
					NpcSpawnTemplate spawnTemplate = spawns.get(0);
					spawn.setSpawnTemplate(spawnTemplate);
					
					int respawn = 0;
					int respawnRandom = 0;
					SchedulingPattern respawnPattern = null;
					if (spawnTemplate.getRespawnTime() != null)
					{
						respawn = (int) spawnTemplate.getRespawnTime().getSeconds();
					}
					if (spawnTemplate.getRespawnTimeRandom() != null)
					{
						respawnRandom = (int) spawnTemplate.getRespawnTimeRandom().getSeconds();
					}
					if (spawnTemplate.getRespawnPattern() != null)
					{
						respawnPattern = spawnTemplate.getRespawnPattern();
					}
					
					if ((respawn > 0) || (respawnPattern != null))
					{
						spawn.setRespawnDelay(respawn, respawnRandom);
						spawn.setRespawnPattern(respawnPattern);
						spawn.startRespawn();
					}
					else
					{
						spawn.stopRespawn();
						LOGGER.Warn(GetType().Name + ": Found database spawns without respawn for npc: " + template.getId() + " - " + template.getName() + " " + spawnTemplate);
						continue;
					}
					
					addNewSpawn(spawn, record.RespawnTime, record.CurrentHp, record.CurrentMp, false);
				}
				else
				{
					LOGGER.Error(GetType().Name + ": Could not load npc #" + id + " from DB");
					
					// Remove non existent NPC respawn.
					try
					{
						ctx.NpcRespawns.Where(r => r.Id == id).ExecuteDelete();
					}
					catch (Exception e)
					{
						LOGGER.Error(GetType().Name + ": Could not remove npc #" + id + " from DB: " + e);
					}
				}
			}
			
			LOGGER.Info(GetType().Name +": Loaded " + _npcs.size() + " instances.");
			LOGGER.Info(GetType().Name +": Scheduled " + _schedules.size() + " instances.");
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Couldnt load npc_respawns table" + e);
		}
	}
	
	private void scheduleSpawn(int npcId)
	{
		Npc npc = _spawns.get(npcId).doSpawn();
		if (npc != null)
		{
			npc.setDBStatus(RaidBossStatus.ALIVE);
			
			StatSet info = new StatSet();
			info.set("currentHP", npc.getCurrentHp());
			info.set("currentMP", npc.getCurrentMp());
			info.set("respawnTime", 0);
			_storedInfo.put(npcId, info);
			_npcs.put(npcId, npc);
			LOGGER.Info(GetType().Name +": Spawning NPC " + npc.getName());
		}
		
		_schedules.remove(npcId);
	}
	
	/**
	 * Update status.
	 * @param npc the npc
	 * @param isNpcDead the is npc dead
	 */
	public void updateStatus(Npc npc, bool isNpcDead)
	{
		StatSet info = _storedInfo.get(npc.getId());
		if (info == null)
		{
			return;
		}
		
		if (isNpcDead)
		{
			npc.setDBStatus(RaidBossStatus.DEAD);
			
			SchedulingPattern respawnPattern = npc.getSpawn().getRespawnPattern();
			int respawnMinDelay;
			int respawnMaxDelay;
			int respawnDelay;
			long respawnTime;
			if (respawnPattern != null)
			{
				respawnTime = respawnPattern.next(System.currentTimeMillis());
				respawnMinDelay = (int) (respawnTime - System.currentTimeMillis());
				respawnMaxDelay = respawnMinDelay;
				respawnDelay = respawnMinDelay;
			}
			else
			{
				respawnMinDelay = (int) (npc.getSpawn().getRespawnMinDelay() * Config.RAID_MIN_RESPAWN_MULTIPLIER);
				respawnMaxDelay = (int) (npc.getSpawn().getRespawnMaxDelay() * Config.RAID_MAX_RESPAWN_MULTIPLIER);
				respawnDelay = Rnd.get(respawnMinDelay, respawnMaxDelay);
				respawnTime = System.currentTimeMillis() + respawnDelay;
			}
			
			info.set("currentHP", npc.getMaxHp());
			info.set("currentMP", npc.getMaxMp());
			info.set("respawnTime", respawnTime);
			if ((!_schedules.containsKey(npc.getId()) && ((respawnMinDelay > 0) || (respawnMaxDelay > 0))) || (respawnPattern != null))
			{
				LOGGER.Info(GetType().Name +": Updated " + npc.getName() + " respawn time to " + Util.formatDate(new JSType.Date(respawnTime), "dd.MM.yyyy HH:mm"));
				_schedules.put(npc.getId(), ThreadPool.schedule(() => scheduleSpawn(npc.getId()), respawnDelay));
				updateDb();
			}
		}
		else
		{
			npc.setDBStatus(RaidBossStatus.ALIVE);
			
			info.set("currentHP", npc.getCurrentHp());
			info.set("currentMP", npc.getCurrentMp());
			info.set("respawnTime", 0);
		}
		_storedInfo.put(npc.getId(), info);
	}
	
	/**
	 * Adds the new spawn.
	 * @param spawn the spawn dat
	 * @param respawnTime the respawn time
	 * @param currentHP the current hp
	 * @param currentMP the current mp
	 * @param storeInDb the store in db
	 */
	public void addNewSpawn(Spawn spawn, DateTime? respawnTime, double currentHP, double currentMP, bool storeInDb)
	{
		if (spawn == null)
		{
			return;
		}
		if (_spawns.containsKey(spawn.getId()))
		{
			return;
		}
		
		int npcId = spawn.getId();
		DateTime time = DateTime.UtcNow;
		SpawnTable.getInstance().addNewSpawn(spawn, false);
		if ((respawnTime is null) || (time > respawnTime))
		{
			Npc npc = spawn.doSpawn();
			if (npc != null)
			{
				npc.setCurrentHp(currentHP);
				npc.setCurrentMp(currentMP);
				npc.setDBStatus(RaidBossStatus.ALIVE);
				
				_npcs.put(npcId, npc);
				
				StatSet info = new StatSet();
				info.set("currentHP", currentHP);
				info.set("currentMP", currentMP);
				info.set("respawnTime", 0);
				_storedInfo.put(npcId, info);
			}
		}
		else
		{
			TimeSpan spawnTime = respawnTime.Value - DateTime.UtcNow;
			_schedules.put(npcId, ThreadPool.schedule(() => scheduleSpawn(npcId), spawnTime));
		}
		
		_spawns.put(npcId, spawn);
		
		if (storeInDb)
		{
			try 
			{
				using GameServerDbContext ctx = new();
				ctx.NpcRespawns.Add(new NpcRespawn()
				{
					Id = spawn.getId(),
					X = spawn.getX(),
					Y = spawn.getY(),
					Z = spawn.getZ(),
					Heading = spawn.getHeading(),
					RespawnTime = respawnTime,
					CurrentHp = currentHP,
					CurrentMp = currentMP,
				});

				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				// problem with storing spawn
				LOGGER.Warn(GetType().Name + ": Could not store npc #" + npcId + " in the DB: " + e);
			}
		}
	}
	
	public Npc addNewSpawn(Spawn spawn, bool storeInDb)
	{
		if (spawn == null)
		{
			return null;
		}
		
		int npcId = spawn.getId();
		Spawn existingSpawn = _spawns.get(npcId);
		if (existingSpawn != null)
		{
			return existingSpawn.getLastSpawn();
		}
		
		SpawnTable.getInstance().addNewSpawn(spawn, false);
		
		Npc npc = spawn.doSpawn();
		if (npc == null)
		{
			throw new ArgumentException("Spawn npc is null");
		}
		
		npc.setDBStatus(RaidBossStatus.ALIVE);
		
		StatSet info = new StatSet();
		info.set("currentHP", npc.getMaxHp());
		info.set("currentMP", npc.getMaxMp());
		info.set("respawnTime", 0);
		_npcs.put(npcId, npc);
		_storedInfo.put(npcId, info);
		
		_spawns.put(npcId, spawn);
		
		if (storeInDb)
		{
			try 
			{
				using GameServerDbContext ctx = new();
				ctx.NpcRespawns.Add(new NpcRespawn()
				{
					Id = spawn.getId(),
					X = spawn.getX(),
					Y = spawn.getY(),
					Z = spawn.getZ(),
					Heading = spawn.getHeading(),
					CurrentHp = npc.getMaxHp(),
					CurrentMp = npc.getMaxMp(),
				});
			}
			catch (Exception e)
			{
				// problem with storing spawn
				LOGGER.Warn(GetType().Name + ": Could not store npc #" + npcId + " in the DB: " + e);
			}
		}
		
		return npc;
	}
	
	/**
	 * Delete spawn.
	 * @param spawn the spawn dat
	 * @param updateDb the update db
	 */
	public void deleteSpawn(Spawn spawn, bool updateDb)
	{
		if (spawn == null)
		{
			return;
		}
		
		int npcId = spawn.getId();
		_spawns.remove(npcId);
		_npcs.remove(npcId);
		_storedInfo.remove(npcId);
		
		ScheduledFuture task = _schedules.remove(npcId);
		if (task != null)
		{
			task.cancel(true);
		}
		
		if (updateDb)
		{
			try 
			{
				using GameServerDbContext ctx = new();
				ctx.NpcRespawns.Where(r => r.Id == npcId).ExecuteDelete();
			}
			catch (Exception e)
			{
				// problem with deleting spawn
				LOGGER.Warn(GetType().Name + ": Could not remove npc #" + npcId + " from DB: " + e);
			}
		}
		
		SpawnTable.getInstance().deleteSpawn(spawn, false);
	}
	
	/**
	 * Update database.
	 */
	private void updateDb()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			foreach (var entry in _storedInfo)
			{
				int npcId = entry.Key;
				if (npcId == null)
				{
					continue;
				}
				
				Npc npc = _npcs.get(npcId);
				if (npc == null)
				{
					continue;
				}
				
				if (npc.getDBStatus() == RaidBossStatus.ALIVE)
				{
					updateStatus(npc, false);
				}
				
				StatSet info = entry.Value;
				if (info == null)
				{
					continue;
				}
				
				try
				{
					DateTime? respawnTime = info.getLong("respawnTime");
					double currentHp = npc.isDead() ? npc.getMaxHp() : info.getDouble("currentHP");
					double currentMp = npc.isDead() ? npc.getMaxMp() : info.getDouble("currentMP");
					ctx.NpcRespawns.Where(r => r.Id == npcId).ExecuteUpdate(s =>
						s.SetProperty(r => r.RespawnTime, respawnTime).SetProperty(r => r.CurrentHp, currentHp)
							.SetProperty(r => r.CurrentMp, currentMp));
				}
				catch (Exception e)
				{
					LOGGER.Error(GetType().Name + ": Couldnt update npc_respawns table " + e);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": SQL error while updating database spawn to database: " + e);
		}
	}
	
	/**
	 * Gets the all npc status.
	 * @return the all npc status
	 */
	public String[] getAllNpcsStatus()
	{
		String[] msg = new String[(_npcs == null) ? 0 : _npcs.size()];
		if (_npcs == null)
		{
			msg[0] = "None";
			return msg;
		}
		
		int index = 0;
		foreach (Npc npc in _npcs.values())
		{
			msg[index++] = npc.getName() + ": " + npc.getDBStatus();
		}
		
		return msg;
	}
	
	/**
	 * Gets the npc status.
	 * @param npcId the npc id
	 * @return the raid npc status
	 */
	public String getNpcsStatus(int npcId)
	{
		String msg = "NPC Status..." + Environment.NewLine;
		if (_npcs == null)
		{
			msg += "None";
			return msg;
		}
		
		if (_npcs.containsKey(npcId))
		{
			Npc npc = _npcs.get(npcId);
			msg += npc.getName() + ": " + npc.getDBStatus();
		}
		
		return msg;
	}
	
	/**
	 * Gets the raid npc status.
	 * @param npcId the npc id
	 * @return the raid npc status
	 */
	public RaidBossStatus getStatus(int npcId)
	{
		if (_npcs.containsKey(npcId))
		{
			return _npcs.get(npcId).getDBStatus();
		}
		else if (_schedules.containsKey(npcId))
		{
			return RaidBossStatus.DEAD;
		}
		else
		{
			return RaidBossStatus.UNDEFINED;
		}
	}
	
	/**
	 * Gets the valid template.
	 * @param npcId the npc id
	 * @return the valid template
	 */
	public NpcTemplate getValidTemplate(int npcId)
	{
		return NpcData.getInstance().getTemplate(npcId);
	}
	
	/**
	 * Notify spawn night npc.
	 * @param npc the npc
	 */
	public void notifySpawnNightNpc(Npc npc)
	{
		StatSet info = new StatSet();
		info.set("currentHP", npc.getCurrentHp());
		info.set("currentMP", npc.getCurrentMp());
		info.set("respawnTime", 0);
		npc.setDBStatus(RaidBossStatus.ALIVE);
		
		_storedInfo.put(npc.getId(), info);
		_npcs.put(npc.getId(), npc);
	}
	
	/**
	 * Checks if the npc is defined.
	 * @param npcId the npc id
	 * @return {@code true} if is defined
	 */
	public bool isDefined(int npcId)
	{
		return _spawns.containsKey(npcId);
	}
	
	/**
	 * Gets a specific NPC by id.
	 * @param id The id of the NPC.
	 * @return the Npc
	 */
	public Npc getNpc(int id)
	{
		return _npcs.get(id);
	}
	
	/**
	 * Gets the npcs.
	 * @return the npcs
	 */
	public Map<int, Npc> getNpcs()
	{
		return _npcs;
	}
	
	/**
	 * Gets the spawns.
	 * @return the spawns
	 */
	public Map<int, Spawn> getSpawns()
	{
		return _spawns;
	}
	
	/**
	 * Gets the stored info.
	 * @return the stored info
	 */
	public Map<int, StatSet> getStoredInfo()
	{
		return _storedInfo;
	}
	
	/**
	 * Saves and clears the raid npces status, including all schedules.
	 */
	public void cleanUp()
	{
		updateDb();
		
		_npcs.clear();
		
		foreach (ScheduledFuture schedule in _schedules.values())
		{
			schedule.cancel(true);
		}
		
		_schedules.clear();
		
		_storedInfo.clear();
		_spawns.clear();
	}
	
	/**
	 * Gets the single instance of DBSpawnManager.
	 * @return single instance of DBSpawnManager
	 */
	public static DBSpawnManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly DBSpawnManager INSTANCE = new DBSpawnManager();
	}
}