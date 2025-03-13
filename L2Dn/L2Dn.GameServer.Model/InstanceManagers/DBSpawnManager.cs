using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Database spawn manager.
 * @author godson, UnAfraid
 */
public sealed class DbSpawnManager
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(DbSpawnManager));
	private readonly Map<int, NpcState> _npcStates = new();

	/**
	 * Instantiates a new raid npc spawn manager.
	 */
	private DbSpawnManager()
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

		foreach (NpcState state in _npcStates.Values)
		{
			if (state.Spawn != null)
				deleteSpawn(state.Spawn, false);
		}

		_npcStates.Clear();

		try
		{
			List<int> spawnsToRemove = [];
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (DbNpcRespawn record in ctx.NpcRespawns)
			{
				int npcId = record.Id;
				NpcTemplate? template = NpcData.getInstance().getTemplate(npcId);
				if (template is null)
				{
					_logger.Error(GetType().Name + ": Database spawn for non-existent npc " + npcId);
					spawnsToRemove.Add(npcId);
					continue;
				}

				Spawn spawn = new(template);
				spawn.Location = new Location(record.X, record.Y, record.Z, record.Heading);
				spawn.setAmount(1);

				List<NpcSpawnTemplate> spawns = SpawnData.getInstance()
					.getNpcSpawns(npc => npc.getId() == template.getId() && npc.hasDBSave());

				if (spawns.Count == 0)
				{
					_logger.Warn(GetType().Name + ": Couldn't find spawn declaration for npc: " + template.getId() +
						" - " + template.getName());

					deleteSpawn(spawn, true);
					continue;
				}

				if (spawns.Count > 1)
				{
					_logger.Warn(GetType().Name + ": Found multiple database spawns for npc: " + template.getId() +
						" - " + template.getName() + " " + spawns);

					continue;
				}

				NpcSpawnTemplate spawnTemplate = spawns[0];
				spawn.setSpawnTemplate(spawnTemplate);

				TimeSpan? respawn = spawnTemplate.getRespawnTime();
				TimeSpan? respawnRandom = spawnTemplate.getRespawnTimeRandom();
				SchedulingPattern? respawnPattern = spawnTemplate.getRespawnPattern();

				if (respawn != null || respawnPattern != null)
				{
					spawn.setRespawnDelay(respawn, respawnRandom);
					spawn.setRespawnPattern(respawnPattern);
					spawn.startRespawn();
				}
				else
				{
					spawn.stopRespawn();
					_logger.Warn(GetType().Name + ": Found database spawns without respawn for npc: " +
						template.getId() + " - " + template.getName() + " " + spawnTemplate);

					continue;
				}

				addNewSpawn(spawn, record.RespawnTime, record.CurrentHp, record.CurrentMp, false);
			}

			_logger.Info(GetType().Name + ": Loaded " + _npcStates.Count + " instances.");

			_logger.Info(GetType().Name + ": Scheduled " + _npcStates.Count(s => s.Value.Future is not null) +
				" instances.");

			if (spawnsToRemove.Count != 0)
			{
				try
				{
					// Remove non-existent NPC respawns.
					ctx.NpcRespawns.Where(r => spawnsToRemove.Contains(r.Id)).ExecuteDelete();
				}
				catch (Exception e)
				{
					_logger.Error(GetType().Name + ": Could not remove npcs " + string.Join(", ", spawnsToRemove) +
						" from DB: " + e);
				}
			}
		}
		catch (Exception e)
		{
			_logger.Error(GetType().Name + ": Couldnt load npc_respawns table" + e);
		}
	}

	private void scheduleSpawn(int npcId)
	{
		NpcState state = _npcStates[npcId];
		Npc? npc = state.Spawn?.doSpawn();
		if (npc != null)
		{
			npc.setDBStatus(RaidBossStatus.ALIVE);

			state.CurrentHp = npc.getCurrentHp();
			state.CurrentMp = npc.getCurrentMp();
			state.RespawnTime = null;
			state.Npc = npc;
			_logger.Info(GetType().Name + ": Spawning NPC " + npc.getName());
			UpdateRecord(state);
		}

		state.Future = null;
	}

	/**
	 * Update status.
	 * @param npc the npc
	 * @param isNpcDead the is npc dead
	 */
	public void updateStatus(Npc npc, bool isNpcDead)
	{
		if (!_npcStates.TryGetValue(npc.getId(), out NpcState? npcState))
			return;

        Spawn? npcSpawn = npc.getSpawn();
        if (npcSpawn == null)
            return;

		if (isNpcDead)
		{
			npc.setDBStatus(RaidBossStatus.DEAD);

			SchedulingPattern? respawnPattern = npcSpawn.getRespawnPattern();
			TimeSpan respawnMinDelay;
			TimeSpan respawnMaxDelay;
			TimeSpan respawnDelay;
			DateTime respawnTime;
			if (respawnPattern != null)
			{
				respawnTime = respawnPattern.next(DateTime.UtcNow);
				respawnMinDelay = respawnTime - DateTime.UtcNow;
				respawnMaxDelay = respawnMinDelay;
				respawnDelay = respawnMinDelay;
			}
			else
			{
				respawnMinDelay = npcSpawn.getRespawnMinDelay() * Config.RAID_MIN_RESPAWN_MULTIPLIER;
				respawnMaxDelay = npcSpawn.getRespawnMaxDelay() * Config.RAID_MAX_RESPAWN_MULTIPLIER;
				respawnDelay = Rnd.get(respawnMinDelay, respawnMaxDelay);
				respawnTime = DateTime.UtcNow + respawnDelay;
			}

			npcState.CurrentHp = npc.getMaxHp();
			npcState.CurrentMp = npc.getMaxMp();
			npcState.RespawnTime = respawnTime;

			if ((npcState.Future is null && (respawnMinDelay > TimeSpan.Zero || respawnMaxDelay > TimeSpan.Zero)) ||
			    respawnPattern != null)
			{
				_logger.Info(GetType().Name + ": Updated " + npc.getName() + " respawn time to " +
					respawnTime.ToString("dd.MM.yyyy HH:mm"));

				npcState.Future = ThreadPool.schedule(() => scheduleSpawn(npc.getId()), respawnDelay);
			}
		}
		else
		{
			npc.setDBStatus(RaidBossStatus.ALIVE);

			npcState.CurrentHp = npc.getCurrentHp();
			npcState.CurrentMp = npc.getCurrentMp();
			npcState.RespawnTime = null;
		}

		UpdateRecord(npcState);
	}

	/**
	 * Adds the new spawn.
	 * @param spawn the spawn dat
	 * @param respawnTime the respawn time
	 * @param currentHP the current hp
	 * @param currentMP the current mp
	 * @param storeInDb the store in db
	 */
	public void addNewSpawn(Spawn spawn, DateTime? respawnTime, double currentHp, double currentMp, bool storeInDb)
	{
		NpcState npcState = _npcStates.GetOrAdd(spawn.getId(), id => new NpcState() { NpcId = id });
		if (npcState.Spawn is not null)
			return;

		int npcId = spawn.getId();
		SpawnTable.getInstance().addNewSpawn(spawn, false);
		if (respawnTime is null || respawnTime < DateTime.UtcNow)
		{
			Npc? npc = spawn.doSpawn();
			if (npc != null)
			{
				npc.setCurrentHp(currentHp);
				npc.setCurrentMp(currentMp);
				npc.setDBStatus(RaidBossStatus.ALIVE);

				npcState.CurrentHp = currentHp;
				npcState.CurrentMp = currentMp;
				npcState.RespawnTime = null;
			}
		}
		else
		{
			TimeSpan spawnTime = respawnTime.Value - DateTime.UtcNow;
			npcState.Future = ThreadPool.schedule(() => scheduleSpawn(npcId), spawnTime);
		}

		npcState.Spawn = spawn;

		if (storeInDb)
			UpdateRecord(npcState);
	}

	public Npc? addNewSpawn(Spawn spawn, bool storeInDb)
	{
		int npcId = spawn.getId();
		if (_npcStates.TryGetValue(npcId, out NpcState? npcState) && npcState.Spawn != null)
			return npcState.Spawn.getLastSpawn();

		SpawnTable.getInstance().addNewSpawn(spawn, false);

		Npc? npc = spawn.doSpawn();
		if (npc == null)
			throw new InvalidOperationException("Spawn npc is null");

		npc.setDBStatus(RaidBossStatus.ALIVE);

		if (npcState is null)
			npcState = _npcStates.GetOrAdd(npcId, new NpcState() { NpcId = npcId });

		npcState.CurrentHp = npc.getMaxHp();
		npcState.CurrentMp = npc.getMaxMp();
		npcState.RespawnTime = null;
		npcState.Spawn = spawn;
		npcState.Npc = npc;

		if (storeInDb)
			UpdateRecord(npcState);

		return npc;
	}

	/**
	 * Delete spawn.
	 * @param spawn the spawn dat
	 * @param updateDb the update db
	 */
	public void deleteSpawn(Spawn spawn, bool updateDb)
	{
		int npcId = spawn.getId();
		if (!_npcStates.Remove(npcId, out NpcState? npcState))
			return;

		npcState.Future?.cancel(true);

		if (updateDb)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.NpcRespawns.Where(r => r.Id == npcId).ExecuteDelete();
			}
			catch (Exception e)
			{
				// problem with deleting spawn
				_logger.Warn(GetType().Name + ": Could not remove npc #" + npcId + " from DB: " + e);
			}
		}

		SpawnTable.getInstance().deleteSpawn(spawn, false);
	}

	private void UpdateRecord(NpcState npcState)
	{
		int npcId = npcState.NpcId;
		Npc? npc = npcState.Npc;
		Spawn? spawn = npcState.Spawn;

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			DbNpcRespawn? record = ctx.NpcRespawns.SingleOrDefault(r => r.Id == npcId);
			if (record is null)
			{
				record = new DbNpcRespawn() { Id = npcId };
				ctx.NpcRespawns.Add(record);
			}

			record.RespawnTime = npcState.RespawnTime;
			if (npc is not null)
			{
				(record.CurrentHp, record.CurrentMp) = npc.isDead()
					? (npc.getMaxHp(), npc.getMaxMp())
					: (npc.getCurrentHp(), npc.getCurrentMp());
			}
			else
			{
				record.CurrentHp = npcState.CurrentHp;
				record.CurrentMp = npcState.CurrentMp;
			}

			if (spawn is not null)
			{
				record.X = spawn.Location.X;
				record.Y = spawn.Location.Y;
				record.Z = spawn.Location.Z;
				record.Heading = spawn.Location.Heading;
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			_logger.Warn(GetType().Name + ": Could not store npc #" + npcId + " in the DB: " + e);
		}
	}

	/**
	 * Update database.
	 */
	private void updateDb()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (var entry in _npcStates)
			{
				int npcId = entry.Key;
				NpcState npcState = entry.Value;
				Npc? npc = npcState.Npc;
				if (npc is null)
					continue;

				if (npc.getDBStatus() == RaidBossStatus.ALIVE)
				{
					npcState.CurrentHp = npc.getCurrentHp();
					npcState.CurrentMp = npc.getCurrentMp();
					npcState.RespawnTime = null;
				}

				try
				{
					DateTime? respawnTime = npcState.RespawnTime;
					double currentHp = npc.isDead() ? npc.getMaxHp() : npcState.CurrentHp;
					double currentMp = npc.isDead() ? npc.getMaxMp() : npcState.CurrentMp;
					ctx.NpcRespawns.Where(r => r.Id == npcId).ExecuteUpdate(s =>
						s.SetProperty(r => r.RespawnTime, respawnTime).SetProperty(r => r.CurrentHp, currentHp)
							.SetProperty(r => r.CurrentMp, currentMp));
				}
				catch (Exception e)
				{
					_logger.Error(GetType().Name + ": Couldnt update NpcRespawns table " + e);
				}
			}
		}
		catch (Exception e)
		{
			_logger.Warn(GetType().Name + ": SQL error while updating database spawn to database: " + e);
		}
	}

	/**
	 * Gets the raid npc status.
	 * @param npcId the npc id
	 * @return the raid npc status
	 */
	public RaidBossStatus getStatus(int npcId)
	{
		if (!_npcStates.TryGetValue(npcId, out NpcState? npcState))
			return RaidBossStatus.UNDEFINED;

		if (npcState.Npc is not null)
			return npcState.Npc.getDBStatus();

		if (npcState.Future is not null)
			return RaidBossStatus.DEAD;

		return RaidBossStatus.UNDEFINED;
	}

	/**
	 * Checks if the npc is defined.
	 * @param npcId the npc id
	 * @return {@code true} if is defined
	 */
	public bool isDefined(int npcId)
	{
		return _npcStates.ContainsKey(npcId);
	}

	/**
	 * Gets a specific NPC by id.
	 * @param id The id of the NPC.
	 * @return the Npc
	 */
	public Npc? getNpc(int id)
	{
		return _npcStates.GetValueOrDefault(id)?.Npc;
	}

	/**
	 * Gets the npcs.
	 * @return the npcs
	 */
	public IEnumerable<Npc> getNpcs()
	{
		return _npcStates.Select(p => p.Value.Npc).Where(x => x is not null)!;
	}

	/**
	 * Saves and clears the raid npces status, including all schedules.
	 */
	public void cleanUp()
	{
		updateDb();

		foreach (NpcState npcState in _npcStates.Values)
			npcState.Future?.cancel(true);

		_npcStates.Clear();
	}

	/**
	 * Gets the single instance of DBSpawnManager.
	 * @return single instance of DBSpawnManager
	 */
	public static DbSpawnManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly DbSpawnManager INSTANCE = new();
	}

	private sealed class NpcState
	{
		public int NpcId;
		public DateTime? RespawnTime;
		public double CurrentHp;
		public double CurrentMp;

		public Npc? Npc;
		public Spawn? Spawn;
		public ScheduledFuture? Future;
	}
}