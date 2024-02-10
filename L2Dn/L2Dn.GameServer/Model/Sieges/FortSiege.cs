using L2Dn.GameServer.Model.Events;

namespace L2Dn.GameServer.Model.Sieges;

//public class FortSiege implements Siegable
public class FortSiege: ListenersContainer, Siegable
{
	protected static final Logger LOGGER = Logger.getLogger(FortSiege.class.getName());
	
	public static final String ORC_FORTRESS_GREG_UPPER_LEFT_SPAWN = "orc_fortress_greg_upper_left";
	public static final String ORC_FORTRESS_GREG_UPPER_RIGHT_SPAWN = "orc_fortress_greg_upper_right";
	public static final String ORC_FORTRESS_GREG_BOTTOM_RIGHT_SPAWN = "orc_fortress_greg_bottom_right";
	public static final String GREG_SPAWN_VAR = "GREG_SPAWN";
	
	boolean _hasSpawnedPreparationNpcs = false;
	
	private static final AtomicReference<SpawnTemplate> SPAWN_PREPARATION_NPCS = new AtomicReference<>();
	
	private static final ZoneType FORTRESS_ZONE = ZoneManager.getInstance().getZoneByName("orc_fortress_general_area");
	
	ScheduledFuture<?> _siegeGregSentryTask = null;
	
	int _flagCount = 0;
	
	// SQL
	private static final String DELETE_FORT_SIEGECLANS_BY_CLAN_ID = "DELETE FROM fortsiege_clans WHERE fort_id = ? AND clan_id = ?";
	private static final String DELETE_FORT_SIEGECLANS = "DELETE FROM fortsiege_clans WHERE fort_id = ?";
	
	public class ScheduleEndSiegeTask implements Runnable
	{
		@Override
		public void run()
		{
			if (!_isInProgress)
			{
				return;
			}
			
			try
			{
				_siegeEnd = null;
				endSiege();
			}
			catch (Exception e)
			{
				LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: ScheduleEndSiegeTask() for Fort: " + _fort.getName() + " " + e.getMessage(), e);
			}
		}
	}
	
