using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Impl.Sieges;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using FortManager = L2Dn.GameServer.InstanceManagers.FortManager;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Sieges;

public class FortSiege: ListenersContainer, Siegable
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(FortSiege));
	
	public const String ORC_FORTRESS_GREG_UPPER_LEFT_SPAWN = "orc_fortress_greg_upper_left";
	public const String ORC_FORTRESS_GREG_UPPER_RIGHT_SPAWN = "orc_fortress_greg_upper_right";
	public const String ORC_FORTRESS_GREG_BOTTOM_RIGHT_SPAWN = "orc_fortress_greg_bottom_right";
	public const String GREG_SPAWN_VAR = "GREG_SPAWN";
	
	private bool _hasSpawnedPreparationNpcs = false;
	
	private static readonly AtomicReference<SpawnTemplate> SPAWN_PREPARATION_NPCS = new();
	
	private static readonly ZoneType FORTRESS_ZONE = ZoneManager.getInstance().getZoneByName("orc_fortress_general_area");
	
	private ScheduledFuture _siegeGregSentryTask = null;
	
	private int _flagCount = 0;
	
	public class ScheduleEndSiegeTask: Runnable
	{
		private readonly FortSiege _fortSiege;

		public ScheduleEndSiegeTask(FortSiege fortSiege)
		{
			_fortSiege = fortSiege;
		}
        
		public void run()
		{
			if (!_fortSiege._isInProgress)
			{
				return;
			}
			
			try
			{
				_fortSiege._siegeEnd = null;
				_fortSiege.endSiege();
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Exception: ScheduleEndSiegeTask() for Fort: " + _fortSiege._fort.getName() + " " + e);
			}
		}
	}
	
	public class ScheduleGregSentrySpawnTask: Runnable
	{
		private readonly FortSiege _fortSiege;

		public ScheduleGregSentrySpawnTask(FortSiege fortSiege)
		{
			_fortSiege = fortSiege;
		}

		public void run()
		{
			FORTRESS_ZONE.broadcastPacket(new ExShowScreenMessagePacket(2, -1, 2, 0, 0, 0, 0, true, 8000, false, null,
				NpcStringId.FLAG_SENTRY_GREG_HAS_APPEARED, null));
			
			if (!_fortSiege._isInProgress)
			{
				return;
			}

			try
			{
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate
					.getGroupsByName(ORC_FORTRESS_GREG_UPPER_LEFT_SPAWN).forEach(holder =>
					{
						holder.spawnAll();
						foreach (NpcSpawnTemplate nst in holder.getSpawns())
						{
							foreach (Npc npc in nst.getSpawnedNpcs())
							{
								Spawn spawn = npc.getSpawn();
								if (spawn != null)
								{
									spawn.stopRespawn();
								}
							}
						}
					}));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate
					.getGroupsByName(ORC_FORTRESS_GREG_UPPER_RIGHT_SPAWN).forEach(holder =>
					{
						holder.spawnAll();
						foreach (NpcSpawnTemplate nst in holder.getSpawns())
						{
							foreach (Npc npc in nst.getSpawnedNpcs())
							{
								Spawn spawn = npc.getSpawn();
								if (spawn != null)
								{
									spawn.stopRespawn();
								}
							}
						}
					}));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate
					.getGroupsByName(ORC_FORTRESS_GREG_BOTTOM_RIGHT_SPAWN).forEach(holder =>
					{
						holder.spawnAll();
						foreach (NpcSpawnTemplate nst in holder.getSpawns())
						{
							foreach (Npc npc in nst.getSpawnedNpcs())
							{
								Spawn spawn = npc.getSpawn();
								if (spawn != null)
								{
									spawn.stopRespawn();
								}
							}
						}
					}));
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Exception: ScheduleGregSentrySpawn() for Fort: " +
				            _fortSiege._fort.getName() + " " + e);
			}
		}
	}
	
	public class ScheduleStartSiegeTask: Runnable
	{
		private readonly FortSiege _fortSiege;
		private readonly Fort _fortInst;
		private readonly int _time;
		private readonly TimeSpan _initialDelay;
		
		public ScheduleStartSiegeTask(FortSiege fortSiege, int time)
		{
			_fortInst = fortSiege._fort;
			_fortSiege = fortSiege;
			_time = time;
		}
		
		public ScheduleStartSiegeTask(FortSiege fortSiege, int time, TimeSpan initialDelay)
		{
			_fortInst = fortSiege._fort;
			_fortSiege = fortSiege;
			_time = time;
			_initialDelay = initialDelay;
		}
		
		public void run()
		{
			if (_fortSiege._isInProgress)
			{
				return;
			}

			if (!_fortSiege._hasSpawnedPreparationNpcs)
			{
				_fortSiege._hasSpawnedPreparationNpcs = true;

				SPAWN_PREPARATION_NPCS.set(SpawnData.getInstance()
					.getSpawns()
					.FirstOrDefault(t => t.getName() != null && t.getName().contains("orc_fortress_preparation_npcs")));
				
				SPAWN_PREPARATION_NPCS.get().getGroups().forEach(x => x.spawnAll());
			}

			try
			{
				SystemMessagePacket sm;
				if ((_initialDelay != TimeSpan.Zero) && (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS))
				{
					int nextTask = 0;
					if (_initialDelay >= TimeSpan.FromMilliseconds(1200000))
					{
						nextTask = 1200;
					}
					else
					{
						_fortSiege._isInPreparation = true;
						if (_initialDelay >= TimeSpan.FromMilliseconds(600000))
						{
							nextTask = 600;
						}
						else if (_initialDelay >= TimeSpan.FromMilliseconds(300000))
						{
							nextTask = 300;
						}
						else if (_initialDelay >= TimeSpan.FromMilliseconds(60000))
						{
							nextTask = 60;
						}
						else if (_initialDelay >= TimeSpan.FromMilliseconds(30000))
						{
							nextTask = 30;
						}
						else if (_initialDelay >= TimeSpan.FromMilliseconds(10000))
						{
							nextTask = 10;
						}
						else if (_initialDelay >= TimeSpan.FromMilliseconds(5000))
						{
							nextTask = 5;
						}
						else
						{
							nextTask = 0;
						}
						
						Broadcast.toAllOnlinePlayers(new OrcFortressSiegeInfoHUDPacket(_fortInst.getResidenceId(), 0, DateTime.UtcNow, _initialDelay));
					}
					
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, nextTask), _initialDelay - TimeSpan.FromMilliseconds(nextTask * 1000)); // Prepare task for @nextTask minutes left.
					// LOGGER.info("scheduling " + nextTask + " in " + ((_initialDelayInMilliseconds / 1000) - nextTask) + " sec");
				}
				else if ((_time == 3600) && (_fortInst.getResidenceId() != FortManager.ORC_FORTRESS)) // 1hr remains
				{
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 600), 3000000); // Prepare task for 10 minutes left.
				}
				else if ((_time == 1200) && (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)) // 20min remains
				{
					_fortSiege._isInPreparation = true;
					sm = new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN);
					sm.Params.addInt(20);
					_fortSiege.announceToPlayer(sm);
					Broadcast.toAllOnlinePlayers(new OrcFortressSiegeInfoHUDPacket(_fortInst.getResidenceId(), 0, DateTime.UtcNow, TimeSpan.FromSeconds(1200)));
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 600), 600000); // Prepare task for 10 minutes left.
				}
				else if (_time == 600) // 10min remains
				{
					_fortSiege._fort.despawnSuspiciousMerchant();
					sm = new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN);
					sm.Params.addInt(10);
					_fortSiege.announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 300), 300000); // Prepare task for 5 minutes left.
				}
				else if (_time == 300) // 5min remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fortSiege._fort.despawnSuspiciousMerchant();
					}
					sm = new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN);
					sm.Params.addInt(5);
					_fortSiege.announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 60), 240000); // Prepare task for 1 minute left.
				}
				else if (_time == 60) // 1min remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fortSiege._fort.despawnSuspiciousMerchant();
					}
					sm = new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN);
					sm.Params.addInt(1);
					_fortSiege.announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 30), 30000); // Prepare task for 30 seconds left.
				}
				else if (_time == 30) // 30seconds remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fortSiege._fort.despawnSuspiciousMerchant();
					}
					
					sm = new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_SEC);
					sm.Params.addInt(30);
					_fortSiege.announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 10), 20000); // Prepare task for 10 seconds left.
				}
				else if (_time == 10) // 10seconds remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fortSiege._fort.despawnSuspiciousMerchant();
					}
					
					sm = new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_SEC);
					sm.Params.addInt(10);
					_fortSiege.announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 5), 5000); // Prepare task for 5 seconds left.
				}
				else if (_time == 5) // 5seconds remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fortSiege._fort.despawnSuspiciousMerchant();
					}
					
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 1), 4000); // Prepare task for 1 seconds left.
				}
				else if (_time == 1) // 1seconds remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fortSiege._fort.despawnSuspiciousMerchant();
					}
					
					ThreadPool.schedule(new ScheduleStartSiegeTask(_fortSiege, 0), 1000); // Prepare task start siege.
				}
				else if (_time == 0) // start siege
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fortSiege._isInPreparation = false;
					}
					_fortInst.getSiege().startSiege();
				}
				else
				{
					LOGGER.Warn(GetType().Name + ": Exception: ScheduleStartSiegeTask(): unknown siege time: " + _time);
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Exception: ScheduleStartSiegeTask() for Fort: " + _fortInst.getName() + " " + e);
			}
		}
	}

	public class ScheduleSuspiciousMerchantSpawn: Runnable
	{
		private readonly FortSiege _fortSiege;

		public ScheduleSuspiciousMerchantSpawn(FortSiege fortSiege)
		{
			_fortSiege = fortSiege;
		}

		public void run()
		{
			if (_fortSiege._isInProgress)
			{
				return;
			}

			try
			{
				_fortSiege._fort.spawnSuspiciousMerchant();
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Exception: ScheduleSuspicoiusMerchantSpawn() for Fort: " +
				            _fortSiege._fort.getName() + " " + e);
			}
		}
	}

	public class ScheduleSiegeRestore: Runnable
	{
		private readonly FortSiege _fortSiege;

		public ScheduleSiegeRestore(FortSiege fortSiege)
		{
			_fortSiege = fortSiege;
		}
        
		public void run()
		{
			if (!_fortSiege._isInProgress)
			{
				return;
			}
			
			try
			{
				_fortSiege._siegeRestore = null;
				_fortSiege.resetSiege();
				_fortSiege.announceToPlayer(new SystemMessagePacket(SystemMessageId.THE_BARRACKS_FUNCTION_HAS_BEEN_RESTORED));
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Exception: ScheduleSiegeRestore() for Fort: " + _fortSiege._fort.getName() + " " + e);
			}
		}
	}
	
	private readonly Set<SiegeClan> _attackerClans = new();
	
	// Fort setting
	protected Set<Spawn> _commanders = new();
	protected readonly Fort _fort;
	bool _isInProgress = false;
	private List<Spawn> _siegeGuards = new();
	ScheduledFuture? _siegeEnd = null;
	ScheduledFuture? _siegeRestore = null;
	ScheduledFuture? _siegeStartTask = null;
	// Orc Fortress
	bool _isInPreparation = false;
	
	public FortSiege(Fort fort)
	{
		_fort = fort;
		checkAutoTask();
		FortSiegeManager.getInstance().addSiege(this);
		if (_fort.getResidenceId() == FortManager.ORC_FORTRESS)
		{
			Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_FORT_SIEGE_START, ev => announceStartToPlayers((OnFortSiegeStart)ev), this));
			Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_FORT_SIEGE_FINISH, ev => announceEndToPlayers((OnFortSiegeFinish)ev), this));
			Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_PLAYER_LOGIN, ev => showHUDToPlayer((OnPlayerLogin)ev), this));
		}
	}
	
	private void announceStartToPlayers(OnFortSiegeStart ev)
	{
		Broadcast.toAllOnlinePlayers(new OrcFortressSiegeInfoHUDPacket(ev.getSiege().getFort().getResidenceId(), 1, DateTime.UnixEpoch, TimeSpan.FromSeconds(30 * 60)));
		Broadcast.toAllOnlinePlayers(new SystemMessagePacket(SystemMessageId.SEAL_THE_SEAL_TOWER_AND_CONQUER_ORC_FORTRESS));
	}
	
	private void announceEndToPlayers(OnFortSiegeFinish ev)
	{
		Broadcast.toAllOnlinePlayers(new OrcFortressSiegeInfoHUDPacket(ev.getSiege().getFort().getResidenceId(), 0, DateTime.UnixEpoch, TimeSpan.Zero));
	}
	
	private void showHUDToPlayer(OnPlayerLogin ev)
	{
		if (_isInPreparation)
		{
			TimeSpan remainingTimeInSeconds = _fort.getSiegeDate() - DateTime.UtcNow;
			ev.getPlayer().sendPacket(new OrcFortressSiegeInfoHUDPacket(_fort.getResidenceId(), 0, DateTime.UtcNow, remainingTimeInSeconds));
		}
		else if (_isInProgress)
		{
			TimeSpan remainingTimeInSeconds = _siegeEnd.getDelay();
			ev.getPlayer().sendPacket(new OrcFortressSiegeInfoHUDPacket(_fort.getResidenceId(), 1, DateTime.UtcNow, remainingTimeInSeconds));
		}
	}
	
	/**
	 * When siege ends.
	 */
	public void endSiege()
	{
		if (_isInProgress)
		{
			// Orc Fortress spawn.
			if (_siegeGregSentryTask != null)
			{
				_siegeGregSentryTask.cancel(true);
				_siegeGregSentryTask = null;
			}
			
			if (_fort.getResidenceId() == FortManager.ORC_FORTRESS)
			{
				foreach (Player player in World.getInstance().getPlayers())
				{
					Item weap = player.getActiveWeaponInstance();
					if ((weap != null) && (weap.getId() == FortManager.ORC_FORTRESS_FLAG))
					{
						FortSiegeManager.getInstance().dropCombatFlag(player, getFort().getResidenceId());
					}
				}
				
				foreach (WorldObject obj in World.getInstance().getVisibleObjects())
				{
					if (obj is Item)
					{
						if (obj.getId() == FortManager.ORC_FORTRESS_FLAG)
						{
							obj.decayMe();
						}
					}
				}
				
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_fortress").forEach(holder => holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_runners").forEach(holder => holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_fortress_inside").forEach(holder => holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_fortress_jeras_guards").forEach(holder => holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_UPPER_LEFT_SPAWN).forEach(holder => holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_UPPER_RIGHT_SPAWN).forEach(holder => holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_BOTTOM_RIGHT_SPAWN).forEach(holder => holder.despawnAll()));
				SPAWN_PREPARATION_NPCS.get().getGroups().forEach(x => x.despawnAll());
			}
			
			_isInProgress = false; // Flag so that siege instance can be started
			_hasSpawnedPreparationNpcs = false;
			removeFlags(); // Removes all flags. Note: Remove flag before teleporting players
			unSpawnFlags();
			
			teleportPlayer(FortTeleportWhoType.Attacker, TeleportWhereType.TOWN); // Teleport to the closest town
			updatePlayerSiegeStateFlags(true);
			
			int ownerId = -1;
			if (_fort.getOwnerClan() != null)
			{
				ownerId = _fort.getOwnerClan().getId();
			}
			_fort.getZone().banishForeigners(ownerId);
			_fort.getZone().setActive(false);
			_fort.getZone().updateZoneStatusForCharactersInside();
			_fort.getZone().setSiegeInstance(null);
			
			saveFortSiege(); // Save fort specific data
			clearSiegeClan(); // Clear siege clan from db
			removeCommanders(); // Remove commander from this fort
			_fort.spawnNpcCommanders(); // Spawn NPC commanders
			unspawnSiegeGuard(); // Remove all spawned siege guard from this fort
			_fort.CloseOrcFortressDoors();
			if (_fort.getResidenceId() != FortManager.ORC_FORTRESS)
			{
				_fort.resetDoors();
				
			}
			else
			{
				_fort.CloseOrcFortressDoors();
				LOGGER.Info("FortSiege: Closed Orc Fortress doors.");
			}
			_fort.SetOrcFortressOwnerNpcs(true);
			ThreadPool.schedule(new ScheduleSuspiciousMerchantSpawn(this), FortSiegeManager.getInstance().getSuspiciousMerchantRespawnDelay() * 60 * 1000); // Prepare 3hr task for suspicious merchant respawn
			setSiegeDateTime(true); // store suspicious merchant spawn in DB
			if (_siegeEnd != null)
			{
				_siegeEnd.cancel(true);
				_siegeEnd = null;
			}
			if (_siegeRestore != null)
			{
				_siegeRestore.cancel(true);
				_siegeRestore = null;
			}
			
			if ((_fort.getOwnerClan() != null) && (_fort.getFlagPole().getMeshIndex() == 0))
			{
				_fort.setVisibleFlag(true);
			}
			
			LOGGER.Info(GetType().Name + ": Siege of " + _fort.getName() + " fort finished.");
			
			// Notify to scripts.
			if (EventDispatcher.getInstance().hasListener(EventType.ON_FORT_SIEGE_FINISH, getFort()))
			{
				EventDispatcher.getInstance().notifyEventAsync(new OnFortSiegeFinish(this), getFort());
			}
		}
	}
	
	/**
	 * When siege starts
	 */
	public void startSiege()
	{
		if (!_isInProgress)
		{
			if (_siegeStartTask != null) // used admin command "admin_startfortsiege"
			{
				_siegeStartTask.cancel(true);
				_fort.despawnSuspiciousMerchant();
			}
			_siegeStartTask = null;
			
			if (_attackerClans.isEmpty() && (_fort.getResidenceId() != FortManager.ORC_FORTRESS))
			{
				return;
			}
			
			// Orc Fortress
			_isInProgress = true; // Flag so that same siege instance cannot be started again
			loadSiegeClan(); // Load siege clan from db
			updatePlayerSiegeStateFlags(false);
			// teleportPlayer(FortTeleportWhoType.Attacker, TeleportWhereType.TOWN); // Teleport to the closest town
			
			setFlagCount(0);
			_fort.despawnNpcCommanders(); // Despawn NPC commanders
			spawnCommanders(); // Spawn commanders
			
			if (_fort.getResidenceId() != FortManager.ORC_FORTRESS)
			{
				_fort.resetDoors();
			}
			else
			{
				_fort.OpenOrcFortressDoors();
				LOGGER.Info("FortSiege: Opened Orc Fortress doors.");
			}
			
			_fort.SetOrcFortressOwnerNpcs(false);
			spawnSiegeGuard(); // Spawn siege guard
			_fort.setVisibleFlag(false);
			_fort.getZone().setSiegeInstance(this);
			_fort.getZone().setActive(true);
			_fort.getZone().updateZoneStatusForCharactersInside();
			
			// Schedule a task to prepare auto siege end
			if (_fort.getResidenceId() == FortManager.ORC_FORTRESS)
			{
				_siegeGregSentryTask = ThreadPool.schedule(new ScheduleGregSentrySpawnTask(this), 20 * 60 * 1000); // 20 min
				// _siegeGregSentryTask = ThreadPool.schedule(new ScheduleGregSentrySpawnTask(), 2 * 60 * 1000L); // Prepare Greg Sentry spawn task, Test only
				_siegeEnd = ThreadPool.schedule(new ScheduleEndSiegeTask(this), 30 * 60 * 1000); // Prepare auto end task
			}
			else
			{
				_siegeEnd = ThreadPool.schedule(new ScheduleEndSiegeTask(this), FortSiegeManager.getInstance().getSiegeLength() * 60 * 1000); // Prepare auto end task
			}
			
			announceToPlayer(new SystemMessagePacket(SystemMessageId.THE_FORTRESS_BATTLE_HAS_BEGUN));
			saveFortSiege();
			
			if (_fort.getResidenceId() == FortManager.ORC_FORTRESS)
			{
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_fortress").forEach(holder =>
				{
					holder.spawnAll();
					foreach (NpcSpawnTemplate nst in holder.getSpawns())
					{
						foreach (Npc npc in nst.getSpawnedNpcs())
						{
							Spawn spawn = npc.getSpawn();
							if (spawn != null)
							{
								spawn.setRespawnDelay(TimeSpan.FromSeconds(5));
							}
						}
					}
				}));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_runners").forEach(holder =>
				{
					holder.spawnAll();
					foreach (NpcSpawnTemplate nst in holder.getSpawns())
					{
						foreach (Npc npc in nst.getSpawnedNpcs())
						{
							Spawn spawn = npc.getSpawn();
							if (spawn != null)
							{
								spawn.setRespawnDelay(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
							}
						}
					}
					
				}));
				
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_fortress_inside").forEach(holder =>
				{
					holder.spawnAll();
					foreach (NpcSpawnTemplate nst in holder.getSpawns())
					{
						foreach (Npc npc in nst.getSpawnedNpcs())
						{
							Spawn spawn = npc.getSpawn();
							if (spawn != null)
							{
								spawn.setRespawnDelay(TimeSpan.FromSeconds(5));
							}
						}
					}
					
				}));
			}
			SpawnData.getInstance().getSpawns().forEach(spawnTemplate => spawnTemplate.getGroupsByName("orc_fortress_jeras_guards").forEach(holder =>
			{
				holder.spawnAll();
				foreach (NpcSpawnTemplate nst in holder.getSpawns())
				{
					foreach (Npc npc in nst.getSpawnedNpcs())
					{
						Spawn spawn = npc.getSpawn();
						if (spawn != null)
						{
							spawn.setRespawnDelay(TimeSpan.FromSeconds(160));
						}
					}
				}
				
			}));
		}
		
		LOGGER.Info(GetType().Name + ": Siege of " + _fort.getName() + " fort started.");
		
		// Notify to scripts.
		if (EventDispatcher.getInstance().hasListener(EventType.ON_FORT_SIEGE_START, getFort()))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnFortSiegeStart(this), getFort());
			
		}
	}
	
	/**
	 * Announce to player.
	 * @param sm the system message to send to player
	 */
	public void announceToPlayer(SystemMessagePacket sm)
	{
		// announce messages only for participants
		Clan clan;
		foreach (SiegeClan siegeclan in _attackerClans)
		{
			clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			foreach (Player member in clan.getOnlineMembers(0))
			{
				if (member != null)
				{
					member.sendPacket(sm);
				}
			}
		}
		if (_fort.getOwnerClan() != null)
		{
			clan = ClanTable.getInstance().getClan(getFort().getOwnerClan().getId());
			foreach (Player member in clan.getOnlineMembers(0))
			{
				if (member != null)
				{
					member.sendPacket(sm);
				}
			}
		}
	}
	
	public void announceToPlayer(SystemMessagePacket sm, String s)
	{
		sm.Params.addString(s);
		announceToPlayer(sm);
	}
	
	public void updatePlayerSiegeStateFlags(bool clear)
	{
		Clan clan;
		foreach (SiegeClan siegeclan in _attackerClans)
		{
			clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			foreach (Player member in clan.getOnlineMembers(0))
			{
				if (member == null)
				{
					continue;
				}
				
				if (clear)
				{
					member.setSiegeState(0);
					member.setSiegeSide(0);
					member.setInSiege(false);
					member.stopFameTask();
				}
				else
				{
					member.setSiegeState(1);
					member.setSiegeSide(_fort.getResidenceId());
					if (checkIfInZone(member))
					{
						member.setInSiege(true);
						member.startFameTask(TimeSpan.FromMilliseconds(Config.FORTRESS_ZONE_FAME_TASK_FREQUENCY * 1000), Config.FORTRESS_ZONE_FAME_AQUIRE_POINTS);
					}
				}
				member.broadcastUserInfo();
			}
		}
		if (_fort.getOwnerClan() != null)
		{
			clan = ClanTable.getInstance().getClan(getFort().getOwnerClan().getId());
			foreach (Player member in clan.getOnlineMembers(0))
			{
				if (member == null)
				{
					continue;
				}
				
				if (clear)
				{
					member.setSiegeState((byte) 0);
					member.setSiegeSide(0);
					member.setInSiege(false);
					member.stopFameTask();
				}
				else
				{
					member.setSiegeState((byte) 2);
					member.setSiegeSide(_fort.getResidenceId());
					if (checkIfInZone(member))
					{
						member.setInSiege(true);
						member.startFameTask(TimeSpan.FromMilliseconds(Config.FORTRESS_ZONE_FAME_TASK_FREQUENCY * 1000), Config.FORTRESS_ZONE_FAME_AQUIRE_POINTS);
					}
				}
				member.broadcastUserInfo();
			}
		}
	}
	
	/**
	 * @param object
	 * @return true if object is inside the zone
	 */
	public bool checkIfInZone(WorldObject obj)
	{
		return checkIfInZone(obj.getX(), obj.getY(), obj.getZ());
	}
	
	/**
	 * @param x
	 * @param y
	 * @param z
	 * @return true if object is inside the zone
	 */
	public bool checkIfInZone(int x, int y, int z)
	{
		return (_isInProgress && (_fort.checkIfInZone(x, y, z))); // Fort zone during siege
	}
	
	/**
	 * @param clan The Clan of the player
	 * @return true if clan is attacker
	 */
	public bool checkIsAttacker(Clan clan)
	{
		return (getAttackerClan(clan) != null);
	}
	
	/**
	 * @param clan The Clan of the player
	 * @return true if clan is defender
	 */
	public bool checkIsDefender(Clan clan)
	{
		return (clan != null) && (_fort.getOwnerClan() == clan);
	}
	
	/** Clear all registered siege clans from database for fort */
	public void clearSiegeClan()
	{
		try
		{
			using GameServerDbContext ctx = new();
			int fortId = _fort.getResidenceId();
			ctx.FortSiegeClans.Where(r => r.FortId == fortId).ExecuteDelete();
			
			if (_fort.getOwnerClan() != null)
			{
				int clanId = _fort.getOwnerClan().getId();
				ctx.FortSiegeClans.Where(r => r.ClanId == clanId).ExecuteDelete();
			}
			
			_attackerClans.clear();
			
			// if siege is in progress, end siege
			if (_isInProgress)
			{
				endSiege();
			}
			
			// if siege isn't in progress (1hr waiting time till siege starts), cancel waiting time
			if (_siegeStartTask != null)
			{
				_siegeStartTask.cancel(true);
				_siegeStartTask = null;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Exception: clearSiegeClan(): " + e);
		}
	}
	
	/** Set the date for the next siege. */
	private void clearSiegeDate()
	{
		_fort.setSiegeDate(DateTime.MinValue); // TODO: something wrong here
	}
	
	/**
	 * @return list of Player registered as attacker in the zone.
	 */
	public List<Player> getAttackersInZone()
	{
		List<Player> players = new();
		foreach (SiegeClan siegeclan in _attackerClans)
		{
			Clan clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			foreach (Player player in clan.getOnlineMembers(0))
			{
				if (player == null)
				{
					continue;
				}
				
				if (player.isInSiege())
				{
					players.add(player);
				}
			}
		}
		return players;
	}
	
	/**
	 * @return list of Player in the zone.
	 */
	public List<Player> getPlayersInZone()
	{
		return _fort.getZone().getPlayersInside();
	}
	
	/**
	 * @return list of Player owning the fort in the zone.
	 */
	public List<Player> getOwnersInZone()
	{
		List<Player> players = new();
		if (_fort.getOwnerClan() != null)
		{
			Clan clan = ClanTable.getInstance().getClan(getFort().getOwnerClan().getId());
			if (clan != _fort.getOwnerClan())
			{
				return null;
			}
			
			foreach (Player player in clan.getOnlineMembers(0))
			{
				if (player == null)
				{
					continue;
				}
				
				if (player.isInSiege())
				{
					players.add(player);
				}
			}
		}
		return players;
	}
	
	/**
	 * TODO: To DP AI<br>
	 * Commander was killed
	 * @param instance
	 */
	public void killedCommander(FortCommander instance)
	{
		int RisidenceId = _fort.getResidenceId();
		if ((_fort != null) && (!_commanders.isEmpty() && (RisidenceId != 122)))
		{
			Spawn spawn = instance.getSpawn();
			if (spawn != null)
			{
				foreach (FortSiegeSpawn spawn2 in FortSiegeManager.getInstance().getCommanderSpawnList(getFort().getResidenceId()))
				{
					if (spawn2.getId() == spawn.getId())
					{
						NpcStringId? npcString = null;
						switch (spawn2.getMessageId())
						{
							case 1:
							{
								npcString = NpcStringId.YOU_MAY_HAVE_BROKEN_OUR_ARROWS_BUT_YOU_WILL_NEVER_BREAK_OUR_WILL_ARCHERS_RETREAT;
								break;
							}
							case 2:
							{
								npcString = NpcStringId.AIIEEEE_COMMAND_CENTER_THIS_IS_GUARD_UNIT_WE_NEED_BACKUP_RIGHT_AWAY;
								break;
							}
							case 3:
							{
								npcString = NpcStringId.AT_LAST_THE_MAGIC_CIRCLE_THAT_PROTECTS_THE_FORTRESS_HAS_WEAKENED_VOLUNTEERS_STAND_BACK;
								break;
							}
							case 4:
							{
								npcString = NpcStringId.I_FEEL_SO_MUCH_GRIEF_THAT_I_CAN_T_EVEN_TAKE_CARE_OF_MYSELF_THERE_ISN_T_ANY_REASON_FOR_ME_TO_STAY_HERE_ANY_LONGER;
								break;
							}
						}
						
						if (npcString != null)
						{
							instance.broadcastSay(ChatType.NPC_SHOUT, npcString.Value);
						}
					}
				}
				
				_commanders.remove(spawn);
				if (_commanders.isEmpty())
				{
					// spawn fort flags
					spawnFlag(_fort.getResidenceId());
					// cancel door/commanders respawn
					if (_siegeRestore != null)
					{
						_siegeRestore.cancel(true);
					}
					// open doors in main building
					foreach (Door door in _fort.getDoors())
					{
						if (door.isShowHp())
						{
							continue;
						}
						
						// TODO this also opens control room door at big fort
						door.openMe();
					}
					_fort.getSiege().announceToPlayer(new SystemMessagePacket(SystemMessageId.ALL_BARRACKS_ARE_OCCUPIED));
				}
				// schedule restoring doors/commanders respawn
				else if (_siegeRestore == null)
				{
					_fort.getSiege().announceToPlayer(new SystemMessagePacket(SystemMessageId.THE_BARRACKS_HAVE_BEEN_SEIZED));
					_siegeRestore = ThreadPool.schedule(new ScheduleSiegeRestore(this), FortSiegeManager.getInstance().getCountDownLength() * 60 * 1000);
				}
				else
				{
					_fort.getSiege().announceToPlayer(new SystemMessagePacket(SystemMessageId.THE_BARRACKS_HAVE_BEEN_SEIZED));
				}
			}
			else
			{
				LOGGER.Warn(GetType().Name + ": FortSiege.killedCommander(): killed commander, but commander not registered for fortress. NpcId: " + instance.getId() + " FortId: " + _fort.getResidenceId());
			}
		}
	}
	
	/**
	 * Remove the flag that was killed
	 * @param flag
	 */
	public void killedFlag(Npc flag)
	{
		if (flag == null)
		{
			return;
		}
		
		foreach (SiegeClan clan in _attackerClans)
		{
			if (clan.removeFlag(flag))
			{
				return;
			}
		}
	}
	
	/**
	 * Register clan as attacker.
	 * @param player The Player of the player trying to register.
	 * @param checkConditions True if should be checked conditions, false otherwise
	 * @return Number that defines what happened.<br>
	 *         0 - Player don't have clan.<br>
	 *         1 - Player don't have enough adena to register.<br>
	 *         2 - Is not right time to register Fortress now.<br>
	 *         3 - Players clan is already registered to siege.<br>
	 *         4 - Players clan is successfully registered to siege.
	 */
	public int addAttacker(Player player, bool checkConditions)
	{
		if (player.getClan() == null)
		{
			return 0; // Player dont have clan
		}
		
		if (checkConditions)
		{
			if (_fort.getSiege().getAttackerClans().isEmpty() && (player.getInventory().getAdena() < 250000))
			{
				return 1; // Player don't have enough adena to register
			}
			
			foreach (Fort fort in FortManager.getInstance().getForts())
			{
				if (fort.getSiege().getAttackerClan(player.getClanId().Value) != null)
				{
					return 3; // Players clan is already registered to siege
				}
				
				if ((fort.getOwnerClan() == player.getClan()) && (fort.getSiege().isInProgress() || (fort.getSiege()._siegeStartTask != null)))
				{
					return 3; // Players clan is already registered to siege
				}
			}
		}
		
		saveSiegeClan(player.getClan());
		if (_attackerClans.size() == 1)
		{
			if (checkConditions)
			{
				player.reduceAdena("FortressSiege", 250000, null, true);
			}
			startAutoTask(true);
		}
		return 4; // Players clan is successfully registered to siege
	}
	
	/**
	 * Remove clan from siege
	 * @param clan The clan being removed
	 */
	public void removeAttacker(Clan clan)
	{
		if ((clan == null) || (clan.getFortId() == getFort().getResidenceId()) || !FortSiegeManager.getInstance().checkIsRegistered(clan, getFort().getResidenceId()))
		{
			return;
		}
		removeSiegeClan(clan.getId());
	}
	
	/**
	 * This function does not do any checks and should not be called from bypass !
	 * @param clanId
	 */
	private void removeSiegeClan(int? clanId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			int fortId = _fort.getResidenceId();
			var query = ctx.FortSiegeClans.Where(r => r.FortId == fortId);
			if (clanId != null)
				query = query.Where(r => r.ClanId == clanId);

			query.ExecuteDelete();
			
			loadSiegeClan();
			if (_attackerClans.isEmpty())
			{
				if (_isInProgress)
				{
					endSiege();
				}
				else
				{
					saveFortSiege(); // Clear siege time in DB
				}
				
				if (_siegeStartTask != null)
				{
					_siegeStartTask.cancel(true);
					_siegeStartTask = null;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Exception on removeSiegeClan: " + e);
		}
	}
	
	/**
	 * Start the auto tasks
	 */
	public void checkAutoTask()
	{
		if (_siegeStartTask != null)
		{
			return;
		}
		
		if ((_fort.getResidenceId() == FortManager.ORC_FORTRESS) && Config.ORC_FORTRESS_ENABLE)
		{
			if (_siegeStartTask != null)
			{
				return;
			}
			
			ThreadPool.execute(new ScheduleSuspiciousMerchantSpawn(this));
			
			DateTime cal = DateTime.Now; // local time
			cal = new DateTime(new DateOnly(cal.Year, cal.Month, cal.Day), Config.ORC_FORTRESS_TIME);
			if (cal < DateTime.Now)
			{
				cal = cal.AddDays(1);
			}

			cal = cal.ToUniversalTime();
			_fort.setSiegeDate(cal);
			saveSiegeDate();
			
			TimeSpan initialDelay = cal - DateTime.UtcNow;
			_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, 0, initialDelay), 0);
		}
		else
		{
			TimeSpan? delay = getFort().getSiegeDate() - DateTime.UtcNow;
			if (delay < TimeSpan.Zero)
			{
				// siege time in past
				saveFortSiege();
				clearSiegeClan(); // remove all clans
				// spawn suspicious merchant immediately
				ThreadPool.execute(new ScheduleSuspiciousMerchantSpawn(this));
			}
			else
			{
				loadSiegeClan();
				if (_attackerClans.isEmpty())
				{
					// no attackers - waiting for suspicious merchant spawn
					ThreadPool.schedule(new ScheduleSuspiciousMerchantSpawn(this), delay ?? TimeSpan.Zero);
				}
				else
				{
					// preparing start siege task
					if (delay > TimeSpan.FromMilliseconds(3600000)) // more than hour, how this can happens ? spawn suspicious merchant
					{
						ThreadPool.execute(new ScheduleSuspiciousMerchantSpawn(this));
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, 3600), delay.Value - TimeSpan.FromMilliseconds(3600000));
					}
					if (delay > TimeSpan.FromMilliseconds(600000)) // more than 10 min, spawn suspicious merchant
					{
						ThreadPool.execute(new ScheduleSuspiciousMerchantSpawn(this));
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, 600), delay.Value - TimeSpan.FromMilliseconds(600000));
					}
					else if (delay > TimeSpan.FromMilliseconds(300000))
					{
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, 300), delay.Value - TimeSpan.FromMilliseconds(300000));
					}
					else if (delay > TimeSpan.FromMilliseconds(60000))
					{
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, 60), delay.Value - TimeSpan.FromMilliseconds(60000));
					}
					else
					{
						// lower than 1 min, set to 1 min
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, 60), 0);
					}
					
					LOGGER.Info(GetType().Name + ": Siege of " + _fort.getName() + " fort: " + _fort.getSiegeDate());
				}
			}
		}
	}
	
	/**
	 * Start the auto task
	 * @param setTime
	 */
	public void startAutoTask(bool setTime)
	{
		if (_siegeStartTask != null)
		{
			return;
		}
		
		if (setTime)
		{
			setSiegeDateTime(false);
		}
		
		if (_fort.getOwnerClan() != null)
		{
			_fort.getOwnerClan().broadcastToOnlineMembers(new SystemMessagePacket(SystemMessageId.A_FORTRESS_IS_UNDER_ATTACK));
		}
		
		// Execute siege auto start
		_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, 3600), 0);
	}
	
	/**
	 * Teleport players
	 * @param teleportWho
	 * @param teleportWhere
	 */
	public void teleportPlayer(FortTeleportWhoType teleportWho, TeleportWhereType teleportWhere)
	{
		List<Player> players;
		switch (teleportWho)
		{
			case FortTeleportWhoType.Owner:
			{
				players = getOwnersInZone();
				break;
			}
			case FortTeleportWhoType.Attacker:
			{
				players = getAttackersInZone();
				break;
			}
			default:
			{
				players = _fort.getZone().getPlayersInside();
				break;
			}
		}
		
		foreach (Player player in players)
		{
			if (player.canOverrideCond(PlayerCondOverride.FORTRESS_CONDITIONS) || player.isJailed())
			{
				continue;
			}
			
			player.teleToLocation(teleportWhere);
		}
	}
	
	/**
	 * Add clan as attacker<
	 * @param clanId
	 */
	private void addAttacker(int clanId)
	{
		_attackerClans.add(new SiegeClan(clanId, SiegeClanType.ATTACKER)); // Add registered attacker to attacker list
	}
	
	/**
	 * @param clan
	 * @return {@code true} if the clan has already registered to a siege for the same day, {@code false} otherwise.
	 */
	public bool checkIfAlreadyRegisteredForSameDay(Clan clan)
	{
		foreach (FortSiege siege in FortSiegeManager.getInstance().getSieges())
		{
			if (siege == this)
			{
				continue;
			}
			
			if (siege.getSiegeDate().DayOfWeek == getSiegeDate().DayOfWeek)
			{
				if (siege.checkIsAttacker(clan))
				{
					return true;
				}
				if (siege.checkIsDefender(clan))
				{
					return true;
				}
			}
		}
		
		return false;
	}
	
	private void setSiegeDateTime(bool merchant)
	{
		DateTime newDate = DateTime.UtcNow;
		if (merchant)
		{
			newDate = newDate.AddMinutes(FortSiegeManager.getInstance().getSuspiciousMerchantRespawnDelay());
		}
		else
		{
			newDate = newDate.AddHours(1);
		}
		
		_fort.setSiegeDate(newDate);
		saveSiegeDate();
	}
	
	/** Load siege clans. */
	private void loadSiegeClan()
	{
		_attackerClans.clear();
		try 
		{
			using GameServerDbContext ctx = new();
			int fortId = _fort.getResidenceId();
			var query = ctx.FortSiegeClans.Where(r => r.FortId == fortId).Select(r => r.ClanId);

			foreach (int clanId in query)
			{
				addAttacker(clanId);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Exception: loadSiegeClan(): " + e);
		}
	}
	
	/** Remove commanders. */
	private void removeCommanders()
	{
		// Remove all instance of commanders for this fort
		foreach (Spawn spawn in _commanders)
		{
			if (spawn != null)
			{
				spawn.stopRespawn();
				if (spawn.getLastSpawn() != null)
				{
					spawn.getLastSpawn().deleteMe();
				}
			}
		}
		_commanders.clear();
	}
	
	/** Remove all flags. */
	private void removeFlags()
	{
		foreach (SiegeClan sc in _attackerClans)
		{
			if (sc != null)
			{
				sc.removeFlags();
			}
		}
	}
	
	/** Save fort siege related to database. */
	private void saveFortSiege()
	{
		clearSiegeDate(); // clear siege date
		saveSiegeDate(); // Save the new date
	}
	
	/** Save siege date to database. */
	private void saveSiegeDate()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			int fortId = _fort.getResidenceId();
			DateTime? siegeDate = _fort.getSiegeDate();
			ctx.Forts.Where(r => r.Id == fortId)
				.ExecuteUpdate(s => s.SetProperty(r => r.SiegeDate, siegeDate));
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Exception: saveSiegeDate(): " + e);
		}
	}
	
	/**
	 * Save registration to database.
	 * @param clan
	 */
	private void saveSiegeClan(Clan clan)
	{
		if (getAttackerClans().Count >= FortSiegeManager.getInstance().getAttackerMaxClans())
		{
			return;
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.FortSiegeClans.Add(new FortSiegeClan()
			{
				FortId = (short)_fort.getResidenceId(),
				ClanId = clan.getId()
			});

			ctx.SaveChanges();
			
			addAttacker(clan.getId());
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Exception: saveSiegeClan(Pledge clan): " + e);
		}
	}
	
	/** Spawn commanders. */
	private void spawnCommanders()
	{
		// Set commanders array size if one does not exist
		try
		{
			_commanders.clear();
			foreach (FortSiegeSpawn _sp in FortSiegeManager.getInstance().getCommanderSpawnList(getFort().getResidenceId()))
			{
				Spawn spawnDat = new Spawn(_sp.getId());
				spawnDat.setAmount(1);
				spawnDat.setXYZ(_sp.getLocation());
				spawnDat.setHeading(_sp.getLocation().getHeading());
				spawnDat.setRespawnDelay(TimeSpan.FromSeconds(60));
				spawnDat.doSpawn(false);
				spawnDat.stopRespawn();
				_commanders.add(spawnDat);
			}
		}
		catch (Exception e)
		{
			// problem with initializing spawn, go to next one
			LOGGER.Error(GetType().Name + ": FortSiege.spawnCommander: Spawn could not be initialized: " + e);
		}
	}
	
	private void spawnFlag(int id)
	{
		foreach (CombatFlag cf in FortSiegeManager.getInstance().getFlagList(id))
		{
			cf.spawnMe();
		}
	}
	
	private void unSpawnFlags()
	{
		if (FortSiegeManager.getInstance().getFlagList(getFort().getResidenceId()) == null)
		{
			return;
		}
		
		foreach (CombatFlag cf in FortSiegeManager.getInstance().getFlagList(getFort().getResidenceId()))
		{
			cf.unSpawnMe();
		}
	}
	
	public void addFlagCount(int count)
	{
		_flagCount += count;
		_flagCount = _flagCount < 0 ? 0 : _flagCount;
	}
	
	public int getFlagCount()
	{
		return _flagCount;
	}
	
	public void setFlagCount(int count)
	{
		_flagCount = 0;
	}
	
	public void loadSiegeGuard()
	{
		_siegeGuards.Clear();
		try 
		{
			using GameServerDbContext ctx = new();
			int fortId = _fort.getResidenceId();
			var query = ctx.FortSiegeGuards.Where(r => r.FortId == fortId);
			foreach (var record in query)
			{
				Spawn spawn = new Spawn(record.NpcId);
				spawn.setAmount(1);
				spawn.setXYZ(record.X, record.Y, record.Z);
				spawn.setHeading(record.Heading);
				spawn.setRespawnDelay(record.RespawnDelay);
				spawn.setLocationId(0);
				
				_siegeGuards.add(spawn);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error loading siege guard for fort " + _fort.getName() + ": " + e);
		}
	}
	
	/**
	 * Spawn siege guard.
	 */
	private void spawnSiegeGuard()
	{
		try
		{
			foreach (Spawn spawnDat in _siegeGuards)
			{
				spawnDat.doSpawn(false);
				if (spawnDat.getRespawnDelay() == 0)
				{
					spawnDat.stopRespawn();
				}
				else
				{
					spawnDat.startRespawn();
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error spawning siege guards for fort " + _fort.getName() + ":" + e);
		}
	}
	
	private void unspawnSiegeGuard()
	{
		try
		{
			foreach (Spawn spawnDat in _siegeGuards)
			{
				spawnDat.stopRespawn();
				if (spawnDat.getLastSpawn() != null)
				{
					spawnDat.getLastSpawn().doDie(spawnDat.getLastSpawn());
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error unspawning siege guards for fort " + _fort.getName() + ":" + e);
		}
	}
	
	public SiegeClan getAttackerClan(Clan clan)
	{
		if (clan == null)
		{
			return null;
		}
		return getAttackerClan(clan.getId());
	}
	
	public SiegeClan getAttackerClan(int clanId)
	{
		foreach (SiegeClan sc in _attackerClans)
		{
			if ((sc != null) && (sc.getClanId() == clanId))
			{
				return sc;
			}
		}
		return null;
	}
	
	public ICollection<SiegeClan> getAttackerClans()
	{
		return _attackerClans;
	}
	
	public Fort getFort()
	{
		return _fort;
	}
	
	public bool isInProgress()
	{
		return _isInProgress;
	}
	
	public DateTime getSiegeDate()
	{
		return _fort.getSiegeDate();
	}
	
	public Set<Npc> getFlag(Clan clan)
	{
		if (clan != null)
		{
			SiegeClan sc = getAttackerClan(clan);
			if (sc != null)
			{
				return sc.getFlag();
			}
		}
		return null;
	}
	
	public void resetSiege()
	{
		// reload commanders and repair doors
		removeCommanders();
		spawnCommanders();
		_fort.resetDoors();
	}
	
	public Set<Spawn> getCommanders()
	{
		return _commanders;
	}
	
	public SiegeClan getDefenderClan(int clanId)
	{
		return null;
	}
	
	public SiegeClan getDefenderClan(Clan clan)
	{
		return null;
	}
	
	public ICollection<SiegeClan> getDefenderClans()
	{
		return null;
	}
	
	public bool giveFame()
	{
		return true;
	}
	
	public int getFameFrequency()
	{
		return Config.FORTRESS_ZONE_FAME_TASK_FREQUENCY;
	}
	
	public int getFameAmount()
	{
		return Config.FORTRESS_ZONE_FAME_AQUIRE_POINTS;
	}
	
	public void updateSiege()
	{
	}
}