	public class ScheduleGregSentrySpawnTask implements Runnable
	{
		@Override
		public void run()
		{
			FORTRESS_ZONE.broadcastPacket(new ExShowScreenMessage(2, -1, 2, 0, 0, 0, 0, true, 8000, false, null, NpcStringId.FLAG_SENTRY_GREG_HAS_APPEARED, null));
			if (!_isInProgress)
			{
				return;
			}
			try
			{
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_UPPER_LEFT_SPAWN).forEach(holder ->
				{
					holder.spawnAll();
					for (NpcSpawnTemplate nst : holder.getSpawns())
					{
						for (Npc npc : nst.getSpawnedNpcs())
						{
							Spawn spawn = npc.getSpawn();
							if (spawn != null)
							{
								spawn.stopRespawn();
							}
						}
					}
				}));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_UPPER_RIGHT_SPAWN).forEach(holder ->
				{
					holder.spawnAll();
					for (NpcSpawnTemplate nst : holder.getSpawns())
					{
						for (Npc npc : nst.getSpawnedNpcs())
						{
							Spawn spawn = npc.getSpawn();
							if (spawn != null)
							{
								spawn.stopRespawn();
							}
						}
					}
				}));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_BOTTOM_RIGHT_SPAWN).forEach(holder ->
				{
					holder.spawnAll();
					for (NpcSpawnTemplate nst : holder.getSpawns())
					{
						for (Npc npc : nst.getSpawnedNpcs())
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
				LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: ScheduleGregSentrySpawn() for Fort: " + _fort.getName() + " " + e.getMessage(), e);
			}
		}
	}
	
	public class ScheduleStartSiegeTask implements Runnable
	{
		private final Fort _fortInst;
		private final int _time;
		
		private final long _initialDelayInMilliseconds;
		
		public ScheduleStartSiegeTask(int time)
		{
			_fortInst = _fort;
			_time = time;
			_initialDelayInMilliseconds = 0;
		}
		
		public ScheduleStartSiegeTask(int time, long initialDelayInMilliseconds)
		{
			_fortInst = _fort;
			_time = time;
			_initialDelayInMilliseconds = initialDelayInMilliseconds;
		}
		
		@Override
		public void run()
		{
			if (_isInProgress)
			{
				return;
			}
			
			if (!_hasSpawnedPreparationNpcs)
			{
				_hasSpawnedPreparationNpcs = true;
				
				SPAWN_PREPARATION_NPCS.set(SpawnData.getInstance().getSpawns().stream().filter(t -> t.getName() != null).filter(t -> t.getName().contains("orc_fortress_preparation_npcs")).findAny().orElse(null));
				SPAWN_PREPARATION_NPCS.get().getGroups().forEach(SpawnGroup::spawnAll);
			}
			try
			{
				final SystemMessage sm;
				if ((_initialDelayInMilliseconds != 0) && (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS))
				{
					int nextTask = 0;
					if (_initialDelayInMilliseconds >= 1200000)
					{
						nextTask = 1200;
					}
					else
					{
						_isInPreparation = true;
						if (_initialDelayInMilliseconds >= 600000)
						{
							nextTask = 600;
						}
						else if (_initialDelayInMilliseconds >= 300000)
						{
							nextTask = 300;
						}
						else if (_initialDelayInMilliseconds >= 60000)
						{
							nextTask = 60;
						}
						else if (_initialDelayInMilliseconds >= 30000)
						{
							nextTask = 30;
						}
						else if (_initialDelayInMilliseconds >= 10000)
						{
							nextTask = 10;
						}
						else if (_initialDelayInMilliseconds >= 5000)
						{
							nextTask = 5;
						}
						else
						{
							nextTask = 0;
						}
						
						Broadcast.toAllOnlinePlayers(new OrcFortressSiegeInfoHUD(_fortInst.getResidenceId(), 0, (int) Calendar.getInstance().getTimeInMillis() / 1000, (int) _initialDelayInMilliseconds / 1000));
					}
					
					ThreadPool.schedule(new ScheduleStartSiegeTask(nextTask), _initialDelayInMilliseconds - (nextTask * 1000)); // Prepare task for @nextTask minutes left.
					// LOGGER.info("scheduling " + nextTask + " in " + ((_initialDelayInMilliseconds / 1000) - nextTask) + " sec");
				}
				else if ((_time == 3600) && (_fortInst.getResidenceId() != FortManager.ORC_FORTRESS)) // 1hr remains
				
				{
					ThreadPool.schedule(new ScheduleStartSiegeTask(600), 3000000); // Prepare task for 10 minutes left.
				}
				else if ((_time == 1200) && (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)) // 20min remains
				{
					_isInPreparation = true;
					sm = new SystemMessage(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN);
					sm.addInt(20);
					announceToPlayer(sm);
					Broadcast.toAllOnlinePlayers(new OrcFortressSiegeInfoHUD(_fortInst.getResidenceId(), 0, (int) Calendar.getInstance().getTimeInMillis() / 1000, 1200));
					ThreadPool.schedule(new ScheduleStartSiegeTask(600), 600000); // Prepare task for 10 minutes left.
				}
				else if (_time == 600) // 10min remains
				{
					_fort.despawnSuspiciousMerchant();
					sm = new SystemMessage(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN);
					sm.addInt(10);
					announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(300), 300000); // Prepare task for 5 minutes left.
				}
				else if (_time == 300) // 5min remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fort.despawnSuspiciousMerchant();
					}
					sm = new SystemMessage(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN);
					sm.addInt(5);
					announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(60), 240000); // Prepare task for 1 minute left.
				}
				else if (_time == 60) // 1min remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fort.despawnSuspiciousMerchant();
					}
					sm = new SystemMessage(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN);
					sm.addInt(1);
					announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(30), 30000); // Prepare task for 30 seconds left.
				}
				else if (_time == 30) // 30seconds remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fort.despawnSuspiciousMerchant();
					}
					
					sm = new SystemMessage(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_SEC);
					sm.addInt(30);
					announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(10), 20000); // Prepare task for 10 seconds left.
				}
				else if (_time == 10) // 10seconds remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fort.despawnSuspiciousMerchant();
					}
					
					sm = new SystemMessage(SystemMessageId.THE_FORTRESS_BATTLE_STARTS_IN_S1_SEC);
					sm.addInt(10);
					announceToPlayer(sm);
					ThreadPool.schedule(new ScheduleStartSiegeTask(5), 5000); // Prepare task for 5 seconds left.
				}
				else if (_time == 5) // 5seconds remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fort.despawnSuspiciousMerchant();
					}
					
					ThreadPool.schedule(new ScheduleStartSiegeTask(1), 4000); // Prepare task for 1 seconds left.
				}
				else if (_time == 1) // 1seconds remains
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_fort.despawnSuspiciousMerchant();
					}
					
					ThreadPool.schedule(new ScheduleStartSiegeTask(0), 1000); // Prepare task start siege.
				}
				else if (_time == 0) // start siege
				{
					if (_fortInst.getResidenceId() == FortManager.ORC_FORTRESS)
					{
						_isInPreparation = false;
					}
					_fortInst.getSiege().startSiege();
				}
				else
				{
					LOGGER.warning(getClass().getSimpleName() + ": Exception: ScheduleStartSiegeTask(): unknown siege time: " + _time);
				}
			}
			catch (Exception e)
			{
				LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: ScheduleStartSiegeTask() for Fort: " + _fortInst.getName() + " " + e.getMessage(), e);
			}
		}
	}
	
	public class ScheduleSuspiciousMerchantSpawn implements Runnable
	{
		@Override
		public void run()
		{
			if (_isInProgress)
			{
				return;
			}
			
			try
			{
				_fort.spawnSuspiciousMerchant();
			}
			catch (Exception e)
			{
				LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: ScheduleSuspicoiusMerchantSpawn() for Fort: " + _fort.getName() + " " + e.getMessage(), e);
			}
		}
	}
	
	public class ScheduleSiegeRestore implements Runnable
	{
		@Override
		public void run()
		{
			if (!_isInProgress)
			{
				return;
			}
			
			try
			{
				_siegeRestore = null;
				resetSiege();
				announceToPlayer(new SystemMessage(SystemMessageId.THE_BARRACKS_FUNCTION_HAS_BEEN_RESTORED));
			}
			catch (Exception e)
			{
				LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: ScheduleSiegeRestore() for Fort: " + _fort.getName() + " " + e.getMessage(), e);
			}
		}
	}
	
	private final Set<SiegeClan> _attackerClans = ConcurrentHashMap.newKeySet();
	
	// Fort setting
	protected Set<Spawn> _commanders = ConcurrentHashMap.newKeySet();
	protected final Fort _fort;
	boolean _isInProgress = false;
	private final Collection<Spawn> _siegeGuards = new LinkedList<>();
	ScheduledFuture<?> _siegeEnd = null;
	ScheduledFuture<?> _siegeRestore = null;
	ScheduledFuture<?> _siegeStartTask = null;
	// Orc Fortress
	boolean _isInPreparation = false;
	
	public FortSiege(Fort fort)
	{
		_fort = fort;
		checkAutoTask();
		FortSiegeManager.getInstance().addSiege(this);
		if (_fort.getResidenceId() == FortManager.ORC_FORTRESS)
		{
			Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_FORT_SIEGE_START, (OnFortSiegeStart event) -> announceStartToPlayers(event), this));
			Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_FORT_SIEGE_FINISH, (OnFortSiegeFinish event) -> announceEndToPlayers(event), this));
			Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_PLAYER_LOGIN, (OnPlayerLogin event) -> showHUDToPlayer(event), this));
			
		}
	}
	
	private void announceStartToPlayers(OnFortSiegeStart event)
	{
		Broadcast.toAllOnlinePlayers(new OrcFortressSiegeInfoHUD(event.getSiege().getFort().getResidenceId(), 1, 0, 30 * 60));
		Broadcast.toAllOnlinePlayers(new SystemMessage(SystemMessageId.SEAL_THE_SEAL_TOWER_AND_CONQUER_ORC_FORTRESS));
	}
	
	private void announceEndToPlayers(OnFortSiegeFinish event)
	{
		Broadcast.toAllOnlinePlayers(new OrcFortressSiegeInfoHUD(event.getSiege().getFort().getResidenceId(), 0, 0, 0));
	}
	
	private void showHUDToPlayer(OnPlayerLogin event)
	{
		if (_isInPreparation)
		{
			final int remainingTimeInSeconds = (int) (_fort.getSiegeDate().getTimeInMillis() - Calendar.getInstance().getTimeInMillis()) / 1000;
			event.getPlayer().sendPacket(new OrcFortressSiegeInfoHUD(_fort.getResidenceId(), 0, (int) Calendar.getInstance().getTimeInMillis() / 1000, remainingTimeInSeconds));
		}
		else if (_isInProgress)
		{
			final int remainingTimeInSeconds = (int) _siegeEnd.getDelay(TimeUnit.SECONDS);
			event.getPlayer().sendPacket(new OrcFortressSiegeInfoHUD(_fort.getResidenceId(), 1, (int) Calendar.getInstance().getTimeInMillis() / 1000, remainingTimeInSeconds));
		}
	}
	
	/**
	 * When siege ends.
	 */
	@Override
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
				for (Player player : World.getInstance().getPlayers())
				{
					Item weap = player.getActiveWeaponInstance();
					if ((weap != null) && (weap.getId() == FortManager.ORC_FORTRESS_FLAG))
					{
						FortSiegeManager.getInstance().dropCombatFlag(player, getFort().getResidenceId());
					}
				}
				
				for (WorldObject obj : World.getInstance().getVisibleObjects())
				{
					if (obj instanceof Item)
					{
						if (obj.getId() == FortManager.ORC_FORTRESS_FLAG)
						{
							obj.decayMe();
						}
					}
				}
				
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName("orc_fortress").forEach(holder -> holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName("orc_runners").forEach(holder -> holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName("orc_fortress_inside").forEach(holder -> holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName("orc_fortress_jeras_guards").forEach(holder -> holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_UPPER_LEFT_SPAWN).forEach(holder -> holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_UPPER_RIGHT_SPAWN).forEach(holder -> holder.despawnAll()));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName(ORC_FORTRESS_GREG_BOTTOM_RIGHT_SPAWN).forEach(holder -> holder.despawnAll()));
				SPAWN_PREPARATION_NPCS.get().getGroups().forEach(SpawnGroup::despawnAll);
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
				LOGGER.info("FortSiege: Closed Orc Fortress doors.");
			}
			_fort.SetOrcFortressOwnerNpcs(true);
			ThreadPool.schedule(new ScheduleSuspiciousMerchantSpawn(), FortSiegeManager.getInstance().getSuspiciousMerchantRespawnDelay() * 60 * 1000); // Prepare 3hr task for suspicious merchant respawn
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
			
			LOGGER.info(getClass().getSimpleName() + ": Siege of " + _fort.getName() + " fort finished.");
			
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
	@Override
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
				LOGGER.info("FortSiege: Opened Orc Fortress doors.");
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
				_siegeGregSentryTask = ThreadPool.schedule(new ScheduleGregSentrySpawnTask(), 20 * 60 * 1000L); // 20 min
				// _siegeGregSentryTask = ThreadPool.schedule(new ScheduleGregSentrySpawnTask(), 2 * 60 * 1000L); // Prepare Greg Sentry spawn task, Test only
				_siegeEnd = ThreadPool.schedule(new ScheduleEndSiegeTask(), 30 * 60 * 1000); // Prepare auto end task
			}
			else
			{
				_siegeEnd = ThreadPool.schedule(new ScheduleEndSiegeTask(), FortSiegeManager.getInstance().getSiegeLength() * 60 * 1000); // Prepare auto end task
			}
			
			announceToPlayer(new SystemMessage(SystemMessageId.THE_FORTRESS_BATTLE_HAS_BEGUN));
			saveFortSiege();
			
			if (_fort.getResidenceId() == FortManager.ORC_FORTRESS)
			{
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName("orc_fortress").forEach(holder ->
				{
					holder.spawnAll();
					for (NpcSpawnTemplate nst : holder.getSpawns())
					{
						for (Npc npc : nst.getSpawnedNpcs())
						{
							Spawn spawn = npc.getSpawn();
							if (spawn != null)
							{
								spawn.setRespawnDelay(5);
							}
						}
					}
				}));
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName("orc_runners").forEach(holder ->
				{
					holder.spawnAll();
					for (NpcSpawnTemplate nst : holder.getSpawns())
					{
						for (Npc npc : nst.getSpawnedNpcs())
						{
							Spawn spawn = npc.getSpawn();
							if (spawn != null)
							{
								spawn.setRespawnDelay(5, 10);
							}
						}
					}
					
				}));
				
				SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName("orc_fortress_inside").forEach(holder ->
				{
					holder.spawnAll();
					for (NpcSpawnTemplate nst : holder.getSpawns())
					{
						for (Npc npc : nst.getSpawnedNpcs())
						{
							Spawn spawn = npc.getSpawn();
							if (spawn != null)
							{
								spawn.setRespawnDelay(5);
							}
						}
					}
					
				}));
			}
			SpawnData.getInstance().getSpawns().forEach(spawnTemplate -> spawnTemplate.getGroupsByName("orc_fortress_jeras_guards").forEach(holder ->
			{
				holder.spawnAll();
				for (NpcSpawnTemplate nst : holder.getSpawns())
				{
					for (Npc npc : nst.getSpawnedNpcs())
					{
						Spawn spawn = npc.getSpawn();
						if (spawn != null)
						{
							spawn.setRespawnDelay(160);
						}
					}
				}
				
			}));
		}
		
		LOGGER.info(getClass().getSimpleName() + ": Siege of " + _fort.getName() + " fort started.");
		
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
	public void announceToPlayer(SystemMessage sm)
	{
		// announce messages only for participants
		Clan clan;
		for (SiegeClan siegeclan : _attackerClans)
		{
			clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			for (Player member : clan.getOnlineMembers(0))
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
			for (Player member : clan.getOnlineMembers(0))
			{
				if (member != null)
				{
					member.sendPacket(sm);
				}
			}
		}
	}
	
	public void announceToPlayer(SystemMessage sm, String s)
	{
		sm.addString(s);
		announceToPlayer(sm);
	}
	
	public void updatePlayerSiegeStateFlags(boolean clear)
	{
		Clan clan;
		for (SiegeClan siegeclan : _attackerClans)
		{
			clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			for (Player member : clan.getOnlineMembers(0))
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
					member.setSiegeState((byte) 1);
					member.setSiegeSide(_fort.getResidenceId());
					if (checkIfInZone(member))
					{
						member.setInSiege(true);
						member.startFameTask(Config.FORTRESS_ZONE_FAME_TASK_FREQUENCY * 1000, Config.FORTRESS_ZONE_FAME_AQUIRE_POINTS);
					}
				}
				member.broadcastUserInfo();
			}
		}
		if (_fort.getOwnerClan() != null)
		{
			clan = ClanTable.getInstance().getClan(getFort().getOwnerClan().getId());
			for (Player member : clan.getOnlineMembers(0))
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
						member.startFameTask(Config.FORTRESS_ZONE_FAME_TASK_FREQUENCY * 1000, Config.FORTRESS_ZONE_FAME_AQUIRE_POINTS);
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
	public boolean checkIfInZone(WorldObject object)
	{
		return checkIfInZone(object.getX(), object.getY(), object.getZ());
	}
	
	/**
	 * @param x
	 * @param y
	 * @param z
	 * @return true if object is inside the zone
	 */
	public boolean checkIfInZone(int x, int y, int z)
	{
		return (_isInProgress && (_fort.checkIfInZone(x, y, z))); // Fort zone during siege
	}
	
	/**
	 * @param clan The Clan of the player
	 * @return true if clan is attacker
	 */
	@Override
	public boolean checkIsAttacker(Clan clan)
	{
		return (getAttackerClan(clan) != null);
	}
	
	/**
	 * @param clan The Clan of the player
	 * @return true if clan is defender
	 */
	@Override
	public boolean checkIsDefender(Clan clan)
	{
		return (clan != null) && (_fort.getOwnerClan() == clan);
	}
	
	/** Clear all registered siege clans from database for fort */
	public void clearSiegeClan()
	{
		try (Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("DELETE FROM fortsiege_clans WHERE fort_id=?"))
		{
			ps.setInt(1, _fort.getResidenceId());
			ps.execute();
			
			if (_fort.getOwnerClan() != null)
			{
				try (PreparedStatement delete = con.prepareStatement("DELETE FROM fortsiege_clans WHERE clan_id=?"))
				{
					delete.setInt(1, _fort.getOwnerClan().getId());
					delete.execute();
				}
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
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: clearSiegeClan(): " + e.getMessage(), e);
		}
	}
	
	/** Set the date for the next siege. */
	private void clearSiegeDate()
	{
		_fort.getSiegeDate().setTimeInMillis(0);
	}
	
	/**
	 * @return list of Player registered as attacker in the zone.
	 */
	@Override
	public List<Player> getAttackersInZone()
	{
		final List<Player> players = new LinkedList<>();
		for (SiegeClan siegeclan : _attackerClans)
		{
			final Clan clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			for (Player player : clan.getOnlineMembers(0))
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
		final List<Player> players = new LinkedList<>();
		if (_fort.getOwnerClan() != null)
		{
			final Clan clan = ClanTable.getInstance().getClan(getFort().getOwnerClan().getId());
			if (clan != _fort.getOwnerClan())
			{
				return null;
			}
			
			for (Player player : clan.getOnlineMembers(0))
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
			final Spawn spawn = instance.getSpawn();
			if (spawn != null)
			{
				for (FortSiegeSpawn spawn2 : FortSiegeManager.getInstance().getCommanderSpawnList(getFort().getResidenceId()))
				{
					if (spawn2.getId() == spawn.getId())
					{
						NpcStringId npcString = null;
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
							instance.broadcastSay(ChatType.NPC_SHOUT, npcString);
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
					for (Door door : _fort.getDoors())
					{
						if (door.isShowHp())
						{
							continue;
						}
						
						// TODO this also opens control room door at big fort
						door.openMe();
					}
					_fort.getSiege().announceToPlayer(new SystemMessage(SystemMessageId.ALL_BARRACKS_ARE_OCCUPIED));
				}
				// schedule restoring doors/commanders respawn
				else if (_siegeRestore == null)
				{
					_fort.getSiege().announceToPlayer(new SystemMessage(SystemMessageId.THE_BARRACKS_HAVE_BEEN_SEIZED));
					_siegeRestore = ThreadPool.schedule(new ScheduleSiegeRestore(), FortSiegeManager.getInstance().getCountDownLength() * 60 * 1000);
				}
				else
				{
					_fort.getSiege().announceToPlayer(new SystemMessage(SystemMessageId.THE_BARRACKS_HAVE_BEEN_SEIZED));
				}
			}
			else
			{
				LOGGER.warning(getClass().getSimpleName() + ": FortSiege.killedCommander(): killed commander, but commander not registered for fortress. NpcId: " + instance.getId() + " FortId: " + _fort.getResidenceId());
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
		
		for (SiegeClan clan : _attackerClans)
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
	public int addAttacker(Player player, boolean checkConditions)
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
			
			for (Fort fort : FortManager.getInstance().getForts())
			{
				if (fort.getSiege().getAttackerClan(player.getClanId()) != null)
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
	private void removeSiegeClan(int clanId)
	{
		final String query = (clanId != 0) ? DELETE_FORT_SIEGECLANS_BY_CLAN_ID : DELETE_FORT_SIEGECLANS;
		try (Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(query))
		{
			statement.setInt(1, _fort.getResidenceId());
			if (clanId != 0)
			{
				statement.setInt(2, clanId);
			}
			statement.execute();
			
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
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception on removeSiegeClan: " + e.getMessage(), e);
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
			
			ThreadPool.execute(new ScheduleSuspiciousMerchantSpawn());
			
			final Calendar cal = Calendar.getInstance();
			cal.set(Calendar.HOUR_OF_DAY, Config.ORC_FORTRESS_HOUR);
			cal.set(Calendar.MINUTE, Config.ORC_FORTRESS_MINUTE);
			cal.set(Calendar.SECOND, 0);
			cal.set(Calendar.MILLISECOND, 0);
			if (cal.getTimeInMillis() < Calendar.getInstance().getTimeInMillis())
			{
				cal.add(Calendar.DAY_OF_MONTH, 1);
			}
			
			_fort.setSiegeDate(cal);
			saveSiegeDate();
			
			final long initialDelayInMilliseconds = _fort.getSiegeDate().getTimeInMillis() - Calendar.getInstance().getTimeInMillis();
			_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(0, initialDelayInMilliseconds), 0);
			
		}
		else
		{
			
			final long delay = getFort().getSiegeDate().getTimeInMillis() - System.currentTimeMillis();
			if (delay < 0)
			{
				// siege time in past
				saveFortSiege();
				clearSiegeClan(); // remove all clans
				// spawn suspicious merchant immediately
				ThreadPool.execute(new ScheduleSuspiciousMerchantSpawn());
			}
			else
			{
				loadSiegeClan();
				if (_attackerClans.isEmpty())
				{
					// no attackers - waiting for suspicious merchant spawn
					ThreadPool.schedule(new ScheduleSuspiciousMerchantSpawn(), delay);
				}
				else
				{
					// preparing start siege task
					if (delay > 3600000) // more than hour, how this can happens ? spawn suspicious merchant
					{
						ThreadPool.execute(new ScheduleSuspiciousMerchantSpawn());
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(3600), delay - 3600000);
					}
					if (delay > 600000) // more than 10 min, spawn suspicious merchant
					{
						ThreadPool.execute(new ScheduleSuspiciousMerchantSpawn());
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(600), delay - 600000);
					}
					else if (delay > 300000)
					{
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(300), delay - 300000);
					}
					else if (delay > 60000)
					{
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(60), delay - 60000);
					}
					else
					{
						// lower than 1 min, set to 1 min
						_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(60), 0);
					}
					
					LOGGER.info(getClass().getSimpleName() + ": Siege of " + _fort.getName() + " fort: " + _fort.getSiegeDate().getTime());
				}
			}
		}
	}
	
	/**
	 * Start the auto task
	 * @param setTime
	 */
	public void startAutoTask(boolean setTime)
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
			_fort.getOwnerClan().broadcastToOnlineMembers(new SystemMessage(SystemMessageId.A_FORTRESS_IS_UNDER_ATTACK));
		}
		
		// Execute siege auto start
		_siegeStartTask = ThreadPool.schedule(new ScheduleStartSiegeTask(3600), 0);
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
			case Owner:
			{
				players = getOwnersInZone();
				break;
			}
			case Attacker:
			{
				players = getAttackersInZone();
				break;
			}
			default:
			{
				players = _fort.getZone().getPlayersInside();
			}
		}
		
		for (Player player : players)
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
	public boolean checkIfAlreadyRegisteredForSameDay(Clan clan)
	{
		for (FortSiege siege : FortSiegeManager.getInstance().getSieges())
		{
			if (siege == this)
			{
				continue;
			}
			
			if (siege.getSiegeDate().get(Calendar.DAY_OF_WEEK) == getSiegeDate().get(Calendar.DAY_OF_WEEK))
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
	
	private void setSiegeDateTime(boolean merchant)
	{
		final Calendar newDate = Calendar.getInstance();
		if (merchant)
		{
			newDate.add(Calendar.MINUTE, FortSiegeManager.getInstance().getSuspiciousMerchantRespawnDelay());
		}
		else
		{
			newDate.add(Calendar.MINUTE, 60);
		}
		_fort.setSiegeDate(newDate);
		saveSiegeDate();
	}
	
	/** Load siege clans. */
	private void loadSiegeClan()
	{
		_attackerClans.clear();
		try (Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT clan_id FROM fortsiege_clans WHERE fort_id=?"))
		{
			ps.setInt(1, _fort.getResidenceId());
			try (ResultSet rs = ps.executeQuery())
			{
				while (rs.next())
				{
					addAttacker(rs.getInt("clan_id"));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: loadSiegeClan(): " + e.getMessage(), e);
		}
	}
	
	/** Remove commanders. */
	private void removeCommanders()
	{
		// Remove all instance of commanders for this fort
		for (Spawn spawn : _commanders)
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
		for (SiegeClan sc : _attackerClans)
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
		try (Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("UPDATE fort SET siegeDate = ? WHERE id = ?"))
		{
			ps.setLong(1, _fort.getSiegeDate().getTimeInMillis());
			ps.setInt(2, _fort.getResidenceId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: saveSiegeDate(): " + e.getMessage(), e);
		}
	}
	
	/**
	 * Save registration to database.
	 * @param clan
	 */
	private void saveSiegeClan(Clan clan)
	{
		if (getAttackerClans().size() >= FortSiegeManager.getInstance().getAttackerMaxClans())
		{
			return;
		}
		
		try (Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement("INSERT INTO fortsiege_clans (clan_id,fort_id) values (?,?)"))
		{
			statement.setInt(1, clan.getId());
			statement.setInt(2, _fort.getResidenceId());
			statement.execute();
			
			addAttacker(clan.getId());
		}
		catch (Exception e)
		{
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Exception: saveSiegeClan(Pledge clan): " + e.getMessage(), e);
		}
	}
	
	/** Spawn commanders. */
	private void spawnCommanders()
	{
		// Set commanders array size if one does not exist
		try
		{
			_commanders.clear();
			for (FortSiegeSpawn _sp : FortSiegeManager.getInstance().getCommanderSpawnList(getFort().getResidenceId()))
			{
				final Spawn spawnDat = new Spawn(_sp.getId());
				spawnDat.setAmount(1);
				spawnDat.setXYZ(_sp.getLocation());
				spawnDat.setHeading(_sp.getLocation().getHeading());
				spawnDat.setRespawnDelay(60);
				spawnDat.doSpawn(false);
				spawnDat.stopRespawn();
				_commanders.add(spawnDat);
			}
		}
		catch (Exception e)
		{
			// problem with initializing spawn, go to next one
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": FortSiege.spawnCommander: Spawn could not be initialized: " + e.getMessage(), e);
		}
	}
	
	private void spawnFlag(int id)
	{
		for (CombatFlag cf : FortSiegeManager.getInstance().getFlagList(id))
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
		
		for (CombatFlag cf : FortSiegeManager.getInstance().getFlagList(getFort().getResidenceId()))
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
		_siegeGuards.clear();
		try (Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT npcId, x, y, z, heading, respawnDelay FROM fort_siege_guards WHERE fortId = ?"))
		{
			final int fortId = _fort.getResidenceId();
			ps.setInt(1, fortId);
			try (ResultSet rs = ps.executeQuery())
			{
				while (rs.next())
				{
					final Spawn spawn = new Spawn(rs.getInt("npcId"));
					spawn.setAmount(1);
					spawn.setXYZ(rs.getInt("x"), rs.getInt("y"), rs.getInt("z"));
					spawn.setHeading(rs.getInt("heading"));
					spawn.setRespawnDelay(rs.getInt("respawnDelay"));
					spawn.setLocationId(0);
					
					_siegeGuards.add(spawn);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Error loading siege guard for fort " + _fort.getName() + ": " + e.getMessage(), e);
		}
	}
	
	/**
	 * Spawn siege guard.
	 */
	private void spawnSiegeGuard()
	{
		try
		{
			for (Spawn spawnDat : _siegeGuards)
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
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Error spawning siege guards for fort " + _fort.getName() + ":" + e.getMessage(), e);
		}
	}
	
	private void unspawnSiegeGuard()
	{
		try
		{
			for (Spawn spawnDat : _siegeGuards)
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
			LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Error unspawning siege guards for fort " + _fort.getName() + ":" + e.getMessage(), e);
		}
	}
	
	@Override
	public SiegeClan getAttackerClan(Clan clan)
	{
		if (clan == null)
		{
			return null;
		}
		return getAttackerClan(clan.getId());
	}
	
	@Override
	public SiegeClan getAttackerClan(int clanId)
	{
		for (SiegeClan sc : _attackerClans)
		{
			if ((sc != null) && (sc.getClanId() == clanId))
			{
				return sc;
			}
		}
		return null;
	}
	
	@Override
	public Collection<SiegeClan> getAttackerClans()
	{
		return _attackerClans;
	}
	
	public Fort getFort()
	{
		return _fort;
	}
	
	public boolean isInProgress()
	{
		return _isInProgress;
	}
	
	@Override
	public Calendar getSiegeDate()
	{
		return _fort.getSiegeDate();
	}
	
	@Override
	public Set<Npc> getFlag(Clan clan)
	{
		if (clan != null)
		{
			final SiegeClan sc = getAttackerClan(clan);
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
	
	@Override
	public SiegeClan getDefenderClan(int clanId)
	{
		return null;
	}
	
	@Override
	public SiegeClan getDefenderClan(Clan clan)
	{
		return null;
	}
	
	@Override
	public List<SiegeClan> getDefenderClans()
	{
		return null;
	}
	
	@Override
	public boolean giveFame()
	{
		return true;
	}
	
	@Override
	public int getFameFrequency()
	{
		return Config.FORTRESS_ZONE_FAME_TASK_FREQUENCY;
	}
	
	@Override
	public int getFameAmount()
	{
		return Config.FORTRESS_ZONE_FAME_AQUIRE_POINTS;
	}
	
	@Override
	public void updateSiege()
	{
	}
}
